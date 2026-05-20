using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class PhysicalAcceleration
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWPhysicalAcceleration", Guids.PowerPhysicalAcceleration)
            .CopyFrom(AbilityRefs.Haste)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetDisplayName(Loc.Str("PW.PhysicalAcceleration.Name", "Physical Acceleration"))
            .SetDescription(Loc.Str("PW.PhysicalAcceleration.Desc",
                "You psionically accelerate your body. You gain the benefits of haste for 1 round per manifester level: +1 bonus on attack rolls, +1 dodge bonus to AC and Reflex saves, +30 ft. enhancement to speed, and one extra attack at your highest base attack bonus."))
            .AddSpellListComponent(3, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
