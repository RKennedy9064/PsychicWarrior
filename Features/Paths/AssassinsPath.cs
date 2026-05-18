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
            tranceActivatableGuid: Guids.AssassinsTranceActivatable,
            displayName: "Assassin's Trance",
            featureDescription: "Your psionic focus sharpens your strikes against unsuspecting enemies. You gain a +2 competence bonus to damage rolls.",
            icon: icon,
            addBuffComponents: b => b.AddStatBonus(
                descriptor: ModifierDescriptor.Competence,
                stat: StatType.AdditionalDamage,
                value: 2));

        // +7 ≈ 2d6 average; removes after next attack
        var maneuverBuff = BuffConfigurator.New("AssassinsManeuverBuff", Guids.AssassinsManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.AssassinsManeuver.BuffName", "Assassin's Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.AssassinsManeuver.BuffDesc",
                "Your next attack deals +7 bonus damage from precision psionic energy."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 7)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.AssassinsManeuverBuff))
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("AssassinsManeuver", Guids.AssassinsManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.AssassinsManeuverAb.Name", "Assassin's Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.AssassinsManeuverAb.Desc",
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
            .AddAbilityShowIfCasterHasFact(not: true, unitFact: Guids.AssassinsExpandedFeature)
            .Configure();

        // +14 ≈ 4d6 average; removes after next attack
        var expandedBuff = BuffConfigurator.New("AssassinsExpandedManeuverBuff", Guids.AssassinsExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.AssassinsExpanded.BuffName", "Assassin's Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.AssassinsExpanded.BuffDesc",
                "Your next attack deals +14 bonus damage from precision psionic energy."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 14)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.AssassinsExpandedBuff))
            .Configure();

        var expandedAbility = AbilityConfigurator.New("AssassinsExpandedManeuverAbility", Guids.AssassinsExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.AssassinsExpandedAb.Name", "Assassin's Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.AssassinsExpandedAb.Desc",
                "Swift Action. Expend psionic focus to channel greater precision energy into your next strike, dealing +14 bonus damage."))
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

        FeatureConfigurator.New("AssassinsExpandedManeuver", Guids.AssassinsExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.AssassinsExpandedFeat.Name", "Assassin's Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.AssassinsExpandedFeat.Desc",
                "Your Assassin's Maneuver upgrades to deal +14 bonus damage on your next attack."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.AssassinsExpandedAbility })
            .AddPrerequisiteFeature(Guids.AssassinsPath)
            .Configure();

        FeatureConfigurator.New("AssassinsPath", Guids.AssassinsPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.AssassinsPath.Name", "Assassin's Path"))
            .SetDescription(LocalizationTool.CreateString("PW.AssassinsPath.Desc",
                "You focus on dealing deadly precision damage. You gain a permanent +2 competence bonus to damage rolls (trance) " +
                "and can expend psionic focus for +7 bonus damage on your next strike (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.AssassinsManeuverAbility })
            .Configure();
    }
}
