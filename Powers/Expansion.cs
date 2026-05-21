using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
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

public static class Expansion
{
    public static void Configure()
    {
        var icon = AbilityRefs.EnlargePerson.Reference.Get().Icon;

        AbilityConfigurator.New("PWExpansion", Guids.PowerExpansion)
            .SetDisplayName(Loc.Str("PW.Expansion.Name", "Expansion"))
            .SetDescription(Loc.Str("PW.Expansion.Desc",
                "Your body grows to Large size, granting +2 Strength, –2 Dexterity, –1 penalty to attack rolls and AC. Lasts 1 minute per manifester level."))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .Add(new ContextActionLog { Message = "[Expansion] growing to Large size (1 min/ML)", LogRank = true })
                    .ApplyBuff(BuffRefs.EnlargePersonBuff.ToString(), ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellListComponent(1, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
