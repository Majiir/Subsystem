using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class AttributesPatch
    {
        public Dictionary<string, EntityTypePatch> Entities { get; set; } = new Dictionary<string, EntityTypePatch>();
    }
}
