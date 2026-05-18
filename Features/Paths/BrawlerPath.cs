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
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features.Paths;

public static class BrawlerPath
{
    public static void Configure()
    {
        var icon = FeatureRefs.ImprovedUnarmedStrike.Reference.Get().Icon;
        var maneuverIcon = AbilityRefs.BullsStrength.Reference.Get().Icon;

        // Trance: Wis modifier as competence bonus to CMB (grapple spirit)
        var trance = FeatureConfigurator.New("BrawlerTrance", Guids.BrawlerTrance)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerTrance.Name", "Brawler Trance"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerTrance.Desc",
                "Your Wisdom modifier is added as a competence bonus to your Combat Maneuver Bonus, " +
                "reflecting your psionic insight in grappling and close combat."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddContextStatBonus(
                stat: StatType.AdditionalCMB,
                descriptor: ModifierDescriptor.Competence,
                value: new ContextValue { ValueType = ContextValueType.Rank })
            .AddContextRankConfig(ContextRankConfigs.StatBonus(StatType.Wisdom))
            .Configure();

        // Maneuver buff: +7 competence to damage (≈2d6) on next attack, then removes itself
        var maneuverBuff = BuffConfigurator.New("BrawlerManeuverBuff", Guids.BrawlerManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerManeuver.BuffName", "Brawler Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerManeuver.BuffDesc",
                "Your next attack deals +7 bonus damage from psionic energy channeled through your strike."))
            .SetIcon(maneuverIcon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 7)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.BrawlerManeuverBuff))
            .Configure();

        var maneuverAbility = AbilityConfigurator.New("BrawlerManeuver", Guids.BrawlerManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerManeuverAb.Name", "Brawler Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerManeuverAb.Desc",
                "Swift Action. Expend psionic focus to channel energy into your next strike, dealing +7 bonus damage on hit."))
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

        // Expanded buff: +14 competence to damage (≈4d6) on next attack
        var expandedBuff = BuffConfigurator.New("BrawlerExpandedManeuverBuff", Guids.BrawlerExpandedManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerExpanded.BuffName", "Brawler Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerExpanded.BuffDesc",
                "Your next attack deals +14 bonus damage from psionic energy."))
            .SetIcon(maneuverIcon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 14)
            .AddInitiatorAttackRollTrigger(ActionsBuilder.New().RemoveBuff(Guids.BrawlerExpandedManeuverBuff))
            .Configure();

        var expandedAbility = AbilityConfigurator.New("BrawlerExpandedManeuverAbility", Guids.BrawlerExpandedManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerExpandedAb.Name", "Brawler Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerExpandedAb.Desc",
                "Swift Action. Expend psionic focus to channel greater energy into your next strike, dealing +14 bonus damage on hit."))
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

        FeatureConfigurator.New("BrawlerExpandedManeuver", Guids.BrawlerExpandedManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerExpandedFeat.Name", "Brawler Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerExpandedFeat.Desc",
                "Your Brawler Maneuver upgrades to deal +14 bonus damage on your next attack."))
            .SetIcon(maneuverIcon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.BrawlerExpandedManeuverAbility })
            .AddPrerequisiteFeature(Guids.BrawlerPath)
            .Configure();

        FeatureConfigurator.New("BrawlerPath", Guids.BrawlerPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerPath.Name", "Brawler Path"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerPath.Desc",
                "You focus on close combat and grappling. Your Wisdom modifier adds to your CMB (trance) " +
                "and you can expend psionic focus to deal +7 bonus damage on your next strike (maneuver)."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { trance.ToString(), Guids.BrawlerManeuver })
            .Configure();
    }
}
