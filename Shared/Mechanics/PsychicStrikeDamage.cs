using System;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Shared.Mechanics;

[Serializable]
public class PsychicStrikeDamage : UnitFactComponentDelegate,
    IInitiatorRulebookHandler<RuleAttackWithWeapon>,
    IRulebookHandler<RuleAttackWithWeapon>,
    ISubscriber,
    IInitiatorRulebookSubscriber
{
    private static BlueprintCharacterClass _skClass;
    private static BlueprintBuff _chargeBuff;

    public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

    public void OnEventDidTrigger(RuleAttackWithWeapon evt)
    {
        if (!evt.AttackRoll.IsHit) return;

        _chargeBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.PsychicStrikeChargeBuff);
        var buff = Owner.Buffs.GetBuff(_chargeBuff);
        if (buff == null) return;

        _skClass ??= BlueprintTool.Get<BlueprintCharacterClass>(Guids.SoulKnifeClass);
        int level = Owner.Progression.GetClassLevel(_skClass);
        int numDice = level switch
        {
            >= 19 => 5,
            >= 15 => 4,
            >= 11 => 3,
            >= 7  => 2,
            >= 3  => 1,
            _     => 0
        };
        if (numDice <= 0) return;

        buff.Remove();

        var raw = WeaponInheritedDamage.Build(evt.Weapon, new DiceFormula(numDice, DiceType.D8), alignmentBypassAll: false);
        Rulebook.Trigger(new RuleDealDamage(evt.Initiator, evt.Target, new DamageBundle(raw)));
    }
}
