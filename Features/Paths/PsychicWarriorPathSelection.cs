using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features.Paths;

public static class PsychicWarriorPathSelection
{
    public static void Configure()
    {
        // Weaponmaster Icons
        // FIXED: Swapped WeaponFocus for PowerAttack to avoid ParametrizedFeature issues
        var wmTranceIcon = FeatureRefs.PowerAttackFeature.Reference.Get().Icon;
        var wmManeuverIcon = AbilityRefs.DivineFavor.Reference.Get().Icon;

        var weaponmasterTrance = FeatureConfigurator.New("WeaponmasterTrance", Guids.WeaponmasterTrance)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMTrance.Name", "Weaponmaster Trance"))
            .SetDescription(LocalizationTool.CreateString("PW.WMTrance.Desc", "You gain a +1 competence bonus to attack rolls."))
            .SetIcon(wmTranceIcon)
            .SetIsClassFeature()
            .AddComponent(new AddStatBonus { Stat = StatType.AdditionalAttackBonus, Value = 1, Descriptor = ModifierDescriptor.Competence })
            .Configure();

        var weaponmasterManeuverBuff = BuffConfigurator.New("WeaponmasterManeuverBuff", Guids.WeaponmasterManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMManeuver.Name", "Weaponmaster Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMManeuver.Desc", "You gain a +2 dodge bonus to AC for 1 round."))
            .SetIcon(wmManeuverIcon)
            .AddComponent(new AddStatBonus { Stat = StatType.AC, Value = 2, Descriptor = ModifierDescriptor.Dodge })
            .Configure();

        var weaponmasterManeuver = AbilityConfigurator.New("WeaponmasterManeuver", Guids.WeaponmasterManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMManeuverAb.Name", "Weaponmaster Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMManeuverAb.Desc", "Swift Action. Expend Psionic Focus to gain a +2 dodge bonus to AC for 1 round."))
            .SetIcon(wmManeuverIcon)
            .SetActionType(UnitCommand.CommandType.Swift)
            .AddComponent(new AbilityCasterHasFacts
            {
                m_Facts = new[] { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) }
            })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(weaponmasterManeuverBuff, ContextDuration.Fixed(1)))
            .Configure();

        var weaponmasterPath = FeatureConfigurator.New("WeaponmasterPath", Guids.WeaponmasterPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.WeaponmasterPath.Name", "Weaponmaster Path"))
            .SetDescription(LocalizationTool.CreateString("PW.WeaponmasterPath.Desc", "Focuses on martial superiority. Grants Weaponmaster Trance and Maneuver."))
            .SetIcon(wmTranceIcon)
            .SetIsClassFeature()
            .AddFacts([weaponmasterTrance.ToString(), weaponmasterManeuver.ToString()])
            .Configure();


        // Brawler Icons
        var brawlerTranceIcon = FeatureRefs.ImprovedUnarmedStrike.Reference.Get().Icon;
        // FIXED: Corrected grammatical spelling to BullsStrength
        var brawlerManeuverIcon = AbilityRefs.BullsStrength.Reference.Get().Icon;

        var brawlerTrance = FeatureConfigurator.New("BrawlerTrance", Guids.BrawlerTrance)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerTrance.Name", "Brawler Trance"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerTrance.Desc", "You gain a +1 competence bonus to Athletics checks."))
            .SetIcon(brawlerTranceIcon)
            .SetIsClassFeature()
            .AddComponent(new AddStatBonus { Stat = StatType.SkillAthletics, Value = 1, Descriptor = ModifierDescriptor.Competence })
            .Configure();

        var brawlerManeuverBuff = BuffConfigurator.New("BrawlerManeuverBuff", Guids.BrawlerManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerManeuver.Name", "Brawler Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerManeuver.Desc", "You gain a +2 competence bonus to unarmed and natural attack damage rolls for 1 round."))
            .SetIcon(brawlerManeuverIcon)
            .AddComponent(new AddStatBonus { Stat = StatType.AdditionalDamage, Value = 2, Descriptor = ModifierDescriptor.Competence })
            .Configure();

        var brawlerManeuver = AbilityConfigurator.New("BrawlerManeuver", Guids.BrawlerManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerManeuverAb.Name", "Brawler Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerManeuverAb.Desc", "Swift Action. Expend Psionic Focus to gain a +2 bonus to damage rolls for 1 round."))
            .SetIcon(brawlerManeuverIcon)
            .SetActionType(UnitCommand.CommandType.Swift)
            .AddComponent(new AbilityCasterHasFacts
            {
                m_Facts = new[] { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) }
            })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(brawlerManeuverBuff, ContextDuration.Fixed(1)))
            .Configure();

        var brawlerPath = FeatureConfigurator.New("BrawlerPath", Guids.BrawlerPath)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerPath.Name", "Brawler Path"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerPath.Desc", "Focuses on unarmed combat and athletics. Grants Brawler Trance and Maneuver."))
            .SetIcon(brawlerTranceIcon)
            .SetIsClassFeature()
            .AddFacts([brawlerTrance.ToString(), brawlerManeuver.ToString()])
            .Configure();

        FeatureSelectionConfigurator.New("PsychicWarriorPathSelection", Guids.PathSelectionLevel1)
            .SetDisplayName(LocalizationTool.CreateString("PW.PathSelection.Name", "Psychic Warrior Path"))
            .SetDescription(LocalizationTool.CreateString("PW.PathSelection.Desc", "At 1st level, a psychic warrior chooses a path..."))
            .SetIsClassFeature()
            .AddToAllFeatures(weaponmasterPath)
            .AddToAllFeatures(brawlerPath)
            .Configure();

        // --- PathExpandedManeuver (Level 3): each path gains a second maneuver option ---

        var wmExpandedBuff = BuffConfigurator.New("WeaponmasterExpandedManeuverBuff", Guids.WeaponmasterExpandedManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMExpanded.Name", "Weaponmaster Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMExpanded.Desc", "You gain a +2 competence bonus to weapon damage rolls for 1 round."))
            .SetIcon(wmManeuverIcon)
            .AddComponent(new AddStatBonus { Stat = StatType.AdditionalDamage, Value = 2, Descriptor = ModifierDescriptor.Competence })
            .Configure();

        var wmExpandedAbility = AbilityConfigurator.New("WeaponmasterExpandedManeuverAbility", Guids.WeaponmasterExpandedManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMExpandedAb.Name", "Weaponmaster Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMExpandedAb.Desc", "Swift Action. Expend Psionic Focus to gain a +2 competence bonus to weapon damage rolls for 1 round."))
            .SetIcon(wmManeuverIcon)
            .SetActionType(UnitCommand.CommandType.Swift)
            .AddComponent(new AbilityCasterHasFacts
            {
                m_Facts = new[] { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) }
            })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(wmExpandedBuff, ContextDuration.Fixed(1)))
            .Configure();

        var wmExpandedFeature = FeatureConfigurator.New("WeaponmasterExpandedManeuver", Guids.WeaponmasterExpandedManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.WMExpandedFeat.Name", "Weaponmaster Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.WMExpandedFeat.Desc", "Swift Action. Expend Psionic Focus to gain a +2 competence bonus to weapon damage rolls for 1 round."))
            .SetIcon(wmManeuverIcon)
            .SetIsClassFeature()
            .AddFacts([Guids.WeaponmasterExpandedManeuverAbility])
            .AddPrerequisiteFeature(Guids.WeaponmasterPath)
            .Configure();

        var brawlerExpandedBuff = BuffConfigurator.New("BrawlerExpandedManeuverBuff", Guids.BrawlerExpandedManeuverBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerExpanded.Name", "Brawler Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerExpanded.Desc", "You gain a +2 dodge bonus to AC for 1 round."))
            .SetIcon(brawlerManeuverIcon)
            .AddComponent(new AddStatBonus { Stat = StatType.AC, Value = 2, Descriptor = ModifierDescriptor.Dodge })
            .Configure();

        var brawlerExpandedAbility = AbilityConfigurator.New("BrawlerExpandedManeuverAbility", Guids.BrawlerExpandedManeuverAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerExpandedAb.Name", "Brawler Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerExpandedAb.Desc", "Swift Action. Expend Psionic Focus to gain a +2 dodge bonus to AC for 1 round."))
            .SetIcon(brawlerManeuverIcon)
            .SetActionType(UnitCommand.CommandType.Swift)
            .AddComponent(new AbilityCasterHasFacts
            {
                m_Facts = new[] { BlueprintTool.GetRef<BlueprintUnitFactReference>(Guids.PsionicFocusBuff) }
            })
            .AddAbilityEffectRunAction(
                ActionsBuilder.New()
                    .RemoveBuff(Guids.PsionicFocusBuff)
                    .ApplyBuff(brawlerExpandedBuff, ContextDuration.Fixed(1)))
            .Configure();

        var brawlerExpandedFeature = FeatureConfigurator.New("BrawlerExpandedManeuver", Guids.BrawlerExpandedManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.BrawlerExpandedFeat.Name", "Brawler Expanded Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.BrawlerExpandedFeat.Desc", "Swift Action. Expend Psionic Focus to gain a +2 dodge bonus to AC for 1 round."))
            .SetIcon(brawlerManeuverIcon)
            .SetIsClassFeature()
            .AddFacts([Guids.BrawlerExpandedManeuverAbility])
            .AddPrerequisiteFeature(Guids.BrawlerPath)
            .Configure();

        FeatureSelectionConfigurator.New("PathExpandedManeuver", Guids.PathExpandedManeuver)
            .SetDisplayName(LocalizationTool.CreateString("PW.PathExpandedManeuver.Name", "Expanded Path Maneuver"))
            .SetDescription(LocalizationTool.CreateString("PW.PathExpandedManeuver.Desc",
                "At 3rd level, you gain an additional maneuver from your chosen path, expanding your tactical options in combat."))
            .SetIsClassFeature()
            .SetIgnorePrerequisites(false)
            .AddToAllFeatures(wmExpandedFeature, brawlerExpandedFeature)
            .Configure();

        FeatureSelectionConfigurator.New("SecondaryPathSelection", Guids.SecondaryPathSelection)
            .SetDisplayName(LocalizationTool.CreateString("PW.SecondaryPath.Name", "Secondary Path"))
            .SetDescription(LocalizationTool.CreateString("PW.SecondaryPath.Desc",
                "At 9th level, a psychic warrior chooses a second warrior's path. He gains the trance and maneuver of that path. " +
                "The psychic warrior may only benefit from one trance at a time (see Twisting Paths)."))
            .SetIsClassFeature()
            .AddToAllFeatures(weaponmasterPath, brawlerPath)
            .Configure();

        // TODO (Phase 6): Once trances are redesigned as toggled buffs, implement as a swift-action
        // ability that removes the active trance buff and applies the other path's trance buff.
        FeatureConfigurator.New("TwistingPaths", Guids.TwistingPaths)
            .SetDisplayName(LocalizationTool.CreateString("PW.TwistingPaths.Name", "Twisting Paths"))
            .SetDescription(LocalizationTool.CreateString("PW.TwistingPaths.Desc",
                "At 11th level, while maintaining psionic focus and using a trance, the psychic warrior can spend a swift action " +
                "to switch to the trance of his other path."))
            .SetIsClassFeature()
            .Configure();

        // TODO (Phase 6): Once trances are redesigned as toggled buffs, implement as a free-action
        // ability (1/day, +1/day per 3 levels after 15) that activates both trance buffs simultaneously
        // for 5 minutes.
        FeatureConfigurator.New("Pathweaving", Guids.Pathweaving)
            .SetDisplayName(LocalizationTool.CreateString("PW.Pathweaving.Name", "Pathweaving"))
            .SetDescription(LocalizationTool.CreateString("PW.Pathweaving.Desc",
                "At 15th level, while maintaining psionic focus, the psychic warrior can spend a free action once per day " +
                "to gain the benefits of both trances simultaneously for 5 minutes. He gains one additional use per day for " +
                "every three psychic warrior levels beyond 15th."))
            .SetIsClassFeature()
            .Configure();
    }
}