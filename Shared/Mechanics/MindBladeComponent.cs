using System;
using System.Collections.Generic;
using System.Linq;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using PsychicWarrior.Utils;

namespace PsychicWarrior.Shared.Mechanics;

[Serializable]
public class MindBladeComponent : UnitFactComponentDelegate, IUnitLevelUpHandler
{
    public WeaponCategory Category;
    public BlueprintItemWeaponReference WeaponRef;

    [NonSerialized]
    private int _appliedEnhancementLevel;

    private static readonly Dictionary<WeaponCategory, BlueprintItemWeapon> s_Cache = [];
    private static readonly LogWrapper Log = LogWrapper.Get("PsychicWarrior");

    private BlueprintItemWeapon FindWeapon()
    {
        if (s_Cache.TryGetValue(Category, out var cached))
            return cached;

        var weapon = WeaponRef?.Get();
        if (weapon != null)
            s_Cache[Category] = weapon;
        else
            Log.Warn($"[MB] FindWeapon({Category}): WeaponRef is null or Get() returned null");
        return weapon;
    }

    public override void OnActivate()
    {
        foreach (var buff in Owner.Descriptor.Buffs.Enumerable.ToList())
        {
            if (buff != Fact && buff.Blueprint.ComponentsArray.OfType<MindBladeComponent>().Any())
            {
                buff.Remove();
                break;
            }
        }

        var weapon = FindWeapon();
        if (weapon == null) return;

        var slot = Owner.Descriptor.Body.PrimaryHand;
        if (slot.HasItem && slot.Item?.Blueprint == weapon) return;

        var item = new ItemEntityWeapon(weapon) { IsIdentified = true };
        Owner.Inventory.Add(item);
        slot.InsertItem(item);
        slot.Lock.Retain();

        var visualEnch = BlueprintTool.Get<BlueprintWeaponEnchantment>(Guids.CallWeaponryVisualEnchantment);
        if (visualEnch != null)
            item.AddEnchantment(visualEnch, null);

        var skClass = BlueprintTool.Get<BlueprintCharacterClass>(Guids.SoulKnifeClass);
        var classLevel = Owner.Descriptor.Progression.GetClassLevel(skClass);
        _appliedEnhancementLevel = ApplyEnchantment(item, 0, classLevel);

        EventBus.Subscribe(this);
        Log.Info($"[MB] Mind blade equipped ({weapon.name}) for {Owner.CharacterName}");
    }

    public override void OnDeactivate()
    {
        EventBus.Unsubscribe(this);

        var weapon = FindWeapon();
        if (weapon == null) return;

        var slot = Owner.Descriptor.Body.PrimaryHand;
        if (slot.HasItem && slot.Item?.Blueprint == weapon)
        {
            slot.Lock.Release();
            var item = slot.Item;
            slot.RemoveItem();
            item?.Collection?.Remove(item);
            Log.Info($"[MB] Mind blade dismissed for {Owner.CharacterName}");
        }

        _appliedEnhancementLevel = 0;
    }

    public void HandleUnitBeforeLevelUp(UnitEntityData unit) { }

    public void HandleUnitAfterLevelUp(UnitEntityData unit, LevelUpController controller)
    {
        if (unit != Owner) return;

        var slot = Owner.Descriptor.Body.PrimaryHand;
        if (!slot.HasItem) return;

        var weapon = FindWeapon();
        if (weapon == null || slot.Item?.Blueprint != weapon) return;
        if (slot.Item is not ItemEntityWeapon item) return;

        var skClass = BlueprintTool.Get<BlueprintCharacterClass>(Guids.SoulKnifeClass);
        var classLevel = Owner.Descriptor.Progression.GetClassLevel(skClass);
        _appliedEnhancementLevel = ApplyEnchantment(item, _appliedEnhancementLevel, classLevel);
    }

    private int ApplyEnchantment(ItemEntityWeapon item, int currentLevel, int classLevel)
    {
        var newLevel = GetEnhancementLevel(classLevel);

        if (currentLevel > 0)
        {
            var oldEnch = GetEnhancementEnchantment(currentLevel);
            if (oldEnch != null)
            {
                var existing = item.GetEnchantment(oldEnch);
                if (existing != null) item.RemoveEnchantment(existing);
            }
        }

        if (newLevel > 0)
        {
            var ench = GetEnhancementEnchantment(newLevel);
            if (ench != null) item.AddEnchantment(ench, null);
        }

        return newLevel;
    }

    // Enhanced Mind Blade direct enhancement bonus (max direct bonus column from SK table)
    private static int GetEnhancementLevel(int classLevel)
    {
        if (classLevel >= 15) return 5;
        if (classLevel >= 13) return 4;
        if (classLevel >= 9)  return 3;
        if (classLevel >= 7)  return 2;
        if (classLevel >= 3)  return 1;
        return 0;
    }

    private static BlueprintWeaponEnchantment GetEnhancementEnchantment(int level) => level switch
    {
        1 => WeaponEnchantmentRefs.Enhancement1.Reference.Get(),
        2 => WeaponEnchantmentRefs.Enhancement2.Reference.Get(),
        3 => WeaponEnchantmentRefs.Enhancement3.Reference.Get(),
        4 => WeaponEnchantmentRefs.Enhancement4.Reference.Get(),
        5 => WeaponEnchantmentRefs.Enhancement5.Reference.Get(),
        _ => null
    };
}
