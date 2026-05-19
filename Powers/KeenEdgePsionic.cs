using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

/// <summary>
/// Keen Edge, Psionic (Metacreativity → Transmutation) — Double the threat range of a weapon you
/// touch. Reuses WoTR's existing Keen Edge spell mechanics (target a weapon, apply keen weapon
/// quality for 10 min/level).
/// </summary>
public static class KeenEdgePsionic
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWKeenEdge", Guids.PowerKeenEdgePsionic)
            .CopyFrom(AbilityRefs.KeenEdge)
            .SetDisplayName(LocalizationTool.CreateString("PW.KeenEdgePsionic.Name", "Keen Edge, Psionic", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.KeenEdgePsionic.Desc",
                "Psionic energy hones the edge of one weapon you touch, doubling its threat range for 10 minutes per manifester level. The effect does not stack with other effects that increase threat range.",
                tagEncyclopediaEntries: false))
            .SetIcon(AbilityRefs.KeenEdge.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .AddSpellListComponent(3, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
