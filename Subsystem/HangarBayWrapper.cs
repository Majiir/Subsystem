using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem
{
    public class HangarBayWrapper : HangarBay
    {
        public HangarBayWrapper(HangarBay other)
        {
            Name = other.Name;
            EntityType = other.EntityType;
            MaxCount = other.MaxCount;
            UsesStrictClassMatching = other.UsesStrictClassMatching;
            HoldsClass = other.HoldsClass;
            SlotCount = other.SlotCount;
            UndockPresCtrlBone = other.UndockPresCtrlBone;
            UndockTotalSeconds = other.UndockTotalSeconds;
            UndockAnimationSeconds = other.UndockAnimationSeconds;
            UndockSlotStaggerSeconds = other.UndockSlotStaggerSeconds;
            UndockXOffsetPos = other.UndockXOffsetPos;
            UndockYOffsetPos = other.UndockYOffsetPos;
            UndockSlotXSeperationOffset = other.UndockSlotXSeperationOffset;
            DegreesOffsetUndockAngle = other.DegreesOffsetUndockAngle;
            UndockSpeed = other.UndockSpeed;
            DockPresCtrlBone = other.DockPresCtrlBone;
            DockBringInAnimationSeconds = other.DockBringInAnimationSeconds;
            DockSlotStaggerSeconds = other.DockSlotStaggerSeconds;
            MaxDamageCoolingSeconds = other.MaxDamageCoolingSeconds;
            MaxPayloadCoolingSeconds = other.MaxPayloadCoolingSeconds;
            MinDockCoolingSeconds = other.MinDockCoolingSeconds;
            DockReceivingXOffset = other.DockReceivingXOffset;
            DockReceivingYOffset = other.DockReceivingYOffset;
            DoorAnimationSeconds = other.DoorAnimationSeconds;
            UndockLiftTime = other.UndockLiftTime;
        }

        public string Name { get; set; }

        public string EntityType { get; set; }

        public int MaxCount { get; set; }

        public bool UsesStrictClassMatching { get; set; }

        public UnitClass HoldsClass { get; set; }

        public int SlotCount { get; set; }

        public string UndockPresCtrlBone { get; set; }

        public Fixed64 UndockTotalSeconds { get; set; }

        public Fixed64 UndockAnimationSeconds { get; set; }

        public Fixed64 UndockSlotStaggerSeconds { get; set; }

        public Fixed64 UndockXOffsetPos { get; set; }

        public Fixed64 UndockYOffsetPos { get; set; }

        public Fixed64 UndockSlotXSeperationOffset { get; set; }

        public Fixed64 DegreesOffsetUndockAngle { get; set; }

        public Fixed64 UndockSpeed { get; set; }

        public string DockPresCtrlBone { get; set; }

        public Fixed64 DockBringInAnimationSeconds { get; set; }

        public Fixed64 DockSlotStaggerSeconds { get; set; }

        public Fixed64 MaxDamageCoolingSeconds { get; set; }

        public Fixed64 MaxPayloadCoolingSeconds { get; set; }

        public Fixed64 MinDockCoolingSeconds { get; set; }

        public Fixed64 DockReceivingXOffset { get; set; }

        public Fixed64 DockReceivingYOffset { get; set; }

        public Fixed64 DoorAnimationSeconds { get; set; }

        public Fixed64 UndockLiftTime { get; set; }
    }
}
