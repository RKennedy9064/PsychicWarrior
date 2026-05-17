using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Spells;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Classes;

public static class PsychicWarriorSpellbook
{
    public static void Configure()
    {
        var spellList = SpellListConfigurator.New("PsychicWarriorSpellList", Guids.SpellList)
            .OnConfigure(bp =>
            {
                bp.SpellsByLevel = new SpellLevelList[7];
                for (int i = 0; i < 7; i++)
                {
                    bp.SpellsByLevel[i] = new SpellLevelList(i);
                }

                bp.SpellsByLevel[0].m_Spells.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.PowerMinorPrecognition));
                bp.SpellsByLevel[1].m_Spells.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.PowerVigor));
                bp.SpellsByLevel[1].m_Spells.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.PowerForceScreen));
            })
            .Configure();

        SpellbookConfigurator.New("PsychicWarriorSpellbook", Guids.Spellbook)
            .SetName(LocalizationTool.CreateString("PW.Spellbook.Name", "Psychic Warrior Powers"))
            .SetCharacterClass(Guids.PsychicWarriorClass)
            .SetSpellsPerDay(SpellsTableRefs.InquisitorSpellSlotsTable.ToString())
            .SetSpellsKnown(SpellsTableRefs.InquisitorSpellsKnownTable.ToString())
            .SetSpellList(spellList)
            .SetCastingAttribute(StatType.Wisdom)
            .SetSpontaneous(true)
            .SetIsArcane(false)
            .SetAllSpellsKnown(false)
            .SetCantripsType(CantripsType.Cantrips)
            .Configure();
    }
}