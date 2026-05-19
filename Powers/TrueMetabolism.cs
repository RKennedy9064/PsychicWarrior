using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class TrueMetabolism
{
    public static void Configure()
    {
        var icon = AbilityRefs.Restoration.Reference.Get().Icon;

        var buff = BuffConfigurator.New("PWTrueMetabolismBuff", Guids.PowerTrueMetabolismBuff)
            .SetDisplayName(Loc.Str("PW.TrueMetabolism.BuffName", "True Metabolism", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.TrueMetabolism.BuffDesc",
                "You are regenerating rapidly, healing 5 hit points each round.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .AddEffectFastHealing(heal: 5)
            .Configure();

        AbilityConfigurator.New("PWTrueMetabolism", Guids.PowerTrueMetabolism)
            .SetDisplayName(Loc.Str("PW.TrueMetabolism.Name", "True Metabolism", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.TrueMetabolism.Desc",
                "Your cells regenerate at a remarkable rate. You gain fast healing 5 for 1 round per manifester level.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(5, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().ApplyBuff(
                    buff,
                    ContextDuration.Variable(ContextValues.Rank(), DurationRate.Rounds)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
