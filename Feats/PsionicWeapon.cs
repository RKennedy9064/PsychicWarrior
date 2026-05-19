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

public static class PsionicWeapon
{
    public static void Configure()
    {
        var feat = FeatureConfigurator.New("PsionicWeaponFeat", Guids.PsionicWeaponFeat)
            .SetDisplayName(Loc.Str("PW.PsionicWeapon.Name", "Psionic Weapon"))
            .SetDescription(Loc.Str("PW.PsionicWeapon.Desc", "While psionically focused, your melee attacks deal an additional 1d6 damage."))
            .SetIcon(FeatureRefs.VitalStrikeFeature.Reference.Get().Icon)
            .SetGroups(FeatureGroup.CombatFeat, FeatureGroup.Feat)
            .AddPrerequisiteFeature(Guids.GainPsionicFocusFeature)
            .AddInitiatorAttackWithWeaponTrigger(
                action: ActionsBuilder.New().Conditional(
                    ConditionsBuilder.New().CasterHasFact(Guids.PsionicFocusBuff),
                    ifTrue: ActionsBuilder.New().DealDamage(
                        DamageTypes.Physical(),
                        ContextDice.Value(DiceType.D6, 1))),
                onlyHit: true,
                checkWeaponRangeType: true,
                rangeType: WeaponRangeType.Melee)
            .AddRecommendedClass(Guids.PsychicWarriorClass)
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
