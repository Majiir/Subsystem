using BBI.Game.Data;
using BBI.Core.Utility.FixedPoint;

namespace Subsystem.Wrappers
{
    public class TargetPriorizationAttributesWrapper : TargetPriorizationAttributes
    {
        public TargetPriorizationAttributesWrapper(TargetPriorizationAttributes other)
        {
            WeaponEffectivenessWeight = other.WeaponEffectivenessWeight;
            TargetThreatWeight = other.TargetThreatWeight;
            DistanceWeight = other.DistanceWeight;
            AngleWeight = other.AngleWeight;
            TargetPriorityWeight = other.TargetPriorityWeight;
            AutoTargetStickyBias = other.AutoTargetStickyBias;
            ManualTargetStickyBias = other.ManualTargetStickyBias;
            TargetSameCommanderBias = other.TargetSameCommanderBias;
            TargetWithinFOVBias = other.TargetWithinFOVBias;
        }

        public Fixed64 WeaponEffectivenessWeight { get; set; }
        public Fixed64 TargetThreatWeight { get; set; }
        public Fixed64 DistanceWeight { get; set; }
        public Fixed64 AngleWeight { get; set; }
        public Fixed64 TargetPriorityWeight { get; set; }
        public Fixed64 AutoTargetStickyBias { get; set; }
        public Fixed64 ManualTargetStickyBias { get; set; }
        public Fixed64 TargetSameCommanderBias { get; set; }
        public Fixed64 TargetWithinFOVBias { get; set; }

    }
}
