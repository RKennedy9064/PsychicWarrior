using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Talents (Ex): Each psychic warrior gains two 0-level talents of their choice. 
/// These talents do not count against the psychic warrior's powers known.
/// </summary>
public static class TalentsSelection
{
    public static void Configure()
    {
        // Create the feature selection for talents
        // In the future, this will let players select from 0-level powers
        FeatureSelectionConfigurator.New("TalentsSelection", Guids.TalentsSelection)
            .SetDisplayName(LocalizationTool.CreateString("PW.Talents.Name", "Psychic Warrior Talents"))
            .SetDescription(LocalizationTool.CreateString(
                "PW.Talents.Desc",
                "You gain two 0-level psychic talents of your choice. These do not count against your powers known."))
            .SetIgnorePrerequisites(false)
            .SetIsClassFeature(true)
            .Configure();
    }
}
