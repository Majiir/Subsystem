using BBI.Core.Data;
using BBI.Game.Data;

namespace Subsystem
{
    public class AttributeLoader
    {
        public static void LoadAttributes(EntityTypeCollection entityTypeCollection)
        {
            var entityType = entityTypeCollection.GetEntityType("G_Baserunner_MP");
            var unitAttributes = entityType.Get<UnitAttributes>();
            entityType.Replace(unitAttributes, new UnitAttributesWrapper(unitAttributes)
            {
                MaxHealth = 20000,
                Armour = 50,
                Resource2Cost = 55,
            });
        }
    }
}
