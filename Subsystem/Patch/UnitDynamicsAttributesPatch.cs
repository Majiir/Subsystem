using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class UnitDynamicsAttributesPatch
    {
        public UnitDriveType? DriveType { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? MaxSpeed { get; set; }
        public double? ReverseFactor { get; set; }
        public double? AccelerationTime { get; set; }
        public double? BrakingTime { get; set; }
        public double? MaxSpeedTurnRadius { get; set; }
        public double? MaxEaseIntoTurnTime { get; set; }
        public double? DriftType { get; set; }
        public double? ReverseDriftMultiplier { get; set; }
        public double? DriftOvershootFactor { get; set; }
        public double? FishTailingTimeIntervals { get; set; }
        public double? FishTailControlRecover { get; set; }
        public double? MinDriftSlipSpeed { get; set; }
        public double? MaxDriftRecoverTime { get; set; }
        public double? MinCruiseSpeed { get; set; }
        public double? DeathDriftTime { get; set; }
        public bool? PermanentlyImmobile { get; set; }

    }
}
