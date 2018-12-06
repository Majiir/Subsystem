using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class UnitAttributesPatch
    {
        public double PowerLevelChargeTimeSeconds { get; set; }
        public double PowerLevelDrainTimeSeconds { get; set; }
        public int HeatThreshold { get; set; }
        public int CooldownRate { get; set; }
        public int OverheatDamage { get; set; }
        public int NearOverheatWarningMargin { get; set; }
        public int OverheatReminderPeriod { get; set; }
        public Dictionary<string, PowerSystemAttributesPatch> PowerSystems { get; set; } = new Dictionary<string, PowerSystemAttributesPatch>();
        public InventoryAttributesPatch ReservePowerPool { get; set; }
        public InventoryAttributesPatch OverheatingPool { get; set; }
        public InventoryAttributesPatch HeatSystem { get; set; }
        public PowerSystemAttributes FindAttributesWithPowerSystemType(PowerSystemType powerSystemType);
        public PowerShuntViewAttributes View { get; set; }
    }
}
