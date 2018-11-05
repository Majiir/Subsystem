using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class EntityTypeToSpawnAttributesWrapper : EntityTypeToSpawnAttributes
    {
        public EntityTypeToSpawnAttributesWrapper()
        {
        }

        public EntityTypeToSpawnAttributesWrapper(EntityTypeToSpawnAttributes other)
        {
            EntityTypeToSpawn = other.EntityTypeToSpawn;
            SpawnRotationOffsetDegrees = other.SpawnRotationOffsetDegrees;
        }

        public string EntityTypeToSpawn { get; set; }

        public Fixed64 SpawnRotationOffsetDegrees { get; set; }
    }
}
