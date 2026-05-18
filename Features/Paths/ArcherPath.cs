using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features.Paths;

public static class ArcherPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.PointBlankShot.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Archer",
            tranceFeatureGuid: Guids.ArcherTrance,
            tranceBuffGuid: Guids.ArcherTranceBuff,
            tranceActivatableGuid: Guids.ArcherTranceActivatable,
            displayName: "Archer Trance",
            featureDescription: "Your psionic focus steadies your aim. You gain a +1 competence bonus to attack rolls.",
            icon: icon,
            addBuffComponents: b => b.AddStatBonus(
                descriptor: ModifierDescriptor.Competence,
                stat: StatType.AdditionalAttackBonus,
                value: 1));

        var maneuverBuff = BuffConfigurator.New("ArcherManeuverBuff", Guids.ArcherManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.ArcherManeuver.BuffName", "Archer Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.ArcherManeuver.BuffDesc",
                "Your next attack gains a +4 competence bonus to the attack roll."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 4)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.ArcherManeuverBuff))
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("ArcherManeuver", Guids.ArcherManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.ArcherManeuverAb.Name", "Archer Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.ArcherManeuverAb.Desc",
                "Swift Action. Expend psionic focus to channel precision into your next attack, gaining a +4 competence bonus to the attack roll."))
            .SetIcon(icon)
            .SetType(AbilityType.Extraordinary)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Swift)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityCasterHasFacts(new() { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(maneuverBuff, ContextDuration.Fixed(1)))
            .AddAbilityShowIfCasterHasFact(not: true, unitFact: Guids.ArcherExpandedFeature)
            .Configure();

        var expandedBuff = BuffConfigurator.New("ArcherExpandedManeuverBuff", Guids.ArcherExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.ArcherExpanded.BuffName", "Archer Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.ArcherExpanded.BuffDesc",
                "You gain a +4 competence bonus to CMB for combat maneuvers for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalCMB, value: 4)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("ArcherExpandedManeuverAbility", Guids.ArcherExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.ArcherExpandedAb.Name", "Archer Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.ArcherExpandedAb.Desc",
                "Swift Action. Expend psionic focus to gain a +4 competence bonus to CMB for combat maneuvers for 1 round."))
            .SetIcon(icon)
            .SetType(AbilityType.Extraordinary)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Swift)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityCasterHasFacts(new() { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(expandedBuff, ContextDuration.Fixed(1)))
            .Configure();

        FeatureConfigurator.New("ArcherExpandedManeuver", Guids.ArcherExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.ArcherExpandedFeat.Name", "Archer Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.ArcherExpandedFeat.Desc",
                "Your Archer Maneuver upgrades: expend psionic focus to gain +4 competence to CMB for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.ArcherExpandedAbility })
            .AddPrerequisiteFeature(Guids.ArcherPath)
            .Configure();

        FeatureConfigurator.New("ArcherPath", Guids.ArcherPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.ArcherPath.Name", "Archer Path"))
            .SetDescription(LocalizationTool.CreateString("PW.ArcherPath.Desc",
                "You focus on ranged precision. You gain a +1 competence bonus to attack rolls (trance) and can expend psionic focus to enhance your next attack (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.ArcherManeuverAbility })
            .Configure();
    }
}
