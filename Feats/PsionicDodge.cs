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

public static class PsionicDodge
{
    public static void Configure()
    {
        FeatureConfigurator.New("PsionicDodgeFeat", Guids.PsionicDodgeFeat)
            .SetDisplayName(Loc.Str("PW.PsionicDodge.Name", "Psionic Dodge"))
            .SetDescription(Loc.Str("PW.PsionicDodge.Desc",
                "While psionically focused, you gain a +1 dodge bonus to Armor Class."))
            .SetIcon(FeatureRefs.Dodge.Reference.Get().Icon)
            .SetGroups(FeatureGroup.CombatFeat, FeatureGroup.Feat)
            .AddPrerequisiteFeature(Guids.GainPsionicFocusFeature)
            .Configure();

        BuffConfigurator.For(Guids.PsionicFocusBuff)
            .AddComponent(new AddStatBonusIfHasFactFixed(
                stat: StatType.AC,
                bonus: ContextValues.Constant(1),
                requiredFacts: new List<Blueprint<BlueprintUnitFactReference>> { Guids.PsionicDodgeFeat },
                descriptor: ModifierDescriptor.Dodge))
            .Configure();

        SafeAddFeatToSelection(FeatureSelectionRefs.BasicFeatSelection.ToString(), Guids.PsionicDodgeFeat);
        SafeAddFeatToSelection(FeatureSelectionRefs.FighterFeatSelection.ToString(), Guids.PsionicDodgeFeat);
        SafeAddFeatToSelection(Guids.BonusFeatSelection, Guids.PsionicDodgeFeat);
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
