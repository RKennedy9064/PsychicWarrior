// Features/PsychicWarriorBonusFeat.cs
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Localization;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

public static class PsychicWarriorBonusFeat
{
    public static void Configure()
    {
        // By copying the Fighter Feat Selection, we instantly get access to all combat feats
        FeatureSelectionConfigurator.New("PsychicWarriorBonusFeat", Guids.BonusFeatSelection)
            .CopyFrom(FeatureSelectionRefs.FighterFeatSelection.ToString())
            .SetDisplayName(LocalizationTool.CreateString("PW.BonusFeat.Name", "Psychic Warrior Bonus Feat"))
            .SetDescription(LocalizationTool.CreateString("PW.BonusFeat.Desc", "A psychic warrior gains a bonus combat feat at 1st level, 2nd level, and every three levels thereafter."))
            .SetIsClassFeature()
            .Configure();
    }
}