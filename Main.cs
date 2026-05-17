using HarmonyLib;
using UnityModManagerNet;
using BlueprintCore.Utils;

namespace PsychicWarrior;

public static class Main
{
    public static bool Load(UnityModManager.ModEntry modEntry)
    {
        LogWrapper logger = LogWrapper.Get("PsychicWarrior");

        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll();

        logger.Info("PsychicWarrior loaded");

        return true;
    }
}