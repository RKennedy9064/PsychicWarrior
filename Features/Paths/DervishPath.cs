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

public static class DervishPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.TwoWeaponFighting.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Dervish",
            tranceFeatureGuid: Guids.DervishTrance,
            tranceBuffGuid: Guids.DervishTranceBuff,
            tranceToggleStdGuid: Guids.DervishTranceToggleStd,
            tranceToggleSwiftGuid: Guids.DervishTranceToggleSwift,
            parentAbilityGuid: Guids.DervishPathParent,
            maneuverAbilityGuid: Guids.DervishManeuverAbility,
            expandedManeuverAbilityGuid: Guids.DervishExpandedAbility,
            displayName: "Dervish",
            featureDescription: "Your psionic focus sharpens your dual-weapon strikes. You gain a +1 competence bonus to attack rolls.",
            icon: icon,
            addBuffComponents: b => b.AddStatBonus(
                descriptor: ModifierDescriptor.Competence,
                stat: StatType.AdditionalAttackBonus,
                value: 1));

        var maneuverBuff = BuffConfigurator.New("DervishManeuverBuff", Guids.DervishManeuverBuff)
            .SetDisplayName(Loc.Str("PW.DervishManeuver.BuffName", "Dervish Maneuver"))
            .SetDescription(Loc.Str("PW.DervishManeuver.BuffDesc",
                "You enter a whirling combat stance, gaining +2 competence to attack rolls and +2 competence to damage rolls for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("DervishManeuver", Guids.DervishManeuverAbility)
            .SetDisplayName(Loc.Str("PW.DervishManeuverAb.Name", "Dervish Maneuver"))
            .SetDescription(Loc.Str("PW.DervishManeuverAb.Desc",
                "Swift Action. Expend psionic focus to enter a whirling combat stance, gaining +2 competence to attack and damage for 1 round."))
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

        // Expanded — Whirlwind Step: Haste-like spinning attack for 1 round
        var expandedBuff = BuffConfigurator.New("DervishExpandedManeuverBuff", Guids.DervishExpandedBuff)
            .CopyFrom(BuffRefs.HasteBuff)
            .SetDisplayName(Loc.Str("PW.DervishExpanded.BuffName", "Whirlwind Step"))
            .SetDescription(Loc.Str("PW.DervishExpanded.BuffDesc",
                "You spin in a deadly whirlwind: gain the benefits of haste (extra attack, +1 dodge AC, +1 Reflex, +30 ft speed)."))
            .SetIcon(icon)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("DervishExpandedManeuverAbility", Guids.DervishExpandedAbility)
            .SetDisplayName(Loc.Str("PW.DervishExpandedAb.Name", "Whirlwind Step"))
            .SetDescription(Loc.Str("PW.DervishExpandedAb.Desc",
                "Swift Action. Expend psionic focus to enter a whirlwind dance: gain haste for 1 round (extra attack on full attacks, +1 dodge AC, +1 Reflex, +30 ft speed)."))
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
            .AddAbilityShowIfCasterHasFact(not: false, unitFact: Guids.DervishExpandedFeature)
            .Configure();

        FeatureConfigurator.New("DervishExpandedManeuver", Guids.DervishExpandedFeature)
            .SetDisplayName(Loc.Str("PW.DervishExpandedFeat.Name", "Whirlwind Step"))
            .SetDescription(Loc.Str("PW.DervishExpandedFeat.Desc",
                "You learn the Whirlwind Step maneuver: a swift-action haste-like buff for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerDervishExpanded)
            .AddPrerequisiteFeature(Guids.DervishPath)
            .Configure();

        FeatureConfigurator.New("DervishPath", Guids.DervishPath)
            .SetDisplayName(Loc.Str("PW.DervishPath.Name", "Dervish Path"))
            .SetDescription(Loc.Str("PW.DervishPath.Desc",
                "You focus on swift dual-weapon mastery. You gain a +1 competence bonus to attack rolls (trance) and can expend psionic focus to enter a whirling combat stance (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString() })
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerDervishManeuver)
            .Configure();
    }
}
