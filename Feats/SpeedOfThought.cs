using System.Collections.Generic;
using System.Linq;
using BlueprintCore.Blueprints.Components.Replacements;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Feats;

public static class SpeedOfThought
{
    public static void Configure()
    {
        FeatureConfigurator.New("SpeedOfThoughtFeat", Guids.SpeedOfThoughtFeat)
            .SetDisplayName(Loc.Str("PW.SpeedOfThought.Name", "Speed of Thought"))
            .SetDescription(Loc.Str("PW.SpeedOfThought.Desc",
                "While psionically focused, you gain a +10 ft enhancement bonus to your land speed."))
            .SetIcon(FeatureRefs.Dodge.Reference.Get().Icon)
            .SetGroups(FeatureGroup.Feat)
            .AddPrerequisiteFeature(Guids.GainPsionicFocusFeature)
            .Configure();

        // Inject the conditional speed bonus into PsionicFocusBuff so it fires whenever
        // the unit has both this feat and an active psionic focus.
        BuffConfigurator.For(Guids.PsionicFocusBuff)
            .AddComponent(new AddStatBonusIfHasFactFixed(
                stat: StatType.Speed,
                bonus: ContextValues.Constant(10),
                requiredFacts: new List<Blueprint<BlueprintUnitFactReference>> { Guids.SpeedOfThoughtFeat },
                descriptor: ModifierDescriptor.Enhancement))
            .Configure();

        SafeAddFeatToSelection(FeatureSelectionRefs.BasicFeatSelection.ToString(), Guids.SpeedOfThoughtFeat);
        SafeAddFeatToSelection(Guids.BonusFeatSelection, Guids.SpeedOfThoughtFeat);
    }

    private static void SafeAddFeatToSelection(string selectionGuid, string featGuid)
    {
        FeatureSelectionConfigurator.For(selectionGuid)
            .OnConfigure(bp =>
            {
                var featRef = BlueprintTool.GetRef<BlueprintFeatureReference>(featGuid);
                bp.m_AllFeatures = [.. bp.m_AllFeatures.Where(f => f.Guid != featRef.Guid), featRef];
            })
            .Configure();
    }
}
