using System;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic.ActivatableAbilities;
using PsychicWarrior.Shared.Mechanics;
using PsychicWarrior.Utils;

namespace PsychicWarrior.HarmonyPatches;

/// <summary>
/// Enhanced Mind Blade pool: the ability toggles use the game's <see cref="ActivatableAbilityResourceLogic"/>
/// against a custom pool resource, which gives the shaman-style "remaining points on the icon" display and
/// auto-darkening for free. Two gaps in the stock logic are patched here:
///
/// 1. <c>CalcResourceCost</c> spends a flat 1 per toggle (it has no weight field). We override it to charge
///    the ability's real +N cost (1/2/4) so a +2 burst reserves 2 points, etc.
/// 2. <c>IsAvailable</c> only requires ≥1 point, so a +2 toggle could be switched on with a single point left
///    (and then under-spend / not spend). We require the full cost so weighted toggles darken correctly.
///
/// Both are scoped to our toggles via <see cref="MindBladeEnchantments.AbilityToggleCosts"/>; every other
/// activatable ability is untouched. Cost is looked up by the toggle's blueprint GUID.
/// </summary>
[HarmonyPatch(typeof(ActivatableAbilityResourceLogic))]
public static class MindBladeEnchantPoolPatch
{
    private static BlueprintAbilityResource _pool;
    private static BlueprintAbilityResource Pool =>
        _pool ??= BlueprintTool.Get<BlueprintAbilityResource>(Guids.EnhancedMindBladePoolResource);

    private static bool TryGetCost(ActivatableAbilityResourceLogic instance, out int cost)
    {
        cost = 0;
        var guid = instance?.Fact?.Blueprint?.AssetGuid.ToString();
        return guid != null && MindBladeEnchantments.AbilityToggleCosts.TryGetValue(guid, out cost);
    }

    [HarmonyPatch("CalcResourceCost")]
    [HarmonyPostfix]
    public static void CalcResourceCost_Postfix(ActivatableAbilityResourceLogic __instance, ref int __result)
    {
        if (TryGetCost(__instance, out var cost))
            __result = cost;
    }

    [HarmonyPatch("IsAvailable", new Type[] { typeof(EntityFactComponent) })]
    [HarmonyPostfix]
    public static void IsAvailable_Postfix(ActivatableAbilityResourceLogic __instance, ref bool __result)
    {
        if (!__result) return;
        if (!TryGetCost(__instance, out var cost) || cost <= 1) return;

        // Already on? Its cost is already reserved — leave available so it can be switched off.
        if (__instance.Fact != null && __instance.Fact.IsOn) return;

        var owner = __instance.Owner;
        var pool = Pool;
        if (owner == null || pool == null) return;

        if (owner.Resources.GetResourceAmount(pool) < cost)
            __result = false;
    }
}
