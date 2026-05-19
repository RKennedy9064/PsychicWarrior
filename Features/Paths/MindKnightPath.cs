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

public static class MindKnightPath
{
    public static void Configure()
    {
        var icon = AbilityRefs.DivineFavor.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "MindKnight",
            tranceFeatureGuid: Guids.MindKnightTrance,
            tranceBuffGuid: Guids.MindKnightTranceBuff,
            tranceToggleStdGuid: Guids.MindKnightTranceToggleStd,
            tranceToggleSwiftGuid: Guids.MindKnightTranceToggleSwift,
            parentAbilityGuid: Guids.MindKnightPathParent,
            maneuverAbilityGuid: Guids.MindKnightManeuverAbility,
            expandedManeuverAbilityGuid: Guids.MindKnightExpandedAbility,
            displayName: "Mind Knight",
            featureDescription: "Your psionic focus sharpens your mental acuity in battle. You gain a +1 competence bonus to Initiative and a +1 competence bonus to attack rolls.",
            icon: icon,
            addBuffComponents: b => b
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.Initiative, value: 1)
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 1));

        var maneuverBuff = BuffConfigurator.New("MindKnightManeuverBuff", Guids.MindKnightManeuverBuff)
            .SetDisplayName(Loc.Str("PW.MindKnightManeuver.BuffName", "Mind Knight Maneuver"))
            .SetDescription(Loc.Str("PW.MindKnightManeuver.BuffDesc",
                "Your mind sharpens your blade. You gain +2 competence to attack rolls and +2 competence to damage for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("MindKnightManeuver", Guids.MindKnightManeuverAbility)
            .SetDisplayName(Loc.Str("PW.MindKnightManeuverAb.Name", "Mind Knight Maneuver"))
            .SetDescription(Loc.Str("PW.MindKnightManeuverAb.Desc",
                "Swift Action. Expend psionic focus to sharpen your mental connection to your weapon, gaining +2 competence to attack and damage for 1 round."))
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

        // Expanded — Mental Strike: +4 Initiative + +4 dodge AC for 1 round (combat awareness)
        var expandedBuff = BuffConfigurator.New("MindKnightExpandedManeuverBuff", Guids.MindKnightExpandedBuff)
            .SetDisplayName(Loc.Str("PW.MindKnightExpanded.BuffName", "Mental Strike"))
            .SetDescription(Loc.Str("PW.MindKnightExpanded.BuffDesc",
                "Heightened combat awareness: +4 Initiative and +4 dodge bonus to AC for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Insight, stat: StatType.Initiative, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Dodge, stat: StatType.AC, value: 4)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("MindKnightExpandedManeuverAbility", Guids.MindKnightExpandedAbility)
            .SetDisplayName(Loc.Str("PW.MindKnightExpandedAb.Name", "Mental Strike"))
            .SetDescription(Loc.Str("PW.MindKnightExpandedAb.Desc",
                "Swift Action. Expend psionic focus to sharpen combat awareness: +4 insight Initiative and +4 dodge bonus to AC for 1 round."))
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
            .AddAbilityShowIfCasterHasFact(not: false, unitFact: Guids.MindKnightExpandedFeature)
            .Configure();

        FeatureConfigurator.New("MindKnightExpandedManeuver", Guids.MindKnightExpandedFeature)
            .SetDisplayName(Loc.Str("PW.MindKnightExpandedFeat.Name", "Mental Strike"))
            .SetDescription(Loc.Str("PW.MindKnightExpandedFeat.Desc",
                "You learn the Mental Strike maneuver: a swift-action self-buff granting +4 insight Initiative and +4 dodge bonus to AC for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerMindKnightExpanded)
            .AddPrerequisiteFeature(Guids.MindKnightPath)
            .Configure();

        FeatureConfigurator.New("MindKnightPath", Guids.MindKnightPath)
            .SetDisplayName(Loc.Str("PW.MindKnightPath.Name", "Mind Knight Path"))
            .SetDescription(Loc.Str("PW.MindKnightPath.Desc",
                "You focus on mental precision in combat. You gain +1 competence to Initiative and attack rolls (trance) and can expend psionic focus for +2 competence to attack and damage (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString() })
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerMindKnightManeuver)
            .Configure();
    }
}
