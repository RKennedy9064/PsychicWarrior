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
                "Your skin thickens and toughens, granting a natural armor bonus to AC scaling with manifester level (+1 at ML 1, +2 at ML 7, +3 at ML 13, +4 at ML 19)."))
            .SetIcon(AbilityRefs.Barkskin.Reference.Get().Icon)
            .AddContextStatBonus(descriptor: ModifierDescriptor.NaturalArmor, stat: Kingmaker.EntitySystem.Stats.StatType.AC, value: ContextValues.Rank())
            .AddContextRankConfig(
                ContextRankConfigs.CasterLevel().WithCustomProgression(
                    (6, 1), (12, 2), (18, 3), (20, 4)))
            .Configure();

        AbilityConfigurator.New("PWThickenSkin", Guids.PowerThickenSkin)
            .SetDisplayName(Loc.Str("PW.ThickenSkin.Name", "Thicken Skin"))
            .SetDescription(Loc.Str("PW.ThickenSkin.Desc",
                "Your skin thickens and toughens, granting a natural armor bonus to AC scaling with manifester level (+1 at ML 1, +2 at ML 7, +3 at ML 13, +4 at ML 19)."))
            .SetIcon(AbilityRefs.Barkskin.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(1, Guids.SpellList)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .Add(new ContextActionLog { Message = "[ThickenSkin] applying natural armor (rank=ML; buff scales +1/+2/+3/+4 at ML 1/7/13/19)", LogRank = true })
                    .ApplyBuff(buff, ContextDuration.Fixed(1, DurationRate.Hours)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
