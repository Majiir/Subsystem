using BBI.Game.Data;
using System.Collections.Generic;

namespace Subsystem.Patch
{
    public class StatusEffectAttributesPatch : IRemovable
    {
        public StatusEffectLifetime? Lifetime { get; set; }
        public double? Duration { get; set; }
        public WeaponFireTrigger? WeaponFireTriggerEndEvent { get; set; }
        public int? MaxStacks { get; set; }
        public StackingBehaviour? StackingBehaviour { get; set; }

        public Dictionary<string, UnitTypeBuffPatch> BuffsToApplyToTarget { get; set; } = new Dictionary<string, UnitTypeBuffPatch>();
        public Dictionary<string, UnitTypeBuffPatch> UnitTypeBuffsToApply { get; set; } = new Dictionary<string, UnitTypeBuffPatch>();

        public Dictionary<string, ModifierAttributesPatch> Modifiers { get; set; } = new Dictionary<string, ModifierAttributesPatch>();

        public bool Remove { get; set; }
    }
}
