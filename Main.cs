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
        try
        {
            harmony.PatchAll();
            logger.Info("PsychicWarrior: PatchAll succeeded");
        }
        catch (System.Exception e)
        {
            logger.Error($"PsychicWarrior: PatchAll threw: {e}");
            UnityEngine.Debug.LogError($"[PsychicWarrior] PatchAll threw: {e}");
        }

        return true;
    }
}