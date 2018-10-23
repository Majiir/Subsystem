using BBI.Core.Data;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class AttributeBuffWrapper
    {
        public AttributeBuffWrapper(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public ushort AttributeID { get; set; }
        public Buff.Category Category { get; set; }
        public AttributeBuffMode Mode { get; set; }
        public int Value { get; set; }
    }
}
