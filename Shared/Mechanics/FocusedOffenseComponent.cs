using System;
using BlueprintCore.Utils;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Shared.Mechanics;

[Serializable]
public class FocusedOffenseComponent : UnitFactComponentDelegate,
    IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>,
    IRulebookHandler<RuleCalculateAttackBonusWithoutTarget>,
    IInitiatorRulebookHandler<RuleCalculateWeaponStats>,
    IRulebookHandler<RuleCalculateWeaponStats>,
    ISubscriber,
    IInitiatorRulebookSubscriber
{
    private static BlueprintBuff _focusBuff;

    private bool HasFocus()
    {
        _focusBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.PsionicFocusBuff);
        return Owner.Buffs.HasFact(_focusBuff);
    }

    public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
    {
        if (!HasFocus()) return;
        int wis = Owner.Stats.Wisdom.Bonus;
        if (wis <= 0) return;
        evt.AddModifier(wis, Fact, ModifierDescriptor.Insight);
    }

    public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }

    public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
    {
        if (!HasFocus()) return;
        int wis = Owner.Stats.Wisdom.Bonus;
        if (wis <= 0) return;
        evt.AddDamageModifier(wis, Fact, ModifierDescriptor.Insight);
    }

    public void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }
}
