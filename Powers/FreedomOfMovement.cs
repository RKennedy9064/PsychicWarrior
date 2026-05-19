using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Powers;

public static class FreedomOfMovement
{
    public static void Configure()
    {
        AbilityConfigurator.New("PWFreedomOfMovement", Guids.PowerFreedomOfMovement)
            .CopyFrom(AbilityRefs.FreedomOfMovement)
            .SetDisplayName(Loc.Str("PW.FreedomOfMovement.Name", "Freedom of Movement, Psionic", tagEncyclopediaEntries: false))
            .SetDescription(Loc.Str("PW.FreedomOfMovement.Desc",
                "This power enables you or a creature you touch to move and attack normally for 1 minute per manifester level, even under the influence of magic that usually impedes movement.",
                tagEncyclopediaEntries: false))
            .SetIcon(AbilityRefs.FreedomOfMovement.Reference.Get().Icon)
            .SetType(AbilityType.Supernatural)
            .AddSpellListComponent(4, Guids.SpellList)
            .AddSpellComponent(SpellSchool.Abjuration)
            .Configure();
    }
}
