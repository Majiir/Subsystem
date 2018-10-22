using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class UnitMovementAttributesPatch
    {
        public UnitDriveType? DriveType { get; set; }
        public UnitDynamicsAttributesPatch Dynamics { get; set; }
    }
}
