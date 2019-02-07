using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class UnitTypeBuffWrapper : UnitTypeBuff
    {
        public UnitTypeBuffWrapper()
        {
        }

        public UnitTypeBuffWrapper(UnitTypeBuff other)
        {
            UnitType = other.UnitType;
            UseAsPrefix = other.UseAsPrefix;
            UnitClass = other.UnitClass;
            ClassOperator = other.ClassOperator;
            BuffSet = other.BuffSet;
        }

        public string UnitType { get; set; }
        public bool UseAsPrefix { get; set; }
        public UnitClass UnitClass { get; set; }
        public FlagOperator ClassOperator { get; set; }
        public AttributeBuffSet BuffSet { get; set; }
    }
}
