using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

/// <summary>
/// Body Adjustment (Conjuration) — Heal yourself 2d8 + caster level (max +10).
/// </summary>
public static class BodyAdjustment
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWBodyAdjustment", Guids.PowerBodyAdjustment)
            .SetDisplayName(LocalizationTool.CreateString("PW.BodyAdjustment.Name", "Body Adjustment", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.BodyAdjustment.Desc",
                "Through psionic biomanipulation, you heal yourself for 2d8 + your manifester level (max +10) hit points.",
                tagEncyclopediaEntries: false))
            .SetIcon(AbilityRefs.CureModerateWounds.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(2, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().HealTarget(
                    value: ContextDice.Value(DiceType.D8, 2, ContextValues.Rank())))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel(max: 10))
            .AddSpellComponent(SpellSchool.Conjuration)
            .Configure();
    }
}
