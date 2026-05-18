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
            tranceActivatableGuid: Guids.DervishTranceActivatable,
            displayName: "Dervish Trance",
            featureDescription: "Your psionic focus sharpens your dual-weapon strikes. You gain a +1 competence bonus to attack rolls.",
            icon: icon,
            addBuffComponents: b => b.AddStatBonus(
                descriptor: ModifierDescriptor.Competence,
                stat: StatType.AdditionalAttackBonus,
                value: 1));

        var maneuverBuff = BuffConfigurator.New("DervishManeuverBuff", Guids.DervishManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.DervishManeuver.BuffName", "Dervish Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.DervishManeuver.BuffDesc",
                "You enter a whirling combat stance, gaining +2 competence to attack rolls and +2 competence to damage rolls for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("DervishManeuver", Guids.DervishManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.DervishManeuverAb.Name", "Dervish Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.DervishManeuverAb.Desc",
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
            .AddAbilityShowIfCasterHasFact(not: true, unitFact: Guids.DervishExpandedFeature)
            .Configure();

        var expandedBuff = BuffConfigurator.New("DervishExpandedManeuverBuff", Guids.DervishExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.DervishExpanded.BuffName", "Dervish Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.DervishExpanded.BuffDesc",
                "Enhanced whirling stance: +4 competence to attack rolls and +4 competence to damage rolls for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 4)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("DervishExpandedManeuverAbility", Guids.DervishExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.DervishExpandedAb.Name", "Dervish Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.DervishExpandedAb.Desc",
                "Swift Action. Expend psionic focus to enter an enhanced whirling stance, gaining +4 competence to attack and damage for 1 round."))
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

        FeatureConfigurator.New("DervishExpandedManeuver", Guids.DervishExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.DervishExpandedFeat.Name", "Dervish Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.DervishExpandedFeat.Desc",
                "Your Dervish Maneuver upgrades to grant +4 competence to attack and damage for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.DervishExpandedAbility })
            .AddPrerequisiteFeature(Guids.DervishPath)
            .Configure();

        FeatureConfigurator.New("DervishPath", Guids.DervishPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.DervishPath.Name", "Dervish Path"))
            .SetDescription(LocalizationTool.CreateString("PW.DervishPath.Desc",
                "You focus on swift dual-weapon mastery. You gain a +1 competence bonus to attack rolls (trance) and can expend psionic focus to enter a whirling combat stance (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.DervishManeuverAbility })
            .Configure();
    }
}
