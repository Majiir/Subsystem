using BBI.Core;
using BBI.Core.Data;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using LitJson;
using Subsystem.Patch;
using Subsystem.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace Subsystem
{
    public class AttributeLoader
    {
        private readonly StringWriter writer;
        private readonly StringLogger logger;

        public AttributeLoader()
        {
            writer = new StringWriter();
            logger = new StringLogger(writer);
        }

        public void LoadAttributes(EntityTypeCollection entityTypeCollection)
        {
            try
            {
                var jsonPath = Path.Combine(Application.dataPath, "patch.json");
                var json = File.ReadAllText(jsonPath);

                var attributesPatch = JsonMapper.ToObject<AttributesPatch>(json);

                ApplyAttributesPatch(entityTypeCollection, attributesPatch);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SUBSYSTEM] Error applying patch file: {e}");
            }
        }

        public void ApplyAttributesPatch(EntityTypeCollection entityTypeCollection, AttributesPatch attributesPatch)
        {
            foreach (var kvp in attributesPatch.Entities)
            {
                var entityTypeName = kvp.Key;
                var entityTypePatch = kvp.Value;

                var entityType = entityTypeCollection.GetEntityType(entityTypeName);

                using (logger.BeginScope($"EntityType: {entityTypeName}"))
                {
                    if (entityType == null)
                    {
                        logger.Log($"NOTICE: EntityType not found");
                        continue;
                    }

                    applyUnnamedComponentPatch<ExperienceAttributesPatch, ExperienceAttributes, ExperienceAttributesWrapper>(entityType, entityTypePatch.ExperienceAttributes, x => new ExperienceAttributesWrapper(x), ApplyExperienceAttributesPatch);
                    applyUnnamedComponentPatch<UnitAttributesPatch, UnitAttributes, UnitAttributesWrapper>(entityType, entityTypePatch.UnitAttributes, x => new UnitAttributesWrapper(x), ApplyUnitAttributesPatch);
                    applyUnnamedComponentPatch<ResearchItemAttributesPatch, ResearchItemAttributes, ResearchItemAttributesWrapper>(entityType, entityTypePatch.ResearchItemAttributes, x => new ResearchItemAttributesWrapper(x), ApplyResearchItemAttributesPatch);
                    applyUnnamedComponentPatch<UnitHangarAttributesPatch, UnitHangarAttributes, UnitHangarAttributesWrapper>(entityType, entityTypePatch.UnitHangarAttributes, x => new UnitHangarAttributesWrapper(x), ApplyUnitHangarAttributesPatch);
                    applyUnnamedComponentPatch<DetectableAttributesPatch, DetectableAttributes, DetectableAttributesWrapper>(entityType, entityTypePatch.DetectableAttributes, x => new DetectableAttributesWrapper(x), ApplyDetectableAttributesPatch);
                    applyUnnamedComponentPatch<UnitMovementAttributesPatch, UnitMovementAttributes, UnitMovementAttributesWrapper>(entityType, entityTypePatch.UnitMovementAttributes, x => new UnitMovementAttributesWrapper(x), ApplyUnitMovementAttributesPatch);

                    applyNamedComponentPatch<AbilityAttributesPatch, AbilityAttributes, AbilityAttributesWrapper>(entityType, entityTypePatch.AbilityAttributes, x => new AbilityAttributesWrapper(x), ApplyAbilityAttributesPatch);
                    applyNamedComponentPatch<StorageAttributesPatch, StorageAttributes, StorageAttributesWrapper>(entityType, entityTypePatch.StorageAttributes, x => new StorageAttributesWrapper(x), ApplyStorageAttributesPatch);
                    applyNamedComponentPatch<WeaponAttributesPatch, WeaponAttributes, WeaponAttributesWrapper>(entityType, entityTypePatch.WeaponAttributes, x => new WeaponAttributesWrapper(x), (patch, wrapper) =>
                    {
                        ApplyWeaponAttributesPatch(patch, wrapper);
                        rebindWeaponAttributes(entityType, wrapper);
                    });
                }
            }

            File.WriteAllText(Path.Combine(Application.dataPath, "Subsystem.log"), writer.ToString());
            Debug.Log($"[SUBSYSTEM] Applied attributes patch. See Subsystem.log for details.");
        }

        private void applyUnnamedComponentPatch<TPatch, TAttributes, TWrapper>(EntityTypeAttributes entityType, TPatch patch, Func<TAttributes, TWrapper> createWrapper, Action<TPatch, TWrapper> applyPatch)
            where TAttributes : class
            where TWrapper : TAttributes
        {
            if (patch != null)
            {
                applyComponentPatch(entityType, patch, createWrapper, applyPatch);
            }
        }

        private void applyNamedComponentPatch<TPatch, TAttributes, TWrapper>(EntityTypeAttributes entityType, Dictionary<string, TPatch> patch, Func<TAttributes, TWrapper> createWrapper, Action<TPatch, TWrapper> applyPatch)
            where TAttributes : class
            where TWrapper : TAttributes
        {
            foreach (var kvp in patch)
            {
                var elementName = kvp.Key;
                var elementPatch = kvp.Value;

                applyComponentPatch(entityType, elementPatch, createWrapper, applyPatch, elementName);
            }
        }

        private void applyComponentPatch<TPatch, TAttributes, TWrapper>(EntityTypeAttributes entityType, TPatch patch, Func<TAttributes, TWrapper> createWrapper, Action<TPatch, TWrapper> applyPatch, string name = null)
            where TAttributes : class
            where TWrapper : TAttributes
        {
            using (logger.BeginScope($"{typeof(TAttributes).Name}: {name}"))
            {
                var attributes = name != null
                    ? entityType.Get<TAttributes>(name)
                    : entityType.Get<TAttributes>();

                if (attributes == null)
                {
                    logger.Log($"ERROR: {typeof(TAttributes).Name} not found");
                    return;
                }

                var wrapper = createWrapper(attributes);

                applyPatch(patch, wrapper);

                entityType.Replace(attributes, wrapper);
            }
        }

        private void applyPropertyPatch<TProperty>(TProperty? newValue, Expression<Func<TProperty>> expression) where TProperty : struct
        {
            if (newValue.HasValue)
            {
                setProperty(newValue.Value, expression, x => x);
            }
        }

        private void applyPropertyPatch<TProperty>(TProperty newValue, Expression<Func<TProperty>> expression) where TProperty : class
        {
            if (newValue != null)
            {
                setProperty(newValue, expression, x => x);
            }
        }

        private void applyPropertyPatch<TValue, TProperty>(TValue? newValue, Expression<Func<TProperty>> expression, Func<TValue, TProperty> projection) where TValue : struct
        {
            if (newValue.HasValue)
            {
                setProperty(newValue.Value, expression, projection);
            }
        }

        private void applyPropertyPatch<TValue, TProperty>(TValue newValue, Expression<Func<TProperty>> expression, Func<TValue, TProperty> projection) where TValue : class
        {
            if (newValue != null)
            {
                setProperty(newValue, expression, projection);
            }
        }

        private void setProperty<TValue, TProperty>(TValue newValue, Expression<Func<TProperty>> expression, Func<TValue, TProperty> projection)
        {
            var value = projection(newValue);

            var accessor = new PropertyAccessor<TProperty>(expression);

            var oldValue = accessor.Get();
            accessor.Set(value);

            logger.Log($"{accessor.Name}: {value} (was: {oldValue})");
        }

        private static void rebindWeaponAttributes(EntityTypeAttributes entityType, WeaponAttributesWrapper weaponAttributesWrapper)
        {
            var unitAttributes = entityType.Get<UnitAttributes>();
            if (unitAttributes != null)
            {
                for (var i = 0; i < unitAttributes.WeaponLoadout.Length; i++)
                {
                    var weaponBinding = unitAttributes.WeaponLoadout[i];
                    if (weaponBinding.Weapon.Name == weaponAttributesWrapper.Name)
                    {
                        unitAttributes.WeaponLoadout[i] = new WeaponBinding(
                            weaponID: weaponBinding.WeaponID,
                            weaponBindingIndex: weaponBinding.WeaponBindingIndex,
                            weapon: weaponAttributesWrapper,
                            ammoID: weaponBinding.AmmoID,
                            turretIndex: weaponBinding.TurretIndex,
                            defaultTurretAngleOffsetRadians: weaponBinding.DefaultTurretAngleOffsetRadians,
                            disabledOnSpawn: weaponBinding.DisabledOnSpawn,
                            weaponOffsetFromUnitOrigin: weaponBinding.OffsetFromUnitCenterInLocalSpace,
                            showAmmoOnHUD: weaponBinding.ShowAmmoOnHUD
                        );
                    }
                }
            }
        }

        public void ApplyExperienceAttributesPatch(ExperienceAttributesPatch experienceAttributesPatch, ExperienceAttributesWrapper experienceAttributesWrapper)
        {
            var wrappers = experienceAttributesWrapper.Levels.Select(x => new ExperienceLevelAttributesWrapper(x)).ToList();

            applyExperienceLevelsPatch(experienceAttributesPatch.Levels, wrappers);

            experienceAttributesWrapper.Levels = wrappers.ToArray();
        }

        private void applyListPatch<TPatch, TWrapper>(Dictionary<string, TPatch> patch, List<TWrapper> wrappers, Func<TWrapper> createWrapper, Action<TPatch, TWrapper> applyPatch, string elementName)
            where TWrapper : class
        {
            foreach (var kvp in patch.OrderBy(x => x.Key))
            {
                if (!int.TryParse(kvp.Key, out var index))
                {
                    logger.Log($"ERROR: Non-integer key: {kvp.Key}");
                    break;
                }

                var elementPatch = kvp.Value;

                using (logger.BeginScope($"{elementName}: {index}"))
                {
                    var remove = elementPatch is IRemovable removable && removable.Remove;

                    if (index < wrappers.Count)
                    {
                        if (remove)
                        {
                            logger.Log("(removed)");
                            wrappers[index] = null;
                            continue;
                        }

                        var elementWrapper = wrappers[index];

                        applyPatch(elementPatch, elementWrapper);

                        wrappers[index] = elementWrapper;
                    }
                    else if (index == wrappers.Count)
                    {
                        if (remove)
                        {
                            logger.Log("WARNING: Remove flag set for non-existent entry");
                            continue;
                        }

                        logger.Log("(created)");
                        var elementWrapper = createWrapper(); // Deal with INamed?

                        applyPatch(elementPatch, elementWrapper);

                        wrappers.Add(elementWrapper);
                    }
                    else // if (index > wrappers.Count)
                    {
                        logger.Log("ERROR: Non-consecutive index");
                        continue;
                    }
                }
            }

            wrappers.RemoveAll(x => x == null);
        }

        private void applyExperienceLevelsPatch(Dictionary<string, ExperienceLevelAttributesPatch> patch, List<ExperienceLevelAttributesWrapper> wrappers)
        {
            applyListPatch(patch, wrappers, () => new ExperienceLevelAttributesWrapper(), ApplyExperienceLevelAttributesPatch, nameof(ExperienceLevelAttributes));
        }

        public void ApplyExperienceLevelAttributesPatch(ExperienceLevelAttributesPatch experienceLevelAttributesPatch, ExperienceLevelAttributesWrapper experienceLevelAttributesWrapper)
        {
            applyPropertyPatch(experienceLevelAttributesPatch.BuffTooltipLocID, () => experienceLevelAttributesWrapper.BuffTooltipLocID);
            applyPropertyPatch(experienceLevelAttributesPatch.RequiredExperience, () => experienceLevelAttributesWrapper.RequiredExperience);

            var attributeBuffSetWrapper = new AttributeBuffSetWrapper(experienceLevelAttributesWrapper.Buff);
            experienceLevelAttributesWrapper.Buff = attributeBuffSetWrapper;

            applyAttributeBuffSetPatch(experienceLevelAttributesPatch.Buff, attributeBuffSetWrapper);
        }

        private void applyAttributeBuffSetPatch(Dictionary<string, AttributeBuffPatch> patch, AttributeBuffSetWrapper wrapper)
        {
            applyListPatch(patch, wrapper.Buffs, () => new AttributeBuffWrapper(), applyAttributeBuffPatch, nameof(AttributeBuff));
        }

        private void applyAttributeBuffPatch(AttributeBuffPatch patch, AttributeBuffWrapper wrapper)
        {
            applyPropertyPatch(patch.Name, () => wrapper.Name);
            applyPropertyPatch(patch.Attribute, () => wrapper.AttributeID, Buff.AttributeIDFromBuffCategoryAndID);
            applyPropertyPatch(patch.Attribute, () => wrapper.Category, Buff.CategoryFromBuffCategoryAndID);
            applyPropertyPatch(patch.Mode, () => wrapper.Mode);
            applyPropertyPatch(patch.Value, () => wrapper.Value);
        }

        public void ApplyUnitAttributesPatch(UnitAttributesPatch unitAttributesPatch, UnitAttributesWrapper unitAttributesWrapper)
        {
            applyPropertyPatch(unitAttributesPatch.Class, () => unitAttributesWrapper.Class);
            applyPropertyPatch(unitAttributesPatch.SelectionFlags, () => unitAttributesWrapper.SelectionFlags);
            applyPropertyPatch(unitAttributesPatch.MaxHealth, () => unitAttributesWrapper.MaxHealth);
            applyPropertyPatch(unitAttributesPatch.Armour, () => unitAttributesWrapper.Armour);
            applyPropertyPatch(unitAttributesPatch.DamageReceivedMultiplier, () => unitAttributesWrapper.DamageReceivedMultiplier, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.AccuracyReceivedMultiplier, () => unitAttributesWrapper.AccuracyReceivedMultiplier, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.PopCapCost, () => unitAttributesWrapper.PopCapCost);
            applyPropertyPatch(unitAttributesPatch.ExperienceValue, () => unitAttributesWrapper.ExperienceValue);
            applyPropertyPatch(unitAttributesPatch.ProductionTime, () => unitAttributesWrapper.ProductionTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.AggroRange, () => unitAttributesWrapper.AggroRange, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.LeashRange, () => unitAttributesWrapper.LeashRange, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.AlertRange, () => unitAttributesWrapper.AlertRange, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.RepairPickupRange, () => unitAttributesWrapper.RepairPickupRange, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.UnitPositionReaggroConditions, () => unitAttributesWrapper.UnitPositionReaggroConditions);
            applyPropertyPatch(unitAttributesPatch.LeashPositionReaggroConditions, () => unitAttributesWrapper.LeashPositionReaggroConditions);
            applyPropertyPatch(unitAttributesPatch.LeadPriority, () => unitAttributesWrapper.LeadPriority);
            applyPropertyPatch(unitAttributesPatch.Selectable, () => unitAttributesWrapper.Selectable);
            applyPropertyPatch(unitAttributesPatch.Controllable, () => unitAttributesWrapper.Controllable);
            applyPropertyPatch(unitAttributesPatch.Targetable, () => unitAttributesWrapper.Targetable);
            applyPropertyPatch(unitAttributesPatch.NonAutoTargetable, () => unitAttributesWrapper.NonAutoTargetable);
            applyPropertyPatch(unitAttributesPatch.RetireTargetable, () => unitAttributesWrapper.RetireTargetable);
            applyPropertyPatch(unitAttributesPatch.HackedReturnTargetable, () => unitAttributesWrapper.HackedReturnTargetable);
            applyPropertyPatch(unitAttributesPatch.HackableProperties, () => unitAttributesWrapper.HackableProperties);
            applyPropertyPatch(unitAttributesPatch.ExcludeFromUnitStats, () => unitAttributesWrapper.ExcludeFromUnitStats);
            applyPropertyPatch(unitAttributesPatch.BlocksLOF, () => unitAttributesWrapper.BlocksLOF);
            applyPropertyPatch(unitAttributesPatch.WorldHeightOffset, () => unitAttributesWrapper.WorldHeightOffset, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.DoNotPersist, () => unitAttributesWrapper.DoNotPersist);
            applyPropertyPatch(unitAttributesPatch.LevelBound, () => unitAttributesWrapper.LevelBound);
            applyPropertyPatch(unitAttributesPatch.StartsInHangar, () => unitAttributesWrapper.StartsInHangar);
            applyPropertyPatch(unitAttributesPatch.SensorRadius, () => unitAttributesWrapper.SensorRadius, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.ContactRadius, () => unitAttributesWrapper.ContactRadius, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitAttributesPatch.NumProductionQueues, () => unitAttributesWrapper.NumProductionQueues);
            applyPropertyPatch(unitAttributesPatch.ProductionQueueDepth, () => unitAttributesWrapper.ProductionQueueDepth);
            applyPropertyPatch(unitAttributesPatch.ShowProductionQueues, () => unitAttributesWrapper.ShowProductionQueues);
            applyPropertyPatch(unitAttributesPatch.NoTextNotifications, () => unitAttributesWrapper.NoTextNotifications);
            applyPropertyPatch(unitAttributesPatch.NotificationFlags, () => unitAttributesWrapper.NotificationFlags);
            applyPropertyPatch(unitAttributesPatch.FireRateDisplay, () => unitAttributesWrapper.FireRateDisplay);
            applyPropertyPatch(unitAttributesPatch.PriorityAsTarget, () => unitAttributesWrapper.PriorityAsTarget, x => Fixed64.UnsafeFromDouble(x));

            applyPropertyPatch(unitAttributesPatch.BaseThreat, () => unitAttributesWrapper.ThreatData.BaseThreat);
            applyPropertyPatch(unitAttributesPatch.ThreatTier, () => unitAttributesWrapper.ThreatData.Tier);

            applyPropertyPatch(unitAttributesPatch.ThreatCounters, () => unitAttributesWrapper.ThreatCounters, c => c.Select(x => new ThreatCounter(x)));
            applyPropertyPatch(unitAttributesPatch.ThreatCounteredBys, () => unitAttributesWrapper.ThreatCounteredBys, c => c.Select(x => new ThreatCounter(x)));

            applyPropertyPatch(unitAttributesPatch.Resource1Cost, () => unitAttributesWrapper.Resource1Cost);
            applyPropertyPatch(unitAttributesPatch.Resource2Cost, () => unitAttributesWrapper.Resource2Cost);
        }

        public void ApplyResearchItemAttributesPatch(ResearchItemAttributesPatch researchItemAttributesPatch, ResearchItemAttributesWrapper researchItemAttributesWrapper)
        {
            applyPropertyPatch(researchItemAttributesPatch.TypeOfResearch, () => researchItemAttributesWrapper.TypeOfResearch);
            applyPropertyPatch(researchItemAttributesPatch.IconSpriteName, () => researchItemAttributesWrapper.IconSpriteName);
            applyPropertyPatch(researchItemAttributesPatch.LocalizedResearchTitleStringID, () => researchItemAttributesWrapper.LocalizedResearchTitleStringID);
            applyPropertyPatch(researchItemAttributesPatch.LocalizedShortDescriptionStringID, () => researchItemAttributesWrapper.LocalizedShortDescriptionStringID);
            applyPropertyPatch(researchItemAttributesPatch.LocalizedLongDescriptionStringID, () => researchItemAttributesWrapper.LocalizedLongDescriptionStringID);
            applyPropertyPatch(researchItemAttributesPatch.ResearchTime, () => researchItemAttributesWrapper.ResearchTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(researchItemAttributesPatch.Dependencies, () => researchItemAttributesWrapper.Dependencies);
            applyPropertyPatch(researchItemAttributesPatch.ResearchVOCode, () => researchItemAttributesWrapper.ResearchVOCode);
            applyPropertyPatch(researchItemAttributesPatch.Resource1Cost, () => researchItemAttributesWrapper.Resource1Cost);
            applyPropertyPatch(researchItemAttributesPatch.Resource2Cost, () => researchItemAttributesWrapper.Resource2Cost);
        }

        public void ApplyAbilityAttributesPatch(AbilityAttributesPatch abilityAttributesPatch, AbilityAttributesWrapper abilityAttributesWrapper)
        {
            applyPropertyPatch(abilityAttributesPatch.AbilityType, () => abilityAttributesWrapper.AbilityType);
            applyPropertyPatch(abilityAttributesPatch.TargetingType, () => abilityAttributesWrapper.TargetingType);
            applyPropertyPatch(abilityAttributesPatch.TargetAlignment, () => abilityAttributesWrapper.TargetAlignment);
            applyPropertyPatch(abilityAttributesPatch.AbilityMapTargetLayers, () => abilityAttributesWrapper.AbilityMapTargetLayers);
            applyPropertyPatch(abilityAttributesPatch.GroundAutoTargetAlignment, () => abilityAttributesWrapper.GroundAutoTargetAlignment);
            applyPropertyPatch(abilityAttributesPatch.EdgeOfTargetShapeMinDistance, () => abilityAttributesWrapper.EdgeOfTargetShapeMinDistance, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(abilityAttributesPatch.CasterMovesToTarget, () => abilityAttributesWrapper.CasterMovesToTarget);
            applyPropertyPatch(abilityAttributesPatch.GroupActivationType, () => abilityAttributesWrapper.GroupActivationType);
            applyPropertyPatch(abilityAttributesPatch.StartsRemovedInGameMode, () => abilityAttributesWrapper.StartsRemovedInGameMode);
            applyPropertyPatch(abilityAttributesPatch.CooldownTimeSecs, () => abilityAttributesWrapper.CooldownTimeSecs, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(abilityAttributesPatch.WarmupTimeSecs, () => abilityAttributesWrapper.WarmupTimeSecs, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(abilityAttributesPatch.SharedCooldownChannel, () => abilityAttributesWrapper.SharedCooldownChannel);
            applyPropertyPatch(abilityAttributesPatch.SkipCastOnArrivalConditions, () => abilityAttributesWrapper.SkipCastOnArrivalConditions);
            applyPropertyPatch(abilityAttributesPatch.IsToggleable, () => abilityAttributesWrapper.IsToggleable);
            applyPropertyPatch(abilityAttributesPatch.CastOnDeath, () => abilityAttributesWrapper.CastOnDeath);

            var cost = new CostAttributesWrapper(abilityAttributesWrapper.Cost);
            abilityAttributesWrapper.Cost = cost;

            applyPropertyPatch(abilityAttributesPatch.Resource1Cost, () => cost.Resource1Cost);
            applyPropertyPatch(abilityAttributesPatch.Resource2Cost, () => cost.Resource2Cost);
        }

        public void ApplyStorageAttributesPatch(StorageAttributesPatch storageAttributesPatch, StorageAttributesWrapper storageAttributesWrapper)
        {
            applyPropertyPatch(storageAttributesPatch.LinkToPlayerBank, () => storageAttributesWrapper.LinkToPlayerBank);
            applyPropertyPatch(storageAttributesPatch.IsResourceController, () => storageAttributesWrapper.IsResourceController);

            var loadout = storageAttributesWrapper.InventoryLoadout.ToList();

            foreach (var kvp in storageAttributesPatch.InventoryLoadout)
            {
                var inventoryId = kvp.Key;
                var inventoryPatch = kvp.Value;

                using (logger.BeginScope($"InventoryBinding: {inventoryId}"))
                {
                    var index = loadout.FindIndex(b => b.InventoryID == inventoryId);

                    InventoryAttributesWrapper inventoryAttributesWrapper;

                    if (index < 0)
                    {
                        index = loadout.Count;

                        var name = $"SUBSYSTEM-{storageAttributesWrapper.Name}-{inventoryId}";
                        logger.Log($"(created InventoryAttributes: {name})");

                        inventoryAttributesWrapper = new InventoryAttributesWrapper(
                            name: name,
                            inventoryId: inventoryId
                        );
                    }
                    else
                    {
                        var inventoryBinding = loadout[index];
                        inventoryAttributesWrapper = new InventoryAttributesWrapper(inventoryBinding.InventoryAttributes);
                    }

                    ApplyInventoryAttributesPatch(inventoryPatch, inventoryAttributesWrapper);

                    var newBinding = new InventoryBinding(
                        inventoryID: inventoryId,
                        inventoryBindingIndex: index,
                        inventoryAttributes: inventoryAttributesWrapper
                    );

                    if (index == loadout.Count)
                    {
                        loadout.Add(newBinding);
                    }
                    else
                    {
                        loadout[index] = newBinding;
                    }
                }
            }

            storageAttributesWrapper.InventoryLoadout = loadout.ToArray();
        }

        private void ApplyInventoryAttributesPatch(InventoryAttributesPatch inventoryPatch, InventoryAttributesWrapper inventoryAttributesWrapper)
        {
            applyPropertyPatch(inventoryPatch.Capacity, () => inventoryAttributesWrapper.Capacity);
            applyPropertyPatch(inventoryPatch.HasUnlimitedCapacity, () => inventoryAttributesWrapper.HasUnlimitedCapacity);
            applyPropertyPatch(inventoryPatch.StartingAmount, () => inventoryAttributesWrapper.StartingAmount);
        }

        public void ApplyWeaponAttributesPatch(WeaponAttributesPatch weaponAttributesPatch, WeaponAttributesWrapper weaponAttributesWrapper)
        {
            applyPropertyPatch(weaponAttributesPatch.ExcludeFromAutoTargetAcquisition, () => weaponAttributesWrapper.ExcludeFromAutoTargetAcquisition);
            applyPropertyPatch(weaponAttributesPatch.ExcludeFromAutoFire, () => weaponAttributesWrapper.ExcludeFromAutoFire);
            applyPropertyPatch(weaponAttributesPatch.ExcludeFromHeightAdvantage, () => weaponAttributesWrapper.ExcludeFromHeightAdvantage);
            applyPropertyPatch(weaponAttributesPatch.DamageType, () => weaponAttributesWrapper.DamageType);
            applyPropertyPatch(weaponAttributesPatch.IsTracer, () => weaponAttributesWrapper.IsTracer);
            applyPropertyPatch(weaponAttributesPatch.TracerSpeed, () => weaponAttributesWrapper.TracerSpeed, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.TracerLength, () => weaponAttributesWrapper.TracerLength, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.BaseDamagePerRound, () => weaponAttributesWrapper.BaseDamagePerRound, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.BaseWreckDamagePerRound, () => weaponAttributesWrapper.BaseWreckDamagePerRound, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.FiringRecoil, () => weaponAttributesWrapper.FiringRecoil);
            applyPropertyPatch(weaponAttributesPatch.WindUpTimeMS, () => weaponAttributesWrapper.WindUpTimeMS);
            applyPropertyPatch(weaponAttributesPatch.RateOfFire, () => weaponAttributesWrapper.RateOfFire);
            applyPropertyPatch(weaponAttributesPatch.NumberOfBursts, () => weaponAttributesWrapper.NumberOfBursts);
            applyPropertyPatch(weaponAttributesPatch.DamagePacketsPerShot, () => weaponAttributesWrapper.DamagePacketsPerShot);
            applyPropertyPatch(weaponAttributesPatch.BurstPeriodMinTimeMS, () => weaponAttributesWrapper.BurstPeriodMinTimeMS);
            applyPropertyPatch(weaponAttributesPatch.BurstPeriodMaxTimeMS, () => weaponAttributesWrapper.BurstPeriodMaxTimeMS);
            applyPropertyPatch(weaponAttributesPatch.CooldownTimeMS, () => weaponAttributesWrapper.CooldownTimeMS);
            applyPropertyPatch(weaponAttributesPatch.WindDownTimeMS, () => weaponAttributesWrapper.WindDownTimeMS);
            applyPropertyPatch(weaponAttributesPatch.ReloadTimeMS, () => weaponAttributesWrapper.ReloadTimeMS);
            applyPropertyPatch(weaponAttributesPatch.LineOfSightRequired, () => weaponAttributesWrapper.LineOfSightRequired);
            applyPropertyPatch(weaponAttributesPatch.LeadsTarget, () => weaponAttributesWrapper.LeadsTarget);
            applyPropertyPatch(weaponAttributesPatch.KillSkipsUnitDeathSequence, () => weaponAttributesWrapper.KillSkipsUnitDeathSequence);
            applyPropertyPatch(weaponAttributesPatch.RevealTriggers, () => weaponAttributesWrapper.RevealTriggers);
            applyPropertyPatch(weaponAttributesPatch.UnitStatusAttackingTriggers, () => weaponAttributesWrapper.UnitStatusAttackingTriggers);
            applyPropertyPatch(weaponAttributesPatch.TargetStyle, () => weaponAttributesWrapper.TargetStyle);

            applyWeaponModifiers(weaponAttributesPatch, weaponAttributesWrapper);

            applyPropertyPatch(weaponAttributesPatch.AreaOfEffectFalloffType, () => weaponAttributesWrapper.AreaOfEffectFalloffType);
            applyPropertyPatch(weaponAttributesPatch.AreaOfEffectRadius, () => weaponAttributesWrapper.AreaOfEffectRadius, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.ExcludeWeaponOwnerFromAreaOfEffect, () => weaponAttributesWrapper.ExcludeWeaponOwnerFromAreaOfEffect);
            applyPropertyPatch(weaponAttributesPatch.FriendlyFireDamageScalar, () => weaponAttributesWrapper.FriendlyFireDamageScalar, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.WeaponOwnerFriendlyFireDamageScalar, () => weaponAttributesWrapper.WeaponOwnerFriendlyFireDamageScalar, x => Fixed64.UnsafeFromDouble(x));

            if (weaponAttributesPatch.Turret != null)
            {
                using (logger.BeginScope($"Turret:"))
                {
                    var turret = weaponAttributesWrapper.Turret;
                    var turretWrapper = new TurretAttributesWrapper(turret);

                    ApplyTurretAttributesPatch(weaponAttributesPatch.Turret, turretWrapper);

                    weaponAttributesWrapper.Turret = turretWrapper;
                }
            }

            applyRangeAttributes(WeaponRange.Short, weaponAttributesPatch.RangeAttributesShort, weaponAttributesWrapper);
            applyRangeAttributes(WeaponRange.Medium, weaponAttributesPatch.RangeAttributesMedium, weaponAttributesWrapper);
            applyRangeAttributes(WeaponRange.Long, weaponAttributesPatch.RangeAttributesLong, weaponAttributesWrapper);

            applyPropertyPatch(weaponAttributesPatch.ProjectileEntityTypeToSpawn, () => weaponAttributesWrapper.ProjectileEntityTypeToSpawn);
            applyPropertyPatch(weaponAttributesPatch.StatusEffectsTargetAlignment, () => weaponAttributesWrapper.StatusEffectsTargetAlignment);
            applyPropertyPatch(weaponAttributesPatch.StatusEffectsExcludeTargetType, () => weaponAttributesWrapper.StatusEffectsExcludeTargetType);
            applyPropertyPatch(weaponAttributesPatch.ActiveStatusEffectsIndex, () => weaponAttributesWrapper.ActiveStatusEffectsIndex);

            applyEntityTypesToSpawnOnImpact(weaponAttributesPatch, weaponAttributesWrapper);

            if (weaponAttributesPatch.TargetPrioritizationAttributes != null)
            {
                using (logger.BeginScope($"TargetPrioritizationAttributes:"))
                {
                    var targetPrioritizationWrapper = new TargetPriorizationAttributesWrapper(weaponAttributesWrapper.TargetPriorizationAttributes);

                    ApplyTargetPrioritizationAttributesPatch(weaponAttributesPatch.TargetPrioritizationAttributes, targetPrioritizationWrapper);

                    weaponAttributesWrapper.TargetPriorizationAttributes = targetPrioritizationWrapper;
                }
            }
        }

        private void applyWeaponModifiers(WeaponAttributesPatch weaponAttributesPatch, WeaponAttributesWrapper weaponAttributesWrapper)
        {
            var modifiers = weaponAttributesWrapper.Modifiers.Select(x => new WeaponModifierInfoWrapper(x)).ToList();

            applyListPatch(weaponAttributesPatch.Modifiers, modifiers, () => new WeaponModifierInfoWrapper(), ApplyWeaponModifierInfoPatch, nameof(WeaponModifierInfo));

            weaponAttributesWrapper.Modifiers = modifiers.ToArray();
        }

        public void ApplyWeaponModifierInfoPatch(WeaponModifierInfoPatch weaponModifierInfoPatch, WeaponModifierInfoWrapper weaponModifierInfoWrapper)
        {
            applyPropertyPatch(weaponModifierInfoPatch.TargetClass, () => weaponModifierInfoWrapper.TargetClass);
            applyPropertyPatch(weaponModifierInfoPatch.ClassOperator, () => weaponModifierInfoWrapper.ClassOperator);
            applyPropertyPatch(weaponModifierInfoPatch.Modifier, () => weaponModifierInfoWrapper.Modifier);
            applyPropertyPatch(weaponModifierInfoPatch.Amount, () => weaponModifierInfoWrapper.Amount);
        }

        private void applyEntityTypesToSpawnOnImpact(WeaponAttributesPatch weaponAttributesPatch, WeaponAttributesWrapper weaponAttributesWrapper)
        {
            var wrappers = weaponAttributesWrapper.EntityTypesToSpawnOnImpact.Select(x => new EntityTypeToSpawnAttributesWrapper(x)).ToList();

            applyListPatch(weaponAttributesPatch.EntityTypesToSpawnOnImpact, wrappers, () => new EntityTypeToSpawnAttributesWrapper(), ApplyEntityTypeToSpawnAttributesPatch, nameof(EntityTypeToSpawnAttributes));

            weaponAttributesWrapper.EntityTypesToSpawnOnImpact = wrappers.Where(x => x != null).ToArray();
        }

        public void ApplyEntityTypeToSpawnAttributesPatch(EntityTypeToSpawnAttributesPatch patch, EntityTypeToSpawnAttributesWrapper wrapper)
        {
            applyPropertyPatch(patch.EntityTypeToSpawn, () => wrapper.EntityTypeToSpawn);
            applyPropertyPatch(patch.SpawnRotationOffsetDegrees, () => wrapper.SpawnRotationOffsetDegrees, Fixed64.UnsafeFromDouble);
        }

        private void applyRangeAttributes(WeaponRange weaponRange, RangeBasedWeaponAttributesPatch rangePatch, WeaponAttributesWrapper weaponWrapper)
        {
            if (rangePatch == null) { return; }

            var ranges = weaponWrapper.Ranges;

            var newRanges = ranges.Where(r => r.Range < weaponRange);

            var range = ranges.SingleOrDefault(r => r.Range == weaponRange);

            using (logger.BeginScope($"RangeAttributes{weaponRange}:"))
            {
                if (rangePatch.Remove)
                {
                    logger.Log("(removed)");
                }
                else
                {
                    RangeBasedWeaponAttributesWrapper rangeWrapper;

                    if (range != null)
                    {
                        rangeWrapper = new RangeBasedWeaponAttributesWrapper(range);
                    }
                    else
                    {
                        logger.Log("(created)");
                        rangeWrapper = new RangeBasedWeaponAttributesWrapper(weaponRange);
                    }

                    ApplyRangeBasedWeaponAttributesPatch(rangePatch, rangeWrapper);

                    newRanges = newRanges.Concat(new[] { rangeWrapper });
                }
            }

            newRanges = newRanges.Concat(ranges.Where(r => r.Range > weaponRange));

            weaponWrapper.Ranges = newRanges.ToArray();
        }

        public void ApplyRangeBasedWeaponAttributesPatch(RangeBasedWeaponAttributesPatch rangePatch, RangeBasedWeaponAttributesWrapper rangeWrapper)
        {
            applyPropertyPatch(rangePatch.Accuracy, () => rangeWrapper.Accuracy, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(rangePatch.Distance, () => rangeWrapper.Distance, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(rangePatch.MinDistance, () => rangeWrapper.MinDistance, x => Fixed64.UnsafeFromDouble(x));
        }

        public void ApplyTurretAttributesPatch(TurretAttributesPatch turretPatch, TurretAttributesWrapper turretWrapper)
        {
            applyPropertyPatch(turretPatch.FieldOfFire, () => turretWrapper.FieldOfFire, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(turretPatch.FieldOfView, () => turretWrapper.FieldOfView, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(turretPatch.RotationSpeed, () => turretWrapper.RotationSpeed, x => Fixed64.UnsafeFromDouble(x));
        }

        public void ApplyTargetPrioritizationAttributesPatch(TargetPrioritizationAttributesPatch targetPrioritizationPatch, TargetPriorizationAttributesWrapper targetPrioritizationWrapper)
        {
            applyPropertyPatch(targetPrioritizationPatch.WeaponEffectivenessWeight, () => targetPrioritizationWrapper.WeaponEffectivenessWeight, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.TargetThreatWeight, () => targetPrioritizationWrapper.TargetThreatWeight, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.DistanceWeight, () => targetPrioritizationWrapper.DistanceWeight, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.AngleWeight, () => targetPrioritizationWrapper.AngleWeight, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.TargetPriorityWeight, () => targetPrioritizationWrapper.TargetPriorityWeight, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.AutoTargetStickyBias, () => targetPrioritizationWrapper.AutoTargetStickyBias, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.ManualTargetStickyBias, () => targetPrioritizationWrapper.ManualTargetStickyBias, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.TargetSameCommanderBias, () => targetPrioritizationWrapper.TargetSameCommanderBias, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(targetPrioritizationPatch.TargetWithinFOVBias, () => targetPrioritizationWrapper.TargetWithinFOVBias, x => Fixed64.UnsafeFromDouble(x));
        }
        
        public void ApplyUnitHangarAttributesPatch(UnitHangarAttributesPatch unitHangarPatch, UnitHangarAttributesWrapper unitHangarWrapper)
        {
            applyPropertyPatch(unitHangarPatch.AlignmentTime, () => unitHangarWrapper.AlignmentTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitHangarPatch.ApproachTime, () => unitHangarWrapper.ApproachTime, x => Fixed64.UnsafeFromDouble(x));

            for (var i = 0; i < unitHangarWrapper.HangarBays.Length; i++)
            {
                var hangarBay = unitHangarWrapper.HangarBays[i];
                if (unitHangarPatch.HangarBays.TryGetValue(hangarBay.Name, out var hangarBayPatch))
                {
                    using (logger.BeginScope($"HangarBay: {hangarBay.Name}"))
                    {
                        var hangarBayWrapper = new HangarBayWrapper(hangarBay);
                        ApplyHangarBayPatch(hangarBayPatch, hangarBayWrapper);
                        unitHangarWrapper.HangarBays[i] = hangarBayWrapper;
                    }
                }
            }
        }

        public void ApplyHangarBayPatch(HangarBayPatch hangarBayPatch, HangarBayWrapper hangarBayWrapper)
        {
            applyPropertyPatch(hangarBayPatch.EntityType, () => hangarBayWrapper.EntityType);
            applyPropertyPatch(hangarBayPatch.MaxCount, () => hangarBayWrapper.MaxCount);
            applyPropertyPatch(hangarBayPatch.UsesStrictClassMatching, () => hangarBayWrapper.UsesStrictClassMatching);
            applyPropertyPatch(hangarBayPatch.HoldsClass, () => hangarBayWrapper.HoldsClass);
            applyPropertyPatch(hangarBayPatch.SlotCount, () => hangarBayWrapper.SlotCount);
            applyPropertyPatch(hangarBayPatch.UndockPresCtrlBone, () => hangarBayWrapper.UndockPresCtrlBone);
            applyPropertyPatch(hangarBayPatch.UndockTotalSeconds, () => hangarBayWrapper.UndockTotalSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.UndockAnimationSeconds, () => hangarBayWrapper.UndockAnimationSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.UndockSlotStaggerSeconds, () => hangarBayWrapper.UndockSlotStaggerSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.UndockXOffsetPos, () => hangarBayWrapper.UndockXOffsetPos, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.UndockYOffsetPos, () => hangarBayWrapper.UndockYOffsetPos, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.UndockSlotXSeperationOffset, () => hangarBayWrapper.UndockSlotXSeperationOffset, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.DegreesOffsetUndockAngle, () => hangarBayWrapper.DegreesOffsetUndockAngle, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.UndockSpeed, () => hangarBayWrapper.UndockSpeed, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.DockPresCtrlBone, () => hangarBayWrapper.DockPresCtrlBone);
            applyPropertyPatch(hangarBayPatch.DockBringInAnimationSeconds, () => hangarBayWrapper.DockBringInAnimationSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.DockSlotStaggerSeconds, () => hangarBayWrapper.DockSlotStaggerSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.MaxDamageCoolingSeconds, () => hangarBayWrapper.MaxDamageCoolingSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.MaxPayloadCoolingSeconds, () => hangarBayWrapper.MaxPayloadCoolingSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.MinDockCoolingSeconds, () => hangarBayWrapper.MinDockCoolingSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.DockReceivingXOffset, () => hangarBayWrapper.DockReceivingXOffset, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.DockReceivingYOffset, () => hangarBayWrapper.DockReceivingYOffset, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.DoorAnimationSeconds, () => hangarBayWrapper.DoorAnimationSeconds, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(hangarBayPatch.UndockLiftTime, () => hangarBayWrapper.UndockLiftTime, x => Fixed64.UnsafeFromDouble(x));
        }

        public void ApplyDetectableAttributesPatch(DetectableAttributesPatch detectablePatch, DetectableAttributesWrapper detectableWrapper)
        {
            applyPropertyPatch(detectablePatch.DisplayLastKnownLocation, () => detectableWrapper.DisplayLastKnownLocation);
            applyPropertyPatch(detectablePatch.LastKnownDuration, () => detectableWrapper.LastKnownDuration, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(detectablePatch.TimeVisibleAfterFiring, () => detectableWrapper.TimeVisibleAfterFiring);
            applyPropertyPatch(detectablePatch.AlwaysVisible, () => detectableWrapper.AlwaysVisible);
            applyPropertyPatch(detectablePatch.MinimumStateAfterDetection, () => detectableWrapper.MinimumStateAfterDetection);
            applyPropertyPatch(detectablePatch.FOWFadeDuration, () => detectableWrapper.FOWFadeDuration, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(detectablePatch.SetHasBeenSeenBeforeOnSpawn, () => detectableWrapper.SetHasBeenSeenBeforeOnSpawn);
        }

        public void ApplyUnitMovementAttributesPatch(UnitMovementAttributesPatch unitMovementAttributesPatch, UnitMovementAttributesWrapper unitMovementAttributesWrapper)
        {
            applyPropertyPatch(unitMovementAttributesPatch.DriveType, () => unitMovementAttributesWrapper.DriveType);

            if (unitMovementAttributesPatch.Dynamics != null)
            {
                using (logger.BeginScope($"UnitDynamicsAttributes:"))
                {
                    var unitDynamicsAttributesWrapper = new UnitDynamicsAttributesWrapper(unitMovementAttributesWrapper.Dynamics);
                    ApplyUnitDynamicsAttributesPatch(unitMovementAttributesPatch.Dynamics, unitDynamicsAttributesWrapper);
                    unitMovementAttributesWrapper.Dynamics = unitDynamicsAttributesWrapper;
                }
            }
        }

        private void ApplyUnitDynamicsAttributesPatch(UnitDynamicsAttributesPatch unitDynamicsAttributesPatch, UnitDynamicsAttributesWrapper unitDynamicsAttributesWrapper)
        {
            applyPropertyPatch(unitDynamicsAttributesPatch.DriveType, () => unitDynamicsAttributesWrapper.DriveType);
            applyPropertyPatch(unitDynamicsAttributesPatch.Length, () => unitDynamicsAttributesWrapper.Length, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.Width, () => unitDynamicsAttributesWrapper.Width, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.MaxSpeed, () => unitDynamicsAttributesWrapper.MaxSpeed, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.ReverseFactor, () => unitDynamicsAttributesWrapper.ReverseFactor, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.AccelerationTime, () => unitDynamicsAttributesWrapper.AccelerationTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.BrakingTime, () => unitDynamicsAttributesWrapper.BrakingTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.MaxSpeedTurnRadius, () => unitDynamicsAttributesWrapper.MaxSpeedTurnRadius, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.MaxEaseIntoTurnTime, () => unitDynamicsAttributesWrapper.MaxEaseIntoTurnTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.DriftType, () => unitDynamicsAttributesWrapper.DriftType, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.ReverseDriftMultiplier, () => unitDynamicsAttributesWrapper.ReverseDriftMultiplier, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.DriftOvershootFactor, () => unitDynamicsAttributesWrapper.DriftOvershootFactor, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.FishTailingTimeIntervals, () => unitDynamicsAttributesWrapper.FishTailingTimeIntervals, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.FishTailControlRecover, () => unitDynamicsAttributesWrapper.FishTailControlRecover, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.MinDriftSlipSpeed, () => unitDynamicsAttributesWrapper.MinDriftSlipSpeed, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.MaxDriftRecoverTime, () => unitDynamicsAttributesWrapper.MaxDriftRecoverTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.MinCruiseSpeed, () => unitDynamicsAttributesWrapper.MinCruiseSpeed, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.DeathDriftTime, () => unitDynamicsAttributesWrapper.DeathDriftTime, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(unitDynamicsAttributesPatch.PermanentlyImmobile, () => unitDynamicsAttributesWrapper.PermanentlyImmobile);
        }
    }
}
