using BBI.Core;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class StorageAttributesWrapper : NamedObjectBase, StorageAttributes
    {
        public StorageAttributesWrapper(StorageAttributes other) : base(other.Name)
        {
            LinkToPlayerBank = other.LinkToPlayerBank;
            IsResourceController = other.IsResourceController;
            InventoryLoadout = other.InventoryLoadout;
        }

        public bool LinkToPlayerBank { get; set; }

        public bool IsResourceController { get; set; }

        public InventoryBinding[] InventoryLoadout { get; set; }
    }
}
