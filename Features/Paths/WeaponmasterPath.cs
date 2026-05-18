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

public static class WeaponmasterPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.PowerAttackFeature.Reference.Get().Icon;
        var maneuverIcon = AbilityRefs.DivineFavor.Reference.Get().Icon;

        // Trance: +1 competence bonus to attack rolls (passive, always active while focused)
        var trance = FeatureConfigurator.New("WeaponmasterTrance", Guids.WeaponmasterTrance)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMTrance.Name", "Weaponmaster Trance"))
            .SetDescription(LocalizationTool.CreateString("PW.WMTrance.Desc",
                "You gain a +1 competence bonus to attack rolls."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddComponent(new AddStatBonus
            {
                Stat = StatType.AdditionalAttackBonus,
                Value = 1,
                Descriptor = ModifierDescriptor.Competence
            })
            .Configure();

        // Maneuver buff: +2 dodge to AC + +2 competence to next attack (riposte spirit)
        var maneuverBuff = BuffConfigurator.New("WeaponmasterManeuverBuff", Guids.WeaponmasterManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMManeuver.BuffName", "Weaponmaster Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMManeuver.BuffDesc",
                "You are in a counter-attack stance, gaining +2 dodge to AC and +2 competence to your next attack roll."))
            .SetIcon(maneuverIcon)
            .AddStatBonus(descriptor: ModifierDescriptor.Dodge, stat: StatType.AC, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .Configure();

        // Maneuver ability: swift, expend focus, apply buff 1 round
        var maneuverAbility = AbilityConfigurator.New("WeaponmasterManeuver", Guids.WeaponmasterManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMManeuverAb.Name", "Weaponmaster Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMManeuverAb.Desc",
                "Swift Action. Expend psionic focus to enter a counter-attack stance, gaining +2 dodge to AC and +2 competence to your next attack roll for 1 round."))
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

        // Expanded buff: +4 dodge to AC + +2 competence to attack AND damage
        var expandedBuff = BuffConfigurator.New("WeaponmasterExpandedManeuverBuff", Guids.WeaponmasterExpandedManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMExpanded.BuffName", "Weaponmaster Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMExpanded.BuffDesc",
                "Enhanced counter-attack stance: +4 dodge to AC, +2 competence to attack and damage rolls."))
            .SetIcon(maneuverIcon)
            .AddStatBonus(descriptor: ModifierDescriptor.Dodge, stat: StatType.AC, value: 4)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 2)
            .Configure();

        var expandedAbility = AbilityConfigurator.New("WeaponmasterExpandedManeuverAbility", Guids.WeaponmasterExpandedManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMExpandedAb.Name", "Weaponmaster Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMExpandedAb.Desc",
                "Swift Action. Expend psionic focus to enter an enhanced counter-attack stance: +4 dodge to AC, +2 competence to attack and damage rolls for 1 round."))
            .SetIcon(maneuverIcon)
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

        FeatureConfigurator.New("WeaponmasterExpandedManeuver", Guids.WeaponmasterExpandedManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMExpandedFeat.Name", "Weaponmaster Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMExpandedFeat.Desc",
                "Your Weaponmaster Maneuver upgrades to grant +4 dodge to AC and +2 competence to attack and damage."))
            .SetIcon(maneuverIcon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.WeaponmasterExpandedManeuverAbility })
            .AddPrerequisiteFeature(Guids.WeaponmasterPath)
            .Configure();

        // Path: grants trance + maneuver
        FeatureConfigurator.New("WeaponmasterPath", Guids.WeaponmasterPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.WeaponmasterPath.Name", "Weaponmaster Path"))
            .SetDescription(LocalizationTool.CreateString("PW.WeaponmasterPath.Desc",
                "You focus on martial superiority in melee. You gain a +1 competence bonus to attack rolls (trance) and can expend psionic focus to enter a counter-attack stance (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.WeaponmasterManeuver })
            .Configure();
    }
}
