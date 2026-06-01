using System;
using BlueprintCore.Utils;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Shared.Mechanics;

/// <summary>
/// Blade skill: Focused Defense. While maintaining psionic focus, add the soulknife's Wisdom
/// modifier as a dodge bonus to Armor Class. Mirrors <see cref="FocusedOffenseComponent"/>, but
/// applies to the defender's AC calculation instead of attack/damage.
/// </summary>
[Serializable]
public class FocusedDefenseComponent : UnitFactComponentDelegate,
    ITargetRulebookHandler<RuleCalculateAC>,
    IRulebookHandler<RuleCalculateAC>,
    ISubscriber,
    ITargetRulebookSubscriber
{
    private static BlueprintBuff _focusBuff;

    private bool HasFocus()
    {
        _focusBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.PsionicFocusBuff);
        return Owner.Buffs.HasFact(_focusBuff);
    }

    public void OnEventAboutToTrigger(RuleCalculateAC evt)
    {
        if (!HasFocus()) return;
        int wis = Owner.Stats.Wisdom.Bonus;
        if (wis <= 0) return;
        evt.AddModifier(wis, Fact, ModifierDescriptor.Dodge);
    }

    public void OnEventDidTrigger(RuleCalculateAC evt) { }
}
