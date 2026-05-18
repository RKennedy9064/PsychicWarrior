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

public static class InfiltratorPath
{
    public static void Configure()
    {
        var icon = AbilityRefs.CauseFear.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Infiltrator",
            tranceFeatureGuid: Guids.InfiltratorTrance,
            tranceBuffGuid: Guids.InfiltratorTranceBuff,
            tranceActivatableGuid: Guids.InfiltratorTranceActivatable,
            displayName: "Infiltrator Trance",
            featureDescription: "Your psionic focus makes you more effective at social manipulation and precision strikes. You gain a +2 competence bonus to Persuasion and +1 competence bonus to damage rolls.",
            icon: icon,
            addBuffComponents: b => b
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SkillPersuasion, value: 2)
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 1));

        var maneuverBuff = BuffConfigurator.New("InfiltratorManeuverBuff", Guids.InfiltratorManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.InfiltratorManeuver.BuffName", "Infiltrator Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InfiltratorManeuver.BuffDesc",
                "Your psionic aura radiates menace. You gain +4 competence to Persuasion and +2 competence to damage for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SkillPersuasion, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("InfiltratorManeuver", Guids.InfiltratorManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.InfiltratorManeuverAb.Name", "Infiltrator Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InfiltratorManeuverAb.Desc",
                "Swift Action. Expend psionic focus to project a menacing aura, gaining +4 competence to Persuasion and +2 competence to damage for 1 round."))
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
            .AddAbilityShowIfCasterHasFact(not: true, unitFact: Guids.InfiltratorExpandedFeature)
            .Configure();

        var expandedBuff = BuffConfigurator.New("InfiltratorExpandedManeuverBuff", Guids.InfiltratorExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.InfiltratorExpanded.BuffName", "Infiltrator Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InfiltratorExpanded.BuffDesc",
                "Enhanced menacing aura: +6 competence to Persuasion and +4 competence to damage for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SkillPersuasion, value: 6)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 4)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("InfiltratorExpandedManeuverAbility", Guids.InfiltratorExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.InfiltratorExpandedAb.Name", "Infiltrator Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InfiltratorExpandedAb.Desc",
                "Swift Action. Expend psionic focus to project an enhanced menacing aura, gaining +6 competence to Persuasion and +4 competence to damage for 1 round."))
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

        FeatureConfigurator.New("InfiltratorExpandedManeuver", Guids.InfiltratorExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.InfiltratorExpandedFeat.Name", "Infiltrator Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InfiltratorExpandedFeat.Desc",
                "Your Infiltrator Maneuver upgrades to grant +6 competence to Persuasion and +4 competence to damage for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.InfiltratorExpandedAbility })
            .AddPrerequisiteFeature(Guids.InfiltratorPath)
            .Configure();

        FeatureConfigurator.New("InfiltratorPath", Guids.InfiltratorPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.InfiltratorPath.Name", "Infiltrator Path"))
            .SetDescription(LocalizationTool.CreateString("PW.InfiltratorPath.Desc",
                "You focus on deception and precise lethal strikes. You gain +2 competence to Persuasion and +1 to damage (trance), and can expend psionic focus for enhanced menace (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.InfiltratorManeuverAbility })
            .Configure();
    }
}
