using System.Collections.Generic;

namespace Subsystem
{
    public class AttributesPatch
    {
        public Dictionary<string, EntityTypePatch> Entities { get; set; } = new Dictionary<string, EntityTypePatch>();
    }
}
