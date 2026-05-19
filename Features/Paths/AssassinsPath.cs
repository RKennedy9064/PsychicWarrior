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

public static class AssassinsPath
{
    public static void Configure()
    {
        var icon = AbilityRefs.VampiricTouch.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Assassins",
            tranceFeatureGuid: Guids.AssassinsTrance,
            tranceBuffGuid: Guids.AssassinsTranceBuff,
            tranceToggleStdGuid: Guids.AssassinsTranceToggleStd,
            tranceToggleSwiftGuid: Guids.AssassinsTranceToggleSwift,
            parentAbilityGuid: Guids.AssassinsPathParent,
            maneuverAbilityGuid: Guids.AssassinsManeuverAbility,
            expandedManeuverAbilityGuid: Guids.AssassinsExpandedAbility,
            displayName: "Assassin's",
            featureDescription: "Your psionic focus sharpens your strikes against unsuspecting enemies. You gain a +2 competence bonus to damage rolls.",
            icon: icon,
            addBuffComponents: b => b.AddStatBonus(
                descriptor: ModifierDescriptor.Competence,
                stat: StatType.AdditionalDamage,
                value: 2));

        // +7 ≈ 2d6 average; removes after next attack
        var maneuverBuff = BuffConfigurator.New("AssassinsManeuverBuff", Guids.AssassinsManeuverBuff)
            .SetDisplayName(Loc.Str("PW.AssassinsManeuver.BuffName", "Assassin's Maneuver"))
            .SetDescription(Loc.Str("PW.AssassinsManeuver.BuffDesc",
                "Your next attack deals +7 bonus damage from precision psionic energy."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 7)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.AssassinsManeuverBuff))
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("AssassinsManeuver", Guids.AssassinsManeuverAbility)
            .SetDisplayName(Loc.Str("PW.AssassinsManeuverAb.Name", "Assassin's Maneuver"))
            .SetDescription(Loc.Str("PW.AssassinsManeuverAb.Desc",
                "Swift Action. Expend psionic focus to channel precision energy into your next strike, dealing +7 bonus damage."))
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

        // Expanded — Mindblade Strike: +4 saves and +4 competence damage for 1 round (mind-over-body focus)
        var expandedBuff = BuffConfigurator.New("AssassinsExpandedManeuverBuff", Guids.AssassinsExpandedBuff)
            .SetDisplayName(Loc.Str("PW.AssassinsExpanded.BuffName", "Mindblade Strike"))
            .SetDescription(Loc.Str("PW.AssassinsExpanded.BuffDesc",
                "Pure mental focus: +4 competence damage, +4 competence to all saves."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SaveFortitude, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SaveReflex, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SaveWill, value: 4)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("AssassinsExpandedManeuverAbility", Guids.AssassinsExpandedAbility)
            .SetDisplayName(Loc.Str("PW.AssassinsExpandedAb.Name", "Mindblade Strike"))
            .SetDescription(Loc.Str("PW.AssassinsExpandedAb.Desc",
                "Swift Action. Expend psionic focus and enter a state of pure mental focus: +4 competence damage and +4 competence to all saves for 1 round."))
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
            .AddAbilityShowIfCasterHasFact(not: false, unitFact: Guids.AssassinsExpandedFeature)
            .Configure();

        FeatureConfigurator.New("AssassinsExpandedManeuver", Guids.AssassinsExpandedFeature)
            .SetDisplayName(Loc.Str("PW.AssassinsExpandedFeat.Name", "Mindblade Strike"))
            .SetDescription(Loc.Str("PW.AssassinsExpandedFeat.Desc",
                "You learn the Mindblade Strike maneuver: a swift-action self-buff granting +4 competence damage and +4 competence to all saves for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerAssassinsExpanded)
            .AddPrerequisiteFeature(Guids.AssassinsPath)
            .Configure();

        FeatureConfigurator.New("AssassinsPath", Guids.AssassinsPath)
            .SetDisplayName(Loc.Str("PW.AssassinsPath.Name", "Assassin's Path"))
            .SetDescription(Loc.Str("PW.AssassinsPath.Desc",
                "You focus on dealing deadly precision damage. You gain a permanent +2 competence bonus to damage rolls (trance) " +
                "and can expend psionic focus for +7 bonus damage on your next strike (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString() })
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerAssassinsManeuver)
            .Configure();
    }
}
