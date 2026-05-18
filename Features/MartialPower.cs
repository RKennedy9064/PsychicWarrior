using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.ActivatableAbilities;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Martial Power (Su): At 6th level, if the psychic warrior makes an attack (but not a ranged touch
/// attack), he can manifest one of his path powers as part of that attack action.
///
/// Current implementation: a toggleable activatable. While active, your melee attacks carry the
/// channelled path power — +2 to hit and +6 damage. Default-on; toggle off if you don't want it.
/// Full RAW "manifest a specific path power as part of the attack" stays Phase 12.
/// </summary>
public static class MartialPower
{
    public static void Configure()
    {
        var icon = FeatureRefs.PowerAttackFeature.Reference.Get().Icon;

        // Persistent buff while the activatable is on
        var buff = BuffConfigurator.New("MartialPowerBuff", Guids.MartialPowerBuff)
            .SetDisplayName(LocalizationTool.CreateString("PW.MartialPowerBuff.Name", "Martial Power", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.MartialPowerBuff.Desc",
                "You channel a path power through your attacks: +2 to hit and +6 damage on melee strikes.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalAttackBonus, value: 2)
            .AddStatBonus(descriptor: ModifierDescriptor.Competence, stat: StatType.AdditionalDamage, value: 6)
            .Configure();

        // Activatable toggle — default on, free to flip
        ActivatableAbilityConfigurator.New("MartialPowerActivatable", Guids.MartialPowerAbility)
            .SetDisplayName(LocalizationTool.CreateString("PW.MartialPowerAbility.Name", "Martial Power", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString("PW.MartialPowerAbility.Desc",
                "Toggle. While active, your melee attacks carry a channelled path power, gaining +2 to hit and +6 damage.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetBuff(buff)
            .SetActivationType(AbilityActivationType.Immediately)
            .SetIsOnByDefault(true)
            .Configure();

        FeatureConfigurator.New("MartialPower", Guids.MartialPowerFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.MartialPower.Name", "Martial Power", tagEncyclopediaEntries: false))
            .SetDescription(LocalizationTool.CreateString(
                "PW.MartialPower.Desc",
                "At 6th level, you can channel one of your path powers through your melee attacks. While the Martial Power toggle is active, your strikes gain +2 to hit and +6 damage.",
                tagEncyclopediaEntries: false))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts(new() { Guids.MartialPowerAbility })
            .Configure();
    }
}
