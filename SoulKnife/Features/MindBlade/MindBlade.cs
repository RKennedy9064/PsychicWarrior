using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.ActivatableAbilities;
using PsychicWarrior.Shared.Mechanics;
using PsychicWarrior.Utils;

namespace PsychicWarrior.SoulKnife.Features.MindBlade;

public static class MindBlade
{
    private static readonly HashSet<WeaponCategory> Excluded =
    [
        WeaponCategory.UnarmedStrike,
        WeaponCategory.KineticBlast,
        WeaponCategory.Touch,
        WeaponCategory.Ray,
        WeaponCategory.Bomb,
    ];

    private static readonly LogWrapper Log = LogWrapper.Get("PsychicWarrior");

    public static void Configure()
    {
        var icon = AbilityRefs.MagicWeapon.Reference.Get().Icon;

        var categories = new List<WeaponCategory>();
        foreach (WeaponSubCategory sub in new[] { WeaponSubCategory.Simple, WeaponSubCategory.Martial, WeaponSubCategory.Exotic })
        {
            foreach (var cat in WeaponCategoryExtension.GetAllCategories(sub))
            {
                if (!Excluded.Contains(cat) && !categories.Contains(cat))
                    categories.Add(cat);
            }
        }

        var conf = FeatureSelectionConfigurator.New("SKMindBladeSelection", Guids.MindBladeFeature)
            .SetDisplayName(Loc.Str("SK.MB.Selection.Name", "Form Mind Blade"))
            .SetDescription(Loc.Str("SK.MB.Selection.Desc",
                "Choose the weapon form your mind blade will take. You will always manifest this weapon when you activate your mind blade. " +
                "The chosen weapon is treated as magical and masterwork. A soulknife is proficient with her mind blade regardless of the form it takes."))
            .SetIcon(icon)
            .SetIsClassFeature(true);

        foreach (var cat in categories)
        {
            var f = MakeWeapon(cat, icon);
            conf = conf.AddToAllFeatures(f);
        }

        conf.Configure();
    }

    private static string Guid(string key)
    {
        using var md5 = MD5.Create();
        return new System.Guid(md5.ComputeHash(Encoding.UTF8.GetBytes(key))).ToString();
    }

    private static string FormatName(WeaponCategory cat) =>
        Regex.Replace(cat.ToString(), @"(?<!^)([A-Z])", " $1");

    private static BlueprintItemWeaponReference FindWeaponRef(WeaponCategory cat)
    {
        var catName = cat.ToString();
        var flags = BindingFlags.Public | BindingFlags.Static;
        var refsType = typeof(ItemWeaponRefs);

        var allNames = new List<string>();
        foreach (var f in refsType.GetFields(flags)) allNames.Add(f.Name);
        foreach (var p in refsType.GetProperties(flags)) allNames.Add(p.Name);

        var result = TryExtractRef(refsType, $"Standard{catName}", flags);
        if (result != null) return result;

        foreach (var memberName in allNames)
        {
            if (!memberName.StartsWith("Standard", StringComparison.OrdinalIgnoreCase)) continue;
            if (!memberName.EndsWith(catName, StringComparison.OrdinalIgnoreCase)) continue;
            result = TryExtractRef(refsType, memberName, flags);
            if (result != null) return result;
        }

        result = TryExtractRef(refsType, $"{catName}Plus1", flags);
        if (result != null) return result;

        foreach (var memberName in allNames)
        {
            if (!memberName.StartsWith(catName, StringComparison.OrdinalIgnoreCase)) continue;
            if (!memberName.EndsWith("Plus1", StringComparison.Ordinal)) continue;
            result = TryExtractRef(refsType, memberName, flags);
            if (result != null) return result;
        }

        Log.Warn($"[MB] No weapon ref found for category {catName}");
        return null;
    }

    private static BlueprintItemWeaponReference TryExtractRef(Type refsType, string memberName, BindingFlags flags)
    {
        try
        {
            object value = null;
            var field = refsType.GetField(memberName, flags);
            if (field != null)
                value = field.GetValue(null);
            else
            {
                var prop = refsType.GetProperty(memberName, flags);
                if (prop != null)
                    value = prop.GetValue(null);
            }
            if (value == null) return null;
            var refFieldInfo = value.GetType().GetField("Reference", BindingFlags.Public | BindingFlags.Instance);
            if (refFieldInfo == null) return null;
            if (refFieldInfo.GetValue(value) is not BlueprintReferenceBase refBase) return null;
            var guid = refBase.deserializedGuid.ToString();
            if (string.IsNullOrEmpty(guid) || guid == "00000000-0000-0000-0000-000000000000") return null;
            return BlueprintTool.GetRef<BlueprintItemWeaponReference>(guid);
        }
        catch (Exception ex)
        {
            Log.Warn($"[MB] Exception extracting ref for {memberName}: {ex.Message}");
            return null;
        }
    }

    private static BlueprintFeature MakeWeapon(WeaponCategory cat, UnityEngine.Sprite icon)
    {
        var name = FormatName(cat);
        var catStr = cat.ToString();
        var weaponRef = FindWeaponRef(cat);

        var buff = BuffConfigurator.New($"SKMBBuff{catStr}", Guid($"SK.MB.Buff.{catStr}"))
            .SetDisplayName(Loc.Str($"SK.MB.{catStr}.BN", $"Mind Blade ({name})"))
            .SetDescription(Loc.Str($"SK.MB.{catStr}.BD",
                $"Your mind blade is manifested as a {name.ToLower()}. The weapon is bound to your primary hand and cannot be unequipped while active."))
            .SetIcon(icon)
            .AddComponent(new MindBladeComponent { Category = cat, WeaponRef = weaponRef })
            .Configure();

        var activatable = ActivatableAbilityConfigurator.New($"SKMBToggle{catStr}", Guid($"SK.MB.Activatable.{catStr}"))
            .SetDisplayName(Loc.Str($"SK.MB.{catStr}.TN", $"Form Mind Blade ({name})"))
            .SetDescription(Loc.Str($"SK.MB.{catStr}.TD",
                $"Toggle. Manifest your mind blade as a {name.ToLower()}. The blade cannot be unequipped while active."))
            .SetIcon(icon)
            .SetBuff(buff)
            .SetActivationType(AbilityActivationType.Immediately)
            .SetGroup(ActivatableAbilityGroup.None)
            .Configure();

        return FeatureConfigurator.New($"SKMBFeature{catStr}", Guid($"SK.MB.Feature.{catStr}"))
            .SetDisplayName(Loc.Str($"SK.MB.{catStr}.FN", $"Mind Blade ({name})"))
            .SetDescription(Loc.Str($"SK.MB.{catStr}.FD",
                $"You can manifest your mind blade as a {name.ToLower()}. Toggle the ability on your action bar to summon or dismiss the weapon."))
            .SetIcon(icon)
            .SetIsClassFeature()
            .AddFacts([activatable.ToString()])
            .Configure();
    }
}
