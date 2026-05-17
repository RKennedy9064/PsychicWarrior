using System.Linq;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Feats;

public static class PsionicWeapon
{
    public static void Configure()
    {
        var feat = FeatureConfigurator.New("PsionicWeaponFeat", Guids.PsionicWeaponFeat)
            .SetDisplayName(LocalizationTool.CreateString("PW.PsionicWeapon.Name", "Psionic Weapon"))
            .SetDescription(LocalizationTool.CreateString("PW.PsionicWeapon.Desc", "While you maintain Psionic Focus, your attacks deal an additional 2 points of damage."))
            .SetIcon(FeatureRefs.VitalStrikeFeature.Reference.Get().Icon)
            .SetGroups(FeatureGroup.CombatFeat, FeatureGroup.Feat)
            .AddPrerequisiteFeature(Guids.GainPsionicFocusFeature)
            .AddDamageBonusConditional(
                bonus: ContextValues.Constant(2),
                conditions: ConditionsBuilder.New().CasterHasFact(Guids.PsionicFocusBuff))
            .Configure();

        SafeAddFeatToSelection(FeatureSelectionRefs.BasicFeatSelection.ToString(), Guids.PsionicWeaponFeat);
        SafeAddFeatToSelection(FeatureSelectionRefs.FighterFeatSelection.ToString(), Guids.PsionicWeaponFeat);
        SafeAddFeatToSelection(Guids.BonusFeatSelection, Guids.PsionicWeaponFeat);
    }

    private static void SafeAddFeatToSelection(string selectionGuid, string featGuid)
    {
        FeatureSelectionConfigurator.For(selectionGuid)
            .OnConfigure(bp =>
            {
                var featRef = BlueprintTool.GetRef<BlueprintFeatureReference>(featGuid);

                bp.m_AllFeatures = [.. bp.m_AllFeatures.Where(f => f.Guid != featRef.Guid)];
                bp.m_AllFeatures = [.. bp.m_AllFeatures, featRef];
            })
            .Configure();
    }
}