using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Talents (Ex): Each psychic warrior gains two 0-level talents at 1st level.
/// These do not count against the psychic warrior's powers known.
/// Each talent grants access to a minor supernatural ability.
/// </summary>
public static class TalentsSelection
{
    public static void Configure()
    {
        // Minor Precognition
        var talentMinorPrecognition = FeatureConfigurator.New("TalentMinorPrecognition", Guids.TalentMinorPrecognition)
            .SetDisplayName(LocalizationTool.CreateString("PW.TalentMinorPrecognition.Name", "Minor Precognition"))
            .SetDescription(LocalizationTool.CreateString("PW.TalentMinorPrecognition.Desc",
                "Your psionic foresight grants a +1 competence bonus on a single attack roll, saving throw, or skill check."))
            .SetIcon(AbilityRefs.Guidance.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddFacts([Guids.PowerMinorPrecognition])
            .Configure();

        // Burst — speed boost
        var talentBurst = FeatureConfigurator.New("TalentBurst", Guids.TalentBurst)
            .SetDisplayName(LocalizationTool.CreateString("PW.TalentBurst.Name", "Burst"))
            .SetDescription(LocalizationTool.CreateString("PW.TalentBurst.Desc",
                "You gain a +10-foot enhancement bonus to your speed for 1 round."))
            .SetIcon(AbilityRefs.ExpeditiousRetreat.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddFacts([Guids.PowerBurst])
            .Configure();

        // Empty Mind — Will save bonus
        var talentEmptyMind = FeatureConfigurator.New("TalentEmptyMind", Guids.TalentEmptyMind)
            .SetDisplayName(LocalizationTool.CreateString("PW.TalentEmptyMind.Name", "Empty Mind"))
            .SetDescription(LocalizationTool.CreateString("PW.TalentEmptyMind.Desc",
                "You empty your mind, gaining a +2 insight bonus to Will saving throws for 1 minute."))
            .SetIcon(AbilityRefs.MindBlank.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddFacts([Guids.PowerEmptyMind])
            .Configure();

        // Valor — fear resistance
        var talentValor = FeatureConfigurator.New("TalentValor", Guids.TalentValor)
            .SetDisplayName(LocalizationTool.CreateString("PW.TalentValor.Name", "Valor"))
            .SetDescription(LocalizationTool.CreateString("PW.TalentValor.Desc",
                "You steel yourself with psionic resolve, gaining a +1 morale bonus to saving throws against fear for 10 minutes."))
            .SetIcon(AbilityRefs.RemoveFear.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddFacts([Guids.PowerValor])
            .Configure();

        // Telekinetic Punch — force damage
        var talentTelekineticPunch = FeatureConfigurator.New("TalentTelekineticPunch", Guids.TalentTelekineticPunch)
            .SetDisplayName(LocalizationTool.CreateString("PW.TalentTelekineticPunch.Name", "Telekinetic Punch"))
            .SetDescription(LocalizationTool.CreateString("PW.TalentTelekineticPunch.Desc",
                "You deliver a telekinetic punch to your target, dealing 1d4+1 points of force damage."))
            .SetIcon(AbilityRefs.MagicMissile.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddFacts([Guids.PowerTelekineticPunch])
            .Configure();

        // Precognition, Defensive — AC bonus
        var talentPrecognitionDefensive = FeatureConfigurator.New("TalentPrecognitionDefensive", Guids.TalentPrecognitionDefensive)
            .SetDisplayName(LocalizationTool.CreateString("PW.TalentPrecognitionDef.Name", "Precognition, Defensive"))
            .SetDescription(LocalizationTool.CreateString("PW.TalentPrecognitionDef.Desc",
                "Your psionic foresight warns you of incoming attacks, granting a +1 insight bonus to AC for 1 round."))
            .SetIcon(AbilityRefs.TrueStrike.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddFacts([Guids.PowerPrecognitionDefensive])
            .Configure();

        // Deceleration — speed debuff
        var talentDeceleration = FeatureConfigurator.New("TalentDeceleration", Guids.TalentDeceleration)
            .SetDisplayName(LocalizationTool.CreateString("PW.TalentDeceleration.Name", "Deceleration"))
            .SetDescription(LocalizationTool.CreateString("PW.TalentDeceleration.Desc",
                "You project a telekinetic force to slow a target, reducing its movement speed by 10 feet for 1 minute (Will negates)."))
            .SetIcon(AbilityRefs.Slow.Reference.Get().Icon)
            .SetIsClassFeature()
            .AddFacts([Guids.PowerDeceleration])
            .Configure();

        FeatureSelectionConfigurator.New("TalentsSelection", Guids.TalentsSelection)
            .SetDisplayName(LocalizationTool.CreateString("PW.Talents.Name", "Psychic Warrior Talents"))
            .SetDescription(LocalizationTool.CreateString("PW.Talents.Desc",
                "You gain two 0-level psychic talents of your choice. These do not count against your powers known."))
            .SetIcon(AbilityRefs.Guidance.Reference.Get().Icon)
            .SetIgnorePrerequisites(true)
            .SetIsClassFeature(true)
            .AddToAllFeatures(
                talentMinorPrecognition,
                talentBurst,
                talentEmptyMind,
                talentValor,
                talentTelekineticPunch,
                talentPrecognitionDefensive,
                talentDeceleration)
            .Configure();
    }
}
