namespace Subsystem.Patch
{
    public class PowerLevelAttributesPatch
    {
        int PowerUnitsRequired { get; set; }
        int HeatPointsProvided { get; set; }
        StatusEffectAttributes[] StatusEffectsToApply { get; set; }
        //PowerLevelViewAttributes View { get; set; }
    }
}
