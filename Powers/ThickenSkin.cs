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

public static class ThickenSkin
{
    public static void Configure()
    {
        var buff = BuffConfigurator.New("PWThickenSkinBuff", Guids.PowerThickenSkinBuff)
            .SetDisplayName(Loc.Str("PW.ThickenSkin.BuffName", "Thicken Skin"))
            .SetDescription(Loc.Str("PW.ThickenSkin.BuffDesc",
                "Your skin thickens and toughens, granting a +1 natural armor bonus to AC."))
            .SetIcon(AbilityRefs.Barkskin.Reference.Get().Icon)
            .AddStatBonus(descriptor: ModifierDescriptor.NaturalArmor, stat: Kingmaker.EntitySystem.Stats.StatType.AC, value: 1)
            .Configure();

        AbilityConfigurator.New("PWThickenSkin", Guids.PowerThickenSkin)
            .SetDisplayName(Loc.Str("PW.ThickenSkin.Name", "Thicken Skin"))
            .SetDescription(Loc.Str("PW.ThickenSkin.Desc",
                "Your skin thickens and toughens, granting a +1 natural armor bonus to AC for 1 hour."))
            .SetIcon(AbilityRefs.Barkskin.Reference.Get().Icon)
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
