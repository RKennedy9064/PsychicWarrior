using System.Collections.Generic;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

/// <summary>
/// Animal Affinity (Transmutation) — Gain +4 enhancement bonus to one physical ability score
/// (Strength, Dexterity, or Constitution) for 1 minute/level.
///
/// Implemented as a parent ability with three variants (alchemist-mutagen pattern). The parent is
/// what shows up in the spellbook and action bar; clicking expands to show the three stat options.
/// </summary>
public static class AnimalAffinity
{
    public static void Configure()
    {
        var icon = AbilityRefs.BullsStrength.Reference.Get().Icon;
        var iconDex = AbilityRefs.CatsGrace.Reference.Get().Icon;
        var iconCon = AbilityRefs.BearsEndurance.Reference.Get().Icon;

        var strength = BuildVariant(
            "PWAnimalAffinityStrength", Guids.PowerAnimalAffinityStrength,
            "PWAnimalAffinityStrengthBuff", Guids.PowerAnimalAffinityStrengthBuff,
            "Animal Affinity — Strength", "Boar",
            StatType.Strength, icon);

        var dexterity = BuildVariant(
            "PWAnimalAffinityDexterity", Guids.PowerAnimalAffinityDexterity,
            "PWAnimalAffinityDexterityBuff", Guids.PowerAnimalAffinityDexterityBuff,
            "Animal Affinity — Dexterity", "Cat",
            StatType.Dexterity, iconDex);

        var constitution = BuildVariant(
            "PWAnimalAffinityConstitution", Guids.PowerAnimalAffinityConstitution,
            "PWAnimalAffinityConstitutionBuff", Guids.PowerAnimalAffinityConstitutionBuff,
            "Animal Affinity — Constitution", "Bear",
            StatType.Constitution, iconCon);

        // Parent ability — the entry in the spellbook/action bar. Clicking expands the variant menu.
        AbilityConfigurator.New("PWAnimalAffinity", Guids.PowerAnimalAffinity)
            .SetDisplayName(LocalizationTool.CreateString("PW.AnimalAffinity.Name", "Animal Affinity", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.AnimalAffinity.Desc",
                "Channel the spirit of a great beast: gain a +4 enhancement bonus to one physical ability score of your choice (Strength, Dexterity, or Constitution) for 1 minute per manifester level.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddSpellListComponent(2, Guids.SpellList)
            .AddAbilityVariants(variants: new List<Blueprint<BlueprintAbilityReference>>
            {
                Guids.PowerAnimalAffinityStrength,
                Guids.PowerAnimalAffinityDexterity,
                Guids.PowerAnimalAffinityConstitution,
            })
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }

    private static BlueprintAbility BuildVariant(
        string abilityName, string abilityGuid,
        string buffName, string buffGuid,
        string displayName, string beastName,
        StatType stat, UnityEngine.Sprite icon)
    {
        var buff = BuffConfigurator.New(buffName, buffGuid)
            .SetDisplayName(LocalizationTool.CreateString($"PW.{abilityName}.BuffName", displayName, tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{abilityName}.BuffDesc",
                $"You channel the spirit of the {beastName}, gaining a +4 enhancement bonus to {stat}.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Enhancement, stat: stat, value: 4)
            .Configure();

        return AbilityConfigurator.New(abilityName, abilityGuid)
            .SetDisplayName(LocalizationTool.CreateString($"PW.{abilityName}.Name", displayName, tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{abilityName}.Desc",
                $"Channel the spirit of the {beastName}, gaining +4 enhancement bonus to {stat} for 1 minute per manifester level.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetActionType(UnitCommand.CommandType.Standard)
            .SetAnimation(UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
            .AddAbilityEffectRunAction(
                ActionsBuilder.New().ApplyBuff(
                    buff,
                    ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes)))
            .AddContextRankConfig(ContextRankConfigs.CasterLevel())
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
