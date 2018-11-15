using BBI.Core.Data;
using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class AttributeBuffPatch : IRemovable
    {
        public string Name { get; set; }
        public Buff.CategoryAndID? Attribute { get; set; }
        public AttributeBuffMode? Mode { get; set; }
        public int? Value { get; set; }
        public bool Remove { get; set; }
    }
}
