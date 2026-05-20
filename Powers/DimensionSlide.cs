using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class DimensionSlide
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWDimensionSlide", Guids.PowerDimensionSlide)
            .CopyFrom(AbilityRefs.DimensionDoor)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetDisplayName(Loc.Str("PW.DimensionSlide.Name", "Dimension Slide"))
            .SetDescription(Loc.Str("PW.DimensionSlide.Desc",
                "You teleport to any visible location within close range (25 ft. + 5 ft./2 levels). You do not provoke attacks of opportunity from this movement."))
            .AddSpellListComponent(3, Guids.SpellList)
            .Configure();
    }
}
