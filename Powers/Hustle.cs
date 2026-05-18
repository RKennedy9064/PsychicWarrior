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

/// <summary>
/// Hustle (Transmutation) — Gain a haste-like boost for 1 round.
///
/// Simplified from RAW (which grants an extra move action this round). WoTR doesn't model
/// per-round action economy the same way, so we apply a Haste-style buff for 1 round.
/// </summary>
public static class Hustle
{
    public static void Configure()
    {
        var icon = AbilityRefs.Haste.Reference.Get().Icon;

        // Copy the Haste buff for its full mechanical effect (speed, +1 attack on full attack, etc.)
        var buff = BuffConfigurator.New("PWHustleBuff", Guids.PowerHustleBuff)
            .CopyFrom(BuffRefs.HasteBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.Hustle.BuffName", "Hustle", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.Hustle.BuffDesc",
                "Psionic acceleration drives your body, granting haste-like speed and reflexes.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .Configure();

        AbilityConfigurator.New("PWHustle", Guids.PowerHustle)
            .SetDisplayName(LocalizationTool.CreateString("PW.Hustle.Name", "Hustle", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.Hustle.Desc",
                "You psionically accelerate yourself. For 1 round you gain the benefits of haste: +30 ft. speed, +1 dodge bonus to AC and Reflex saves, and an extra attack at your highest BAB on a full attack.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(2, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Fixed(1)))
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
