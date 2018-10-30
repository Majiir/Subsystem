using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class ExperienceLevelAttributesWrapper : ExperienceLevelAttributes
    {
        public ExperienceLevelAttributesWrapper()
        {
            Buff = new AttributeBuffSetWrapper();
        }

        public ExperienceLevelAttributesWrapper(ExperienceLevelAttributes other)
        {
            BuffTooltipLocID = other.BuffTooltipLocID;
            RequiredExperience = other.RequiredExperience;
            Buff = other.Buff;
        }

        public string BuffTooltipLocID { get; set; }

        public int RequiredExperience { get; set; }

        public AttributeBuffSet Buff { get; set; }
    }
}
