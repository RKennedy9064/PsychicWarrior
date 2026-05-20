using System.Linq;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using PsychicWarrior.HarmonyPatches;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Feats;

public static class PsionicCritical
{
    public static void Configure()
    {
        PsionicProficiencyPatch.RegisterPsionicFeat(Guids.PsionicCriticalFeat);

        FeatureConfigurator.New("PsionicCriticalFeat", Guids.PsionicCriticalFeat)
            .SetDisplayName(Loc.Str("PW.PsionicCritical.Name", "Psionic Critical"))
            .SetDescription(Loc.Str("PW.PsionicCritical.Desc",
                "While psionically focused, your critical hits deal an additional 1d8 points of damage."))
            .SetIcon(FeatureRefs.VitalStrikeFeature.Reference.Get().Icon)
            .SetGroups(FeatureGroup.CombatFeat, FeatureGroup.Feat)
            .AddPrerequisiteFeature(Guids.GainPsionicFocusFeature)
            .AddInitiatorAttackWithWeaponTrigger(
                action: ActionsBuilder.New().Conditional(
                    ConditionsBuilder.New().CasterHasFact(Guids.PsionicFocusBuff),
                    ifTrue: ActionsBuilder.New()
                        .Add(new ContextActionLog { Message = "[PsionicCritical] crit+focused — dealing 1d8" })
                        .DealDamage(DamageTypes.Physical(), ContextDice.Value(DiceType.D8, 1, 0))),
                onlyHit: true,
                criticalHit: true)
            .AddRecommendedClass(Guids.PsychicWarriorClass)
            .Configure();

        SafeAddFeatToSelection(FeatureSelectionRefs.BasicFeatSelection.ToString(), Guids.PsionicCriticalFeat);
        SafeAddFeatToSelection(FeatureSelectionRefs.FighterFeatSelection.ToString(), Guids.PsionicCriticalFeat);
        SafeAddFeatToSelection(Guids.BonusFeatSelection, Guids.PsionicCriticalFeat);
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
