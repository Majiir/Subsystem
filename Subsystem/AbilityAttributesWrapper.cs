using BBI.Core;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;

namespace Subsystem
{
    public class AbilityAttributesWrapper : NamedObjectBase, AbilityAttributes
    {
        public AbilityAttributesWrapper(AbilityAttributes other) : base(other.Name)
        {
            AbilityMapTargetLayers = other.AbilityMapTargetLayers;
            AbilityType = other.AbilityType;
            ActivationDependencies = other.ActivationDependencies;
            AirSortie = other.AirSortie;
            ApplyStatusEffect = other.ApplyStatusEffect;
            Autocast = other.Autocast;
            AutoToggleOffConditions = other.AutoToggleOffConditions;
            CasterMovesToTarget = other.CasterMovesToTarget;
            CastOnDeath = other.CastOnDeath;
            ChainCasting = other.ChainCasting;
            ChangeContext = other.ChangeContext;
            Charges = other.Charges;
            CooldownTimeSecs = other.CooldownTimeSecs;
            Cost = other.Cost;
            EdgeOfTargetShapeMinDistance = other.EdgeOfTargetShapeMinDistance;
            GroundAutoTargetAlignment = other.GroundAutoTargetAlignment;
            GroupActivationType = other.GroupActivationType;
            IsToggleable = other.IsToggleable;
            ModifyInventory = other.ModifyInventory;
            ProduceUnit = other.ProduceUnit;
            Repair = other.Repair;
            ResearchDependencies = other.ResearchDependencies;
            Salvage = other.Salvage;
            SelfDestruct = other.SelfDestruct;
            SharedCooldownChannel = other.SharedCooldownChannel;
            SkipCastOnArrivalConditions = other.SkipCastOnArrivalConditions;
            StartsRemovedInGameMode = other.StartsRemovedInGameMode;
            TargetAlignment = other.TargetAlignment;
            TargetHighlighting = other.TargetHighlighting;
            TargetingType = other.TargetingType;
            UseWeapon = other.UseWeapon;
            WarmupTimeSecs = other.WarmupTimeSecs;
        }

        public AbilityClass AbilityType { get; set; }

        public AbilityTargetingType TargetingType { get; set; }

        public AbilityTargetAlignment TargetAlignment { get; set; }

        public UnitClass AbilityMapTargetLayers { get; set; }

        public GroundAutoTargetAlignmentType GroundAutoTargetAlignment { get; set; }

        public Fixed64 EdgeOfTargetShapeMinDistance { get; set; }

        public TargetHighlightingAttributes TargetHighlighting { get; set; }

        public bool CasterMovesToTarget { get; set; }

        public AbilityGroupActivationType GroupActivationType { get; set; }

        public AbilityRemovedInMode StartsRemovedInGameMode { get; set; }

        public Fixed64 CooldownTimeSecs { get; set; }

        public Fixed64 WarmupTimeSecs { get; set; }

        public int SharedCooldownChannel { get; set; }

        public SkipCastOnArrivalConditions SkipCastOnArrivalConditions { get; set; }

        public bool IsToggleable { get; set; }

        public bool CastOnDeath { get; set; }

        public CostAttributes Cost { get; set; }

        public ChargeAttributes Charges { get; set; }

        public AutocastAttributes Autocast { get; set; }

        public ProduceUnitAttributes ProduceUnit { get; set; }

        public UseWeaponAttributes UseWeapon { get; set; }

        public ChangeContextAttributes ChangeContext { get; set; }

        public AirSortieAttributes AirSortie { get; set; }

        public SalvageAttributes Salvage { get; set; }

        public ApplyStatusEffectAttributes ApplyStatusEffect { get; set; }

        public RepairAttributes Repair { get; set; }

        public SelfDestructAttributes SelfDestruct { get; set; }

        public ModifyInventoryAttributes ModifyInventory { get; set; }

        public ResearchDependenciesAttributes ResearchDependencies { get; set; }

        public ActivationDependenciesAttributes ActivationDependencies { get; set; }

        public AutoToggleOffConditionsAttributes AutoToggleOffConditions { get; set; }

        public ChainCastingAttributes ChainCasting { get; set; }
    }
}
