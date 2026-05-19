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
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features.Paths;

public static class AsceticPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.Dodge.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Ascetic",
            tranceFeatureGuid: Guids.AsceticTrance,
            tranceBuffGuid: Guids.AsceticTranceBuff,
            tranceToggleStdGuid: Guids.AsceticTranceToggleStd,
            tranceToggleSwiftGuid: Guids.AsceticTranceToggleSwift,
            parentAbilityGuid: Guids.AsceticPathParent,
            maneuverAbilityGuid: Guids.AsceticManeuverAbility,
            expandedManeuverAbilityGuid: Guids.AsceticExpandedAbility,
            displayName: "Ascetic",
            featureDescription: "Your psionic discipline allows you to move and react with supernatural efficiency. You gain a +1 competence bonus to AC.",
            icon: icon,
            addBuffComponents: b => b.AddStatBonus(
                descriptor: ModifierDescriptor.Competence,
                stat: StatType.AC,
                value: 1));

        var maneuverBuff = BuffConfigurator.New("AsceticManeuverBuff", Guids.AsceticManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.AsceticManeuver.BuffName", "Ascetic Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.AsceticManeuver.BuffDesc",
                "You gain a +4 dodge bonus to AC for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Dodge, stat: StatType.AC, value: 4)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("AsceticManeuver", Guids.AsceticManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.AsceticManeuverAb.Name", "Ascetic Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.AsceticManeuverAb.Desc",
                "Swift Action. Expend psionic focus to enter a defensive stance, gaining +4 dodge to AC for 1 round."))
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
            .Configure();

        // Expanded — Wholeness of Body: instant self-heal equal to caster level
        // (buff retained as a stub so its GUID still registers; not applied)
        BuffConfigurator.New("AsceticExpandedManeuverBuff", Guids.AsceticExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.AsceticExpanded.BuffName", "Wholeness of Body"))
            .SetDescription(LocalizationTool.CreateString("PW.AsceticExpanded.BuffDesc", "Psychometabolic self-healing."))
            .SetIcon(icon)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("AsceticExpandedManeuverAbility", Guids.AsceticExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.AsceticExpandedAb.Name", "Wholeness of Body"))
            .SetDescription(LocalizationTool.CreateString("PW.AsceticExpandedAb.Desc",
                "Swift Action. Expend psionic focus to heal yourself for hit points equal to your manifester level."))
            .SetIcon(icon)
            .SetType(AbilityType.Extraordinary)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Swift)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityCasterHasFacts(new() { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .HealTarget(value: ContextDice.Value(DiceType.Zero, 0, ContextValues.Rank())))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddAbilityShowIfCasterHasFact(not: false, unitFact: Guids.AsceticExpandedFeature)
            .Configure();

        FeatureConfigurator.New("AsceticExpandedManeuver", Guids.AsceticExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.AsceticExpandedFeat.Name", "Wholeness of Body"))
            .SetDescription(LocalizationTool.CreateString("PW.AsceticExpandedFeat.Desc",
                "You learn the Wholeness of Body maneuver: a swift action that heals you for hit points equal to your manifester level."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddPrerequisiteFeature(Guids.AsceticPath)
            .Configure();

        FeatureConfigurator.New("AsceticPath", Guids.AsceticPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.AsceticPath.Name", "Ascetic Path"))
            .SetDescription(LocalizationTool.CreateString("PW.AsceticPath.Desc",
                "You focus on psionic body mastery. You gain a +1 competence bonus to AC (trance) and can expend psionic focus to adopt a defensive stance (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString() })
            .Configure();
    }
}
