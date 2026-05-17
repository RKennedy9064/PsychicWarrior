using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Mechanics;

public static class Focus
{
    public static void Configure()
    {
        var focusIcon = AbilityRefs.TrueStrike.Reference.Get().Icon;

        var focusBuff = BuffConfigurator.New("PsionicFocusBuff", Guids.PsionicFocusBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.Focus.Name", "Psionic Focus"))
            .SetDescription(LocalizationTool.CreateString("PW.Focus.Desc", "You are psionically focused. You can expend this focus to activate certain powers and maneuvers."))
            .SetIcon(focusIcon)
            .SetFxOnStart(BuffRefs.ArcaneAccuracyBuff.Reference.Get().FxOnStart)
            .Configure();

        var gainFocusAbility = AbilityConfigurator.New("GainPsionicFocusAbility", Guids.GainPsionicFocusAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.GainFocusAbility.Name", "Gain Psionic Focus"))
            .SetDescription(LocalizationTool.CreateString("PW.GainFocusAbility.Desc", "Standard Action. Meditate to gain psionic focus."))
            .SetIcon(focusIcon)
            // Supernatural is the mechanically perfect fit: No AoO, No Spell Resistance, infinite uses.
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityEffectRunAction(ActionsBuilder.New().ApplyBuffPermanent(focusBuff))
            .Configure();

        AbilityConfigurator.New("GainPsionicFocusMoveAbility", Guids.GainPsionicFocusMoveAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.GainFocusMoveAbility.Name", "Gain Psionic Focus (Move Action)"))
            .SetDescription(LocalizationTool.CreateString("PW.GainFocusMoveAbility.Desc", "Move Action. Meditate quickly to gain psionic focus."))
            .SetIcon(focusIcon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Move)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityEffectRunAction(ActionsBuilder.New().ApplyBuffPermanent(focusBuff))
            .Configure();

        FeatureConfigurator.New("GainPsionicFocusFeature", Guids.GainPsionicFocusFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.GainFocusFeature.Name", "Psionic Focus"))
            .SetDescription(LocalizationTool.CreateString("PW.GainFocusFeature.Desc", "A psychic warrior can meditate to gain psionic focus. While focused, they can trigger special maneuvers."))
            .SetIsClassFeature()
            .SetIcon(focusIcon)
            .AddFacts([gainFocusAbility.ToString()])
            .Configure();
    }
}