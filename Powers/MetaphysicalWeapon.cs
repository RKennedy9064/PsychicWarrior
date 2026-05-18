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

public static class MetaphysicalWeapon
{
    public static void Configure()
    {
        var buff = BuffConfigurator.New("PWMetaphysicalWeaponBuff", Guids.PowerMetaphysicalWeaponBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.MetaphysicalWeapon.BuffName", "Metaphysical Weapon"))
            .SetDescription(LocalizationTool.CreateString("PW.MetaphysicalWeapon.BuffDesc",
                "Your weapon is imbued with psychic energy, granting a +1 enhancement bonus to attack and damage rolls."))
            .SetIcon(AbilityRefs.MagicWeapon.Reference.Get().Icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Enhancement, stat: Kingmaker.EntitySystem.Stats.StatType.AdditionalAttackBonus, value: 1)
            .AddStatBonus(descriptor: ModifierDescriptor.Enhancement, stat: Kingmaker.EntitySystem.Stats.StatType.AdditionalDamage, value: 1)
            .Configure();

        AbilityConfigurator.New("PWMetaphysicalWeapon", Guids.PowerMetaphysicalWeapon)
            .SetDisplayName(LocalizationTool.CreateString("PW.MetaphysicalWeapon.Name", "Metaphysical Weapon"))
            .SetDescription(LocalizationTool.CreateString("PW.MetaphysicalWeapon.Desc",
                "You imbue your weapon with psychic energy, granting a +1 enhancement bonus to attack rolls and damage rolls for 1 minute per manifester level."))
            .SetIcon(AbilityRefs.MagicWeapon.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(1, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
