using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

/// <summary>
/// Martial Power (Su): At 6th level, if the psychic warrior makes an attack (but not a ranged touch attack), 
/// he can manifest one of his path powers as part of that attack action. The power takes effect immediately 
/// after the attack has been finished. Touch range powers are transmitted through the melee attack to the 
/// attacked target. You gain the benefits of the power on the attack made, even if the power is what grants 
/// the weapon to make the attack. You may only activate a path power in this fashion once per round.
/// </summary>
public static class MartialPower
{
    public static void Configure()
    {
        FeatureConfigurator.New("MartialPower", Guids.MartialPowerFeature)
            .SetDisplayName(LocalizationTool.CreateString("PW.MartialPower.Name", "Martial Power"))
            .SetDescription(LocalizationTool.CreateString(
                "PW.MartialPower.Desc",
                "At 6th level, when you make a melee attack, you can manifest one of your path powers as part of that attack action. " +
                "The power takes effect immediately after the attack. Touch range powers are transmitted through the melee attack. " +
                "You can only activate a path power this way once per round."))
            .SetIsClassFeature()
            .Configure();
    }
}
