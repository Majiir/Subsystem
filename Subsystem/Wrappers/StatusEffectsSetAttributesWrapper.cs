using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class StatusEffectsSetAttributesWrapper : StatusEffectsSetAttributes
    {
        public StatusEffectsSetAttributesWrapper()
        {
        }

        public StatusEffectsSetAttributesWrapper(StatusEffectsSetAttributes other)
        {
            StatusEffects = other.StatusEffects;
        }

        public string[] StatusEffects { get; set; }
    }
}
