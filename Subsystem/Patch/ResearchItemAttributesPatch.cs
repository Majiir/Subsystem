using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class ResearchItemAttributesPatch
    {
        public ResearchType? TypeOfResearch { get; set; }
        public string IconSpriteName { get; set; }
        public string LocalizedResearchTitleStringID { get; set; }
        public string LocalizedShortDescriptionStringID { get; set; }
        public string LocalizedLongDescriptionStringID { get; set; }
        public double? ResearchTime { get; set; }
        public string[] Dependencies { get; set; }
        public string ResearchVOCode { get; set; }

        public int? Resource1Cost { get; set; }
        public int? Resource2Cost { get; set; }
    }
}
