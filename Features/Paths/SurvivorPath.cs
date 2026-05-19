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
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features.Paths;

public static class SurvivorPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.Toughness.Reference.Get().Icon;

        var trance = TranceHelper.BuildTrance(
            baseName: "Survivor",
            tranceFeatureGuid: Guids.SurvivorTrance,
            tranceBuffGuid: Guids.SurvivorTranceBuff,
            tranceToggleStdGuid: Guids.SurvivorTranceToggleStd,
            tranceToggleSwiftGuid: Guids.SurvivorTranceToggleSwift,
            parentAbilityGuid: Guids.SurvivorPathParent,
            maneuverAbilityGuid: Guids.SurvivorManeuverAbility,
            expandedManeuverAbilityGuid: Guids.SurvivorExpandedAbility,
            displayName: "Survivor",
            featureDescription: "Your psionic focus toughens your body against physical harm. You gain DR 2/—.",
            icon: icon,
            addBuffComponents: b => b.AddComponent(new AddDamageResistancePhysical { Value = 2, BypassedByMaterial = false }));

        // Mettle: Fort/Will saves spike for 1 round
        var maneuverBuff = BuffConfigurator.New("SurvivorManeuverBuff", Guids.SurvivorManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.SurvivorManeuver.BuffName", "Survivor Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.SurvivorManeuver.BuffDesc",
                "You steel yourself against magic and harm, gaining +4 competence to Fortitude and Will saves for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SaveFortitude, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SaveWill, value: 4)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("SurvivorManeuver", Guids.SurvivorManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.SurvivorManeuverAb.Name", "Survivor Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.SurvivorManeuverAb.Desc",
                "Swift Action. Expend psionic focus to steel yourself against magic and physical harm, gaining +4 competence to Fortitude and Will saves for 1 round."))
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

        // Expanded — Survivor's Resolve: temp HP equal to caster level for 1 minute
        var expandedBuff = BuffConfigurator.New("SurvivorExpandedManeuverBuff", Guids.SurvivorExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.SurvivorExpanded.BuffName", "Survivor's Resolve"))
            .SetDescription(LocalizationTool.CreateString("PW.SurvivorExpanded.BuffDesc",
                "Psionic resolve hardens flesh — temporary hit points equal to your manifester level."))
            .SetIcon(icon)
            .AddTemporaryHitPointsFromAbilityValue(
                descriptor: ModifierDescriptor.UntypedStackable,
                value: ContextValues.Rank())
            .Configure();

        var expandedAbility = AbilityConfigurator.New("SurvivorExpandedManeuverAbility", Guids.SurvivorExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.SurvivorExpandedAb.Name", "Survivor's Resolve"))
            .SetDescription(LocalizationTool.CreateString("PW.SurvivorExpandedAb.Desc",
                "Swift Action. Expend psionic focus to gain temporary hit points equal to your manifester level (1 minute)."))
            .SetIcon(icon)
            .SetType(AbilityType.Extraordinary)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Swift)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityCasterHasFacts(new() { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(expandedBuff, ContextDuration.Fixed(1, DurationRate.Minutes)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddAbilityShowIfCasterHasFact(not: false, unitFact: Guids.SurvivorExpandedFeature)
            .Configure();

        FeatureConfigurator.New("SurvivorExpandedManeuver", Guids.SurvivorExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.SurvivorExpandedFeat.Name", "Survivor's Resolve"))
            .SetDescription(LocalizationTool.CreateString("PW.SurvivorExpandedFeat.Desc",
                "You learn the Survivor's Resolve maneuver: a swift-action self-buff granting temporary hit points equal to your manifester level for 1 minute."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddPrerequisiteFeature(Guids.SurvivorPath)
            .Configure();

        FeatureConfigurator.New("SurvivorPath", Guids.SurvivorPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.SurvivorPath.Name", "Survivor Path"))
            .SetDescription(LocalizationTool.CreateString("PW.SurvivorPath.Desc",
                "You focus on endurance and resilience. You gain DR 2/— while psionically focused (trance) and can expend psionic focus to spike your Fortitude and Will saves (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString() })
            .Configure();
    }
}
