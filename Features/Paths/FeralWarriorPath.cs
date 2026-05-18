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

public static class FeralWarriorPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.ImprovedUnarmedStrike.Reference.Get().Icon;

        var trance = FeatureConfigurator.New("FeralWarriorTrance", Guids.FeralWarriorTrance)
            .SetDisplayName(LocalizationTool.CreateString("PW.FeralWarriorTrance.Name", "Feral Warrior Trance"))
            .SetDescription(LocalizationTool.CreateString("PW.FeralWarriorTrance.Desc",
                "Your psionic focus channels through your body's natural weapons. " +
                "You gain a +1 competence bonus to attack rolls with natural weapons and unarmed strikes."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddComponent(new AddStatBonus
            {
                Stat = StatType.AdditionalAttackBonus,
                Value = 1,
                Descriptor = ModifierDescriptor.Competence
            })
            .Configure();

        // +7 ≈ 2d6 average; removes after next attack
        var maneuverBuff = BuffConfigurator.New("FeralWarriorManeuverBuff", Guids.FeralWarriorManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.FeralWarriorManeuver.BuffName", "Feral Warrior Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.FeralWarriorManeuver.BuffDesc",
                "Your next attack deals +7 bonus damage channeled through your feral psionic energy."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 7)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.FeralWarriorManeuverBuff))
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("FeralWarriorManeuver", Guids.FeralWarriorManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.FeralWarriorManeuverAb.Name", "Feral Warrior Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.FeralWarriorManeuverAb.Desc",
                "Swift Action. Expend psionic focus to channel feral energy into your next strike, dealing +7 bonus damage."))
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

        // +14 ≈ 4d6 average; removes after next attack
        var expandedBuff = BuffConfigurator.New("FeralWarriorExpandedManeuverBuff", Guids.FeralWarriorExpandedBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.FeralWarriorExpanded.BuffName", "Feral Warrior Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.FeralWarriorExpanded.BuffDesc",
                "Your next attack deals +14 bonus damage from unleashed feral psionic energy."))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 14)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.FeralWarriorExpandedBuff))
            .Configure();

        var expandedAbility = AbilityConfigurator.New("FeralWarriorExpandedManeuverAbility", Guids.FeralWarriorExpandedAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.FeralWarriorExpandedAb.Name", "Feral Warrior Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.FeralWarriorExpandedAb.Desc",
                "Swift Action. Expend psionic focus to channel greater feral energy into your next strike, dealing +14 bonus damage."))
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

        FeatureConfigurator.New("FeralWarriorExpandedManeuver", Guids.FeralWarriorExpandedFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.FeralWarriorExpandedFeat.Name", "Feral Warrior Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.FeralWarriorExpandedFeat.Desc",
                "Your Feral Warrior Maneuver upgrades to deal +14 bonus damage on your next attack."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.FeralWarriorExpandedAbility })
            .AddPrerequisiteFeature(Guids.FeralWarriorPath)
            .Configure();

        FeatureConfigurator.New("FeralWarriorPath", Guids.FeralWarriorPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.FeralWarriorPath.Name", "Feral Warrior Path"))
            .SetDescription(LocalizationTool.CreateString("PW.FeralWarriorPath.Desc",
                "You focus on natural weapon and unarmed combat. You gain a +1 competence bonus to attack rolls (trance) and can expend psionic focus for +7 bonus damage on your next strike (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.FeralWarriorManeuverAbility })
            .Configure();
    }
}
