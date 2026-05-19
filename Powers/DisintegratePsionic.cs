using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class DisintegratePsionic
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWDisintegrate", Guids.PowerDisintegratePsionic)
            .CopyFrom(AbilityRefs.Disintegrate)
            .SetDisplayName(Loc.Str("PW.Disintegrate.Name", "Disintegrate, Psionic", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.Disintegrate.Desc",
                "A thin, green ray springs from your pointing finger. You must make a successful ranged touch attack to hit. Any creature struck by the ray takes 2d6 damage per manifester level (maximum 40d6). Any creature reduced to 0 or fewer hit points by this power is entirely disintegrated.",
                tagEncyclopediaEntries: false))
            .SetIcon(AbilityRefs.Disintegrate.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .AddSpellListComponent(6, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Transmutation)
            .Configure();
    }
}
