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
        var secondary = Owner.Descriptor.Body.SecondaryHand;
        Log.Info($"[MB] OnActivate ({weapon.name}): double={weapon.Double} " +
                 $"primaryHasItem={slot.HasItem} primaryItem={(slot.HasItem ? slot.Item?.Blueprint?.name : "empty")} " +
                 $"secHasItem={secondary?.HasItem} secLocked={secondary?.Lock.Value}");

        if (slot.HasItem && slot.Item?.Blueprint == weapon)
        {
            Log.Info($"[MB] OnActivate ({weapon.name}): early-return, already equipped");
            return;
        }

        // Apply the player's chosen appearance to this form's weapon type *before* equipping,
        // so the new item picks up the right model. (The visual feature also applies on load,
        // but doing it here guarantees correct ordering when the blade is first manifested.)
        ApplyChosenVisual(weapon);

        var item = new ItemEntityWeapon(weapon) { IsIdentified = true };
        Log.Info($"[MB] pre-equip ({weapon.name}): canInsertPrimary={slot.CanInsertItem(item)} primaryLocked={slot.Lock.Value} " +
                 $"| possible={slot.IsPossibleInsertItems()} supported={slot.IsItemSupported(item)} " +
                 $"canEquip={item.CanBeEquippedBy(Owner.Descriptor)} canTakeOneHand={item.CanTakeOneHand(Owner)}");

        Owner.Inventory.Add(item);
        slot.InsertItem(item);
        slot.Lock.Retain();

        Log.Info($"[MB] post-insert ({weapon.name}): primaryHasItem={slot.HasItem} primaryItem={(slot.HasItem ? slot.Item?.Blueprint?.name : "empty")} " +
                 $"itemWielder={(item.Wielder != null)} secHasItem={secondary?.HasItem} secItem={(secondary != null && secondary.HasItem ? secondary.Item?.Blueprint?.name : "empty")}");

        var visualEnch = BlueprintTool.Get<BlueprintWeaponEnchantment>(Guids.CallWeaponryVisualEnchantment);
        if (visualEnch != null)
            item.AddEnchantment(visualEnch, null);

        var skClass = BlueprintTool.Get<BlueprintCharacterClass>(Guids.SoulKnifeClass);
        var classLevel = Owner.Descriptor.Progression.GetClassLevel(skClass);
        _appliedEnhancementLevel = ApplyEnchantment(item, 0, classLevel);

        EventBus.Subscribe(this);
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

    // Finds the visual feature the player selected for this form and applies the chosen weapon's
    // appearance (3D model on the type) and inventory icon (on the item blueprint) before the
    // item is created.
    private void ApplyChosenVisual(BlueprintItemWeapon weapon)
    {
        var weaponType = weapon.m_Type?.Get();
        if (weaponType == null)
        {
            Log.Warn($"[MB] ApplyChosenVisual: weapon {weapon.name} has no resolvable type");
            return;
        }

        int scanned = 0;
        foreach (var feature in Owner.Progression.Features.Enumerable)
        {
            var vc = feature.Blueprint.GetComponent<MindBladeVisualComponent>();
            if (vc == null) continue;
            scanned++;

            // Compare resolved blueprints by identity — string GUID formats differ (dashes vs none).
            var vcTargetType = BlueprintTool.Get<BlueprintWeaponType>(vc.TargetWeaponTypeGuid);
            if (vcTargetType != weaponType) continue;

            var sourceWeapon = vc.SourceWeaponRef?.Get();
            vc.Apply(); // copies the chosen weapon's type onto the form type
            if (sourceWeapon != null)
            {
                // Item-level fields drive the equipped 3D model, name, and inventory icon.
                weapon.m_VisualParameters = sourceWeapon.m_VisualParameters;
                weapon.m_DisplayNameText  = sourceWeapon.m_DisplayNameText;
                if (sourceWeapon.Icon != null) weapon.m_Icon = sourceWeapon.Icon;
            }
            var reach = weaponType.m_AttackRange;
            Log.Info($"[MB] ApplyChosenVisual: matched '{feature.Blueprint.name}', source={sourceWeapon?.name ?? "null"}, " +
                     $"attackRange={reach} name={sourceWeapon?.m_DisplayNameText}");
            return;
        }

        Log.Warn($"[MB] ApplyChosenVisual: no MindBladeVisualComponent matched type {weaponType.name} " +
                 $"(scanned {scanned} visual features on {Owner.CharacterName})");
    }

    public void HandleUnitBeforeLevelUp(UnitEntityData unit) { }

    public void HandleUnitAfterLevelUp(UnitEntityData unit, LevelUpController controller)
    {
        // During char-gen/level-up preview this fires on component instances whose runtime is
        // not attached; accessing Owner then throws "ComponentRuntime is unavailable". Guard it.
        UnitEntityData owner;
        try { owner = Owner; }
        catch { return; }

        if (unit != owner) return;

        var slot = owner.Descriptor.Body.PrimaryHand;
        if (!slot.HasItem) return;

        var weapon = FindWeapon();
        if (weapon == null || slot.Item?.Blueprint != weapon) return;
        if (slot.Item is not ItemEntityWeapon item) return;

        var skClass = BlueprintTool.Get<BlueprintCharacterClass>(Guids.SoulKnifeClass);
        var classLevel = owner.Descriptor.Progression.GetClassLevel(skClass);
        _appliedEnhancementLevel = ApplyEnchantment(item, _appliedEnhancementLevel, classLevel);
    }

    private static BlueprintFeature _improvedEnhancement;

    private int ApplyEnchantment(ItemEntityWeapon item, int currentLevel, int classLevel)
    {
        var newLevel = GetEnhancementLevel(classLevel);

        // Blade skill: Improved Enhancement — +1 enhancement bonus (capped at the +5 we can apply).
        _improvedEnhancement ??= BlueprintTool.Get<BlueprintFeature>(Guids.BladeSkillImprovedEnhancement);
        if (newLevel > 0 && _improvedEnhancement != null && Owner.Descriptor.HasFact(_improvedEnhancement))
            newLevel = System.Math.Min(5, newLevel + 1);

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
