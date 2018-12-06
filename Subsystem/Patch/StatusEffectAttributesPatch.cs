using System.Collections.Generic;
using BBI.Game.Data;

namespace Subsystem.Patch
{
    public class StatusEffectAttributesPatch : IRemovable
    {
        public StatusEffectLifetime Lifetime { get; set; }
        public double Duration { get; set; }
        public WeaponFireTrigger WeaponFireTriggerEndEvent { get; set; }
        public int MaxStacks { get; set; }
        public StackingBehaviour StackingBehaviour { get; set; }

        public Dictionary<string, AttributeBuffPatch> BuffsToApplyToTarget { get; set; } = new Dictionary<string, AttributeBuffPatch>();
        public Dictionary<string, AttributeBuffPatch> UnitTypeBuffsToApply { get; set; } = new Dictionary<string, AttributeBuffPatch>();

        public Dictionary<string, ModifierPatch> Modifiers { get; set; } = new Dictionary<string, ModifierPatch>();

        public bool Remove { get; set; }
    }
}
