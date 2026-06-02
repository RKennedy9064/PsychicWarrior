using System;
using System.Security.Cryptography;
using System.Text;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using PsychicWarrior.Shared.Mechanics;
using PsychicWarrior.Utils;

namespace PsychicWarrior.SoulKnife.Features;

/// <summary>
/// Enhanced Mind Blade: a level-scaled enhancement pool the soulknife allocates between a direct
/// enhancement bonus (an "eat-all" toggle, capped at the level's max direct bonus) and weapon special
/// abilities (one toggle each, cost = the ability's +N equivalent). Modeled on the Spirit Hunter's
/// Spirit Enhancement; budget + application handled by <see cref="MindBladeEnchantments"/>.
/// </summary>
public static class EnhancedMindBlade
{
    public static void Configure()
    {
        MindBladeEnchantments.ManagedEnchantments.Clear();

        // Enhancement enchantments are always managed (added/removed by Recompute).
        foreach (var e in new[]
        {
            WeaponEnchantmentRefs.Enhancement1, WeaponEnchantmentRefs.Enhancement2, WeaponEnchantmentRefs.Enhancement3,
            WeaponEnchantmentRefs.Enhancement4, WeaponEnchantmentRefs.Enhancement5,
        })
            MindBladeEnchantments.ManagedEnchantments.Add(
                BlueprintTool.GetRef<BlueprintWeaponEnchantmentReference>(e.Reference.Guid.ToString()));

        var enhIcon = AbilityRefs.MagicWeaponGreater.Reference.Get().Icon;

        // The eat-all enhancement toggle.
        var enhBuff = BuffConfigurator.New("SKEnhMindBladeEnhanceBuff", Guids.EnhancedMindBladeEnhanceBuff)
            .SetDisplayName(Loc.Str("SK.EMB.Enhance.Name", "Enhance Mind Blade"))
            .SetDescription(Loc.Str("SK.EMB.Enhance.Desc",
                "Spend the remainder of your enhancement pool (up to your maximum direct bonus) on a numeric enhancement bonus."))
            .SetIcon(enhIcon)
            .AddComponent(new MindBladeEnchantComponent { IsEnhancement = true })
            .Configure();

        ActivatableAbilityConfigurator.New("SKEnhMindBladeEnhanceToggle", Guids.EnhancedMindBladeEnhanceToggle)
            .SetDisplayName(Loc.Str("SK.EMB.Enhance.Name", "Enhance Mind Blade"))
            .SetDescription(Loc.Str("SK.EMB.Enhance.Desc",
                "Spend the remainder of your enhancement pool (up to your maximum direct bonus) on a numeric enhancement bonus."))
            .SetIcon(enhIcon)
            .SetBuff(enhBuff)
            .SetActivationType(AbilityActivationType.Immediately)
            .SetGroup(ActivatableAbilityGroup.None)
            .SetIsOnByDefault(true)
            .Configure();

        // Ability table (cost = +N equivalent). Only standard WotR enchantments with clean generic refs.
        var abilities = new (string Key, string Name, int Cost, string Ench, UnityEngine.Sprite Icon)[]
        {
            ("Flaming",        "Flaming",         1, WeaponEnchantmentRefs.Flaming.Reference.Guid.ToString(),        AbilityRefs.BurningHands.Reference.Get().Icon),
            ("Frost",          "Frost",           1, WeaponEnchantmentRefs.Frost.Reference.Guid.ToString(),          AbilityRefs.RayOfFrost.Reference.Get().Icon),
            ("Shock",          "Shock",           1, WeaponEnchantmentRefs.Shock.Reference.Guid.ToString(),          AbilityRefs.CallLightning.Reference.Get().Icon),
            ("Corrosive",      "Corrosive",       1, WeaponEnchantmentRefs.Corrosive.Reference.Guid.ToString(),      AbilityRefs.AcidArrow.Reference.Get().Icon),
            ("Keen",           "Keen",            1, WeaponEnchantmentRefs.Keen.Reference.Guid.ToString(),           AbilityRefs.KeenEdge.Reference.Get().Icon),
            ("GhostTouch",     "Ghost Touch",     1, WeaponEnchantmentRefs.GhostTouch.Reference.Guid.ToString(),     AbilityRefs.Blur.Reference.Get().Icon),
            ("Vicious",        "Vicious",         1, WeaponEnchantmentRefs.ViciousEnchantment.Reference.Guid.ToString(), AbilityRefs.Enervation.Reference.Get().Icon),
            ("Agile",          "Agile",           1, WeaponEnchantmentRefs.Agile.Reference.Guid.ToString(),          FeatureRefs.WeaponFinesse.Reference.Get().Icon),
            ("FlamingBurst",   "Flaming Burst",   2, WeaponEnchantmentRefs.FlamingBurst.Reference.Guid.ToString(),   AbilityRefs.BurningHands.Reference.Get().Icon),
            ("IcyBurst",       "Icy Burst",       2, WeaponEnchantmentRefs.IcyBurst.Reference.Guid.ToString(),       AbilityRefs.RayOfFrost.Reference.Get().Icon),
            ("ShockingBurst",  "Shocking Burst",  2, WeaponEnchantmentRefs.ShockingBurst.Reference.Guid.ToString(),  AbilityRefs.CallLightning.Reference.Get().Icon),
            ("CorrosiveBurst", "Corrosive Burst", 2, WeaponEnchantmentRefs.CorrosiveBurst.Reference.Guid.ToString(), AbilityRefs.AcidArrow.Reference.Get().Icon),
            ("Holy",           "Holy",            2, WeaponEnchantmentRefs.Holy.Reference.Guid.ToString(),           AbilityRefs.BlessWeapon.Reference.Get().Icon),
            ("Unholy",         "Unholy",          2, WeaponEnchantmentRefs.Unholy.Reference.Guid.ToString(),         AbilityRefs.Enervation.Reference.Get().Icon),
            ("Anarchic",       "Anarchic",        2, WeaponEnchantmentRefs.Anarchic.Reference.Guid.ToString(),       AbilityRefs.AlignWeapon.Reference.Get().Icon),
            ("Axiomatic",      "Axiomatic",       2, WeaponEnchantmentRefs.Axiomatic.Reference.Guid.ToString(),      AbilityRefs.AlignWeapon.Reference.Get().Icon),
            ("BrilliantEnergy","Brilliant Energy",4, WeaponEnchantmentRefs.BrilliantEnergy.Reference.Guid.ToString(), AbilityRefs.MagicWeapon.Reference.Get().Icon),
        };

        var toggleGuids = new System.Collections.Generic.List<string>();
        foreach (var a in abilities)
        {
            var enchRef = BlueprintTool.GetRef<BlueprintWeaponEnchantmentReference>(a.Ench);
            MindBladeEnchantments.ManagedEnchantments.Add(enchRef);

            var buff = BuffConfigurator.New($"SKEMBBuff{a.Key}", Det($"SK.EMB.Buff.{a.Key}"))
                .SetDisplayName(Loc.Str($"SK.EMB.{a.Key}.Name", a.Name))
                .SetDescription(Loc.Str($"SK.EMB.{a.Key}.Desc",
                    $"Your mind blade gains the {a.Name} weapon special ability (consumes {a.Cost} point{(a.Cost > 1 ? "s" : "")} from your enhancement pool)."))
                .SetIcon(a.Icon)
                .AddComponent(new MindBladeEnchantComponent { Cost = a.Cost, Enchantment = enchRef })
                .Configure();

            var toggleGuid = Det($"SK.EMB.Toggle.{a.Key}");
            ActivatableAbilityConfigurator.New($"SKEMBToggle{a.Key}", toggleGuid)
                .SetDisplayName(Loc.Str($"SK.EMB.{a.Key}.Name", a.Name))
                .SetDescription(Loc.Str($"SK.EMB.{a.Key}.Desc",
                    $"Toggle. Your mind blade gains the {a.Name} weapon special ability (consumes {a.Cost} point{(a.Cost > 1 ? "s" : "")} from your enhancement pool)."))
                .SetIcon(a.Icon)
                .SetBuff(buff)
                .SetActivationType(AbilityActivationType.Immediately)
                .SetGroup(ActivatableAbilityGroup.None)
                .Configure();
            toggleGuids.Add(toggleGuid);
        }

        var facts = new System.Collections.Generic.List<string> { Guids.EnhancedMindBladeEnhanceToggle };
        facts.AddRange(toggleGuids);

        FeatureConfigurator.New("SKEnhancedMindBlade", Guids.EnhancedMindBladeFeature)
            .SetDisplayName(Loc.Str("SK.EMB.Name", "Enhanced Mind Blade"))
            .SetDescription(Loc.Str("SK.EMB.Desc",
                "At 3rd level, a soulknife's mind blade gains an enhancement pool that grows as she levels. " +
                "She allocates it between a direct enhancement bonus (the Enhance Mind Blade toggle, up to her " +
                "maximum direct bonus) and weapon special abilities (one toggle each). Toggle special abilities " +
                "first, then enable the enhancement toggle to spend the remaining points."))
            .SetIcon(enhIcon)
            .SetIsClassFeature()
            .AddFacts([.. facts])
            .Configure();
    }

    private static string Det(string key)
    {
        using var md5 = MD5.Create();
        return new Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(key))).ToString();
    }
}
