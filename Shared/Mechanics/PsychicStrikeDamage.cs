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
    private static BlueprintFeature _powerfulStrikes;
    private static BlueprintFeature _vampiricBlade;

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

        // Blade skill: Powerful Strikes — +1d8 to psychic strike.
        _powerfulStrikes ??= BlueprintTool.Get<BlueprintFeature>(Guids.BladeSkillPowerfulStrikes);
        if (_powerfulStrikes != null && Owner.Descriptor.HasFact(_powerfulStrikes))
            numDice += 1;

        buff.Remove();

        var raw = WeaponInheritedDamage.Build(evt.Weapon, new DiceFormula(numDice, DiceType.D8), alignmentBypassAll: false);
        var damageRule = Rulebook.Trigger(new RuleDealDamage(evt.Initiator, evt.Target, new DamageBundle(raw)));

        // Blade skill: Vampiric Blade — heal the soulknife for half the psychic strike damage dealt.
        _vampiricBlade ??= BlueprintTool.Get<BlueprintFeature>(Guids.BladeSkillVampiricBlade);
        if (_vampiricBlade != null && Owner.Descriptor.HasFact(_vampiricBlade))
        {
            int healed = damageRule.Result / 2;
            if (healed > 0)
                Rulebook.Trigger(new RuleHealDamage(evt.Initiator, evt.Initiator, healed));
        }
    }
}
