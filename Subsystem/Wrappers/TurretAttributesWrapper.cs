using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class TurretAttributesWrapper : TurretAttributes
    {
        public TurretAttributesWrapper(TurretAttributes other)
        {
            FieldOfView = other.FieldOfView;
            FieldOfFire = other.FieldOfFire;
            RotationSpeed = other.RotationSpeed;
        }

        public Fixed64 FieldOfView { get; set; }

        public Fixed64 FieldOfFire { get; set; }

        public Fixed64 RotationSpeed { get; set; }
    }
}
