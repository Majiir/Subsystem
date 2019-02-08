using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class DetectableAttributesPatch
    {
        public bool? DisplayLastKnownLocation { get; set; }
        public double? LastKnownDuration { get; set; }
        public int? TimeVisibleAfterFiring { get; set; }
        public bool? AlwaysVisible { get; set; }
        public DetectionState? MinimumStateAfterDetection { get; set; }
        public double? FOWFadeDuration { get; set; }
        public bool? SetHasBeenSeenBeforeOnSpawn { get; set; }
    }
}
