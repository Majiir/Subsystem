using BBI.Game.Data;

namespace Subsystem
{
    public class CostAttributesWrapper : CostAttributes
    {
        public CostAttributesWrapper(CostAttributes other)
        {
            Resource1Cost = other.Resource1Cost;
            Resource2Cost = other.Resource2Cost;
        }

        public int Resource1Cost { get; set; }

        public int Resource2Cost { get; set; }
    }
}
