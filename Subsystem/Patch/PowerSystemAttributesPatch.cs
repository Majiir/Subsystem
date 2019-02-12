using BBI.Game.Data;
using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class PowerSystemAttributesPatch
    {
        public PowerSystemType? PowerSystemType { get; set; }
        public int? StartingPowerLevelIndex { get; set; }
        public int? StartingMaxPowerLevelIndex { get; set; }
        public Dictionary<string, PowerLevelAttributesPatch> PowerLevels { get; set; } = new Dictionary<string, PowerLevelAttributesPatch>();
        //PowerSystemViewAttributes View { get; set; }
    }
}
