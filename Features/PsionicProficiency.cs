using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Psionic Proficiency (Ex): A psychic warrior treats his base attack bonus as equal to his psychic warrior 
/// level for the purposes of requirements for psionic feats. Base attack bonuses granted from other classes 
/// are unaffected and are added normally.
/// </summary>
public static class PsionicProficiency
{
    public static void Configure()
    {
        FeatureConfigurator.New("PsionicProficiency", Guids.PsionicProficiency)
            .SetDisplayName(LocalizationTool.CreateString("PW.PsionicProf.Name", "Psionic Proficiency"))
            .SetDescription(LocalizationTool.CreateString(
                "PW.PsionicProf.Desc",
                "You treat your base attack bonus as equal to your Psychic Warrior level for the purposes of requirements for psionic feats."))
            .SetIsClassFeature()
            .Configure();
    }
}
