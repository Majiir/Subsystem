using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using System.Linq;

namespace Subsystem
{
    public class UnitHangarAttributesWrapper : UnitHangarAttributes
    {
        public UnitHangarAttributesWrapper(UnitHangarAttributes other)
        {
            HangarBays = other.HangarBays.ToArray();
            UnitDockingTriggers = other.UnitDockingTriggers;
            HangarDockingTriggers = other.HangarDockingTriggers;
            BoneResetTriggers = other.BoneResetTriggers;
            AlignmentTime = other.AlignmentTime;
            ApproachTime = other.ApproachTime;
        }

        public HangarBay[] HangarBays { get; set; }

        public UnitDockingTrigger[] UnitDockingTriggers { get; set; }

        public HangarDockingTrigger[] HangarDockingTriggers { get; set; }

        public HangarDockingBoneResetTrigger[] BoneResetTriggers { get; set; }

        public Fixed64 AlignmentTime { get; set; }

        public Fixed64 ApproachTime { get; set; }
    }
}
