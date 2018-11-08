using BBI.Game.Data;
using BBI.Core.Utility.FixedPoint;

namespace Subsystem.Wrappers
{
    public class DetectableAttributesWrapper : DetectableAttributes
    {
        public DetectableAttributesWrapper(DetectableAttributes other)
        {
            DisplayLastKnownLocation = other.DisplayLastKnownLocation;
            LastKnownDuration = other.LastKnownDuration;
            TimeVisibleAfterFiring = other.TimeVisibleAfterFiring;
            AlwaysVisible = other.AlwaysVisible;
            MinimumStateAfterDetection = other.MinimumStateAfterDetection;
            FOWFadeDuration = other.FOWFadeDuration;
            SetHasBeenSeenBeforeOnSpawn = other.SetHasBeenSeenBeforeOnSpawn;
        }

        public bool DisplayLastKnownLocation { get; set; }
        public Fixed64 LastKnownDuration { get; set; }
        public int TimeVisibleAfterFiring { get; set; }
        public bool AlwaysVisible { get; set; }
        public DetectionState MinimumStateAfterDetection { get; set; }
        public Fixed64 FOWFadeDuration { get; set; }
        public bool SetHasBeenSeenBeforeOnSpawn { get; set; }

    }
}
