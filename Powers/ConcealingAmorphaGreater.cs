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
/// Concealing Amorpha, Greater (Metacreativity → Conjuration) — Surround yourself with quasi-real
/// psionic matter, gaining displacement (50% miss chance) for 1 round per manifester level.
/// Reuses WoTR's DisplacementBuff for the actual miss-chance mechanic.
/// </summary>
public static class ConcealingAmorphaGreater
{
    public static void Configure()
    {
        var icon = AbilityRefs.Displacement.Reference.Get().Icon;

        // Wrap WoTR's DisplacementBuff so we can apply it with our duration scaling and naming
        var buff = BuffConfigurator.New("PWConcealingAmorphaGreaterBuff", Guids.PowerConcealingAmorphaGreaterBuff)
            .CopyFrom(BuffRefs.DisplacementBuff)
            .SetDisplayName(Loc.Str("PW.ConcealingAmorphaGreater.BuffName", "Concealing Amorpha, Greater", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.ConcealingAmorphaGreater.BuffDesc",
                "Psionic matter surrounds you, granting 50% miss chance against attacks.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .Configure();

        AbilityConfigurator.New("PWConcealingAmorphaGreater", Guids.PowerConcealingAmorphaGreater)
            .SetDisplayName(Loc.Str("PW.ConcealingAmorphaGreater.Name", "Concealing Amorpha, Greater", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.ConcealingAmorphaGreater.Desc",
                "You manifest a layer of quasi-real psionic matter that obscures your true location. Attacks against you have a 50% miss chance for 1 round per manifester level.",
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
            .AddSpellComponent(SpellSchool.Conjuration)
            .Configure();
    }
}
