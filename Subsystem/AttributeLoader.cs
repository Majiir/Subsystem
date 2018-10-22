using BBI.Core;
using BBI.Core.Data;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using LitJson;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace Subsystem
{
    public class AttributeLoader
    {
        private readonly TextWriter logger = new StringWriter();

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
                if (entityType == null)
                {
                    logger.WriteLine($"NOTICE: EntityType {entityTypeName} Not found");
                    logger.WriteLine();
                    continue;
                }

                logger.WriteLine($"EntityType: {entityTypeName}");
                logger.WriteLine();

                if (entityTypePatch.UnitAttributes != null)
                {
                    logger.WriteLine($"  UnitAttributes:");

                    var unitAttributes = entityType.Get<UnitAttributes>();
                    var unitAttributesWrapper = new UnitAttributesWrapper(unitAttributes);

                    ApplyUnitAttributesPatch(entityTypePatch.UnitAttributes, unitAttributesWrapper);

                    logger.WriteLine();

                    entityType.Replace(unitAttributes, unitAttributesWrapper);
                }

                if (entityTypePatch.ResearchItemAttributes != null)
                {
                    logger.WriteLine($"  ResearchItemAttributes:");

                    var researchItemAttributes = entityType.Get<ResearchItemAttributes>();
                    var researchItemAttributesWrapper = new ResearchItemAttributesWrapper(researchItemAttributes);

                    ApplyResearchItemAttributesPatch(entityTypePatch.ResearchItemAttributes, researchItemAttributesWrapper);

                    logger.WriteLine();

                    entityType.Replace(researchItemAttributes, researchItemAttributesWrapper);
                }

                foreach (var kvp2 in entityTypePatch.AbilityAttributes)
                {
                    var abilityAttributesName = kvp2.Key;
                    var abilityAttributesPatch = kvp2.Value;

                    logger.WriteLine($"  AbilityAttributes: {abilityAttributesName}");

                    var abilityAttributes = entityType.Get<AbilityAttributes>(abilityAttributesName);
                    if (abilityAttributes == null)
                    {
                        logger.WriteLine($"ERROR: Ability name {abilityAttributesName} not found");
                        logger.WriteLine();
                        continue;
                    }

                    var abilityAttributesWrapper = new AbilityAttributesWrapper(abilityAttributes);

                    ApplyAbilityAttributesPatch(abilityAttributesPatch, abilityAttributesWrapper);

                    logger.WriteLine();

                    entityType.Replace(abilityAttributes, abilityAttributesWrapper);
                }

                foreach (var kvp2 in entityTypePatch.WeaponAttributes)
                {
                    var weaponAttributesName = kvp2.Key;
                    var weaponAttributesPatch = kvp2.Value;

                    logger.WriteLine($"  WeaponAttributes: {weaponAttributesName}");

                    var weaponAttributes = entityType.Get<WeaponAttributes>(weaponAttributesName);
                    if (weaponAttributes == null)
                    {
                        logger.WriteLine($"ERROR: Weapon name {weaponAttributesName} not found");
                        logger.WriteLine();
                        continue;
                    }

                    var weaponAttributesWrapper = new WeaponAttributesWrapper(weaponAttributes);

                    ApplyWeaponAttributesPatch(weaponAttributesPatch, weaponAttributesWrapper);

                    logger.WriteLine();

                    entityType.Replace(weaponAttributes, weaponAttributesWrapper);

                    rebindWeaponAttributes(entityType, weaponAttributesWrapper);
                }
            }

            Debug.Log($"[SUBSYSTEM] Applied attributes patch:\n\n{logger.ToString()}");
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

            logger.WriteLine($"    {accessor.Name}: {value} (was: {oldValue})");
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
            applyPropertyPatch(weaponAttributesPatch.AreaOfEffectFalloffType, () => weaponAttributesWrapper.AreaOfEffectFalloffType);
            applyPropertyPatch(weaponAttributesPatch.AreaOfEffectRadius, () => weaponAttributesWrapper.AreaOfEffectRadius, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.ExcludeWeaponOwnerFromAreaOfEffect, () => weaponAttributesWrapper.ExcludeWeaponOwnerFromAreaOfEffect);
            applyPropertyPatch(weaponAttributesPatch.FriendlyFireDamageScalar, () => weaponAttributesWrapper.FriendlyFireDamageScalar, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(weaponAttributesPatch.WeaponOwnerFriendlyFireDamageScalar, () => weaponAttributesWrapper.WeaponOwnerFriendlyFireDamageScalar, x => Fixed64.UnsafeFromDouble(x));

            applyRangeAttributes(WeaponRange.Short, weaponAttributesPatch.RangeAttributesShort, weaponAttributesWrapper);
            applyRangeAttributes(WeaponRange.Medium, weaponAttributesPatch.RangeAttributesMedium, weaponAttributesWrapper);
            applyRangeAttributes(WeaponRange.Long, weaponAttributesPatch.RangeAttributesLong, weaponAttributesWrapper);

            applyPropertyPatch(weaponAttributesPatch.ProjectileEntityTypeToSpawn, () => weaponAttributesWrapper.ProjectileEntityTypeToSpawn);
            applyPropertyPatch(weaponAttributesPatch.StatusEffectsTargetAlignment, () => weaponAttributesWrapper.StatusEffectsTargetAlignment);
            applyPropertyPatch(weaponAttributesPatch.StatusEffectsExcludeTargetType, () => weaponAttributesWrapper.StatusEffectsExcludeTargetType);
            applyPropertyPatch(weaponAttributesPatch.ActiveStatusEffectsIndex, () => weaponAttributesWrapper.ActiveStatusEffectsIndex);
        }

        private void applyRangeAttributes(WeaponRange weaponRange, RangeBasedWeaponAttributesPatch rangePatch, WeaponAttributesWrapper weaponWrapper)
        {
            if (rangePatch == null) { return; }

            var ranges = weaponWrapper.Ranges;

            var range = ranges.SingleOrDefault(r => r.Range == weaponRange);

            var rangeWrapper = range != null
                ? new RangeBasedWeaponAttributesWrapper(range)
                : new RangeBasedWeaponAttributesWrapper(weaponRange);

            ApplyRangeBasedWeaponAttributesPatch(rangePatch, rangeWrapper);

            weaponWrapper.Ranges =
                ranges.Where(r => r.Range < weaponRange)
                .Concat(new[] { rangeWrapper })
                .Concat(ranges.Where(r => r.Range > weaponRange))
                .ToArray();
        }

        public void ApplyRangeBasedWeaponAttributesPatch(RangeBasedWeaponAttributesPatch rangePatch, RangeBasedWeaponAttributesWrapper rangeWrapper)
        {
            applyPropertyPatch(rangePatch.Accuracy, () => rangeWrapper.Accuracy, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(rangePatch.Distance, () => rangeWrapper.Distance, x => Fixed64.UnsafeFromDouble(x));
            applyPropertyPatch(rangePatch.MinDistance, () => rangeWrapper.MinDistance, x => Fixed64.UnsafeFromDouble(x));
        }
    }
}
