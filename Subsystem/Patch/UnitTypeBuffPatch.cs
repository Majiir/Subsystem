using BBI.Game.Data;
using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class UnitTypeBuffPatch : IRemovable
    {
        public string UnitType { get; set; }
        public bool? UseAsPrefix { get; set; }
        public UnitClass? UnitClass { get; set; }
        public FlagOperator? ClassOperator { get; set; }
        public Dictionary<string, AttributeBuffPatch> BuffSet { get; set; } = new Dictionary<string, AttributeBuffPatch>();

        public bool Remove { get; set; }
    }
}
