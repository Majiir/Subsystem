using BBI.Game.Data;
using System.Linq;

namespace Subsystem.Wrappers
{
    public class ExperienceAttributesWrapper : ExperienceAttributes
    {
        public ExperienceAttributesWrapper(ExperienceAttributes other)
        {
            Names = other.Names;
            Levels = other.Levels.ToArray();
        }

        public string[] Names { get; set; }

        public ExperienceLevelAttributes[] Levels { get; set; }
    }
}
