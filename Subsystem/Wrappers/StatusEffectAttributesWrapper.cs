using BBI.Core;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem.Wrappers
{
    public class StatusEffectAttributesWrapper : NamedObjectBase, StatusEffectAttributes
    {
        public StatusEffectAttributesWrapper(StatusEffectAttributes other) : base(other.Name)
        {
            Lifetime = other.Lifetime;
            Duration = other.Duration;
            WeaponFireTriggerEndEvent = other.WeaponFireTriggerEndEvent;
            MaxStacks = other.MaxStacks;
            StackingBehaviour = other.StackingBehaviour;
            BuffsToApplyToTarget = other.BuffsToApplyToTarget;
            UnitTypeBuffsToApply = other.UnitTypeBuffsToApply;
            Modifiers = other.Modifiers;
        }

        public StatusEffectLifetime Lifetime { get; set; }

        public Fixed64 Duration { get; set; }

        public WeaponFireTrigger WeaponFireTriggerEndEvent { get; set; }

        public int MaxStacks { get; set; }

        public StackingBehaviour StackingBehaviour { get; set; }

        public UnitTypeBuff[] BuffsToApplyToTarget { get; set; }

        public UnitTypeBuff[] UnitTypeBuffsToApply { get; set; }

        public ModifierAttributes[] Modifiers { get; set; }

    }
}
