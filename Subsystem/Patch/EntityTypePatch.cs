using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class EntityTypePatch
    {
        public UnitAttributesPatch UnitAttributes { get; set; }
        public ResearchItemAttributesPatch ResearchItemAttributes { get; set; }
        public UnitHangarAttributesPatch UnitHangarAttributes { get; set; }
        public UnitMovementAttributesPatch UnitMovementAttributes { get; set; }
        public Dictionary<string, AbilityAttributesPatch> AbilityAttributes { get; set; } = new Dictionary<string, AbilityAttributesPatch>();
        public Dictionary<string, StorageAttributesPatch> StorageAttributes { get; set; } = new Dictionary<string, StorageAttributesPatch>();
        public Dictionary<string, WeaponAttributesPatch> WeaponAttributes { get; set; } = new Dictionary<string, WeaponAttributesPatch>();
    }
}
