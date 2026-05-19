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
/// Mental Barrier (Abjuration) — Project a barrier of mental energy. +4 deflection bonus to AC
/// for 1 round per manifester level.
/// </summary>
public static class MentalBarrier
{
    public static void Configure()
    {
        var icon = AbilityRefs.ShieldOfFaith.Reference.Get().Icon;

        var buff = BuffConfigurator.New("PWMentalBarrierBuff", Guids.PowerMentalBarrierBuff)
            .SetDisplayName(Loc.Str("PW.MentalBarrier.BuffName", "Mental Barrier", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.MentalBarrier.BuffDesc",
                "A barrier of mental force surrounds you, granting +4 deflection bonus to AC.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Deflection, stat: StatType.AC, value: 4)
            .Configure();

        AbilityConfigurator.New("PWMentalBarrier", Guids.PowerMentalBarrier)
            .SetDisplayName(Loc.Str("PW.MentalBarrier.Name", "Mental Barrier", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.MentalBarrier.Desc",
                "You raise a barrier of pure mental energy, gaining a +4 deflection bonus to AC for 1 round per manifester level.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(3, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().ApplyBuff(
                    buff,
                    ContextDuration.Variable(ContextValues.Rank(), DurationRate.Rounds)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Abjuration)
            .Configure();
    }
}
