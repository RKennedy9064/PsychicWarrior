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

        FeatureConfigurator.New("PathExpandedManeuver", Guids.PathExpandedManeuver).SetHideInUI(true).Configure();
        FeatureConfigurator.New("SecondaryPathSelection", Guids.SecondaryPathSelection).SetHideInUI(true).Configure();
        FeatureConfigurator.New("TwistingPaths", Guids.TwistingPaths).SetHideInUI(true).Configure();
        FeatureConfigurator.New("Pathweaving", Guids.Pathweaving).SetHideInUI(true).Configure();
    }
}