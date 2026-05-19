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
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Feats;

public static class PsionicFist
{
    public static void Configure()
    {
        FeatureConfigurator.New("PsionicFistFeat", Guids.PsionicFistFeat)
            .SetDisplayName(Loc.Str("PW.PsionicFist.Name", "Psionic Fist"))
            .SetDescription(Loc.Str("PW.PsionicFist.Desc",
                "While psionically focused, your unarmed strikes deal an additional 1d6 damage."))
            .SetIcon(FeatureRefs.ImprovedUnarmedStrike.Reference.Get().Icon)
            .SetGroups(FeatureGroup.CombatFeat, FeatureGroup.Feat)
            .AddPrerequisiteFeature(Guids.GainPsionicFocusFeature)
            .AddPrerequisiteFeature(FeatureRefs.ImprovedUnarmedStrike.ToString())
            .AddInitiatorAttackWithWeaponTrigger(
                action: ActionsBuilder.New().Conditional(
                    ConditionsBuilder.New().CasterHasFact(Guids.PsionicFocusBuff),
                    ifTrue: ActionsBuilder.New().DealDamage(
                        DamageTypes.Physical(),
                        ContextDice.Value(DiceType.D6, 1))),
                onlyHit: true,
                category: WeaponCategory.UnarmedStrike)
            .AddRecommendedClass(Guids.PsychicWarriorClass)
            .Configure();

        SafeAddFeatToSelection(FeatureSelectionRefs.BasicFeatSelection.ToString(), Guids.PsionicFistFeat);
        SafeAddFeatToSelection(FeatureSelectionRefs.FighterFeatSelection.ToString(), Guids.PsionicFistFeat);
        SafeAddFeatToSelection(Guids.BonusFeatSelection, Guids.PsionicFistFeat);
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
