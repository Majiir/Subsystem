using BBI.Game.Data;

namespace Subsystem
{
    public class HangarBayPatch
    {
        public string EntityType { get; set; }
        public int? MaxCount { get; set; }
        public bool? UsesStrictClassMatching { get; set; }
        public UnitClass? HoldsClass { get; set; }
        public int? SlotCount { get; set; }
        public string UndockPresCtrlBone { get; set; }
        public double? UndockTotalSeconds { get; set; }
        public double? UndockAnimationSeconds { get; set; }
        public double? UndockSlotStaggerSeconds { get; set; }
        public double? UndockXOffsetPos { get; set; }
        public double? UndockYOffsetPos { get; set; }
        public double? UndockSlotXSeperationOffset { get; set; }
        public double? DegreesOffsetUndockAngle { get; set; }
        public double? UndockSpeed { get; set; }
        public string DockPresCtrlBone { get; set; }
        public double? DockBringInAnimationSeconds { get; set; }
        public double? DockSlotStaggerSeconds { get; set; }
        public double? MaxDamageCoolingSeconds { get; set; }
        public double? MaxPayloadCoolingSeconds { get; set; }
        public double? MinDockCoolingSeconds { get; set; }
        public double? DockReceivingXOffset { get; set; }
        public double? DockReceivingYOffset { get; set; }
        public double? DoorAnimationSeconds { get; set; }
        public double? UndockLiftTime { get; set; }

    }
}
