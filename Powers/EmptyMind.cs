using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class EmptyMind
{
    public static void Configure()
    {
        var buff = BuffConfigurator.New("PWEmptyMindBuff", Guids.PowerEmptyMindBuff)
            .SetDisplayName(Loc.Str("PW.EmptyMind.BuffName", "Empty Mind"))
            .SetDescription(Loc.Str("PW.EmptyMind.BuffDesc", "+2 insight bonus to Will saving throws."))
            .SetIcon(AbilityRefs.MindBlank.Reference.Get().Icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Insight, stat: StatType.SaveWill, value: 2)
            .Configure();

        AbilityConfigurator.New("PWEmptyMind", Guids.PowerEmptyMind)
            .SetDisplayName(Loc.Str("PW.EmptyMind.Name", "Empty Mind"))
            .SetDescription(Loc.Str("PW.EmptyMind.Desc",
                "You empty your mind, focusing inward. You gain a +2 insight bonus to Will saving throws for 1 minute."))
            .SetIcon(AbilityRefs.MindBlank.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Fixed(1, DurationRate.Minutes)))
            .Configure();
    }
}
