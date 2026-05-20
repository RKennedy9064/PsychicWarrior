using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class Compression
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWCompression", Guids.PowerCompression)
            .CopyFrom(AbilityRefs.ReducePerson)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetDisplayName(Loc.Str("PW.Compression.Name", "Compression"))
            .SetDescription(Loc.Str("PW.Compression.Desc",
                "Your body shrinks to Small size, granting +2 Dexterity, –2 Strength, +1 bonus to attack rolls and AC. Lasts 1 minute per manifester level."))
            .AddSpellListComponent(1, Guids.SpellList)
            .Configure();
    }
}
