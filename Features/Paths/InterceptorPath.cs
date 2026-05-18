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

public static class InterceptorPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.CombatReflexes.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Interceptor",
            tranceFeatureGuid: Guids.InterceptorTrance,
            tranceBuffGuid: Guids.InterceptorTranceBuff,
            tranceActivatableGuid: Guids.InterceptorTranceActivatable,
            displayName: "Interceptor Trance",
            featureDescription: "Your psionic focus heightens your protective instincts. You gain a +1 competence bonus to attack and damage rolls.",
            icon: icon,
            addBuffComponents: b => b
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 1)
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 1));

        var maneuverBuff = BuffConfigurator.New("InterceptorManeuverBuff", Guids.InterceptorManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.InterceptorManeuver.BuffName", "Interceptor Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InterceptorManeuver.BuffDesc",
                "You enter a counter-attack stance, gaining +2 competence to attack and +2 competence to damage for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("InterceptorManeuver", Guids.InterceptorManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.InterceptorManeuverAb.Name", "Interceptor Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InterceptorManeuverAb.Desc",
                "Swift Action. Expend psionic focus to enter a protective counter-attack stance, gaining +2 competence to attack and damage for 1 round."))
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
            .AddAbilityShowIfCasterHasFact(not: true, unitFact: Guids.InterceptorExpandedFeature)
            .Configure();

        var expandedBuff = BuffConfigurator.New("InterceptorExpandedManeuverBuff", Guids.InterceptorExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.InterceptorExpanded.BuffName", "Interceptor Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InterceptorExpanded.BuffDesc",
                "Enhanced counter-attack stance: +4 competence to attack rolls and +4 competence to damage for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 4)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("InterceptorExpandedManeuverAbility", Guids.InterceptorExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.InterceptorExpandedAb.Name", "Interceptor Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InterceptorExpandedAb.Desc",
                "Swift Action. Expend psionic focus to enter an enhanced counter-attack stance, gaining +4 competence to attack and damage for 1 round."))
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

        FeatureConfigurator.New("InterceptorExpandedManeuver", Guids.InterceptorExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.InterceptorExpandedFeat.Name", "Interceptor Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.InterceptorExpandedFeat.Desc",
                "Your Interceptor Maneuver upgrades to grant +4 competence to attack and damage for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.InterceptorExpandedAbility })
            .AddPrerequisiteFeature(Guids.InterceptorPath)
            .Configure();

        FeatureConfigurator.New("InterceptorPath", Guids.InterceptorPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.InterceptorPath.Name", "Interceptor Path"))
            .SetDescription(LocalizationTool.CreateString("PW.InterceptorPath.Desc",
                "You focus on protecting allies through aggressive counter-attacks. You gain +1 competence to attack and damage (trance) and can expend psionic focus for a +2 bonus to both (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.InterceptorManeuverAbility })
            .Configure();
    }
}
