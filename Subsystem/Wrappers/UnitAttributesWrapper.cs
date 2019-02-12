using BBI.Core;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using System.Collections.Generic;
using System.Linq;

namespace Subsystem.Wrappers
{
    public class UnitAttributesWrapper : NamedObjectBase, UnitAttributes
    {
        public UnitAttributesWrapper(UnitAttributes other) : base(other.Name)
        {
            Class = other.Class;
            SelectionFlags = other.SelectionFlags;
            NavMeshAttributes = other.NavMeshAttributes;
            MaxHealth = other.MaxHealth;
            Armour = other.Armour;
            DamageReceivedMultiplier = other.DamageReceivedMultiplier;
            AccuracyReceivedMultiplier = other.AccuracyReceivedMultiplier;
            PopCapCost = other.PopCapCost;
            ExperienceValue = other.ExperienceValue;
            ProductionTime = other.ProductionTime;
            AggroRange = other.AggroRange;
            LeashRange = other.LeashRange;
            AlertRange = other.AlertRange;
            RepairPickupRange = other.RepairPickupRange;
            UnitPositionReaggroConditions = other.UnitPositionReaggroConditions;
            LeashPositionReaggroConditions = other.LeashPositionReaggroConditions;
            LeadPriority = other.LeadPriority;
            Selectable = other.Selectable;
            Controllable = other.Controllable;
            Targetable = other.Targetable;
            NonAutoTargetable = other.NonAutoTargetable;
            RetireTargetable = other.RetireTargetable;
            HackedReturnTargetable = other.HackedReturnTargetable;
            HackableProperties = other.HackableProperties;
            ExcludeFromUnitStats = other.ExcludeFromUnitStats;
            BlocksLOF = other.BlocksLOF;
            WorldHeightOffset = other.WorldHeightOffset;
            DoNotPersist = other.DoNotPersist;
            LevelBound = other.LevelBound;
            StartsInHangar = other.StartsInHangar;
            SensorRadius = other.SensorRadius;
            ContactRadius = other.ContactRadius;
            NumProductionQueues = other.NumProductionQueues;
            ProductionQueueDepth = other.ProductionQueueDepth;
            ShowProductionQueues = other.ShowProductionQueues;
            NoTextNotifications = other.NoTextNotifications;
            NotificationFlags = other.NotificationFlags;
            FireRateDisplay = other.FireRateDisplay;
            WeaponLoadout = other.WeaponLoadout.ToArray();
            PriorityAsTarget = other.PriorityAsTarget;
            ThreatData = other.ThreatData;
            ThreatCounters = other.ThreatCounters;
            ThreatCounteredBys = other.ThreatCounteredBys;
            Resource1Cost = other.Resource1Cost;
            Resource2Cost = other.Resource2Cost;
        }

        public UnitClass Class { get; set; }

        public UnitSelectionFlags SelectionFlags { get; set; }

        public NavMeshAttributes NavMeshAttributes { get; set; }

        public int MaxHealth { get; set; }

        public int Armour { get; set; }

        public Fixed64 DamageReceivedMultiplier { get; set; }

        public Fixed64 AccuracyReceivedMultiplier { get; set; }

        public int PopCapCost { get; set; }

        public int ExperienceValue { get; set; }

        public Fixed64 ProductionTime { get; set; }

        public Fixed64 AggroRange { get; set; }

        public Fixed64 LeashRange { get; set; }

        public Fixed64 AlertRange { get; set; }

        public Fixed64 RepairPickupRange { get; set; }

        public UnitPositionReaggroConditions UnitPositionReaggroConditions { get; set; }

        public LeashPositionReaggroConditions LeashPositionReaggroConditions { get; set; }

        public int LeadPriority { get; set; }

        public bool Selectable { get; set; }

        public bool Controllable { get; set; }

        public bool Targetable { get; set; }

        public bool NonAutoTargetable { get; set; }

        public bool RetireTargetable { get; set; }

        public bool HackedReturnTargetable { get; set; }

        public HackableProperties HackableProperties { get; set; }

        public bool ExcludeFromUnitStats { get; set; }

        public bool BlocksLOF { get; set; }

        public Fixed64 WorldHeightOffset { get; set; }

        public bool DoNotPersist { get; set; }

        public bool LevelBound { get; set; }

        public bool StartsInHangar { get; set; }

        public Fixed64 SensorRadius { get; set; }

        public Fixed64 ContactRadius { get; set; }

        public int NumProductionQueues { get; set; }

        public int ProductionQueueDepth { get; set; }

        public bool ShowProductionQueues { get; set; }

        public bool NoTextNotifications { get; set; }

        public UnitNotificationFlags NotificationFlags { get; set; }

        public int FireRateDisplay { get; set; }

        public WeaponBinding[] WeaponLoadout { get; set; }

        public Fixed64 PriorityAsTarget { get; set; }

        public ThreatData ThreatData { get; set; }

        public IEnumerable<ThreatCounter> ThreatCounters { get; set; }

        public IEnumerable<ThreatCounter> ThreatCounteredBys { get; set; }

        public int Resource1Cost { get; set; }

        public int Resource2Cost { get; set; }
    }
}
