using BBI.Core.Utility;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class UnitDynamicsAttributesWrapper : UnitDynamicsAttributes
    {
        public UnitDynamicsAttributesWrapper(UnitDynamicsAttributes other)
        {
            DriveType = other.DriveType;
            Length = other.Length;
            Width = other.Width;
            MaxSpeed = other.MaxSpeed;
            ReverseFactor = other.ReverseFactor;
            AccelerationTime = other.AccelerationTime;
            BrakingTime = other.BrakingTime;
            MaxSpeedTurnRadius = other.MaxSpeedTurnRadius;
            MaxEaseIntoTurnTime = other.MaxEaseIntoTurnTime;
            DriftType = other.DriftType;
            ReverseDriftMultiplier = other.ReverseDriftMultiplier;
            DriftOvershootFactor = other.DriftOvershootFactor;
            FishTailingTimeIntervals = other.FishTailingTimeIntervals;
            FishTailControlRecover = other.FishTailControlRecover;
            MinDriftSlipSpeed = other.MinDriftSlipSpeed;
            MaxDriftRecoverTime = other.MaxDriftRecoverTime;
            MinCruiseSpeed = other.MinCruiseSpeed;
            DeathDriftTime = other.DeathDriftTime;
            PermanentlyImmobile = other.PermanentlyImmobile;
            CruiseSpeedVariation = other.CruiseSpeedVariation;
            CruiseDirectionVariation = other.CruiseDirectionVariation;
        }

        public UnitDriveType DriveType { get; set; }

        public Fixed64 Length { get; set; }

        public Fixed64 Width { get; set; }

        public Fixed64 MaxSpeed { get; set; }

        public Fixed64 ReverseFactor { get; set; }

        public Fixed64 AccelerationTime { get; set; }

        public Fixed64 BrakingTime { get; set; }

        public Fixed64 MaxSpeedTurnRadius { get; set; }

        public Fixed64 MaxEaseIntoTurnTime { get; set; }

        public Fixed64 DriftType { get; set; }

        public Fixed64 ReverseDriftMultiplier { get; set; }

        public Fixed64 DriftOvershootFactor { get; set; }

        public Fixed64 FishTailingTimeIntervals { get; set; }

        public Fixed64 FishTailControlRecover { get; set; }

        public Fixed64 MinDriftSlipSpeed { get; set; }

        public Fixed64 MaxDriftRecoverTime { get; set; }

        public Fixed64 MinCruiseSpeed { get; set; }

        public Fixed64 DeathDriftTime { get; set; }

        public bool PermanentlyImmobile { get; set; }

        public WaveletAttributes[] CruiseSpeedVariation { get; set; }

        public WaveletAttributes[] CruiseDirectionVariation { get; set; }
    }
}
