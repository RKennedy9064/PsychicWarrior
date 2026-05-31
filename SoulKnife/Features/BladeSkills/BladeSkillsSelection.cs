using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using PsychicWarrior.Shared.Mechanics;
using PsychicWarrior.Utils;

namespace PsychicWarrior.SoulKnife.Features.BladeSkills;

public static class BladeSkillsSelection
{
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

        var evasion = FeatureConfigurator.New("SKBladeSkillEvasion", Guids.BladeSkillEvasion)
            .SetDisplayName(Loc.Str("SK.Evasion.Name", "Evasion"))
            .SetDescription(Loc.Str("SK.Evasion.Desc",
                "If you make a successful Reflex saving throw against an attack that normally deals half damage on a successful save, you instead take no damage."))
            .SetIcon(FeatureRefs.Evasion.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddEvasion()
            .Configure();

        var powerfulStrikes = FeatureConfigurator.New("SKPowerfulStrikes", Guids.BladeSkillPowerfulStrikes)
            .SetDisplayName(Loc.Str("SK.PowerfulStrikes.Name", "Powerful Strikes"))
            .SetDescription(Loc.Str("SK.PowerfulStrikes.Desc",
                "Your psychic strike deals an additional 1d8 points of damage."))
            .SetIcon(FeatureRefs.PowerAttackFeature.Reference.Get().Icon)
            .SetIsClassFeature()
            .Configure();

        FeatureSelectionConfigurator.New("SKBladeSkillsSelection", Guids.BladeSkillsSelection)
            .SetDisplayName(Loc.Str("SK.BladeSkills.Name", "Blade Skill"))
            .SetDescription(Loc.Str("SK.BladeSkills.Desc",
                "At 2nd level and every two levels thereafter, a soulknife may choose a blade skill to enhance her mind blade or improve her abilities."))
            .SetIcon(AbilityRefs.MagicWeapon.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddToAllFeatures(focusedOffense, evasion, powerfulStrikes)
            .Configure();
    }
}
