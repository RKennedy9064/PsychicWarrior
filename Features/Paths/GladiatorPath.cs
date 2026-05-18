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

public static class GladiatorPath
{
    public static void Configure()
    {
        var icon = AbilityRefs.BullsStrength.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Gladiator",
            tranceFeatureGuid: Guids.GladiatorTrance,
            tranceBuffGuid: Guids.GladiatorTranceBuff,
            tranceActivatableGuid: Guids.GladiatorTranceActivatable,
            displayName: "Gladiator Trance",
            featureDescription: "Your psionic focus enhances your combat maneuver capabilities. You gain a +2 competence bonus to CMB.",
            icon: icon,
            addBuffComponents: b => b.AddStatBonus(
                descriptor: ModifierDescriptor.Competence,
                stat: StatType.AdditionalCMB,
                value: 2));

        var maneuverBuff = BuffConfigurator.New("GladiatorManeuverBuff", Guids.GladiatorManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.GladiatorManeuver.BuffName", "Gladiator Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.GladiatorManeuver.BuffDesc",
                "You gain a +4 competence bonus to CMB for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalCMB, value: 4)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("GladiatorManeuver", Guids.GladiatorManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.GladiatorManeuverAb.Name", "Gladiator Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.GladiatorManeuverAb.Desc",
                "Swift Action. Expend psionic focus to gain a +4 competence bonus to CMB for 1 round, enhancing trip, disarm, and other maneuvers."))
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
            .AddAbilityShowIfCasterHasFact(not: true, unitFact: Guids.GladiatorExpandedFeature)
            .Configure();

        var expandedBuff = BuffConfigurator.New("GladiatorExpandedManeuverBuff", Guids.GladiatorExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.GladiatorExpanded.BuffName", "Gladiator Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.GladiatorExpanded.BuffDesc",
                "You gain a +6 competence bonus to CMB for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalCMB, value: 6)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("GladiatorExpandedManeuverAbility", Guids.GladiatorExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.GladiatorExpandedAb.Name", "Gladiator Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.GladiatorExpandedAb.Desc",
                "Swift Action. Expend psionic focus to gain a +6 competence bonus to CMB for 1 round."))
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

        FeatureConfigurator.New("GladiatorExpandedManeuver", Guids.GladiatorExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.GladiatorExpandedFeat.Name", "Gladiator Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.GladiatorExpandedFeat.Desc",
                "Your Gladiator Maneuver upgrades to grant +6 competence to CMB for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.GladiatorExpandedAbility })
            .AddPrerequisiteFeature(Guids.GladiatorPath)
            .Configure();

        FeatureConfigurator.New("GladiatorPath", Guids.GladiatorPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.GladiatorPath.Name", "Gladiator Path"))
            .SetDescription(LocalizationTool.CreateString("PW.GladiatorPath.Desc",
                "You focus on combat maneuvers and arena fighting. You gain a permanent +2 competence bonus to CMB (trance) and can expend psionic focus for an additional +4 to CMB (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.GladiatorManeuverAbility })
            .Configure();
    }
}
