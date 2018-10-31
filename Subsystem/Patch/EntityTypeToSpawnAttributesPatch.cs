namespace Subsystem.Patch
{
    public class EntityTypeToSpawnAttributesPatch : IRemovable
    {
        public string EntityTypeToSpawn { get; set; }
        public double? SpawnRotationOffsetDegrees { get; set; }
        public bool Remove { get; set; }
    }
}
