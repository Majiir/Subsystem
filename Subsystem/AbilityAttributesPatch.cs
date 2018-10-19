using BBI.Game.Data;

namespace Subsystem
{
    public class AbilityAttributesPatch
    {
        public AbilityClass? AbilityType { get; set; }
        public AbilityTargetingType? TargetingType { get; set; }
        public AbilityTargetAlignment? TargetAlignment { get; set; }
        public UnitClass? AbilityMapTargetLayers { get; set; }
        public GroundAutoTargetAlignmentType? GroundAutoTargetAlignment { get; set; }
        public double? EdgeOfTargetShapeMinDistance { get; set; }

        public bool? CasterMovesToTarget { get; set; }
        public AbilityGroupActivationType? GroupActivationType { get; set; }
        public AbilityRemovedInMode? StartsRemovedInGameMode { get; set; }
        public double? CooldownTimeSecs { get; set; }
        public double? WarmupTimeSecs { get; set; }
        public int? SharedCooldownChannel { get; set; }
        public SkipCastOnArrivalConditions? SkipCastOnArrivalConditions { get; set; }
        public bool? IsToggleable { get; set; }
        public bool? CastOnDeath { get; set; }

        public int? Resource1Cost { get; set; }
        public int? Resource2Cost { get; set; }
    }
}
