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

public static class Valor
{
    public static void Configure()
    {
        var buff = BuffConfigurator.New("PWValorBuff", Guids.PowerValorBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.Valor.BuffName", "Valor"))
            .SetDescription(LocalizationTool.CreateString("PW.Valor.BuffDesc", "+1 morale bonus to saving throws against fear effects."))
            .SetIcon(AbilityRefs.RemoveFear.Reference.Get().Icon)
            .AddSavingThrowBonusAgainstDescriptor(
                spellDescriptor: SpellDescriptor.Fear,
                value: 1,
                modifierDescriptor: ModifierDescriptor.Morale)
            .Configure();

        AbilityConfigurator.New("PWValor", Guids.PowerValor)
            .SetDisplayName(LocalizationTool.CreateString("PW.Valor.Name", "Valor"))
            .SetDescription(LocalizationTool.CreateString("PW.Valor.Desc",
                "You steel yourself with psionic resolve. You gain a +1 morale bonus to saving throws against fear effects for 10 minutes."))
            .SetIcon(AbilityRefs.RemoveFear.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Fixed(10, DurationRate.Minutes)))
            .Configure();
    }
}
