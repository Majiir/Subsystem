using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class PowerLevelAttributesWrapper : PowerLevelAttributes
    {
        public PowerLevelAttributesWrapper()
        {
        }

        public PowerLevelAttributesWrapper(PowerLevelAttributes other)
        {
            PowerUnitsRequired = other.PowerUnitsRequired;
            HeatPointsProvided = other.HeatPointsProvided;
            StatusEffectsToApply = other.StatusEffectsToApply;
            View = other.View;
        }

        public int PowerUnitsRequired { get; set; }
        public int HeatPointsProvided { get; set; }
        public StatusEffectAttributes[] StatusEffectsToApply { get; set; }
        public PowerLevelViewAttributes View { get; set; }
    }
}
