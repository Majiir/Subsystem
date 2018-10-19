using BBI.Core;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem
{
    public class WeaponAttributesWrapper : NamedObjectBase, WeaponAttributes
    {
        public WeaponAttributesWrapper(WeaponAttributes other) : base(other.Name)
        {
            ExcludeFromAutoTargetAcquisition = other.ExcludeFromAutoTargetAcquisition;
            ExcludeFromAutoFire = other.ExcludeFromAutoFire;
            ExcludeFromHeightAdvantage = other.ExcludeFromHeightAdvantage;
            DamageType = other.DamageType;
            IsTracer = other.IsTracer;
            TracerSpeed = other.TracerSpeed;
            TracerLength = other.TracerLength;
            BaseDamagePerRound = other.BaseDamagePerRound;
            BaseWreckDamagePerRound = other.BaseWreckDamagePerRound;
            FiringRecoil = other.FiringRecoil;
            WindUpTimeMS = other.WindUpTimeMS;
            RateOfFire = other.RateOfFire;
            NumberOfBursts = other.NumberOfBursts;
            DamagePacketsPerShot = other.DamagePacketsPerShot;
            BurstPeriodMinTimeMS = other.BurstPeriodMinTimeMS;
            BurstPeriodMaxTimeMS = other.BurstPeriodMaxTimeMS;
            CooldownTimeMS = other.CooldownTimeMS;
            WindDownTimeMS = other.WindDownTimeMS;
            ReloadTimeMS = other.ReloadTimeMS;
            LineOfSightRequired = other.LineOfSightRequired;
            LeadsTarget = other.LeadsTarget;
            KillSkipsUnitDeathSequence = other.KillSkipsUnitDeathSequence;
            RevealTriggers = other.RevealTriggers;
            UnitStatusAttackingTriggers = other.UnitStatusAttackingTriggers;
            TargetStyle = other.TargetStyle;
            Modifiers = other.Modifiers;
            AreaOfEffectFalloffType = other.AreaOfEffectFalloffType;
            AreaOfEffectRadius = other.AreaOfEffectRadius;
            ExcludeWeaponOwnerFromAreaOfEffect = other.ExcludeWeaponOwnerFromAreaOfEffect;
            FriendlyFireDamageScalar = other.FriendlyFireDamageScalar;
            WeaponOwnerFriendlyFireDamageScalar = other.WeaponOwnerFriendlyFireDamageScalar;
            Turret = other.Turret;
            Ranges = other.Ranges;
            ProjectileEntityTypeToSpawn = other.ProjectileEntityTypeToSpawn;
            StatusEffectsTargetAlignment = other.StatusEffectsTargetAlignment;
            StatusEffectsExcludeTargetType = other.StatusEffectsExcludeTargetType;
            ActiveStatusEffectsIndex = other.ActiveStatusEffectsIndex;
            StatusEffectsSets = other.StatusEffectsSets;
            EntityTypesToSpawnOnImpact = other.EntityTypesToSpawnOnImpact;
            TargetPriorizationAttributes = other.TargetPriorizationAttributes;
        }

        public bool ExcludeFromAutoTargetAcquisition { get; set; }

        public bool ExcludeFromAutoFire { get; set; }

        public bool ExcludeFromHeightAdvantage { get; set; }

        public DamageType DamageType { get; set; }

        public bool IsTracer { get; set; }

        public Fixed64 TracerSpeed { get; set; }

        public Fixed64 TracerLength { get; set; }

        public Fixed64 BaseDamagePerRound { get; set; }

        public Fixed64 BaseWreckDamagePerRound { get; set; }

        public float FiringRecoil { get; set; }

        public int WindUpTimeMS { get; set; }

        public int RateOfFire { get; set; }

        public int NumberOfBursts { get; set; }

        public int DamagePacketsPerShot { get; set; }

        public int BurstPeriodMinTimeMS { get; set; }

        public int BurstPeriodMaxTimeMS { get; set; }

        public int CooldownTimeMS { get; set; }

        public int WindDownTimeMS { get; set; }

        public int ReloadTimeMS { get; set; }

        public bool LineOfSightRequired { get; set; }

        public TargetAimingType LeadsTarget { get; set; }

        public bool KillSkipsUnitDeathSequence { get; set; }

        public RevealTrigger RevealTriggers { get; set; }

        public UnitStatusAttackingTrigger UnitStatusAttackingTriggers { get; set; }

        public WeaponTargetStyle TargetStyle { get; set; }

        public WeaponModifierInfo[] Modifiers { get; set; }

        public AOEFalloffType AreaOfEffectFalloffType { get; set; }

        public Fixed64 AreaOfEffectRadius { get; set; }

        public bool ExcludeWeaponOwnerFromAreaOfEffect { get; set; }

        public Fixed64 FriendlyFireDamageScalar { get; set; }

        public Fixed64 WeaponOwnerFriendlyFireDamageScalar { get; set; }

        public TurretAttributes Turret { get; set; }

        public RangeBasedWeaponAttributes[] Ranges { get; set; }

        public string ProjectileEntityTypeToSpawn { get; set; }

        public AbilityTargetAlignment StatusEffectsTargetAlignment { get; set; }

        public UnitClass StatusEffectsExcludeTargetType { get; set; }

        public int ActiveStatusEffectsIndex { get; set; }

        public StatusEffectsSetAttributes[] StatusEffectsSets { get; set; }

        public EntityTypeToSpawnAttributes[] EntityTypesToSpawnOnImpact { get; set; }

        public TargetPriorizationAttributes TargetPriorizationAttributes { get; set; }
    }
}
