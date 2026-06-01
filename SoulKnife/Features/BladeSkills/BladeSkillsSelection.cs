using System.Linq;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using PsychicWarrior.Shared.Mechanics;
using PsychicWarrior.Utils;

namespace PsychicWarrior.SoulKnife.Features.BladeSkills;

public static class BladeSkillsSelection
{
    // General psionic feats a soulknife may pick via the Psionic Training blade skill.
    // (Excludes the Psychic Warrior path-specific "Advanced Path" feats.)
    private static readonly string[] PsionicFeats =
    [
        Guids.PsionicMeditationFeat,
        Guids.PsionicWeaponFeat,
        Guids.PsionicFistFeat,
        Guids.PsionicShotFeat,
        Guids.PsionicBodyFeat,
        Guids.SpeedOfThoughtFeat,
        Guids.PsionicDodgeFeat,
        Guids.CriticalRefocusFeat,
        Guids.GreaterPsionicWeaponFeat,
        Guids.GreaterPsionicFistFeat,
        Guids.GreaterPsionicShotFeat,
        Guids.RapidMetabolismFeat,
        Guids.CombatManifestationFeat,
        Guids.DeepImpactFeat,
        Guids.UpTheWallsFeat,
        Guids.PsionicEndowmentFeat,
        Guids.PsionicCriticalFeat,
        Guids.RecklessOffenseFeat,
        Guids.AlignedAttackFeat,
        Guids.WoundingAttackFeat,
        Guids.FellShotFeat,
        Guids.UnavoidableStrikeFeat,
        Guids.IntuitiveFightingFeat,
    ];

    public static void Configure()
    {
        var focusedOffense = FeatureConfigurator.New("SKFocusedOffense", Guids.BladeSkillFocusedOffense)
            .SetDisplayName(Loc.Str("SK.FocusedOffense.Name", "Focused Offense"))
            .SetDescription(Loc.Str("SK.FocusedOffense.Desc",
                "While maintaining psionic focus, you add your Wisdom modifier as an insight bonus to attack rolls and damage rolls."))
            .SetIcon(FeatureRefs.WeaponSpecializationGreatsword.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddComponent(new FocusedOffenseComponent())
            .Configure();

        var focusedDefense = FeatureConfigurator.New("SKFocusedDefense", Guids.BladeSkillFocusedDefense)
            .SetDisplayName(Loc.Str("SK.FocusedDefense.Name", "Focused Defense"))
            .SetDescription(Loc.Str("SK.FocusedDefense.Desc",
                "While maintaining psionic focus, you add your Wisdom modifier as a dodge bonus to your Armor Class."))
            .SetIcon(FeatureRefs.Dodge.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddComponent(new FocusedDefenseComponent())
            .Configure();

        var evasion = FeatureConfigurator.New("SKBladeSkillEvasion", Guids.BladeSkillEvasion)
            .SetDisplayName(Loc.Str("SK.Evasion.Name", "Evasion"))
            .SetDescription(Loc.Str("SK.Evasion.Desc",
                "If you make a successful Reflex saving throw against an attack that normally deals half damage on a successful save, you instead take no damage."))
            .SetIcon(FeatureRefs.Evasion.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddEvasion()
            .Configure();

        var improvedEvasion = FeatureConfigurator.New("SKBladeSkillImprovedEvasion", Guids.BladeSkillImprovedEvasion)
            .SetDisplayName(Loc.Str("SK.ImprovedEvasion.Name", "Improved Evasion"))
            .SetDescription(Loc.Str("SK.ImprovedEvasion.Desc",
                "This works like Evasion, except that while you take no damage on a successful Reflex saving throw against attacks, you henceforth take only half damage on a failed save."))
            .SetIcon(FeatureRefs.ImprovedEvasion.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddImprovedEvasion()
            .AddPrerequisiteFeature(Guids.BladeSkillEvasion)
            .AddPrerequisiteClassLevel(Guids.SoulKnifeClass, 10)
            .Configure();

        var powerfulStrikes = FeatureConfigurator.New("SKPowerfulStrikes", Guids.BladeSkillPowerfulStrikes)
            .SetDisplayName(Loc.Str("SK.PowerfulStrikes.Name", "Powerful Strikes"))
            .SetDescription(Loc.Str("SK.PowerfulStrikes.Desc",
                "Your psychic strike deals an additional 1d8 points of damage."))
            .SetIcon(FeatureRefs.PowerAttackFeature.Reference.Get().Icon)
            .SetIsClassFeature()
            .Configure();

        var vampiricBlade = FeatureConfigurator.New("SKVampiricBlade", Guids.BladeSkillVampiricBlade)
            .SetDisplayName(Loc.Str("SK.VampiricBlade.Name", "Vampiric Blade"))
            .SetDescription(Loc.Str("SK.VampiricBlade.Desc",
                "When you deal psychic strike damage to a living creature with your mind blade, you heal a number of hit points equal to half the psychic strike damage dealt."))
            .SetIcon(AbilityRefs.VampiricTouch.Reference.Get().Icon)
            .SetIsClassFeature()
            .Configure();

        var improvedEnhancement = FeatureConfigurator.New("SKImprovedEnhancement", Guids.BladeSkillImprovedEnhancement)
            .SetDisplayName(Loc.Str("SK.ImprovedEnhancement.Name", "Improved Enhancement"))
            .SetDescription(Loc.Str("SK.ImprovedEnhancement.Desc",
                "The enhancement bonus of your mind blade increases by 1 (to a maximum of +5)."))
            .SetIcon(AbilityRefs.MagicWeaponGreater.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddPrerequisiteClassLevel(Guids.SoulKnifeClass, 12)
            .Configure();

        // Psionic Training is itself a feature selection; picking it in the Blade Skill list
        // cascades the level-up UI into the psionic feat choice.
        var psionicTraining = FeatureSelectionConfigurator.New("SKPsionicTraining", Guids.BladeSkillPsionicTraining)
            .SetDisplayName(Loc.Str("SK.PsionicTraining.Name", "Psionic Training"))
            .SetDescription(Loc.Str("SK.PsionicTraining.Desc",
                "You gain a psionic feat of your choice. You must meet the feat's prerequisites."))
            .SetIcon(FeatureRefs.MartialWeaponProficiency.Reference.Get().Icon)
            .SetIsClassFeature()
            .SetIgnorePrerequisites(false)
            .OnConfigure(bp =>
                bp.m_AllFeatures = [.. PsionicFeats.Select(g => BlueprintTool.GetRef<Kingmaker.Blueprints.BlueprintFeatureReference>(g))])
            .Configure();

        FeatureSelectionConfigurator.New("SKBladeSkillsSelection", Guids.BladeSkillsSelection)
            .SetDisplayName(Loc.Str("SK.BladeSkills.Name", "Blade Skill"))
            .SetDescription(Loc.Str("SK.BladeSkills.Desc",
                "At 2nd level and every two levels thereafter, a soulknife may choose a blade skill to enhance her mind blade or improve her abilities."))
            .SetIcon(AbilityRefs.MagicWeapon.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddToAllFeatures(
                focusedOffense,
                focusedDefense,
                evasion,
                improvedEvasion,
                powerfulStrikes,
                vampiricBlade,
                improvedEnhancement,
                psionicTraining)
            .Configure();
    }
}
