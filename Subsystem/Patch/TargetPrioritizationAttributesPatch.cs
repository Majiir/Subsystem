namespace Subsystem.Patch
{
    public class TargetPrioritizationAttributesPatch
    {
        public double? WeaponEffectivenessWeight { get; set; }
        public double? TargetThreatWeight { get; set; }
        public double? DistanceWeight { get; set; }
        public double? AngleWeight { get; set; }
        public double? TargetPriorityWeight { get; set; }
        public double? AutoTargetStickyBias { get; set; }
        public double? ManualTargetStickyBias { get; }
        public double? TargetSameCommanderBias { get; set; }
        public double? TargetWithinFOVBias { get; set; }
    }
}
