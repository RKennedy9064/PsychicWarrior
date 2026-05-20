using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class Expansion
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWExpansion", Guids.PowerExpansion)
            .CopyFrom(AbilityRefs.EnlargePerson)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetDisplayName(Loc.Str("PW.Expansion.Name", "Expansion"))
            .SetDescription(Loc.Str("PW.Expansion.Desc",
                "Your body grows to Large size, granting +2 Strength, –2 Dexterity, –1 penalty to attack rolls and AC. Lasts 1 minute per manifester level."))
            .AddSpellListComponent(1, Guids.SpellList)
            .Configure();
    }
}
