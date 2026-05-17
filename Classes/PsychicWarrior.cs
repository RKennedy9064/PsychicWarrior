using BlueprintCore.Blueprints.Configurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Classes;

public static class PsychicWarriorClass
{
    public static void Configure()
    {
        var progression = ProgressionConfigurator.New("PsychicWarrior.Progression", Guids.PsychicWarriorProgression)
            .SetDisplayName(LocalizationTool.CreateString("PW.Progression.Name", "Psychic Warrior"))
            .SetDescription(LocalizationTool.CreateString("PW.Progression.Desc", "A martial psionic combatant."))
            .SetClasses(Guids.PsychicWarriorClass)
            .SetUIGroups(
                UIGroupBuilder.New()
                    .AddGroup(Guids.BonusFeatSelection)
                    .AddGroup(Guids.Proficiencies, Guids.GainPsionicFocusFeature, Guids.PsionicProficiency)
                    .AddGroup(Guids.PathSelectionLevel1, Guids.PathExpandedManeuver, Guids.TalentsSelection, Guids.PathSkillBonusFeature, Guids.MartialPowerFeature)
                    .AddGroup(Guids.SecondaryPathSelection, Guids.TwistingPaths, Guids.Pathweaving))
            .AddToLevelEntries(1, 
                Guids.Proficiencies, 
                Guids.GainPsionicFocusFeature, 
                Guids.PsionicProficiency, 
                Guids.PathSelectionLevel1, 
                Guids.TalentsSelection, 
                Guids.BonusFeatSelection)
            .AddToLevelEntries(2, Guids.BonusFeatSelection)
            .AddToLevelEntries(3, Guids.PathExpandedManeuver)
            .AddToLevelEntries(4, Guids.PathSkillBonusFeature)
            .AddToLevelEntries(5, Guids.BonusFeatSelection)
            .AddToLevelEntries(6, Guids.MartialPowerFeature)
            .AddToLevelEntries(7, Guids.PathSkillBonusFeature)
            .AddToLevelEntries(8, Guids.BonusFeatSelection)
            .AddToLevelEntries(9, Guids.SecondaryPathSelection)
            .AddToLevelEntries(10, Guids.PathSkillBonusFeature)
            .AddToLevelEntries(11, Guids.BonusFeatSelection, Guids.TwistingPaths)
            .AddToLevelEntries(13, Guids.PathSkillBonusFeature)
            .AddToLevelEntries(14, Guids.BonusFeatSelection)
            .AddToLevelEntries(15, Guids.Pathweaving)
            .AddToLevelEntries(16, Guids.PathSkillBonusFeature)
            .AddToLevelEntries(17, Guids.BonusFeatSelection)
            .AddToLevelEntries(19, Guids.PathSkillBonusFeature)
            .AddToLevelEntries(20, Guids.BonusFeatSelection)
            .Configure();

        CharacterClassConfigurator.New("PsychicWarrior.Class", Guids.PsychicWarriorClass)
            .SetLocalizedName(LocalizationTool.CreateString("PW.Class.Name", "Psychic Warrior"))
            .SetLocalizedDescription(LocalizationTool.CreateString("PW.Class.Desc", "A martial psionic combatant..."))
            .SetHitDie(DiceType.D8)
            .SetSkillPoints(4)
            .SetBaseAttackBonus(StatProgressionRefs.BABMedium.ToString())
            .SetFortitudeSave(StatProgressionRefs.SavesHigh.ToString())
            .SetReflexSave(StatProgressionRefs.SavesLow.ToString())
            .SetWillSave(StatProgressionRefs.SavesLow.ToString())
            .SetClassSkills(StatType.SkillAthletics, StatType.SkillMobility, StatType.SkillPerception)
            .SetSpellbook(Guids.Spellbook)
            .SetProgression(progression)
            .SetStartingGold(411)
            .SetPrimaryColor(1)
            .SetSecondaryColor(1)
            .SetDifficulty(1)
            .SetRecommendedAttributes(StatType.Strength, StatType.Wisdom)
            .SetNotRecommendedAttributes(StatType.Charisma)
            .OnConfigure(bp =>
            {
                var fighter = CharacterClassRefs.FighterClass.Reference.Get();
                bp.m_EquipmentEntities = fighter.m_EquipmentEntities;
                bp.m_StartingItems = fighter.m_StartingItems;
            })
            .Configure();
    }
}