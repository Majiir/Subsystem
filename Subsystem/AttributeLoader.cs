using BBI.Core;
using BBI.Core.Data;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using LitJson;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Subsystem
{
    public class AttributeLoader
    {
        public static void LoadAttributes(EntityTypeCollection entityTypeCollection)
        {
            var jsonPath = Path.Combine(Application.dataPath, "patch.json");
            var json = File.ReadAllText(jsonPath);

            var attributesPatch = JsonMapper.ToObject<AttributesPatch>(json);

            ApplyAttributesPatch(entityTypeCollection, attributesPatch);
        }

        public static void ApplyAttributesPatch(EntityTypeCollection entityTypeCollection, AttributesPatch attributesPatch)
        {
            foreach (var kvp in attributesPatch.Entities)
            {
                var entityTypeName = kvp.Key;
                var entityTypePatch = kvp.Value;

                var entityType = entityTypeCollection.GetEntityType(entityTypeName);

                if (entityTypePatch.UnitAttributes != null)
                {
                    var unitAttributes = entityType.Get<UnitAttributes>();
                    var unitAttributesWrapper = new UnitAttributesWrapper(unitAttributes);

                    ApplyUnitAttributesPatch(entityTypePatch.UnitAttributes, unitAttributesWrapper);

                    entityType.Replace(unitAttributes, unitAttributesWrapper);
                }

                if (entityTypePatch.ResearchItemAttributes != null)
                {
                    var researchItemAttributes = entityType.Get<ResearchItemAttributes>();
                    var researchItemAttributesWrapper = new ResearchItemAttributesWrapper(researchItemAttributes);

                    ApplyResearchItemAttributesPatch(entityTypePatch.ResearchItemAttributes, researchItemAttributesWrapper);

                    entityType.Replace(researchItemAttributes, researchItemAttributesWrapper);
                }

                foreach (var kvp2 in entityTypePatch.AbilityAttributes)
                {
                    var abilityAttributesName = kvp2.Key;
                    var abilityAttributesPatch = kvp2.Value;

                    var abilityAttributes = entityType.Get<AbilityAttributes>(abilityAttributesName);
                    var abilityAttributesWrapper = new AbilityAttributesWrapper(abilityAttributes);

                    ApplyAbilityAttributesPatch(abilityAttributesPatch, abilityAttributesWrapper);

                    entityType.Replace(abilityAttributes, abilityAttributesWrapper);
                }

                foreach (var kvp2 in entityTypePatch.WeaponAttributes)
                {
                    var weaponAttributesName = kvp2.Key;
                    var weaponAttributesPatch = kvp2.Value;

                    var weaponAttributes = entityType.Get<WeaponAttributes>(weaponAttributesName);
                    var weaponAttributesWrapper = new WeaponAttributesWrapper(weaponAttributes);

                    ApplyWeaponAttributesPatch(weaponAttributesPatch, weaponAttributesWrapper);

                    entityType.Replace(weaponAttributes, weaponAttributesWrapper);

                    rebindWeaponAttributes(entityType, weaponAttributesWrapper);
                }
            }
        }

        private static void rebindWeaponAttributes(EntityTypeAttributes entityType, WeaponAttributesWrapper weaponAttributesWrapper)
        {
            var unitAttributes = entityType.Get<UnitAttributes>();
            if (unitAttributes != null)
            {
                var unitAttributesWrapper = new UnitAttributesWrapper(unitAttributes);

                var weaponLoadout = unitAttributesWrapper.WeaponLoadout.Select(weaponBinding =>
                    new WeaponBinding(
                        weaponID: weaponBinding.WeaponID,
                        weaponBindingIndex: weaponBinding.WeaponBindingIndex,
                        weapon: weaponAttributesWrapper,
                        ammoID: weaponBinding.AmmoID,
                        turretIndex: weaponBinding.TurretIndex,
                        defaultTurretAngleOffsetRadians: weaponBinding.DefaultTurretAngleOffsetRadians,
                        disabledOnSpawn: weaponBinding.DisabledOnSpawn,
                        weaponOffsetFromUnitOrigin: weaponBinding.OffsetFromUnitCenterInLocalSpace,
                        showAmmoOnHUD: weaponBinding.ShowAmmoOnHUD
                    )
                );

                unitAttributesWrapper.WeaponLoadout = weaponLoadout.ToArray();

                entityType.Replace(unitAttributes, unitAttributesWrapper);
            }
        }

        public static void ApplyUnitAttributesPatch(UnitAttributesPatch unitAttributesPatch, UnitAttributesWrapper unitAttributesWrapper)
        {
            if (unitAttributesPatch.Class.HasValue) { unitAttributesWrapper.Class = unitAttributesPatch.Class.Value; }
            if (unitAttributesPatch.SelectionFlags.HasValue) { unitAttributesWrapper.SelectionFlags = unitAttributesPatch.SelectionFlags.Value; }
            if (unitAttributesPatch.MaxHealth.HasValue) { unitAttributesWrapper.MaxHealth = unitAttributesPatch.MaxHealth.Value; }
            if (unitAttributesPatch.Armour.HasValue) { unitAttributesWrapper.Armour = unitAttributesPatch.Armour.Value; }
            if (unitAttributesPatch.DamageReceivedMultiplier.HasValue) { unitAttributesWrapper.DamageReceivedMultiplier = Fixed64.UnsafeFromDouble(unitAttributesPatch.DamageReceivedMultiplier.Value); }
            if (unitAttributesPatch.AccuracyReceivedMultiplier.HasValue) { unitAttributesWrapper.AccuracyReceivedMultiplier = Fixed64.UnsafeFromDouble(unitAttributesPatch.AccuracyReceivedMultiplier.Value); }
            if (unitAttributesPatch.PopCapCost.HasValue) { unitAttributesWrapper.PopCapCost = unitAttributesPatch.PopCapCost.Value; }
            if (unitAttributesPatch.ExperienceValue.HasValue) { unitAttributesWrapper.ExperienceValue = unitAttributesPatch.ExperienceValue.Value; }
            if (unitAttributesPatch.ProductionTime.HasValue) { unitAttributesWrapper.ProductionTime = Fixed64.UnsafeFromDouble(unitAttributesPatch.ProductionTime.Value); }
            if (unitAttributesPatch.AggroRange.HasValue) { unitAttributesWrapper.AggroRange = Fixed64.UnsafeFromDouble(unitAttributesPatch.AggroRange.Value); }
            if (unitAttributesPatch.LeashRange.HasValue) { unitAttributesWrapper.LeashRange = Fixed64.UnsafeFromDouble(unitAttributesPatch.LeashRange.Value); }
            if (unitAttributesPatch.AlertRange.HasValue) { unitAttributesWrapper.AlertRange = Fixed64.UnsafeFromDouble(unitAttributesPatch.AlertRange.Value); }
            if (unitAttributesPatch.RepairPickupRange.HasValue) { unitAttributesWrapper.RepairPickupRange = Fixed64.UnsafeFromDouble(unitAttributesPatch.RepairPickupRange.Value); }
            if (unitAttributesPatch.UnitPositionReaggroConditions.HasValue) { unitAttributesWrapper.UnitPositionReaggroConditions = unitAttributesPatch.UnitPositionReaggroConditions.Value; }
            if (unitAttributesPatch.LeashPositionReaggroConditions.HasValue) { unitAttributesWrapper.LeashPositionReaggroConditions = unitAttributesPatch.LeashPositionReaggroConditions.Value; }
            if (unitAttributesPatch.LeadPriority.HasValue) { unitAttributesWrapper.LeadPriority = unitAttributesPatch.LeadPriority.Value; }
            if (unitAttributesPatch.Selectable.HasValue) { unitAttributesWrapper.Selectable = unitAttributesPatch.Selectable.Value; }
            if (unitAttributesPatch.Controllable.HasValue) { unitAttributesWrapper.Controllable = unitAttributesPatch.Controllable.Value; }
            if (unitAttributesPatch.Targetable.HasValue) { unitAttributesWrapper.Targetable = unitAttributesPatch.Targetable.Value; }
            if (unitAttributesPatch.NonAutoTargetable.HasValue) { unitAttributesWrapper.NonAutoTargetable = unitAttributesPatch.NonAutoTargetable.Value; }
            if (unitAttributesPatch.RetireTargetable.HasValue) { unitAttributesWrapper.RetireTargetable = unitAttributesPatch.RetireTargetable.Value; }
            if (unitAttributesPatch.HackedReturnTargetable.HasValue) { unitAttributesWrapper.HackedReturnTargetable = unitAttributesPatch.HackedReturnTargetable.Value; }
            if (unitAttributesPatch.HackableProperties.HasValue) { unitAttributesWrapper.HackableProperties = unitAttributesPatch.HackableProperties.Value; }
            if (unitAttributesPatch.ExcludeFromUnitStats.HasValue) { unitAttributesWrapper.ExcludeFromUnitStats = unitAttributesPatch.ExcludeFromUnitStats.Value; }
            if (unitAttributesPatch.BlocksLOF.HasValue) { unitAttributesWrapper.BlocksLOF = unitAttributesPatch.BlocksLOF.Value; }
            if (unitAttributesPatch.WorldHeightOffset.HasValue) { unitAttributesWrapper.WorldHeightOffset = Fixed64.UnsafeFromDouble(unitAttributesPatch.WorldHeightOffset.Value); }
            if (unitAttributesPatch.DoNotPersist.HasValue) { unitAttributesWrapper.DoNotPersist = unitAttributesPatch.DoNotPersist.Value; }
            if (unitAttributesPatch.LevelBound.HasValue) { unitAttributesWrapper.LevelBound = unitAttributesPatch.LevelBound.Value; }
            if (unitAttributesPatch.StartsInHangar.HasValue) { unitAttributesWrapper.StartsInHangar = unitAttributesPatch.StartsInHangar.Value; }
            if (unitAttributesPatch.SensorRadius.HasValue) { unitAttributesWrapper.SensorRadius = Fixed64.UnsafeFromDouble(unitAttributesPatch.SensorRadius.Value); }
            if (unitAttributesPatch.ContactRadius.HasValue) { unitAttributesWrapper.ContactRadius = Fixed64.UnsafeFromDouble(unitAttributesPatch.ContactRadius.Value); }
            if (unitAttributesPatch.NumProductionQueues.HasValue) { unitAttributesWrapper.NumProductionQueues = unitAttributesPatch.NumProductionQueues.Value; }
            if (unitAttributesPatch.ProductionQueueDepth.HasValue) { unitAttributesWrapper.ProductionQueueDepth = unitAttributesPatch.ProductionQueueDepth.Value; }
            if (unitAttributesPatch.ShowProductionQueues.HasValue) { unitAttributesWrapper.ShowProductionQueues = unitAttributesPatch.ShowProductionQueues.Value; }
            if (unitAttributesPatch.NoTextNotifications.HasValue) { unitAttributesWrapper.NoTextNotifications = unitAttributesPatch.NoTextNotifications.Value; }
            if (unitAttributesPatch.NotificationFlags.HasValue) { unitAttributesWrapper.NotificationFlags = unitAttributesPatch.NotificationFlags.Value; }
            if (unitAttributesPatch.FireRateDisplay.HasValue) { unitAttributesWrapper.FireRateDisplay = unitAttributesPatch.FireRateDisplay.Value; }
            if (unitAttributesPatch.PriorityAsTarget.HasValue) { unitAttributesWrapper.PriorityAsTarget = Fixed64.UnsafeFromDouble(unitAttributesPatch.PriorityAsTarget.Value); }

            unitAttributesWrapper.ThreatData = new ThreatData
            {
                BaseThreat = unitAttributesPatch.BaseThreat ?? unitAttributesWrapper.ThreatData.BaseThreat,
                Tier = unitAttributesPatch.ThreatTier ?? unitAttributesWrapper.ThreatData.Tier,
            };

            if (unitAttributesPatch.ThreatCounters != null) { unitAttributesWrapper.ThreatCounters = unitAttributesPatch.ThreatCounters.Select(x => new ThreatCounter(x)); }
            if (unitAttributesPatch.ThreatCounteredBys != null) { unitAttributesWrapper.ThreatCounteredBys = unitAttributesPatch.ThreatCounteredBys.Select(x => new ThreatCounter(x)); }

            if (unitAttributesPatch.Resource1Cost.HasValue) { unitAttributesWrapper.Resource1Cost = unitAttributesPatch.Resource1Cost.Value; }
            if (unitAttributesPatch.Resource2Cost.HasValue) { unitAttributesWrapper.Resource2Cost = unitAttributesPatch.Resource2Cost.Value; }
        }

        public static void ApplyResearchItemAttributesPatch(ResearchItemAttributesPatch researchItemAttributesPatch, ResearchItemAttributesWrapper researchItemAttributesWrapper)
        {
            if (researchItemAttributesPatch.TypeOfResearch.HasValue) { researchItemAttributesWrapper.TypeOfResearch = researchItemAttributesPatch.TypeOfResearch.Value; }
            if (researchItemAttributesPatch.IconSpriteName != null) { researchItemAttributesWrapper.IconSpriteName = researchItemAttributesPatch.IconSpriteName; }
            if (researchItemAttributesPatch.LocalizedResearchTitleStringID != null) { researchItemAttributesWrapper.LocalizedResearchTitleStringID = researchItemAttributesPatch.LocalizedResearchTitleStringID; }
            if (researchItemAttributesPatch.LocalizedShortDescriptionStringID != null) { researchItemAttributesWrapper.LocalizedShortDescriptionStringID = researchItemAttributesPatch.LocalizedShortDescriptionStringID; }
            if (researchItemAttributesPatch.LocalizedLongDescriptionStringID != null) { researchItemAttributesWrapper.LocalizedLongDescriptionStringID = researchItemAttributesPatch.LocalizedLongDescriptionStringID; }
            if (researchItemAttributesPatch.ResearchTime.HasValue) { researchItemAttributesWrapper.ResearchTime = Fixed64.UnsafeFromDouble(researchItemAttributesPatch.ResearchTime.Value); }
            if (researchItemAttributesPatch.Dependencies != null) { researchItemAttributesWrapper.Dependencies = researchItemAttributesPatch.Dependencies; }
            if (researchItemAttributesPatch.ResearchVOCode != null) { researchItemAttributesWrapper.ResearchVOCode = researchItemAttributesPatch.ResearchVOCode; }
            if (researchItemAttributesPatch.Resource1Cost.HasValue) { researchItemAttributesWrapper.Resource1Cost = researchItemAttributesPatch.Resource1Cost.Value; }
            if (researchItemAttributesPatch.Resource2Cost.HasValue) { researchItemAttributesWrapper.Resource2Cost = researchItemAttributesPatch.Resource2Cost.Value; }
        }

        public static void ApplyAbilityAttributesPatch(AbilityAttributesPatch abilityAttributesPatch, AbilityAttributesWrapper abilityAttributesWrapper)
        {
            if (abilityAttributesPatch.AbilityType.HasValue) { abilityAttributesWrapper.AbilityType = abilityAttributesPatch.AbilityType.Value; }
            if (abilityAttributesPatch.TargetingType.HasValue) { abilityAttributesWrapper.TargetingType = abilityAttributesPatch.TargetingType.Value; }
            if (abilityAttributesPatch.TargetAlignment.HasValue) { abilityAttributesWrapper.TargetAlignment = abilityAttributesPatch.TargetAlignment.Value; }
            if (abilityAttributesPatch.AbilityMapTargetLayers.HasValue) { abilityAttributesWrapper.AbilityMapTargetLayers = abilityAttributesPatch.AbilityMapTargetLayers.Value; }
            if (abilityAttributesPatch.GroundAutoTargetAlignment.HasValue) { abilityAttributesWrapper.GroundAutoTargetAlignment = abilityAttributesPatch.GroundAutoTargetAlignment.Value; }
            if (abilityAttributesPatch.EdgeOfTargetShapeMinDistance.HasValue) { abilityAttributesWrapper.EdgeOfTargetShapeMinDistance = Fixed64.UnsafeFromDouble(abilityAttributesPatch.EdgeOfTargetShapeMinDistance.Value); }
            if (abilityAttributesPatch.CasterMovesToTarget.HasValue) { abilityAttributesWrapper.CasterMovesToTarget = abilityAttributesPatch.CasterMovesToTarget.Value; }
            if (abilityAttributesPatch.GroupActivationType.HasValue) { abilityAttributesWrapper.GroupActivationType = abilityAttributesPatch.GroupActivationType.Value; }
            if (abilityAttributesPatch.StartsRemovedInGameMode.HasValue) { abilityAttributesWrapper.StartsRemovedInGameMode = abilityAttributesPatch.StartsRemovedInGameMode.Value; }
            if (abilityAttributesPatch.CooldownTimeSecs.HasValue) { abilityAttributesWrapper.CooldownTimeSecs = Fixed64.UnsafeFromDouble(abilityAttributesPatch.CooldownTimeSecs.Value); }
            if (abilityAttributesPatch.WarmupTimeSecs.HasValue) { abilityAttributesWrapper.WarmupTimeSecs = Fixed64.UnsafeFromDouble(abilityAttributesPatch.WarmupTimeSecs.Value); }
            if (abilityAttributesPatch.SharedCooldownChannel.HasValue) { abilityAttributesWrapper.SharedCooldownChannel = abilityAttributesPatch.SharedCooldownChannel.Value; }
            if (abilityAttributesPatch.SkipCastOnArrivalConditions.HasValue) { abilityAttributesWrapper.SkipCastOnArrivalConditions = abilityAttributesPatch.SkipCastOnArrivalConditions.Value; }
            if (abilityAttributesPatch.IsToggleable.HasValue) { abilityAttributesWrapper.IsToggleable = abilityAttributesPatch.IsToggleable.Value; }
            if (abilityAttributesPatch.CastOnDeath.HasValue) { abilityAttributesWrapper.CastOnDeath = abilityAttributesPatch.CastOnDeath.Value; }

            var cost = new CostAttributesWrapper(abilityAttributesWrapper.Cost);
            abilityAttributesWrapper.Cost = cost;

            if (abilityAttributesPatch.Resource1Cost.HasValue) { cost.Resource1Cost = abilityAttributesPatch.Resource1Cost.Value; }
            if (abilityAttributesPatch.Resource2Cost.HasValue) { cost.Resource2Cost = abilityAttributesPatch.Resource2Cost.Value; }
        }

        public static void ApplyWeaponAttributesPatch(WeaponAttributesPatch weaponAttributesPatch, WeaponAttributesWrapper weaponAttributesWrapper)
        {
            if (weaponAttributesPatch.ExcludeFromAutoTargetAcquisition.HasValue) { weaponAttributesWrapper.ExcludeFromAutoTargetAcquisition = weaponAttributesPatch.ExcludeFromAutoTargetAcquisition.Value; }
            if (weaponAttributesPatch.ExcludeFromAutoFire.HasValue) { weaponAttributesWrapper.ExcludeFromAutoFire = weaponAttributesPatch.ExcludeFromAutoFire.Value; }
            if (weaponAttributesPatch.ExcludeFromHeightAdvantage.HasValue) { weaponAttributesWrapper.ExcludeFromHeightAdvantage = weaponAttributesPatch.ExcludeFromHeightAdvantage.Value; }
            if (weaponAttributesPatch.DamageType.HasValue) { weaponAttributesWrapper.DamageType = weaponAttributesPatch.DamageType.Value; }
            if (weaponAttributesPatch.IsTracer.HasValue) { weaponAttributesWrapper.IsTracer = weaponAttributesPatch.IsTracer.Value; }
            if (weaponAttributesPatch.TracerSpeed.HasValue) { weaponAttributesWrapper.TracerSpeed = Fixed64.UnsafeFromDouble(weaponAttributesPatch.TracerSpeed.Value); }
            if (weaponAttributesPatch.TracerLength.HasValue) { weaponAttributesWrapper.TracerLength = Fixed64.UnsafeFromDouble(weaponAttributesPatch.TracerLength.Value); }
            if (weaponAttributesPatch.BaseDamagePerRound.HasValue) { weaponAttributesWrapper.BaseDamagePerRound = Fixed64.UnsafeFromDouble(weaponAttributesPatch.BaseDamagePerRound.Value); }
            if (weaponAttributesPatch.BaseWreckDamagePerRound.HasValue) { weaponAttributesWrapper.BaseWreckDamagePerRound = Fixed64.UnsafeFromDouble(weaponAttributesPatch.BaseWreckDamagePerRound.Value); }
            if (weaponAttributesPatch.FiringRecoil.HasValue) { weaponAttributesWrapper.FiringRecoil = weaponAttributesPatch.FiringRecoil.Value; }
            if (weaponAttributesPatch.WindUpTimeMS.HasValue) { weaponAttributesWrapper.WindUpTimeMS = weaponAttributesPatch.WindUpTimeMS.Value; }
            if (weaponAttributesPatch.RateOfFire.HasValue) { weaponAttributesWrapper.RateOfFire = weaponAttributesPatch.RateOfFire.Value; }
            if (weaponAttributesPatch.NumberOfBursts.HasValue) { weaponAttributesWrapper.NumberOfBursts = weaponAttributesPatch.NumberOfBursts.Value; }
            if (weaponAttributesPatch.DamagePacketsPerShot.HasValue) { weaponAttributesWrapper.DamagePacketsPerShot = weaponAttributesPatch.DamagePacketsPerShot.Value; }
            if (weaponAttributesPatch.BurstPeriodMinTimeMS.HasValue) { weaponAttributesWrapper.BurstPeriodMinTimeMS = weaponAttributesPatch.BurstPeriodMinTimeMS.Value; }
            if (weaponAttributesPatch.BurstPeriodMaxTimeMS.HasValue) { weaponAttributesWrapper.BurstPeriodMaxTimeMS = weaponAttributesPatch.BurstPeriodMaxTimeMS.Value; }
            if (weaponAttributesPatch.CooldownTimeMS.HasValue) { weaponAttributesWrapper.CooldownTimeMS = weaponAttributesPatch.CooldownTimeMS.Value; }
            if (weaponAttributesPatch.WindDownTimeMS.HasValue) { weaponAttributesWrapper.WindDownTimeMS = weaponAttributesPatch.WindDownTimeMS.Value; }
            if (weaponAttributesPatch.ReloadTimeMS.HasValue) { weaponAttributesWrapper.ReloadTimeMS = weaponAttributesPatch.ReloadTimeMS.Value; }
            if (weaponAttributesPatch.LineOfSightRequired.HasValue) { weaponAttributesWrapper.LineOfSightRequired = weaponAttributesPatch.LineOfSightRequired.Value; }
            if (weaponAttributesPatch.LeadsTarget.HasValue) { weaponAttributesWrapper.LeadsTarget = weaponAttributesPatch.LeadsTarget.Value; }
            if (weaponAttributesPatch.KillSkipsUnitDeathSequence.HasValue) { weaponAttributesWrapper.KillSkipsUnitDeathSequence = weaponAttributesPatch.KillSkipsUnitDeathSequence.Value; }
            if (weaponAttributesPatch.RevealTriggers.HasValue) { weaponAttributesWrapper.RevealTriggers = weaponAttributesPatch.RevealTriggers.Value; }
            if (weaponAttributesPatch.UnitStatusAttackingTriggers.HasValue) { weaponAttributesWrapper.UnitStatusAttackingTriggers = weaponAttributesPatch.UnitStatusAttackingTriggers.Value; }
            if (weaponAttributesPatch.TargetStyle.HasValue) { weaponAttributesWrapper.TargetStyle = weaponAttributesPatch.TargetStyle.Value; }
            if (weaponAttributesPatch.AreaOfEffectFalloffType.HasValue) { weaponAttributesWrapper.AreaOfEffectFalloffType = weaponAttributesPatch.AreaOfEffectFalloffType.Value; }
            if (weaponAttributesPatch.AreaOfEffectRadius.HasValue) { weaponAttributesWrapper.AreaOfEffectRadius = Fixed64.UnsafeFromDouble(weaponAttributesPatch.AreaOfEffectRadius.Value); }
            if (weaponAttributesPatch.ExcludeWeaponOwnerFromAreaOfEffect.HasValue) { weaponAttributesWrapper.ExcludeWeaponOwnerFromAreaOfEffect = weaponAttributesPatch.ExcludeWeaponOwnerFromAreaOfEffect.Value; }
            if (weaponAttributesPatch.FriendlyFireDamageScalar.HasValue) { weaponAttributesWrapper.FriendlyFireDamageScalar = Fixed64.UnsafeFromDouble(weaponAttributesPatch.FriendlyFireDamageScalar.Value); }
            if (weaponAttributesPatch.WeaponOwnerFriendlyFireDamageScalar.HasValue) { weaponAttributesWrapper.WeaponOwnerFriendlyFireDamageScalar = Fixed64.UnsafeFromDouble(weaponAttributesPatch.WeaponOwnerFriendlyFireDamageScalar.Value); }
            if (weaponAttributesPatch.ProjectileEntityTypeToSpawn != null) { weaponAttributesWrapper.ProjectileEntityTypeToSpawn = weaponAttributesPatch.ProjectileEntityTypeToSpawn; }
            if (weaponAttributesPatch.StatusEffectsTargetAlignment.HasValue) { weaponAttributesWrapper.StatusEffectsTargetAlignment = weaponAttributesPatch.StatusEffectsTargetAlignment.Value; }
            if (weaponAttributesPatch.StatusEffectsExcludeTargetType.HasValue) { weaponAttributesWrapper.StatusEffectsExcludeTargetType = weaponAttributesPatch.StatusEffectsExcludeTargetType.Value; }
            if (weaponAttributesPatch.ActiveStatusEffectsIndex.HasValue) { weaponAttributesWrapper.ActiveStatusEffectsIndex = weaponAttributesPatch.ActiveStatusEffectsIndex.Value; }
        }
    }
}
