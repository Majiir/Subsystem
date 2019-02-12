using BBI.Core.Data;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class AttributeBuffWrapper
    {
        public AttributeBuffWrapper()
        {
        }

        public AttributeBuffWrapper(AttributeBuffWrapper other)
        {
            Name = other.Name;
            AttributeID = other.AttributeID;
            Category = other.Category;
            Mode = other.Mode;
            Value = other.Value;
        }

        public string Name { get; set; }
        public ushort AttributeID { get; set; }
        public Buff.Category Category { get; set; }
        public AttributeBuffMode Mode { get; set; }
        public int Value { get; set; }
    }
}
