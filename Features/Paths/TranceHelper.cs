using System;
using System.Collections.Generic;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Commands.Base;
using PsychicWarrior.Utils;
using UnityEngine;

namespace PsychicWarrior.Features.Paths;

/// <summary>
/// Path-power scaffolding — Kineticist-style layout.
///
/// Per path the helper produces:
///   - <c>TranceBuff</c>: stat-bonus buff applied while the trance is on. Auto-removed when
///     Psionic Focus is lost (handled in <c>Mechanics/Focus.cs</c>).
///   - <c>TranceActivatable</c>: standalone action-bar toggle. <c>Group = BarbarianStance</c>
///     enforces only one trance active at a time. <c>WithUnitCommand</c> / Standard action by
///     default; Twisting Paths flips that to Swift via <c>ChangeActivatableAbilitiesCommandType</c>.
///     <c>RestrictionHasFact(PsionicFocusBuff)</c> gates activation on having focus.
///   - <c>ParentAbility</c>: expandable menu for the path's maneuvers. Click to expand and pick
///     Maneuver or Expanded Maneuver. Same UX as Animal Affinity / Alchemist mutagens.
///   - <c>TranceFeature</c>: granted at level 1 by the path. <c>AddFacts</c> adds <i>both</i>
///     the activatable (spinning-border icon) and the parent menu (maneuvers icon).
/// </summary>
internal static class TranceHelper
{
    public static BlueprintFeature BuildTrance(
        string baseName,
        string tranceFeatureGuid,
        string tranceBuffGuid,
        string tranceToggleStdGuid,         // unused — kept for source compatibility
        string tranceToggleSwiftGuid,       // unused — kept for source compatibility
        string parentAbilityGuid,
        string maneuverAbilityGuid,
        string expandedManeuverAbilityGuid,
        string displayName,
        string featureDescription,
        Sprite icon,
        Action<BuffConfigurator> addBuffComponents)
    {
        var tranceBuff = BuffConfigurator.New($"{baseName}TranceBuff", tranceBuffGuid)
            .SetDisplayName(LocalizationTool.CreateString($"PW.{baseName}TranceBuff.Name", displayName, tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{baseName}TranceBuff.Desc", featureDescription, tagEncyclopediaEntries: false))
            .SetIcon(icon);
        addBuffComponents(tranceBuff);
        tranceBuff.Configure();

        // Trance Activatable — separate action-bar icon with spinning border
        ActivatableAbilityConfigurator.New($"{baseName}TranceActivatable", Guids.GetTranceActivatableGuid(baseName))
            .SetDisplayName(LocalizationTool.CreateString($"PW.{baseName}TranceActivatable.Name", displayName + " Trance", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{baseName}TranceActivatable.Desc",
                featureDescription + " Toggling takes a standard action (reduced to swift at PW 11 by Twisting Paths). Only one trance can be active at a time. Requires psionic focus.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetBuff(tranceBuffGuid)
            .SetGroup(ActivatableAbilityGroup.BarbarianStance)
            .SetActivationType(AbilityActivationType.WithUnitCommand)
            .SetActivateWithUnitCommand(UnitCommand.CommandType.Standard)
            .SetIsOnByDefault(false)
            .AddRestrictionHasFact(feature: Guids.PsionicFocusBuff)
            .Configure();

        // Parent ability — maneuver menu (Kineticist-blast-style variant container)
        AbilityConfigurator.New($"{baseName}PathPowers", parentAbilityGuid)
            .SetDisplayName(LocalizationTool.CreateString($"PW.{baseName}PathPowers.Name", displayName + " Maneuvers", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{baseName}PathPowers.Desc",
                $"Click to choose a {displayName.ToLower()} maneuver. At 3rd level, the Expanded Maneuver becomes a second option.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .AddAbilityVariants(variants: new List<Blueprint<BlueprintAbilityReference>>
            {
                maneuverAbilityGuid,
                expandedManeuverAbilityGuid,
            })
            .Configure();

        return FeatureConfigurator.New($"{baseName}Trance", tranceFeatureGuid)
            .SetDisplayName(LocalizationTool.CreateString($"PW.{baseName}Trance.Name", displayName, tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{baseName}Trance.Desc", featureDescription, tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.GetTranceActivatableGuid(baseName), parentAbilityGuid })
            .Configure();
    }
}
