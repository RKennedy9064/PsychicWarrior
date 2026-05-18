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

        var trance = FeatureConfigurator.New("MindKnightTrance", Guids.MindKnightTrance)
            .SetDisplayName(LocalizationTool.CreateString("PW.MindKnightTrance.Name", "Mind Knight Trance"))
            .SetDescription(LocalizationTool.CreateString("PW.MindKnightTrance.Desc",
                "Your psionic focus sharpens your mental acuity in battle. " +
                "You gain a +1 competence bonus to Initiative and a +1 competence bonus to attack rolls."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddComponent(new AddStatBonus
            {
                Stat = StatType.Initiative,
                Value = 1,
                Descriptor = ModifierDescriptor.Competence
            })
            .AddComponent(new AddStatBonus
            {
                Stat = StatType.AdditionalAttackBonus,
                Value = 1,
                Descriptor = ModifierDescriptor.Competence
            })
            .Configure();

        var maneuverBuff = BuffConfigurator.New("MindKnightManeuverBuff", Guids.MindKnightManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.MindKnightManeuver.BuffName", "Mind Knight Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.MindKnightManeuver.BuffDesc",
                "Your mind sharpens your blade. You gain +2 competence to attack rolls and +2 competence to damage for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("MindKnightManeuver", Guids.MindKnightManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.MindKnightManeuverAb.Name", "Mind Knight Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.MindKnightManeuverAb.Desc",
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

        var expandedBuff = BuffConfigurator.New("MindKnightExpandedManeuverBuff", Guids.MindKnightExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.MindKnightExpanded.BuffName", "Mind Knight Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.MindKnightExpanded.BuffDesc",
                "Enhanced mental focus: +4 competence to attack rolls and +4 competence to damage for 1 round."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 4)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("MindKnightExpandedManeuverAbility", Guids.MindKnightExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.MindKnightExpandedAb.Name", "Mind Knight Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.MindKnightExpandedAb.Desc",
                "Swift Action. Expend psionic focus to forge a deeper mental connection to your weapon, gaining +4 competence to attack and damage for 1 round."))
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

        FeatureConfigurator.New("MindKnightExpandedManeuver", Guids.MindKnightExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.MindKnightExpandedFeat.Name", "Mind Knight Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.MindKnightExpandedFeat.Desc",
                "Your Mind Knight Maneuver upgrades to grant +4 competence to attack and damage for 1 round."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.MindKnightExpandedAbility })
            .AddPrerequisiteFeature(Guids.MindKnightPath)
            .Configure();

        FeatureConfigurator.New("MindKnightPath", Guids.MindKnightPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.MindKnightPath.Name", "Mind Knight Path"))
            .SetDescription(LocalizationTool.CreateString("PW.MindKnightPath.Desc",
                "You focus on mental precision in combat. You gain +1 competence to Initiative and attack rolls (trance) and can expend psionic focus for +2 competence to attack and damage (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.MindKnightManeuverAbility })
            .Configure();
    }
}
