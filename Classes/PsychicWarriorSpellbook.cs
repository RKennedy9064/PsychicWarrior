using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Spells;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Classes;

/// <summary>
/// Psychic Warrior spellbook configuration.
/// 
/// Power Points Per Day Table:
/// Lvl  | 1st | 2nd | 3rd | 4th | 5th | 6th | Total
/// -----|-----|-----|-----|-----|-----|-----|-------
/// 1    | 1   | -   | -   | -   | -   | -   | 1
/// 2    | 2   | -   | -   | -   | -   | -   | 2
/// 3    | 4   | -   | -   | -   | -   | -   | 4
/// 4    | 6   | 3   | -   | -   | -   | -   | 9
/// 5    | 8   | 5   | -   | -   | -   | -   | 13
/// 6    | 12  | 7   | 3   | -   | -   | -   | 22
/// 7    | 16  | 10  | 5   | -   | -   | -   | 31
/// 8    | 20  | 13  | 7   | 3   | -   | -   | 43
/// ...and so on through level 20 (20 powers known, up to 6th level powers, 128 power points/day)
/// 
/// In WOTR, we approximate this using spell slots. The Inquisitor progression is a reasonable match.
/// </summary>
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

                // 0-level talents
                bp.SpellsByLevel[0].m_Spells.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.PowerMinorPrecognition));

                // 1st-level powers
                bp.SpellsByLevel[1].m_Spells.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.PowerVigor));
                bp.SpellsByLevel[1].m_Spells.Add(BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.PowerForceScreen));

                // TODO: Add more powers to their respective levels as they are implemented
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