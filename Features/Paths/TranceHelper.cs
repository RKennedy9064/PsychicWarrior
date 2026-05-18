using System;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Commands.Base;
using PsychicWarrior.Utils;
using UnityEngine;

namespace PsychicWarrior.Features.Paths;

/// <summary>
/// Phase 7 trance rework: a path's trance is now a Buff + Activatable Ability + Feature.
/// All trance activatables share <see cref="ActivatableAbilityGroup.BarbarianStance"/>
/// so only one trance can be active at a time (mutual exclusion).
/// </summary>
internal static class TranceHelper
{
    public static BlueprintFeature BuildTrance(
        string baseName,
        string tranceFeatureGuid,
        string tranceBuffGuid,
        string tranceActivatableGuid,
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

        ActivatableAbilityConfigurator.New($"{baseName}TranceActivatable", tranceActivatableGuid)
            .SetDisplayName(LocalizationTool.CreateString($"PW.{baseName}TranceActivatable.Name", displayName, tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{baseName}TranceActivatable.Desc",
                featureDescription + " Requires psionic focus to maintain — losing focus ends the trance. Toggling takes a standard action (reduced to swift at Psychic Warrior 11 by Twisting Paths). Only one trance can be active at a time.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetBuff(tranceBuffGuid)
            .SetGroup(ActivatableAbilityGroup.BarbarianStance)
            .SetActivationType(AbilityActivationType.WithUnitCommand)
            .SetActivateWithUnitCommand(UnitCommand.CommandType.Standard)
            .SetIsOnByDefault(false)
            .AddRestrictionHasFact(feature: Guids.PsionicFocusBuff)
            .Configure();

        return FeatureConfigurator.New($"{baseName}Trance", tranceFeatureGuid)
            .SetDisplayName(LocalizationTool.CreateString($"PW.{baseName}Trance.Name", displayName, tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString($"PW.{baseName}Trance.Desc",
                featureDescription + " Only one trance can be active at a time; toggle from the action bar.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { tranceActivatableGuid })
            .Configure();
    }
}
