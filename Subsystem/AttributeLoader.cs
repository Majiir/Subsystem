using BBI.Core.Data;
using BBI.Game.Data;

namespace Subsystem
{
    public class AttributeLoader
    {
        public static void LoadAttributes(EntityTypeCollection entityTypeCollection)
        {
            var attributesPatch = new AttributesPatch
            {
                Entities = new System.Collections.Generic.Dictionary<string, EntityTypePatch>
                {
                    ["G_Catamaran_MP"] = new EntityTypePatch
                    {
                        UnitAttributes = new UnitAttributesPatch
                        {
                            MaxHealth = 450,
                            Armour = 24,
                        }
                    },
                    ["G_Harvester_MP"] = new EntityTypePatch
                    {
                        UnitAttributes =  new UnitAttributesPatch
                        {
                            MaxHealth = 25,
                            Armour = 100,
                        }
                    },
                    ["G_Carrier_MP"] = new EntityTypePatch
                    {
                        UnitAttributes = new UnitAttributesPatch
                        {
                            MaxHealth = 50000,
                            Armour = 0,
                        }
                    },
                }
            };

            ApplyAttributesPatch(entityTypeCollection, attributesPatch);
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
