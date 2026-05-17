using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class MinorPrecognition
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWMinorPrecognition", Guids.PowerMinorPrecognition)
            .CopyFrom(AbilityRefs.Guidance)
            .SetDisplayName(LocalizationTool.CreateString("PW.MinorPrecognition.Name", "Minor Precognition"))
            .SetDescription(LocalizationTool.CreateString("PW.MinorPrecognition.Desc", "Your psionic foresight grants a +1 competence bonus on a single attack roll, saving throw, or skill check."))
            .Configure();
    }
}