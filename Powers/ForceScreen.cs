using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class ForceScreen
{
    private const string ShieldSpellGuid = "ef768022b0785eb43a18969903c537c4";
    private const string ShieldBuffGuid = "9c0fa9b438ada3f43864be8dd8b3e741";

    public static void Configure()
    {
        AbilityConfigurator.New("PWForceScreen", Guids.PowerForceScreen)
            .CopyFrom(ShieldSpellGuid, typeof(AbilityEffectRunAction))
            .SetDisplayName(Loc.Str("PW.ForceScreen.Name", "Force Screen"))
            .SetDescription(Loc.Str("PW.ForceScreen.Desc", "You create an invisible mobile disk of force that hovers in front of you. It grants a +4 shield bonus to Armor Class."))
            .AddSpellListComponent(1, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .ApplyBuff(
                        buff: ShieldBuffGuid,
                        durationValue: ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes)))
            .AddContextRankConfig(
                ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Abjuration)
            .Configure();
    }
}