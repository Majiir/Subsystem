using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class ExperienceAttributesPatch
    {
        public Dictionary<string, ExperienceLevelAttributesPatch> Levels { get; set; } = new Dictionary<string, ExperienceLevelAttributesPatch>();
    }
}
