using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class PowerLevelAttributesPatch
    {
        public int? PowerUnitsRequired { get; set; }
        public int? HeatPointsProvided { get; set; }
        public Dictionary<string, StatusEffectAttributesPatch> StatusEffectsToApply { get; set; } = new Dictionary<string, StatusEffectAttributesPatch>();
        //PowerLevelViewAttributes View { get; set; }
    }
}
