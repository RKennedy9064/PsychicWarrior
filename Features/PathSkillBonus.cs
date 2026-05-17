using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Path Skill (Ex): At 4th level, the psychic warrior gains a +2 bonus to one skill associated with 
/// a path he is on. Every three levels thereafter, he can choose to increase the bonus to one of his 
/// path skills by +2 (to a maximum of +6 for any one path skill). This may be a skill he has already 
/// chosen or a new skill associated with a path he is on.
/// </summary>
public static class PathSkillBonus
{
    public static void Configure()
    {
        // Create the feature for path skill bonus
        // This is a placeholder that will be properly implemented with skill selection UIs
        FeatureConfigurator.New("PathSkillBonus", Guids.PathSkillBonusFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.PathSkill.Name", "Path Skill Bonus"))
            .SetDescription(LocalizationTool.CreateString(
                "PW.PathSkill.Desc",
                "At 4th level and every 3 levels thereafter, you gain a +2 bonus to one skill associated with your warrior path " +
                "(maximum +6 for any one skill)."))
            .SetIsClassFeature()
            .Configure();

        // Create the selection feature for choosing which skill to boost
        FeatureSelectionConfigurator.New("PathSkillBonusSelection", Guids.PathSkillBonusSelection)
            .SetDisplayName(LocalizationTool.CreateString("PW.PathSkillSel.Name", "Choose Path Skill"))
            .SetDescription(LocalizationTool.CreateString(
                "PW.PathSkillSel.Desc",
                "Select one skill associated with your warrior path to gain a +2 bonus."))
            .SetIgnorePrerequisites(false)
            .SetIsClassFeature(true)
            .Configure();
    }
}
