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
/// Strength of My Enemy (Divination) — Steal +2 Strength from a target enemy for 1 minute/level.
/// Caster gains +2 enhancement to Strength while the enemy suffers -2.
///
/// Simplified from RAW (which steals 1d4+1 per +2 of caster level, up to +6, and the drain is
/// "physical attribute drain" rather than a buff/debuff pair). This pair-buff implementation captures
/// the spirit cleanly.
/// </summary>
public static class StrengthOfMyEnemy
{
    public static void Configure()
    {
        var icon = AbilityRefs.VampiricTouch.Reference.Get().Icon;

        var casterBuff = BuffConfigurator.New("PWStrengthOfMyEnemyCasterBuff", Guids.PowerStrengthOfMyEnemyCasterBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.StrengthOfMyEnemy.CasterBuff.Name", "Strength of My Enemy", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.StrengthOfMyEnemy.CasterBuff.Desc",
                "You have drained Strength from a foe, gaining a +2 enhancement bonus to Strength.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Enhancement, stat: StatType.Strength, value: 2)
            .Configure();

        var enemyDebuff = BuffConfigurator.New("PWStrengthOfMyEnemyEnemyDebuff", Guids.PowerStrengthOfMyEnemyEnemyDebuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.StrengthOfMyEnemy.EnemyDebuff.Name", "Strength Drained", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.StrengthOfMyEnemy.EnemyDebuff.Desc",
                "Your Strength has been psionically drained, suffering a -2 penalty.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.UntypedStackable, stat: StatType.Strength, value: -2)
            .Configure();

        AbilityConfigurator.New("PWStrengthOfMyEnemy", Guids.PowerStrengthOfMyEnemy)
            .SetDisplayName(LocalizationTool.CreateString("PW.StrengthOfMyEnemy.Name", "Strength of My Enemy", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.StrengthOfMyEnemy.Desc",
                "Touch an enemy and drain their Strength. The target suffers a -2 penalty to Strength and you gain a +2 enhancement bonus to Strength for 1 minute per manifester level.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Touch)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(2, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .ApplyBuff(enemyDebuff, ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes))
                    .ApplyBuff(casterBuff, ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes), toCaster: true))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Divination)
            .Configure();
    }
}
