using BBI.Game.Data;

namespace Subsystem
{
    public class UnitAttributesPatch
    {
        public int? MaxHealth { get; set; }
        public int? Armour { get; set; }

        public double? DamageReceivedMultiplier { get; set; }
        public double? AccuracyReceivedMultiplier { get; set; }
        public int? PopCapCost { get; set; }
        public int? ExperienceValue { get; set; }
        public double? ProductionTime { get; set; }
        public double? AggroRange { get; set; }
        public double? LeashRange { get; set; }
        public double? AlertRange { get; set; }
        public double? RepairPickupRange { get; set; }

        public int? LeadPriority { get; set; }
        public bool? Selectable { get; set; }
        public bool? Controllable { get; set; }
        public bool? Targetable { get; set; }
        public bool? NonAutoTargetable { get; set; }
        public bool? RetireTargetable { get; set; }
        public bool? HackedReturnTargetable { get; set; }

        public bool? ExcludeFromUnitStats { get; set; }
        public bool? BlocksLOF { get; set; }
        public double? WorldHeightOffset { get; set; }
        public bool? DoNotPersist { get; set; }
        public bool? LevelBound { get; set; }
        public bool? StartsInHangar { get; set; }
        public double? SensorRadius { get; set; }
        public double? ContactRadius { get; set; }
        public int? NumProductionQueues { get; set; }
        public int? ProductionQueueDepth { get; set; }
        public bool? ShowProductionQueues { get; set; }
        public bool? NoTextNotifications { get; set; }

        public int? FireRateDisplay { get; set; }

        public double? PriorityAsTarget { get; set; }

        public int? Resource1Cost { get; set; }
        public int? Resource2Cost { get; set; }
    }
}
