using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class DimensionDoor
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWDimensionDoor", Guids.PowerDimensionDoor)
            .CopyFrom(AbilityRefs.DimensionDoor)
            .SetDisplayName(Loc.Str("PW.DimensionDoor.Name", "Dimension Door, Psionic", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.DimensionDoor.Desc",
                "You instantly transfer yourself from your current location to any other spot within range. You can travel up to 400 feet + 40 feet per manifester level beyond 8th.",
                tagEncyclopediaEntries: false))
            .SetIcon(AbilityRefs.DimensionDoor.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .AddSpellListComponent(4, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Conjuration)
            .Configure();
    }
}
