using BBI.Core.Data;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using LitJson;
using System.IO;
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

                var unitAttributes = entityType.Get<UnitAttributes>();
                var unitAttributesWrapper = new UnitAttributesWrapper(unitAttributes);

                ApplyUnitAttributesPatch(entityTypePatch.UnitAttributes, unitAttributesWrapper);

                entityType.Replace(unitAttributes, unitAttributesWrapper);
            }
        }

        public static void ApplyUnitAttributesPatch(UnitAttributesPatch unitAttributesPatch, UnitAttributesWrapper unitAttributesWrapper)
        {
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
            if (unitAttributesPatch.FireRateDisplay.HasValue) { unitAttributesWrapper.FireRateDisplay = unitAttributesPatch.FireRateDisplay.Value; }
            if (unitAttributesPatch.PriorityAsTarget.HasValue) { unitAttributesWrapper.PriorityAsTarget = Fixed64.UnsafeFromDouble(unitAttributesPatch.PriorityAsTarget.Value); }
            if (unitAttributesPatch.Resource1Cost.HasValue) { unitAttributesWrapper.Resource1Cost = unitAttributesPatch.Resource1Cost.Value; }
            if (unitAttributesPatch.Resource2Cost.HasValue) { unitAttributesWrapper.Resource2Cost = unitAttributesPatch.Resource2Cost.Value; }
        }
    }
}
