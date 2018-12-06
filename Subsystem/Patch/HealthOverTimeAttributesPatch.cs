using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class HealthOverTimeAttributesPatch
    {
        public string ID { get; set; }
        public int? Amount { get; set; }
        public int? MSTickDuration { get; set; }
        public DamageType? DamageType;
    }
}
