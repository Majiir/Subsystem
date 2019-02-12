using BBI.Game.Data;
using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class PowerShuntAttributesPatch
    {
        public double? PowerLevelChargeTimeSeconds { get; set; }
        public double? PowerLevelDrainTimeSeconds { get; set; }
        public int? HeatThreshold { get; set; }
        public int? CooldownRate { get; set; }
        public int? OverheatDamage { get; set; }
        public int? NearOverheatWarningMargin { get; set; }
        public int? OverheatReminderPeriod { get; set; }
        public Dictionary<string, PowerSystemAttributesPatch> PowerSystems { get; set; } = new Dictionary<string, PowerSystemAttributesPatch>();
        public InventoryAttributesPatch ReservePowerPool { get; set; }
        public InventoryAttributesPatch OverheatingPool { get; set; }
        public InventoryAttributesPatch HeatSystem { get; set; }
        //public PowerShuntViewAttributes View { get; set; }
    }
}
