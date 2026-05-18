using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

/// <summary>
/// Detect Hostile Intent (Divination) — Sense incoming attacks. Grants Uncanny Dodge (cannot be
/// caught flat-footed) plus +2 Initiative for 10 minutes per manifester level.
/// </summary>
public static class DetectHostileIntent
{
    public static void Configure()
    {
        var icon = AbilityRefs.TrueStrike.Reference.Get().Icon;

        var buff = BuffConfigurator.New("PWDetectHostileIntentBuff", Guids.PowerDetectHostileIntentBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.DetectHostileIntent.BuffName", "Detect Hostile Intent", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.DetectHostileIntent.BuffDesc",
                "You cannot be caught flat-footed and gain +2 Initiative.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .AddFacts(new() { FeatureRefs.UncannyDodge.ToString() })
            .AddStatBonus(descriptor: ModifierDescriptor.Insight, stat: StatType.Initiative, value: 2)
            .Configure();

        AbilityConfigurator.New("PWDetectHostileIntent", Guids.PowerDetectHostileIntent)
            .SetDisplayName(LocalizationTool.CreateString("PW.DetectHostileIntent.Name", "Detect Hostile Intent", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.DetectHostileIntent.Desc",
                "You attune your mind to detect incoming attacks. For 10 minutes per manifester level, you cannot be caught flat-footed (Uncanny Dodge) and gain +2 Initiative.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(2, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().ApplyBuff(
                    buff,
                    ContextDuration.Variable(ContextValues.Rank(), DurationRate.TenMinutes)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Divination)
            .Configure();
    }
}
