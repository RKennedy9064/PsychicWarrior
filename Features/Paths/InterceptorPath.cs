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
        var maneuverIcon = AbilityRefs.DivineFavor.Reference.Get().Icon;
        var expandedIcon = AbilityRefs.StoneskinCommunal.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Interceptor",
            tranceFeatureGuid: Guids.InterceptorTrance,
            tranceBuffGuid: Guids.InterceptorTranceBuff,
            tranceToggleStdGuid: Guids.InterceptorTranceToggleStd,
            tranceToggleSwiftGuid: Guids.InterceptorTranceToggleSwift,
            parentAbilityGuid: Guids.InterceptorPathParent,
            maneuverAbilityGuid: Guids.InterceptorManeuverAbility,
            expandedManeuverAbilityGuid: Guids.InterceptorExpandedAbility,
            displayName: "Interceptor",
            featureDescription: "Your psionic focus heightens your protective instincts. You gain a +1 competence bonus to attack and damage rolls.",
            icon: icon,
            addBuffComponents: b => b
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 1)
                .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 1));

        var maneuverBuff = BuffConfigurator.New("InterceptorManeuverBuff", Guids.InterceptorManeuverBuff)
            .SetDisplayName(Loc.Str("PW.InterceptorManeuver.BuffName", "Interceptor Maneuver"))
            .SetDescription(Loc.Str("PW.InterceptorManeuver.BuffDesc",
                "You enter a counter-attack stance, gaining +2 competence to attack and +2 competence to damage for 1 round."))
            .SetIcon(maneuverIcon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("InterceptorManeuver", Guids.InterceptorManeuverAbility)
            .SetDisplayName(Loc.Str("PW.InterceptorManeuverAb.Name", "Interceptor Maneuver"))
            .SetDescription(Loc.Str("PW.InterceptorManeuverAb.Desc",
                "Swift Action. Expend psionic focus to enter a protective counter-attack stance, gaining +2 competence to attack and damage for 1 round."))
            .SetIcon(maneuverIcon)
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

        // Expanded — Save Another: DR 5/— for 1 round (take damage for allies)
        var expandedBuff = BuffConfigurator.New("InterceptorExpandedManeuverBuff", Guids.InterceptorExpandedBuff)
            .SetDisplayName(Loc.Str("PW.InterceptorExpanded.BuffName", "Save Another"))
            .SetDescription(Loc.Str("PW.InterceptorExpanded.BuffDesc",
                "You harden your body to absorb blows meant for allies: DR 5/— for 1 round."))
            .SetIcon(expandedIcon)
            .AddComponent(new AddDamageResistancePhysical { Value = 5, BypassedByMaterial = false })
            .Configure();

        var expandedAbility = AbilityConfigurator.New("InterceptorExpandedManeuverAbility", Guids.InterceptorExpandedAbility)
            .SetDisplayName(Loc.Str("PW.InterceptorExpandedAb.Name", "Save Another"))
            .SetDescription(Loc.Str("PW.InterceptorExpandedAb.Desc",
                "Swift Action. Expend psionic focus to harden your body and absorb blows meant for allies: gain DR 5/— for 1 round."))
            .SetIcon(expandedIcon)
            .SetType(AbilityType.Extraordinary)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Swift)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityCasterHasFacts(new() { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(expandedBuff, ContextDuration.Fixed(1)))
            .AddAbilityShowIfCasterHasFact(not: false, unitFact: Guids.InterceptorExpandedFeature)
            .Configure();

        FeatureConfigurator.New("InterceptorExpandedManeuver", Guids.InterceptorExpandedFeature)
            .SetDisplayName(Loc.Str("PW.InterceptorExpandedFeat.Name", "Save Another"))
            .SetDescription(Loc.Str("PW.InterceptorExpandedFeat.Desc",
                "You learn the Save Another maneuver: a swift-action self-buff granting DR 5/— for 1 round."))
            .SetIcon(expandedIcon)
            .SetIsClassFeature()
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerInterceptorExpanded)
            .AddPrerequisiteFeature(Guids.InterceptorPath)
            .Configure();

        FeatureConfigurator.New("InterceptorPath", Guids.InterceptorPath)
            .SetDisplayName(Loc.Str("PW.InterceptorPath.Name", "Interceptor Path"))
            .SetDescription(Loc.Str("PW.InterceptorPath.Desc",
                "You focus on protecting allies through aggressive counter-attacks. You gain +1 competence to attack and damage (trance) and can expend psionic focus for a +2 bonus to both (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString() })
            .AddFeatureIfHasFact(checkedFact: Guids.MartialPowerFeature, feature: Guids.MartialPowerInterceptorManeuver)
            .Configure();
    }
}
