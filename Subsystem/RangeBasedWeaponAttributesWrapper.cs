using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem
{
    public class RangeBasedWeaponAttributesWrapper : RangeBasedWeaponAttributes
    {
        public RangeBasedWeaponAttributesWrapper(WeaponRange range)
        {
            Range = range;
        }

        public RangeBasedWeaponAttributesWrapper(RangeBasedWeaponAttributes other)
        {
            Range = other.Range;
            Accuracy = other.Accuracy;
            Distance = other.Distance;
            MinDistance = other.MinDistance;
        }

        public WeaponRange Range { get; set; }

        public Fixed64 Accuracy { get; set; }

        public Fixed64 Distance { get; set; }

        public Fixed64 MinDistance { get; set; }
    }
}
