using BBI.Core.Data;
using BBI.Game.Data;
using LitJson;
using System.IO;
using UnityEngine;

namespace Subsystem
{
    public class AttributeLoader
    {
        public static void LoadAttributes(EntityTypeCollection entityTypeCollection)
        {
            var jsonPath = Path.Combine(Application.dataPath, "patch.json");
            var json = File.ReadAllText(jsonPath);

            var attributesPatch = JsonMapper.ToObject<AttributesPatch>(json);

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
