using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features.Paths;

public static class PsychicWarriorPathSelection
{
    public static void Configure()
    {
        FeatureSelectionConfigurator.New("PsychicWarriorPathSelection", Guids.PathSelectionLevel1)
            .SetDisplayName(LocalizationTool.CreateString("PW.PathSelection.Name", "Psychic Warrior Path"))
            .SetDescription(LocalizationTool.CreateString("PW.PathSelection.Desc",
                "At 1st level, a psychic warrior chooses a path that shapes her training and fighting style. " +
                "She gains the trance and maneuver of her chosen path."))
            .SetIsClassFeature()
            .AddToAllFeatures(
                Guids.WeaponmasterPath,
                Guids.BrawlerPath,
                Guids.ArcherPath,
                Guids.AsceticPath,
                Guids.AssassinsPath,
                Guids.DervishPath,
                Guids.FeralWarriorPath,
                Guids.GladiatorPath,
                Guids.InfiltratorPath,
                Guids.InterceptorPath,
                Guids.MindKnightPath,
                Guids.SurvivorPath)
            .Configure();

        FeatureSelectionConfigurator.New("PathExpandedManeuver", Guids.PathExpandedManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.PathExpandedManeuver.Name", "Expanded Path Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.PathExpandedManeuver.Desc",
                "At 3rd level, you gain an additional maneuver from your chosen path, expanding your tactical options in combat."))
            .SetIsClassFeature()
            .SetIgnorePrerequisites(false)
            .AddToAllFeatures(
                Guids.WeaponmasterExpandedManeuver,
                Guids.BrawlerExpandedManeuver,
                Guids.ArcherExpandedFeature,
                Guids.AsceticExpandedFeature,
                Guids.AssassinsExpandedFeature,
                Guids.DervishExpandedFeature,
                Guids.FeralWarriorExpandedFeature,
                Guids.GladiatorExpandedFeature,
                Guids.InfiltratorExpandedFeature,
                Guids.InterceptorExpandedFeature,
                Guids.MindKnightExpandedFeature,
                Guids.SurvivorExpandedFeature)
            .Configure();

        FeatureSelectionConfigurator.New("SecondaryPathSelection", Guids.SecondaryPathSelection)
            .SetDisplayName(LocalizationTool.CreateString("PW.SecondaryPath.Name", "Secondary Path"))
            .SetDescription(LocalizationTool.CreateString("PW.SecondaryPath.Desc",
                "At 9th level, a psychic warrior chooses a second warrior's path. He gains the trance and maneuver of that path. " +
                "The psychic warrior may only benefit from one trance at a time (see Twisting Paths)."))
            .SetIsClassFeature()
            .AddToAllFeatures(
                Guids.WeaponmasterPath,
                Guids.BrawlerPath,
                Guids.ArcherPath,
                Guids.AsceticPath,
                Guids.AssassinsPath,
                Guids.DervishPath,
                Guids.FeralWarriorPath,
                Guids.GladiatorPath,
                Guids.InfiltratorPath,
                Guids.InterceptorPath,
                Guids.MindKnightPath,
                Guids.SurvivorPath)
            .Configure();

        // TODO (Phase 7): implement as a swift-action ability that swaps active trance buff
        FeatureConfigurator.New("TwistingPaths", Guids.TwistingPaths)
            .SetDisplayName(LocalizationTool.CreateString("PW.TwistingPaths.Name", "Twisting Paths"))
            .SetDescription(LocalizationTool.CreateString("PW.TwistingPaths.Desc",
                "At 11th level, while maintaining psionic focus and using a trance, the psychic warrior can spend a swift action " +
                "to switch to the trance of his other path."))
            .SetIsClassFeature()
            .Configure();

        // TODO (Phase 7): implement as a free-action ability (1/day +1/day per 3 levels after 15)
        // that grants both trance benefits simultaneously for 5 minutes.
        FeatureConfigurator.New("Pathweaving", Guids.Pathweaving)
            .SetDisplayName(LocalizationTool.CreateString("PW.Pathweaving.Name", "Pathweaving"))
            .SetDescription(LocalizationTool.CreateString("PW.Pathweaving.Desc",
                "At 15th level, while maintaining psionic focus, the psychic warrior can spend a free action once per day " +
                "to gain the benefits of both trances simultaneously for 5 minutes. He gains one additional use per day for " +
                "every three psychic warrior levels beyond 15th."))
            .SetIsClassFeature()
            .Configure();
    }
}
