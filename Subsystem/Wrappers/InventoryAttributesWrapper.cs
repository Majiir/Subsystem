using BBI.Core;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class InventoryAttributesWrapper : NamedObjectBase, InventoryAttributes
    {
        public InventoryAttributesWrapper(string name, string inventoryId) : base(name)
        {
            InventoryID = inventoryId;
        }

        public InventoryAttributesWrapper(InventoryAttributes other) : base(other.Name)
        {
            InventoryID = other.InventoryID;
            HasUnlimitedCapacity = other.HasUnlimitedCapacity;
            StartingAmount = other.StartingAmount;
            Capacity = other.Capacity;
        }

        public string InventoryID { get; set; }

        public bool HasUnlimitedCapacity { get; set; }

        public int StartingAmount { get; set; }

        public int Capacity { get; set; }
    }
}
