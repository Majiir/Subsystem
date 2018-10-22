using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class WeaponModifierInfoPatch
    {
        public UnitClass? TargetClass { get; set; }
        public FlagOperator? ClassOperator { get; set; }
        public WeaponModifierType? Modifier { get; set; }
        public int? Amount { get; set; }
        public bool Remove { get; set; }
    }
}
