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
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using PsychicWarrior.HarmonyPatches;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Feats;

public static class GreaterPsionicShot
{
    public static void Configure()
    {
        PsionicProficiencyPatch.RegisterPsionicFeat(Guids.GreaterPsionicShotFeat);

        FeatureConfigurator.New("GreaterPsionicShotFeat", Guids.GreaterPsionicShotFeat)
            .SetDisplayName(Loc.Str("PW.GreaterPsionicShot.Name", "Greater Psionic Shot"))
            .SetDescription(Loc.Str("PW.GreaterPsionicShot.Desc",
                "While psionically focused, your ranged attacks deal an additional 1d6 damage (stacks with Psionic Shot for 2d6 total)."))
            .SetIcon(FeatureRefs.PointBlankShot.Reference.Get().Icon)
            .SetGroups(FeatureGroup.CombatFeat, FeatureGroup.Feat)
            .AddPrerequisiteFeature(Guids.PsionicShotFeat)
            .AddPrerequisiteStatValue(StatType.BaseAttackBonus, 5)
            .AddInitiatorAttackWithWeaponTrigger(
                action: ActionsBuilder.New().Conditional(
                    ConditionsBuilder.New().CasterHasFact(Guids.PsionicFocusBuff),
                    ifTrue: ActionsBuilder.New().DealDamage(
                        DamageTypes.Physical(),
                        ContextDice.Value(DiceType.D6, 1))),
                onlyHit: true,
                checkWeaponRangeType: true,
                rangeType: WeaponRangeType.Ranged)
            .Configure();

        SafeAddFeatToSelection(FeatureSelectionRefs.BasicFeatSelection.ToString(), Guids.GreaterPsionicShotFeat);
        SafeAddFeatToSelection(FeatureSelectionRefs.FighterFeatSelection.ToString(), Guids.GreaterPsionicShotFeat);
        SafeAddFeatToSelection(Guids.BonusFeatSelection, Guids.GreaterPsionicShotFeat);
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
