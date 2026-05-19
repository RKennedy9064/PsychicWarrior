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

public static class Biofeedback
{
    public static void Configure()
    {
        var buff = BuffConfigurator.New("PWBiofeedbackBuff", Guids.PowerBiofeedbackBuff)
            .SetDisplayName(Loc.Str("PW.Biofeedback.BuffName", "Biofeedback"))
            .SetDescription(Loc.Str("PW.Biofeedback.BuffDesc",
                "A psionic biofeedback loop reduces all physical damage you take by 2."))
            .SetIcon(AbilityRefs.Stoneskin.Reference.Get().Icon)
            .AddDamageResistancePhysical(value: ContextValues.Constant(2))
            .Configure();

        AbilityConfigurator.New("PWBiofeedback", Guids.PowerBiofeedback)
            .SetDisplayName(Loc.Str("PW.Biofeedback.Name", "Biofeedback"))
            .SetDescription(Loc.Str("PW.Biofeedback.Desc",
                "You create a biofeedback loop that protects you from harm. You gain damage reduction 2/— for 1 hour."))
            .SetIcon(AbilityRefs.Stoneskin.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(1, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .ApplyBuff(buff, ContextDuration.Fixed(1, DurationRate.Hours)))
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
