using BBI.Game.Data;

namespace Subsystem
{
    public class NavMeshAttributesPatch
    {
        public double? DistanceFromObstacles { get; set; }
        public double? DistanceErrorPercentageTolerance { get; set; }
        public UnitClass? BlockedBy { get; set; }
    }
}
