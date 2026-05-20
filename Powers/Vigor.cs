using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class Vigor
{
    public static void Configure()
    {
        var vigorBuff = BuffConfigurator.New("PWVigorBuff", Guids.VigorBuff)
            .SetDisplayName(Loc.Str("PW.VigorBuff.Name", "Vigor"))
            .SetDescription(Loc.Str("PW.VigorBuff.Desc",
                "You are suffused with psionic energy, granting temporary hit points equal to 5 × your manifester level."))
            .SetIcon(AbilityRefs.FalseLife.Reference.Get().Icon)
            .AddTemporaryHitPointsFromAbilityValue(descriptor: ModifierDescriptor.UntypedStackable, value: ContextValues.Rank())
            .Configure();

        AbilityConfigurator.New("PsychicWarriorVigor", Guids.PowerVigor)
            .SetDisplayName(Loc.Str("PW.Vigor.Name", "Vigor"))
            .SetDescription(Loc.Str("PW.Vigor.Desc",
                "You suffuse yourself with psionic power, granting temporary hit points equal to 5 × your manifester level."))
            .SetIcon(AbilityRefs.FalseLife.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(1, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .Add(new ContextActionLog { Message = "[Vigor] applying temp HP (rank=5×ML; 5 at ML1 → 100 at ML20)", LogRank = true })
                    .ApplyBuff(
                        buff: Guids.VigorBuff,
                        durationValue: ContextDuration.Fixed(1, DurationRate.Hours)))
            .AddContextRankConfig(
                ContextRankConfigs.CasterLevel().WithCustomProgression(
                    (1, 5), (2, 10), (3, 15), (4, 20), (5, 25), (6, 30), (7, 35), (8, 40), (9, 45), (10, 50),
                    (11, 55), (12, 60), (13, 65), (14, 70), (15, 75), (16, 80), (17, 85), (18, 90), (19, 95), (20, 100)))
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
