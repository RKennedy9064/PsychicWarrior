using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Utils;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Path Skill (Ex): At 4th level the psychic warrior gains a +2 competence bonus to one skill
/// associated with a path he is on. Every three levels thereafter he can increase the bonus to
/// one path skill by +2 (maximum +6 for any one skill). The same skill may be chosen again.
/// </summary>
public static class PathSkillBonus
{
    public static void Configure()
    {
        // Weaponmaster path skills: Athletics (weapon techniques), Perception (battlefield awareness)
        var wmAthletics = FeatureConfigurator.New("WeaponmasterPathSkillAthletics", Guids.WeaponmasterPathSkillAthletics)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMSkillAthletics.Name", "Path Skill: Athletics (Weaponmaster)"))
            .SetDescription(LocalizationTool.CreateString("PW.WMSkillAthletics.Desc",
                "You gain a +2 competence bonus to Athletics checks. Can be selected up to three times (maximum +6)."))
            .SetIsClassFeature()
            .SetRanks(3)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SkillAthletics, value: 2)
            .AddPrerequisiteFeature(Guids.WeaponmasterPath)
            .Configure();

        var wmPerception = FeatureConfigurator.New("WeaponmasterPathSkillPerception", Guids.WeaponmasterPathSkillPerception)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMSkillPerception.Name", "Path Skill: Perception (Weaponmaster)"))
            .SetDescription(LocalizationTool.CreateString("PW.WMSkillPerception.Desc",
                "You gain a +2 competence bonus to Perception checks. Can be selected up to three times (maximum +6)."))
            .SetIsClassFeature()
            .SetRanks(3)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SkillPerception, value: 2)
            .AddPrerequisiteFeature(Guids.WeaponmasterPath)
            .Configure();

        // Brawler path skills: Athletics (grappling and throws), Mobility (dodging and positioning)
        var brawlerAthletics = FeatureConfigurator.New("BrawlerPathSkillAthletics", Guids.BrawlerPathSkillAthletics)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerSkillAthletics.Name", "Path Skill: Athletics (Brawler)"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerSkillAthletics.Desc",
                "You gain a +2 competence bonus to Athletics checks. Can be selected up to three times (maximum +6)."))
            .SetIsClassFeature()
            .SetRanks(3)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SkillAthletics, value: 2)
            .AddPrerequisiteFeature(Guids.BrawlerPath)
            .Configure();

        var brawlerMobility = FeatureConfigurator.New("BrawlerPathSkillMobility", Guids.BrawlerPathSkillMobility)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerSkillMobility.Name", "Path Skill: Mobility (Brawler)"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerSkillMobility.Desc",
                "You gain a +2 competence bonus to Mobility checks. Can be selected up to three times (maximum +6)."))
            .SetIsClassFeature()
            .SetRanks(3)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.SkillMobility, value: 2)
            .AddPrerequisiteFeature(Guids.BrawlerPath)
            .Configure();

        FeatureSelectionConfigurator.New("PathSkillBonusSelection", Guids.PathSkillBonusSelection)
            .SetDisplayName(LocalizationTool.CreateString("PW.PathSkillSel.Name", "Path Skill"))
            .SetDescription(LocalizationTool.CreateString("PW.PathSkillSel.Desc",
                "Select one skill associated with your warrior path to gain a +2 competence bonus (maximum +6 for any one skill)."))
            .SetIsClassFeature(true)
            .SetIgnorePrerequisites(false)
            .AddToAllFeatures(wmAthletics, wmPerception, brawlerAthletics, brawlerMobility)
            .Configure();
    }
}
