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

        public static void ApplyAttributesPatch(EntityTypeCollection entityTypeCollection, AttributesPatch attributesPatch)
        {
            foreach (var kvp in attributesPatch.Entities)
            {
                var entityTypeName = kvp.Key;
                var entityTypePatch = kvp.Value;

                var entityType = entityTypeCollection.GetEntityType(entityTypeName);

                var unitAttributes = entityType.Get<UnitAttributes>();
                var unitAttributesWrapper = new UnitAttributesWrapper(unitAttributes);

                ApplyUnitAttributesPatch(entityTypePatch.UnitAttributes, unitAttributesWrapper);

                entityType.Replace(unitAttributes, unitAttributesWrapper);
            }
        }

        public static void ApplyUnitAttributesPatch(UnitAttributesPatch unitAttributesPatch, UnitAttributesWrapper unitAttributesWrapper)
        {
            if (unitAttributesPatch.MaxHealth.HasValue)
            {
                unitAttributesWrapper.MaxHealth = unitAttributesPatch.MaxHealth.Value;
            }

            if (unitAttributesPatch.Armour.HasValue)
            {
                unitAttributesWrapper.Armour = unitAttributesPatch.Armour.Value;
            }
        }
    }
}
