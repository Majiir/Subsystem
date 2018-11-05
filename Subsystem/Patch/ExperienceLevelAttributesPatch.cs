using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class ExperienceLevelAttributesPatch : IRemovable
    {
        public string BuffTooltipLocID { get; set; }
        public int? RequiredExperience { get; set; }
        public Dictionary<string, AttributeBuffPatch> Buff { get; set; } = new Dictionary<string, AttributeBuffPatch>();
        public bool Remove { get; set; }
    }
}
