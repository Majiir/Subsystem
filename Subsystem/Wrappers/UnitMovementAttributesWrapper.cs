using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class UnitMovementAttributesWrapper : UnitMovementAttributes
    {
        public UnitMovementAttributesWrapper(UnitMovementAttributes other)
        {
            DriveType = other.DriveType;
            Dynamics = other.Dynamics;
            RandomDynamicsVariance = other.RandomDynamicsVariance;
            Maneuvers = other.Maneuvers;
            Hover = other.Hover;
            Combat = other.Combat;
            Avoidance = other.Avoidance;
            ReversePolarity = other.ReversePolarity;
        }

        public UnitDriveType DriveType { get; set; }

        public UnitDynamicsAttributes Dynamics { get; set; }

        public UnitDynamicsRandomizationParameters RandomDynamicsVariance { get; set; }

        public UnitManeuverAttributes Maneuvers { get; set; }

        public HoverDynamicsAttributes Hover { get; set; }

        public UnitCombatBehavior Combat { get; set; }

        public UnitAvoidanceAttributes Avoidance { get; set; }

        public ReversePolarityAttributes ReversePolarity { get; set; }
    }
}
