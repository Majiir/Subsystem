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
        private readonly StringLogger logger = new StringLogger();

        public static void LoadAttributes(EntityTypeCollection entityTypeCollection)
        {
            try
            {
                var jsonPath = Path.Combine(Application.dataPath, "patch.json");
                var json = File.ReadAllText(jsonPath);

                var attributesPatch = JsonMapper.ToObject<AttributesPatch>(json);

                var loader = new AttributeLoader();
                loader.ApplyAttributesPatch(entityTypeCollection, attributesPatch);
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

                    if (entityTypePatch.ExperienceAttributes != null)
                    {
                        using (logger.BeginScope($"ExperienceAttributes:"))
                        {
                            var experienceAttributes = entityType.Get<ExperienceAttributes>();
                            var experienceAttributesWrapper = new ExperienceAttributesWrapper(experienceAttributes);

                            ApplyExperienceAttributesPatch(entityTypePatch.ExperienceAttributes, experienceAttributesWrapper);

                            entityType.Replace(experienceAttributes, experienceAttributesWrapper);
                        }
                    }

                    if (entityTypePatch.UnitAttributes != null)
                    {
                        using (logger.BeginScope($"UnitAttributes:"))
                        {
                            var unitAttributes = entityType.Get<UnitAttributes>();
                            var unitAttributesWrapper = new UnitAttributesWrapper(unitAttributes);

                            ApplyUnitAttributesPatch(entityTypePatch.UnitAttributes, unitAttributesWrapper);

                            entityType.Replace(unitAttributes, unitAttributesWrapper);
                        }
                    }

                    if (entityTypePatch.ResearchItemAttributes != null)
                    {
                        using (logger.BeginScope($"ResearchItemAttributes:"))
                        {
                            var researchItemAttributes = entityType.Get<ResearchItemAttributes>();
                            var researchItemAttributesWrapper = new ResearchItemAttributesWrapper(researchItemAttributes);

                            ApplyResearchItemAttributesPatch(entityTypePatch.ResearchItemAttributes, researchItemAttributesWrapper);

                            entityType.Replace(researchItemAttributes, researchItemAttributesWrapper);
                        }
                    }

                    if (entityTypePatch.UnitHangarAttributes != null)
                    {
                        using (logger.BeginScope($"UnitHangarAttributes:"))
                        {
                            var unitHangarAttributes = entityType.Get<UnitHangarAttributes>();
                            var unitHangarAttributesWrapper = new UnitHangarAttributesWrapper(unitHangarAttributes);

                            ApplyUnitHangarAttributesPatch(entityTypePatch.UnitHangarAttributes, unitHangarAttributesWrapper);

                            entityType.Replace(unitHangarAttributes, unitHangarAttributesWrapper);
                        }
                    }

                    if (entityTypePatch.UnitMovementAttributes != null)
                    {
                        using (logger.BeginScope($"UnitMovementAttributes:"))
                        {
                            var unitMovementAttributes = entityType.Get<UnitMovementAttributes>();
                            var unitMovementAttributesWrapper = new UnitMovementAttributesWrapper(unitMovementAttributes);

                            ApplyUnitMovementAttributesPatch(entityTypePatch.UnitMovementAttributes, unitMovementAttributesWrapper);

                            entityType.Replace(unitMovementAttributes, unitMovementAttributesWrapper);
                        }
                    }

                    foreach (var kvp2 in entityTypePatch.AbilityAttributes)
                    {
                        var abilityAttributesName = kvp2.Key;
                        var abilityAttributesPatch = kvp2.Value;

                        using (logger.BeginScope($"AbilityAttributes: {abilityAttributesName}"))
                        {
                            var abilityAttributes = entityType.Get<AbilityAttributes>(abilityAttributesName);
                            if (abilityAttributes == null)
                            {
                                logger.Log($"ERROR: AbilityAttributes not found");
                                continue;
                            }

                            var abilityAttributesWrapper = new AbilityAttributesWrapper(abilityAttributes);

                            ApplyAbilityAttributesPatch(abilityAttributesPatch, abilityAttributesWrapper);

                            entityType.Replace(abilityAttributes, abilityAttributesWrapper);
                        }
                    }

                    foreach (var kvp2 in entityTypePatch.StorageAttributes)
                    {
                        var storageAttributesName = kvp2.Key;
                        var storageAttributesPatch = kvp2.Value;

                        using (logger.BeginScope($"StorageAttributes: {storageAttributesName}"))
                        {
                            var storageAttributes = entityType.Get<StorageAttributes>(storageAttributesName);
                            if (storageAttributes == null)
                            {
                                logger.Log($"ERROR: StorageAttributes not found");
                                continue;
                            }

                            var storageAttributesWrapper = new StorageAttributesWrapper(storageAttributes);

                            ApplyStorageAttributesPatch(storageAttributesPatch, storageAttributesWrapper);

                            entityType.Replace(storageAttributes, storageAttributesWrapper);
                        }
                    }

                    foreach (var kvp2 in entityTypePatch.WeaponAttributes)
                    {
                        var weaponAttributesName = kvp2.Key;
                        var weaponAttributesPatch = kvp2.Value;

                        using (logger.BeginScope($"WeaponAttributes: {weaponAttributesName}"))
                        {
                            var weaponAttributes = entityType.Get<WeaponAttributes>(weaponAttributesName);
                            if (weaponAttributes == null)
                            {
                                logger.Log($"ERROR: WeaponAttributes not found");
                                continue;
                            }

                            var weaponAttributesWrapper = new WeaponAttributesWrapper(weaponAttributes);

                            ApplyWeaponAttributesPatch(weaponAttributesPatch, weaponAttributesWrapper);

                            entityType.Replace(weaponAttributes, weaponAttributesWrapper);

                            rebindWeaponAttributes(entityType, weaponAttributesWrapper);
                        }
                    }
                }
            }

            File.WriteAllText(Path.Combine(Application.dataPath, "Subsystem.log"), logger.GetLog());
            Debug.Log($"[SUBSYSTEM] Applied attributes patch. See Subsystem.log for details.");
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

        private void applyExperienceLevelsPatch(Dictionary<string, ExperienceLevelAttributesPatch> patch, List<ExperienceLevelAttributesWrapper> wrappers)
        {
            foreach (var kvp in patch.OrderBy(x => x.Key))
            {
                if (!int.TryParse(kvp.Key, out var index))
                {
                    logger.Log($"ERROR: Non-integer key: {kvp.Key}");
                    break;
                }

                var elementPatch = kvp.Value;

                using (logger.BeginScope($"ExperienceLevelAttributes: {index}"))
                {
                    if (index < wrappers.Count)
                    {
                        if (elementPatch.Remove)
                        {
                            logger.Log("(removed)");
                            wrappers[index] = null;
                            continue;
                        }

                        var elementWrapper = wrappers[index];

                        ApplyExperienceLevelAttributesPatch(elementPatch, elementWrapper);

                        wrappers[index] = elementWrapper;
                    }
                    else if (index == wrappers.Count)
                    {
                        if (elementPatch.Remove)
                        {
                            logger.Log("WARNING: Remove flag set for non-existent entry");
                            continue;
                        }

                        logger.Log("(created)");
                        var elementWrapper = new ExperienceLevelAttributesWrapper();

                        ApplyExperienceLevelAttributesPatch(elementPatch, elementWrapper);

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
            foreach (var kvp in patch.OrderBy(x => x.Key))
            {
                if (!int.TryParse(kvp.Key, out var index))
                {
                    logger.Log($"ERROR: Non-integer buff key: {kvp.Key}");
                    break;
                }

                var buffPatch = kvp.Value;

                using (logger.BeginScope($"AttributeBuff: {index}"))
                {
                    if (index < wrapper.Buffs.Count)
                    {
                        if (buffPatch.Remove)
                        {
                            logger.Log("(removed)");
                            wrapper.Buffs[index] = null;
                            continue;
                        }

                        var buffWrapper = wrapper.Buffs[index];

                        applyAttributeBuffPatch(buffPatch, buffWrapper);

                        wrapper.Buffs[index] = buffWrapper;
                    }
                    else if (index == wrapper.Buffs.Count)
                    {
                        if (buffPatch.Remove)
                        {
                            logger.Log("WARNING: Remove flag set for non-existant entry");
                            continue;
                        }

                        logger.Log("(created)");
                        var buffWrapper = new AttributeBuffWrapper();

                        applyAttributeBuffPatch(buffPatch, buffWrapper);

                        wrapper.Buffs.Add(buffWrapper);
                    }
                    else // if (index > wrapper.Buffs.Count)
                    {
                        logger.Log("ERROR: Non-consecutive index");
                        continue;
                    }
                }
            }

            wrapper.Buffs.RemoveAll(x => x == null);
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

                    applyPropertyPatch(inventoryPatch.Capacity, () => inventoryAttributesWrapper.Capacity);
                    applyPropertyPatch(inventoryPatch.HasUnlimitedCapacity, () => inventoryAttributesWrapper.HasUnlimitedCapacity);
                    applyPropertyPatch(inventoryPatch.StartingAmount, () => inventoryAttributesWrapper.StartingAmount);

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
        }

        private void applyWeaponModifiers(WeaponAttributesPatch weaponAttributesPatch, WeaponAttributesWrapper weaponAttributesWrapper)
        {
            var modifiers = weaponAttributesWrapper.Modifiers.ToList();

            foreach (var kvp in weaponAttributesPatch.Modifiers.OrderBy(x => x.Key))
            {
                if (!int.TryParse(kvp.Key, out var index))
                {
                    logger.Log($"ERROR: Non-integer modifier key: {kvp.Key}");
                    break;
                }

                var modifierPatch = kvp.Value;

                using (logger.BeginScope($"WeaponModifierInfo: {index}"))
                {
                    if (index < modifiers.Count)
                    {
                        if (modifierPatch.Remove)
                        {
                            logger.Log("(removed)");
                            modifiers[index] = null;
                            continue;
                        }

                        var modifierWrapper = new WeaponModifierInfoWrapper(modifiers[index]);

                        ApplyWeaponModifierInfoPatch(modifierPatch, modifierWrapper);

                        modifiers[index] = modifierWrapper;
                    }
                    else if (index == modifiers.Count)
                    {
                        if (modifierPatch.Remove)
                        {
                            logger.Log("WARNING: Remove flag set for non-existant entry");
                            continue;
                        }

                        logger.Log("(created)");
                        var modifierWrapper = new WeaponModifierInfoWrapper();

                        ApplyWeaponModifierInfoPatch(modifierPatch, modifierWrapper);

                        modifiers.Add(modifierWrapper);
                    }
                    else // if (index > modifiers.Count)
                    {
                        logger.Log("ERROR: Non-consecutive index");
                        continue;
                    }
                }
            }

            weaponAttributesWrapper.Modifiers = modifiers.Where(x => x != null).ToArray();
        }

        public void ApplyWeaponModifierInfoPatch(WeaponModifierInfoPatch weaponModifierInfoPatch, WeaponModifierInfoWrapper weaponModifierInfoWrapper)
        {
            applyPropertyPatch(weaponModifierInfoPatch.TargetClass, () => weaponModifierInfoWrapper.TargetClass);
            applyPropertyPatch(weaponModifierInfoPatch.ClassOperator, () => weaponModifierInfoWrapper.ClassOperator);
            applyPropertyPatch(weaponModifierInfoPatch.Modifier, () => weaponModifierInfoWrapper.Modifier);
            applyPropertyPatch(weaponModifierInfoPatch.Amount, () => weaponModifierInfoWrapper.Amount);
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
