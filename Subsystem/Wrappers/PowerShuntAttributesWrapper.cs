using BBI.Core;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using UnityEngine;

namespace Subsystem.Wrappers
{
    public class PowerShuntAttributesWrapper : NamedObjectBase, PowerShuntAttributes
    {
        public PowerShuntAttributesWrapper(PowerShuntAttributes other) : base(other.Name)
        {
            PowerLevelChargeTimeSeconds = other.PowerLevelChargeTimeSeconds;
            PowerLevelDrainTimeSeconds = other.PowerLevelDrainTimeSeconds;
            HeatThreshold = other.HeatThreshold;
            CooldownRate = other.CooldownRate;
            OverheatDamage = other.OverheatDamage;
            NearOverheatWarningMargin = other.NearOverheatWarningMargin;
            OverheatReminderPeriod = other.OverheatReminderPeriod;
            PowerSystems = other.PowerSystems;
            ReservePowerPool = other.ReservePowerPool;
            OverheatingPool = other.OverheatingPool;
            HeatSystem = other.HeatSystem;
            View = other.View;
        }

        public Fixed64 PowerLevelChargeTimeSeconds { get; set; }
        public Fixed64 PowerLevelDrainTimeSeconds { get; set; }
        public int HeatThreshold { get; set; }
        public int CooldownRate { get; set; }
        public int OverheatDamage { get; set; }
        public int NearOverheatWarningMargin { get; set; }
        public int OverheatReminderPeriod { get; set; }
        public PowerSystemAttributes[] PowerSystems { get; set; }
        public InventoryAttributes ReservePowerPool { get; set; }
        public InventoryAttributes OverheatingPool { get; set; }
        public InventoryAttributes HeatSystem { get; set; }
        public PowerShuntViewAttributes View { get; set; }

        public PowerSystemAttributes FindAttributesWithPowerSystemType(PowerSystemType powerSystemType)
        {
            if (!PowerSystems.IsNullOrEmpty<PowerSystemAttributes>())
            {
                for (int i = 0; i < PowerSystems.Length; i++)
                {
                    PowerSystemAttributes powerSystemAttributes = PowerSystems[i];
                    if (powerSystemAttributes.PowerSystemType == powerSystemType)
                    {
                        return powerSystemAttributes;
                    }
                }
            }
            return null;
        }
    }
}
