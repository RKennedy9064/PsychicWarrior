using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Features;

public static class PsychicWarriorProficiencies
{
    public static void Configure()
    {
        FeatureConfigurator.New("PsychicWarriorProficiencies", Guids.Proficiencies)
            .SetDisplayName(LocalizationTool.CreateString("PW.Proficiencies.Name", "Psychic Warrior Proficiencies"))
            .SetDescription(LocalizationTool.CreateString("PW.Proficiencies.Desc", "Psychic Warriors are proficient with all simple and martial weapons, and with all armor (heavy, light, and medium) and shields (except tower shields)."))
            .SetIsClassFeature()
            .AddFacts(
            [
                FeatureRefs.SimpleWeaponProficiency.ToString(),
                FeatureRefs.MartialWeaponProficiency.ToString(),
                FeatureRefs.LightArmorProficiency.ToString(),
                FeatureRefs.MediumArmorProficiency.ToString(),
                FeatureRefs.HeavyArmorProficiency.ToString(),
                FeatureRefs.ShieldsProficiency.ToString()
            ])
            .Configure();
    }
}