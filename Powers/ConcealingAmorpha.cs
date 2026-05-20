using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class ConcealingAmorpha
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWConcealingAmorpha", Guids.PowerConcealingAmorpha)
            .CopyFrom(AbilityRefs.Blur)
            .SetType(AbilityType.Supernatural)
            .SetRange(AbilityRange.Personal)
            .SetDisplayName(Loc.Str("PW.ConcealingAmorpha.Name", "Concealing Amorpha"))
            .SetDescription(Loc.Str("PW.ConcealingAmorpha.Desc",
                "You surround yourself with a distortion of ectoplasmic shimmer that grants you a 20% miss chance for 1 minute per manifester level."))
            .AddSpellListComponent(2, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
