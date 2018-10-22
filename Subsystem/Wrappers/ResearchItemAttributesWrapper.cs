using BBI.Core;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class ResearchItemAttributesWrapper : NamedObjectBase, ResearchItemAttributes
    {
        public ResearchItemAttributesWrapper(ResearchItemAttributes other) : base(other.Name)
        {
            TypeOfResearch = other.TypeOfResearch;
            IconSpriteName = other.IconSpriteName;
            LocalizedResearchTitleStringID = other.LocalizedResearchTitleStringID;
            LocalizedShortDescriptionStringID = other.LocalizedShortDescriptionStringID;
            LocalizedLongDescriptionStringID = other.LocalizedLongDescriptionStringID;
            ResearchTime = other.ResearchTime;
            Dependencies = other.Dependencies;
            ResearchVOCode = other.ResearchVOCode;
            UnitTypeBuffs = other.UnitTypeBuffs;
            CommandsToRun = other.CommandsToRun;
            Resource1Cost = other.Resource1Cost;
            Resource2Cost = other.Resource2Cost;
        }

        public ResearchType TypeOfResearch { get; set; }

        public string IconSpriteName { get; set; }

        public string LocalizedResearchTitleStringID { get; set; }

        public string LocalizedShortDescriptionStringID { get; set; }

        public string LocalizedLongDescriptionStringID { get; set; }

        public Fixed64 ResearchTime { get; set; }

        public string[] Dependencies { get; set; }

        public string ResearchVOCode { get; set; }

        public UnitTypeBuff[] UnitTypeBuffs { get; set; }

        public CommandAttributesBase[] CommandsToRun { get; set; }

        public int Resource1Cost { get; set; }

        public int Resource2Cost { get; set; }
    }
}
