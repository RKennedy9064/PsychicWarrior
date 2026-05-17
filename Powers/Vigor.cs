using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class Vigor
{
    public static void Configure()
    {
        // Copy FalseLifeBuff for its temp HP component; override display only.
        var vigorBuff = BuffConfigurator.New("PWVigorBuff", Guids.VigorBuff)
            .CopyFrom(BuffRefs.FalseLifeBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.VigorBuff.Name", "Vigor"))
            .SetDescription(LocalizationTool.CreateString("PW.VigorBuff.Desc",
                "You are suffused with psionic energy, granting temporary hit points."))
            .SetIcon(AbilityRefs.FalseLife.Reference.Get().Icon)
            .Configure();

        AbilityConfigurator.New("PsychicWarriorVigor", Guids.PowerVigor)
            .SetDisplayName(LocalizationTool.CreateString("PW.Vigor.Name", "Vigor"))
            .SetDescription(LocalizationTool.CreateString("PW.Vigor.Desc",
                "You suffuse yourself with power, gaining 1d10 temporary hit points + 1 per manifester level (maximum +10)."))
            .SetIcon(AbilityRefs.FalseLife.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(1, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .ApplyBuff(
                        buff: Guids.VigorBuff,
                        durationValue: ContextDuration.Fixed(1, DurationRate.Hours)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel(max: 10))
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
