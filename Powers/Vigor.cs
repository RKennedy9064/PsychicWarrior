using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types; // Required in 2.8.6 for ContextDuration
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Abilities.Components;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class Vigor
{
    public static void Configure()
    {
        AbilityConfigurator.New("PsychicWarriorVigor", Guids.PowerVigor)
            .CopyFrom(AbilityRefs.FalseLife, typeof(AbilityEffectRunAction))
            .SetDisplayName(LocalizationTool.CreateString("PW.Vigor.Name", "Vigor"))
            .SetDescription(LocalizationTool.CreateString("PW.Vigor.Desc", "You suffuse yourself with power, gaining 1d10 temporary hit points + 1 per manifester level (maximum +10)."))
            .AddSpellListComponent(1, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .ApplyBuff(
                        buff: BuffRefs.FalseLifeBuff.ToString(),
                        durationValue: ContextDuration.Fixed(1, DurationRate.Hours),
                        asChild: false)
            )
            .AddContextRankConfig(
                ContextRankConfigs.CasterLevel(max: 10))
            .Configure();
    }
}