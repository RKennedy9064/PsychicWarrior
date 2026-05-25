using System.Collections.Generic;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.ActivatableAbilities;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Martial Power (Su) — RAW Psionics Unleashed (level 6). The act of toggling a specific variant
/// IS the activation; there's no separate selector + toggle button. Each variant is its own
/// <see cref="BlueprintActivatableAbility"/>; they all share <see cref="ActivatableAbilityGroup.CombatManeuverStrike"/>
/// so toggling one auto-disables any other Martial Power variant (mutex).
///
/// Per variant:
///   - Standalone Activatable with spinning border when on.
///   - Granted by the corresponding path or expanded-maneuver feature, so only relevant variants
///     ever appear in the action bar.
///   - <c>RestrictionHasFact(MartialPowerFeature)</c> means the Activatable can't be toggled on
///     pre-level 6.
///   - The Activatable's buff carries <c>AddInitiatorAttackWithWeaponTrigger</c> (melee, on-hit).
///     On each round's first hit while the cooldown buff is absent, it applies the maneuver's
///     buff effect and the 1-round <c>MartialPowerUsedBuff</c> cooldown.
///   - Description text spells out the maneuver effect, not just "Set Martial Power to X".
/// </summary>
public static class MartialPower
{
    public static void Configure()
    {
        var cooldownIcon = FeatureRefs.PowerAttackFeature.Reference.Get().Icon;

        // 1-round cooldown buff. Its presence blocks any Martial Power variant from triggering again
        // this round (enforces RAW once-per-round across ALL variants).
        BuffConfigurator.New("MartialPowerUsedBuff", Guids.MartialPowerUsedBuff)
            .SetDisplayName(Loc.Str("PW.MartialPowerUsedBuff.Name", "Martial Power Used"))
            .SetDescription(Loc.Str("PW.MartialPowerUsedBuff.Desc",
                "You have manifested a path power this round. Martial Power cannot trigger again until next round."))
            .SetIcon(cooldownIcon)
            .Configure();

        // Per-path icons — basic uses the path's maneuver icon, expanded uses the path's expanded icon,
        // matching the icons already used on the maneuver abilities themselves.
        var iWeaponmasterBasic  = FeatureRefs.WeaponSpecializationGreatsword.Reference.Get().Icon;
        var iWeaponmasterExp    = AbilityRefs.ExpeditiousRetreat.Reference.Get().Icon;
        var iBrawlerBasic       = AbilityRefs.BullsStrength.Reference.Get().Icon;
        var iBrawlerExp         = FeatureRefs.Toughness.Reference.Get().Icon;
        var iArcherBasic        = FeatureRefs.Manyshot.Reference.Get().Icon;
        var iArcherExp          = AbilityRefs.Haste.Reference.Get().Icon;
        var iAsceticBasic       = AbilityRefs.ShieldOfFaith.Reference.Get().Icon;
        var iAsceticExp         = AbilityRefs.Aid.Reference.Get().Icon;
        var iAssassinsBasic     = AbilityRefs.TrueStrike.Reference.Get().Icon;
        var iAssassinsExp       = AbilityRefs.Heroism.Reference.Get().Icon;
        var iDervishBasic       = FeatureRefs.TwoWeaponFightingImproved.Reference.Get().Icon;
        var iDervishExp         = AbilityRefs.Haste.Reference.Get().Icon;
        var iFeralBasic         = AbilityRefs.MagicFang.Reference.Get().Icon;
        var iFeralExp           = FeatureRefs.ImprovedCriticalLongsword.Reference.Get().Icon;
        var iGladiatorBasic     = FeatureRefs.ImprovedBullRush.Reference.Get().Icon;
        var iGladiatorExp       = AbilityRefs.Bless.Reference.Get().Icon;
        var iInfiltratorBasic   = FeatureRefs.SkillFocusDiplomacy.Reference.Get().Icon;
        var iInfiltratorExp     = AbilityRefs.Blur.Reference.Get().Icon;
        var iInterceptorBasic   = AbilityRefs.DivineFavor.Reference.Get().Icon;
        var iInterceptorExp     = AbilityRefs.StoneskinCommunal.Reference.Get().Icon;
        var iMindKnightBasic    = AbilityRefs.TrueStrike.Reference.Get().Icon;
        var iMindKnightExp      = AbilityRefs.Bless.Reference.Get().Icon;
        var iSurvivorBasic      = FeatureRefs.IronWill.Reference.Get().Icon;
        var iSurvivorExp        = AbilityRefs.FalseLife.Reference.Get().Icon;

        // 24 variant Activatables — one per (path, tier). All share the CombatManeuverStrike group
        // so they're mutually exclusive: toggling one auto-disables any other.
        BuildVariant("Weaponmaster",  "Weaponmaster Maneuver", "+2 dodge AC and +2 competence to attack rolls for 1 round",
                     Guids.MartialPowerWeaponmasterManeuver, Guids.WeaponmasterPath, Guids.WeaponmasterManeuverBuff, iWeaponmasterBasic);
        BuildVariant("Weaponmaster",  "Burst of Speed",        "+30 ft enhancement speed and +4 competence to attack rolls for 1 round",
                     Guids.MartialPowerWeaponmasterExpanded, Guids.WeaponmasterExpandedManeuver, Guids.WeaponmasterExpandedManeuverBuff, iWeaponmasterExp);
        BuildVariant("Brawler",       "Brawler Maneuver",      "+7 competence damage on your next melee strike",
                     Guids.MartialPowerBrawlerManeuver, Guids.BrawlerPath, Guids.BrawlerManeuverBuff, iBrawlerBasic);
        BuildVariant("Brawler",       "Steel Sinews",          "+4 dodge AC and Fast Healing 2 for 1 round",
                     Guids.MartialPowerBrawlerExpanded, Guids.BrawlerExpandedManeuver, Guids.BrawlerExpandedManeuverBuff, iBrawlerExp);
        BuildVariant("Archer",        "Archer Maneuver",       "+4 competence to attack rolls on your next strike",
                     Guids.MartialPowerArcherManeuver, Guids.ArcherPath, Guids.ArcherManeuverBuff, iArcherBasic);
        BuildVariant("Archer",        "Twin Shot",             "Haste for 1 round (extra attack on full attacks, +1 dodge AC, +1 Reflex, +30 ft speed)",
                     Guids.MartialPowerArcherExpanded, Guids.ArcherExpandedFeature, Guids.ArcherExpandedBuff, iArcherExp);
        BuildVariant("Ascetic",       "Ascetic Maneuver",      "+4 dodge bonus to AC for 1 round",
                     Guids.MartialPowerAsceticManeuver, Guids.AsceticPath, Guids.AsceticManeuverBuff, iAsceticBasic);
        BuildVariant("Ascetic",       "Wholeness of Body",     "Heal yourself for HP equal to your manifester level",
                     Guids.MartialPowerAsceticExpanded, Guids.AsceticExpandedFeature, Guids.AsceticExpandedBuff, iAsceticExp);
        BuildVariant("Assassin's",    "Assassin's Maneuver",   "+7 competence damage on your next strike",
                     Guids.MartialPowerAssassinsManeuver, Guids.AssassinsPath, Guids.AssassinsManeuverBuff, iAssassinsBasic);
        BuildVariant("Assassin's",    "Mindblade Strike",      "+4 competence damage and +4 competence to all saves for 1 round",
                     Guids.MartialPowerAssassinsExpanded, Guids.AssassinsExpandedFeature, Guids.AssassinsExpandedBuff, iAssassinsExp);
        BuildVariant("Dervish",       "Dervish Maneuver",      "+2 competence to attack and damage rolls for 1 round",
                     Guids.MartialPowerDervishManeuver, Guids.DervishPath, Guids.DervishManeuverBuff, iDervishBasic);
        BuildVariant("Dervish",       "Whirlwind Step",        "Haste for 1 round (extra attack on full attacks, +1 dodge AC, +1 Reflex, +30 ft speed)",
                     Guids.MartialPowerDervishExpanded, Guids.DervishExpandedFeature, Guids.DervishExpandedBuff, iDervishExp);
        BuildVariant("Feral Warrior", "Feral Warrior Maneuver","+7 competence damage on your next strike",
                     Guids.MartialPowerFeralWarriorManeuver, Guids.FeralWarriorPath, Guids.FeralWarriorManeuverBuff, iFeralBasic);
        BuildVariant("Feral Warrior", "Feral Rend",            "Keen weapon edge (doubled critical threat range) for 1 round",
                     Guids.MartialPowerFeralWarriorExpanded, Guids.FeralWarriorExpandedFeature, Guids.FeralWarriorExpandedBuff, iFeralExp);
        BuildVariant("Gladiator",     "Gladiator Maneuver",    "+4 competence to CMB for 1 round",
                     Guids.MartialPowerGladiatorManeuver, Guids.GladiatorPath, Guids.GladiatorManeuverBuff, iGladiatorBasic);
        BuildVariant("Gladiator",     "Gladiator's Will",      "+4 competence to all saving throws for 1 round",
                     Guids.MartialPowerGladiatorExpanded, Guids.GladiatorExpandedFeature, Guids.GladiatorExpandedBuff, iGladiatorExp);
        BuildVariant("Infiltrator",   "Infiltrator Maneuver",  "+4 competence to Persuasion and +2 competence damage for 1 round",
                     Guids.MartialPowerInfiltratorManeuver, Guids.InfiltratorPath, Guids.InfiltratorManeuverBuff, iInfiltratorBasic);
        BuildVariant("Infiltrator",   "Hidden Step",           "+20 ft enhancement speed and +6 competence to Stealth for 1 round",
                     Guids.MartialPowerInfiltratorExpanded, Guids.InfiltratorExpandedFeature, Guids.InfiltratorExpandedBuff, iInfiltratorExp);
        BuildVariant("Interceptor",   "Interceptor Maneuver",  "+2 competence to attack and damage rolls for 1 round",
                     Guids.MartialPowerInterceptorManeuver, Guids.InterceptorPath, Guids.InterceptorManeuverBuff, iInterceptorBasic);
        BuildVariant("Interceptor",   "Save Another",          "DR 5/— for 1 round",
                     Guids.MartialPowerInterceptorExpanded, Guids.InterceptorExpandedFeature, Guids.InterceptorExpandedBuff, iInterceptorExp);
        BuildVariant("Mind Knight",   "Mind Knight Maneuver",  "+2 competence to attack and damage rolls for 1 round",
                     Guids.MartialPowerMindKnightManeuver, Guids.MindKnightPath, Guids.MindKnightManeuverBuff, iMindKnightBasic);
        BuildVariant("Mind Knight",   "Mental Strike",         "+4 insight Initiative and +4 dodge AC for 1 round",
                     Guids.MartialPowerMindKnightExpanded, Guids.MindKnightExpandedFeature, Guids.MindKnightExpandedBuff, iMindKnightExp);
        BuildVariant("Survivor",      "Survivor Maneuver",     "+4 competence to Fortitude and Will saves for 1 round",
                     Guids.MartialPowerSurvivorManeuver, Guids.SurvivorPath, Guids.SurvivorManeuverBuff, iSurvivorBasic);
        BuildVariant("Survivor",      "Survivor's Resolve",    "Temporary HP equal to your manifester level for 1 minute",
                     Guids.MartialPowerSurvivorExpanded, Guids.SurvivorExpandedFeature, Guids.SurvivorExpandedBuff, iSurvivorExp);

        // Martial Power feature — granted at level 6. Uses AddFeatureIfHasFact to conditionally
        // grant each variant Activatable on application: for each path/expanded feature the unit
        // already has, grant the corresponding Activatable. Path features have a mirror check so
        // paths picked AFTER Martial Power (e.g. secondary path at level 9) also get the grant.
        FeatureConfigurator.New("MartialPower", Guids.MartialPowerFeature)
            .SetDisplayName(Loc.Str("PW.MartialPower.Name", "Martial Power"))
            .SetDescription(Loc.Str(
                "PW.MartialPower.Desc",
                "At 6th level, you can manifest one of your path powers as part of a melee attack. For each path you have selected, a Martial Power toggle becomes available in your action bar. While a Martial Power variant is active, your first melee hit each round auto-manifests that path power as part of the attack — no psionic focus expended. Only one Martial Power variant can be active at a time; once per round across all variants."))
            .SetIcon(cooldownIcon)
            .SetIsClassFeature()
            .AddFeatureIfHasFact(checkedFact: Guids.WeaponmasterPath,        feature: Guids.MartialPowerWeaponmasterManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.WeaponmasterExpandedManeuver, feature: Guids.MartialPowerWeaponmasterExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.BrawlerPath,             feature: Guids.MartialPowerBrawlerManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.BrawlerExpandedManeuver, feature: Guids.MartialPowerBrawlerExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.ArcherPath,              feature: Guids.MartialPowerArcherManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.ArcherExpandedFeature,   feature: Guids.MartialPowerArcherExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.AsceticPath,             feature: Guids.MartialPowerAsceticManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.AsceticExpandedFeature,  feature: Guids.MartialPowerAsceticExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.AssassinsPath,           feature: Guids.MartialPowerAssassinsManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.AssassinsExpandedFeature,feature: Guids.MartialPowerAssassinsExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.DervishPath,             feature: Guids.MartialPowerDervishManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.DervishExpandedFeature,  feature: Guids.MartialPowerDervishExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.FeralWarriorPath,             feature: Guids.MartialPowerFeralWarriorManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.FeralWarriorExpandedFeature,  feature: Guids.MartialPowerFeralWarriorExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.GladiatorPath,           feature: Guids.MartialPowerGladiatorManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.GladiatorExpandedFeature,feature: Guids.MartialPowerGladiatorExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.InfiltratorPath,         feature: Guids.MartialPowerInfiltratorManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.InfiltratorExpandedFeature, feature: Guids.MartialPowerInfiltratorExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.InterceptorPath,         feature: Guids.MartialPowerInterceptorManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.InterceptorExpandedFeature, feature: Guids.MartialPowerInterceptorExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.MindKnightPath,          feature: Guids.MartialPowerMindKnightManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.MindKnightExpandedFeature, feature: Guids.MartialPowerMindKnightExpanded)
            .AddFeatureIfHasFact(checkedFact: Guids.SurvivorPath,            feature: Guids.MartialPowerSurvivorManeuver)
            .AddFeatureIfHasFact(checkedFact: Guids.SurvivorExpandedFeature, feature: Guids.MartialPowerSurvivorExpanded)
            .Configure();
    }

    /// <summary>
    /// Build one of the 24 variant Activatables, plus its companion buff (the buff that carries the
    /// on-hit trigger and gets applied while the toggle is on).
    /// </summary>
    private static void BuildVariant(
        string pathName, string maneuverName, string effectText,
        string variantActivatableGuid,
        string gatingFeatureGuid,        // path feature (for basic) or expanded feature (for expanded)
        string maneuverBuffGuid,         // the same buff the regular maneuver applies
        UnityEngine.Sprite icon)
    {
        var noSpacePath = pathName.Replace(" ", "").Replace("'", "");
        var noSpaceManeuver = maneuverName.Replace(" ", "").Replace("'", "");

        var displayName = $"Martial Power: {maneuverName}";

        // The on-active buff — has the attack trigger
        var triggerBuffGuid = variantActivatableGuid; // we reuse the variant guid by adding a suffix-derived sibling
        // We need a SEPARATE buff GUID per variant. Derive one programmatically: flip the last hex digit.
        // Easier: use a deterministic sibling computed via Guid manipulation. For clarity, we just
        // construct a string-prefixed key — the registered blueprint takes whatever GUID we hand it.
        var activeBuffGuid = DeriveSiblingGuid(variantActivatableGuid);

        BuffConfigurator.New($"MartialPower{noSpaceManeuver}OnBuff", activeBuffGuid)
            .SetDisplayName(Loc.Str($"PW.MartialPower{noSpaceManeuver}OnBuff.Name", displayName))
            .SetDescription(Loc.Str($"PW.MartialPower{noSpaceManeuver}OnBuff.Desc",
                $"Martial Power is set to auto-manifest {maneuverName} ({effectText}) on your first melee hit each round."))
            .SetIcon(icon)
            .AddInitiatorAttackWithWeaponTrigger(
                action: ActionsBuilder.New().Conditional(
                    conditions: ConditionsBuilder.New().CasterHasFact(Guids.MartialPowerUsedBuff, negate: true),
                    ifTrue: ActionsBuilder.New()
                        .ApplyBuff(maneuverBuffGuid, ContextDuration.Fixed(2), toCaster: true)
                        .ApplyBuff(Guids.MartialPowerUsedBuff, ContextDuration.Fixed(1), toCaster: true)),
                onlyHit: true,
                checkWeaponRangeType: true,
                rangeType: WeaponRangeType.Melee)
            .Configure();

        ActivatableAbilityConfigurator.New($"MartialPower{noSpaceManeuver}Activatable", variantActivatableGuid)
            .SetDisplayName(Loc.Str($"PW.MartialPower{noSpaceManeuver}Activatable.Name", displayName))
            .SetDescription(Loc.Str($"PW.MartialPower{noSpaceManeuver}Activatable.Desc",
                $"Toggle. While active, your first melee hit each round auto-manifests {maneuverName} — {effectText} — without expending psionic focus. Only one Martial Power variant can be active at a time. Once per round across all variants."))
            .SetIcon(icon)
            .SetBuff(activeBuffGuid)
            .SetGroup(ActivatableAbilityGroup.CombatManeuverStrike)
            .SetActivationType(AbilityActivationType.Immediately)
            .SetIsOnByDefault(false)
            .Configure();
    }

    /// <summary>
    /// Given a hex-GUID string, produce a different-but-deterministic sibling by flipping the last hex digit.
    /// Used to derive the "on buff" GUID from the variant Activatable GUID without needing a hand-maintained pair list.
    /// </summary>
    private static string DeriveSiblingGuid(string guid)
    {
        // Increment the last hex digit to produce a deterministic sibling GUID.
        // '9' jumps to 'a' to stay within valid hex; 'f' steps back to 'e'.
        var lastChar = char.ToLower(guid[guid.Length - 1]);
        char newChar = lastChar switch
        {
            '9' => 'a',
            'f' => 'e',
            _ => (char)(lastChar + 1)
        };
        return guid.Substring(0, guid.Length - 1) + newChar;
    }
}
