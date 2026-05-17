using System.Linq;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;

namespace PsychicWarrior;

[HarmonyPatch(typeof(BlueprintsCache), "Init")]
public static class BlueprintInit
{
    private static bool Initialized;

    [HarmonyPostfix]
    public static void Postfix()
    {
        if (Initialized) return;
        Initialized = true;

        var logger = LogWrapper.Get("PsychicWarrior");

        try
        {
            // Phase 1: Core Systems
            Mechanics.Focus.Configure();
            Features.PsionicProficiency.Configure();
            Features.TalentsSelection.Configure();
            Features.PathSkillBonus.Configure();
            Features.MartialPower.Configure();

            // Powers
            Powers.MinorPrecognition.Configure();
            Powers.Vigor.Configure();
            Powers.ForceScreen.Configure();

            // Features
            Features.PsychicWarriorProficiencies.Configure();
            Features.PsychicWarriorBonusFeat.Configure();
            Features.Paths.PsychicWarriorPathSelection.Configure();

            // Feats
            Feats.PsionicMeditation.Configure();
            Feats.PsionicWeapon.Configure();

            // Class Definition
            Classes.PsychicWarriorSpellbook.Configure();
            Classes.PsychicWarriorClass.Configure();

            var root = BlueprintRoot.Instance;
            var pwClassRef = BlueprintTool.GetRef<BlueprintCharacterClassReference>(Utils.Guids.PsychicWarriorClass);

            if (!root.Progression.m_CharacterClasses.Contains(pwClassRef))
            {
                root.Progression.m_CharacterClasses = [.. root.Progression.m_CharacterClasses, pwClassRef];
            }
        }
        catch (System.Exception e)
        {
            logger.Error($"Initialization failed: {e}");
        }
    }
}