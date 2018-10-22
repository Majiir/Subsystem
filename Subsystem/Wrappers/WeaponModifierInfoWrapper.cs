using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class WeaponModifierInfoWrapper : WeaponModifierInfo
    {
        public WeaponModifierInfoWrapper()
        {
        }

        public WeaponModifierInfoWrapper(WeaponModifierInfo other)
        {
            TargetClass = other.TargetClass;
            ClassOperator = other.ClassOperator;
            Modifier = other.Modifier;
            Amount = other.Amount;
        }

        public UnitClass TargetClass { get; set; }

        public FlagOperator ClassOperator { get; set; }

        public WeaponModifierType Modifier { get; set; }

        public int Amount { get; set; }
    }
}
