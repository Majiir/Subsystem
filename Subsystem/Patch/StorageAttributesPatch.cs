using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class StorageAttributesPatch
    {
        public bool? LinkToPlayerBank { get; set; }
        public bool? IsResourceController { get; set; }
        public Dictionary<string, InventoryAttributesPatch> InventoryLoadout { get; set; } = new Dictionary<string, InventoryAttributesPatch>();
    }
}
