using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class TrueSeeing
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWTrueSeeing", Guids.PowerTrueSeeing)
            .CopyFrom(AbilityRefs.TrueSeeing)
            .SetDisplayName(Loc.Str("PW.TrueSeeing.Name", "True Seeing, Psionic", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.TrueSeeing.Desc",
                "You confer on the subject the ability to see all things as they actually are. The subject sees through normal and magical darkness, notices secret doors, sees the exact locations of creatures or objects under blur or displacement effects, and sees invisible creatures or objects for 1 minute per manifester level.",
                tagEncyclopediaEntries: false))
            .SetIcon(AbilityRefs.TrueSeeing.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .AddSpellListComponent(5, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Divination)
            .Configure();
    }
}
