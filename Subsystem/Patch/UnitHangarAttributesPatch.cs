using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class UnitHangarAttributesPatch
    {
        public Dictionary<string, HangarBayPatch> HangarBays { get; set; } = new Dictionary<string, HangarBayPatch>();
        public double? AlignmentTime { get; set; }
        public double? ApproachTime { get; set; }
    }
}
