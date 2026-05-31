# Soulknife Implementation Blueprint

## 1. Project Restructuring

### Current Layout
```
PsychicWarrior/
  BlueprintInit.cs
  Classes/           ← PW class & spellbook only
  Features/          ← all PW features + Paths/
  Feats/             ← all psionic feats
  Mechanics/         ← all rulebook components
  Powers/            ← all PW powers
  HarmonyPatches/
  Utils/
```

### Proposed Layout
```
PsychicWarrior/
  BlueprintInit.cs   ← split into PW + SK init sections
  Classes/
    PsychicWarrior/  ← move existing Classes/* here
      Class.cs
      Spellbook.cs
      Features/      ← move Features/* here (excluding shared feats)
        Paths/
        PrebuildFeatureList.cs
        ...
      Powers/        ← move Powers/* here
    SoulKnife/
      Class.cs
      Features/
        MindBlade/
          MindBlade.cs
          EnhancedMindBlade.cs
          PsychicStrike.cs
          ShapeThrowMindBlade.cs
        BladeSkills/
          BladeSkillsSelection.cs
          (individual blade skills)
  Shared/
    Feats/           ← move Feats/* here (all psionic feats are usable by both)
    Mechanics/       ← move Mechanics/* here
  HarmonyPatches/
  Utils/
```

### Migration Steps
1. Create `Classes/PsychicWarrior/` and `Classes/SoulKnife/` folders
2. Move `Classes/PsychicWarrior.cs` → `Classes/PsychicWarrior/Class.cs`
3. Move `Classes/PsychicWarriorSpellbook.cs` → `Classes/PsychicWarrior/Spellbook.cs`
4. Move `Features/` and all subfolders → `Classes/PsychicWarrior/Features/`
5. Move `Powers/` → `Classes/PsychicWarrior/Powers/`
6. Create `Shared/Feats/` and `Shared/Mechanics/`; move `Feats/*` and `Mechanics/*` into them
7. Update all `namespace` declarations (no functional changes, pure rename)
8. Update `BlueprintInit.cs` registration order (no GUID changes)

**Risk**: Low. Namespaces in this project are folder-based but nothing external references them. No GUID changes, no blueprint changes. Pure file moves + namespace string updates.

---

## 2. What Can Be Shared

### Reused As-Is (zero changes needed)
| File | Notes |
|------|-------|
| `Mechanics/Focus.cs` | Psionic Focus is identical for SK |
| `Mechanics/WeaponInheritedDamage.cs` | Utility, class-agnostic |
| `HarmonyPatches/PsionicProficiencyPatch.cs` | Extend to register SK as a psionic class |
| `Utils/Guids.cs` | Add SK GUIDs in a new section |
| `Utils/Loc.cs` | Shared |
| All psionic feats | SK can take the same feats PW can (Psionic Weapon, Psionic Fist, etc.) |

### Needs Extension
| File | Change |
|------|--------|
| `Features/PsychicWarriorBonusFeat.cs` | Rename to `Shared/Feats/PsionicBonusFeatSelection.cs`; PW and SK each get their own bonus feat selection that populates from the shared psionic feat list |
| `HarmonyPatches/PsionicProficiencyPatch.cs` | Add SK class GUID to the registration list |
| `BlueprintInit.cs` | Add SK initialization phase after PW |

### NOT Shared (PW-specific, stays in PsychicWarrior/)
- Paths, Path selection, TranceHelper, path maneuvers
- PW spellbook and power slots
- PW progression (pp scaling, manifester level)
- Advanced Path feats
- PrebuildPsychicWarriorFeatureList / EternalWarrior capstone

---

## 3. Soulknife Class Stats

- **HD**: d10
- **BAB**: Full (+1/level)
- **Saves**: Fort slow (1/3), Ref fast (1/2 + 2), Will fast (1/2 + 2)
- **Class Skills**: Acrobatics, Athletics, Autolore, Perception, Persuasion, Stealth, Trickery, Use Magic Device
- **Skills/level**: 4 + Int
- **Proficiencies**: All simple and martial weapons, light armor, medium armor, shields (not tower)

---

## 4. Core Features — Implementation Plan

### 4.1 Form Mind Blade ⭐⭐⭐ (High complexity)
**What it does**: As a move action, manifest a psychic weapon in one of three forms:
- Light (1d6, 19-20/×2, throwable 20 ft)
- One-handed (1d8, 19-20/×2, throwable 15 ft)
- Two-handed (2d6, 19-20/×2, not throwable)

Reshaping between forms: full-round action.

**WotR approach**: Re-use the Call Weaponry infrastructure from Mind Knight. Key differences vs Call Weaponry:
- No per-weapon blueprint selection — one fixed weapon type per form
- Three separate toggle abilities (light/one-handed/two-handed) pointing to three fixed mind blade weapon blueprints
- Weapon blueprint created at mod load time (like Call Weaponry blueprints), one per form
- Enhancement is applied programmatically as the character levels (same pattern as CW enhancement patches)

**New files needed**:
- `Classes/SoulKnife/Features/MindBlade/MindBlade.cs` — creates the three weapon blueprints and the three toggle abilities
- `Mechanics/MindBladeComponent.cs` — handles slot-lock, enhancement scaling on level, similar to `CallWeaponryComponent.cs`

**What to reuse**: The slot-lock, visual enchantment, and toggle-on/off patterns from `Powers/CallWeaponry.cs` and `Mechanics/CallWeaponryComponent.cs` can be extracted into a shared base or simply copied and adapted.

---

### 4.2 Psychic Strike ⭐⭐ (Medium complexity)
**What it does**: Move action to charge the mind blade with +Xd8 damage on the next hit. Charge persists until used (not lost on miss). Can be recharged as a swift action by expending psionic focus.

| SK Level | Dice |
|----------|------|
| 3–6      | +1d8 |
| 7–10     | +2d8 |
| 11–14    | +3d8 |
| 15–18    | +4d8 |
| 19–20    | +5d8 |

Mindless creatures are immune.

**WotR approach**:
- A buff (`PsychicStrikeChargeBuff`) applied by the move-action ability. The buff persists indefinitely and is NOT removed on miss (unlike most maneuver buffs that use `AddInitiatorAttackRollTrigger`).
- A `RuleAttackWithWeapon` handler checks `IsHit`, checks the charge buff, deals extra d8 via `RuleDealDamage`, then removes the buff.
- Check `evt.Target` for mindlessness: `target.Descriptor.Ensure<UnitPartBrain>()?.IsIntelligent == false` or check for the undead/construct immunity — look for a reliable WotR API.
- Swift recharge: second ability that removes psionic focus buff and re-applies the charge buff.

**New files needed**:
- `Classes/SoulKnife/Features/MindBlade/PsychicStrike.cs` — configures the move-action ability, swift recharge ability, and charge buff
- `Mechanics/PsychicStrikeDamage.cs` — `RuleAttackWithWeapon` handler (same pattern as `PsionicCriticalDamage.cs`)

---

### 4.3 Enhanced Mind Blade ⭐⭐⭐ (High complexity)
**What it does**: Cumulative enhancement bonuses allocated freely between enhancement bonus and special weapon abilities. Reassigning takes 8 hours (or a full-round action at 20th).

| SK Level | Total Pool | Max Direct Bonus |
|----------|-----------|-----------------|
| 3–4      | +1        | +1              |
| 5–6      | +2        | +1              |
| 7–8      | +3        | +2              |
| 9–10     | +4        | +3              |
| 11–12    | +5        | +3              |
| 13–14    | +6        | +4              |
| 15–16    | +7        | +5              |
| 17–18    | +8        | +5              |
| 19–20    | +9        | +5              |

**WotR approach** (two sub-problems):

**A — Enhancement bonus**: Easy. The Mind Blade weapon blueprint's enchantment value is patched on level-up exactly like Call Weaponry. A `RuleCalculateWeaponStats` handler adds additional enhancement to the direct bonus portion.

**B — Special ability assignment**: Hard. In tabletop, you freely redistribute points between enhancement and special abilities (Flaming, Keen, etc.) with 8 hours rest. Options:
1. **Simple approach** (recommended): At each level bracket where the pool increases, grant a `FeatureSelection` that lets the player pick a weapon enchantment to permanently add. No re-assignment — locked in. This maps cleanly to WotR's level-up system and is what players will find natural.
2. **Full approach**: A UI allowing redistribution of points. Not feasible in WotR without custom UI code.

The simple approach means implementing `EnhancedMindBladeSelection` — a `BlueprintFeatureSelection` containing weapon enchantment features (Flaming, Frost, Shock, Keen, Defending, etc.) granted at levels 5, 9, 13, 17, 19 (when the pool crosses a +1 boundary for special abilities). The direct enhancement bonus auto-scales via component.

**New files needed**:
- `Classes/SoulKnife/Features/MindBlade/EnhancedMindBlade.cs` — handles level-up selection grants and auto-scaling component
- `Classes/SoulKnife/Features/MindBlade/EnhancedMindBladeSelection.cs` — configures the weapon enchantment feature selection with available special abilities

---

### 4.4 Blade Skills ⭐⭐ (Medium complexity per skill, high volume)
**What it does**: A talent selection (similar to PW's Bonus Feat) gained at every even level (2nd, 4th, 6th ... 20th = 10 picks). Large list (~80 options).

**WotR approach**: Identical pattern to `PsychicWarriorBonusFeat.cs`. Create `BladeSkillsSelection` (`BlueprintFeatureSelection`) and add individual skill blueprints to it. Level requirements enforced via `AddPrerequisiteClassLevel`.

**Phased implementation** — priority tiers:

**Tier 1 — Easy, high value (implement first)**
| Skill | Mechanic |
|-------|----------|
| Mind Blade Finesse | Add Weapon Finesse to mind blades (AddWeaponFocus-type component) |
| Focused Offense | Add Wis mod to attack/damage while focused (stat bonus component) |
| Focused Defense | Add Wis mod as dodge to AC while focused (stat bonus component) |
| Psionic Training | Pick a psionic feat from the shared feat list |
| Power Reserve | Grants 2 PP/level to power point pool |
| Mental Power | Grants extra power known + 2 PP |
| Alter Blade | Allows changing shape as move action (already handled by toggle system) |
| Enhanced Range | Doubles throw range increment |
| Two-Handed Throw | Enables throwing two-handed form |
| Weapon Special (Trip/Disarm/Brace) | Adds weapon property to mind blade blueprint |
| Evasion | `AddEvasion` component, requires Covert Training |
| Combat Slide | 5-ft step on hit or enemy miss (trigger-based component) |
| Vampiric Blade | Redirect psychic strike damage to HP healing |
| Powerful Strikes | Extra 1d8 on psychic strike |
| Dual Imbue | Charge both mind blades simultaneously (slightly less damage) |

**Tier 2 — Medium (implement second)**
| Skill | Mechanic |
|-------|----------|
| Ghost Step | Blink/teleport (short range) by expending focus |
| Blade Rush | Swift action move without AoO |
| Psychokinetic Blast | Splash AoE on psychic strike discharge |
| Psychic Net | Entangling ranged throw |
| Stunning Blade | Stun on psychic strike + focus expend (Fortitude save) |
| Telekinetic Edge | Ignore DR/hardness on psychic strike |
| Reaper's Blade | Recharge psychic strike on kill |
| Exploding Critical | Add psychic strike damage on crit by expending focus |
| Knife to the Soul | Deal ability score damage instead of HP with psychic strike |
| Deadly Blow | Increase crit multiplier by 1 |
| Blade Rush Frenzy | Full-round charge-and-multiattack |
| Wing Clip | Immobilize target on hit (Fort save) |

**Tier 3 — Complex or low value (implement later / skip)**
| Skill | Notes |
|-------|-------|
| Bladewind / Bladestorm | Full-attack vs all adjacent / ranged — AoE attack actions, hard in WotR |
| Fire/Ice/Lightning/Thunder Blade | Energy damage type; toggleable — feasible but many variants |
| Mind Shield / Shield Block | Shield as a mind-manifested item, separate from Mind Blade |
| Covert Training / Trapfinder | Rogue-adjacent features |
| Ghost Step → Cleave Space | Teleport larger distances |
| Dispelling Strike | Targeted dispel on hit |

**New files needed**:
- `Classes/SoulKnife/Features/BladeSkills/BladeSkillsSelection.cs`
- One `.cs` file per implemented blade skill (or group closely related skills)

---

### 4.5 Remaining Core Features (Low–Medium complexity)

| Feature | Level | Complexity | Notes |
|---------|-------|-----------|-------|
| Wild Talent | 1st | ⭐ | Bonus feat (same feat as PW wild talent) — `AddFacts([WildTalent])` |
| Bonus Feat | 1st | ⭐ | `FeatureSelection` with Power Attack, TWF, Weapon Focus (mind blade) |
| Throw Mind Blade | 1st | ⭐ | Range increment set on weapon blueprint; two-handed throw blocked without skill |
| Shape Mind Blade | 1st | ⭐ | Three toggle abilities switching which blueprint is equipped |
| Quick Draw | 5th | ⭐ | `AddFacts([QuickDraw])` — already in game |
| Mind Blade Mastery | 20th | ⭐⭐ | Null psionics immunity is irrelevant in WotR; implement the special-ability reconfiguration if the full assignment system is built, else skip |

---

## 5. GUIDs Needed (new section in Guids.cs)

```
// ── SoulKnife class ─────────────────────────────────────────────────
SoulKnifeClass
SoulKnifeProgression
SoulKnifeProficiencies

// ── Mind Blade ────────────────────────────────────────────────────────
MindBladeLight          (weapon blueprint)
MindBladeOneHanded      (weapon blueprint)
MindBladeTwoHanded      (weapon blueprint)
MindBladeLightToggle    (ability)
MindBladeOneHandedToggle
MindBladeTwoHandedToggle
MindBladeLightBuff
MindBladeOneHandedBuff
MindBladeTwoHandedBuff
MindBladeEnchantment    (visual/enhancement enchantment)

// ── Psychic Strike ───────────────────────────────────────────────────
PsychicStrikeFeature
PsychicStrikeChargeBuff
PsychicStrikeChargeAbility      (move action)
PsychicStrikeRechargeAbility    (swift, expend focus)

// ── Enhanced Mind Blade ──────────────────────────────────────────────
EnhancedMindBladeFeature
EnhancedMindBladeSelection      (FeatureSelection for weapon specials)

// ── Blade Skills ─────────────────────────────────────────────────────
BladeSkillsSelection
(one GUID per implemented skill)

// ── Class features ────────────────────────────────────────────────────
SoulKnifeBonusFeat              (FeatureSelection: PA / TWF / WF)
SoulKnifeWildTalent             (or reuse PW's)
SoulKnifeQuickDraw              (or reuse game's)
MindBladeMastery
```

---

## 6. Implementation Phases

### Phase 1 — Skeleton (playable but incomplete)
1. Project restructure (namespace renames only)
2. `SoulKnifeClass.cs` with all saves/BAB/HD/proficiencies
3. Mind Blade core (light form only, no enhancement, no throw initially)
4. Psychic Strike (charge/discharge, no immunity check yet)
5. `BladeSkillsSelection` stub with 3–5 Tier 1 skills
6. Bonus Feat + Wild Talent

**Deliverable**: A functional skeleton class that can reach level 20 in the level-up screen.

### Phase 2 — Core features complete
1. All three Mind Blade forms + reshape toggles
2. Enhanced Mind Blade auto-scaling bonus
3. Enhanced Mind Blade special ability selection
4. Throw Mind Blade (all forms)
5. Quick Draw at 5th
6. All Tier 1 blade skills
7. Psychic strike mindless-creature immunity

### Phase 3 — Polish
1. Tier 2 blade skills
2. Mind Blade Mastery capstone
3. Prebuild (sample character)
4. Icon pass (all SK abilities use appropriate WotR icons)
5. `PopulateClassSpecificFeats` equivalent for SK blade skills

---

## 7. Open Questions / Decisions Needed

1. **Enhancement assignment UI**: Simple locked-in selection at level-up (recommended) vs full point-buy redistribution system? The latter requires significant custom UI work.

2. **Dual mind blade reduction**: Tabletop reduces enhancement pool by 1 when dual-wielding. Implement automatically (detect dual-wield in component), or skip the penalty?

3. **Psychic strike immunity**: Mindless creatures immune. Identify the WotR API for this before building the component.

4. **Shared psionic feats access**: SK should be able to take all the same psionic feats as PW. This requires either (a) making the shared feat selection available to SK builds, or (b) ensuring SK is recognized as "psionic" by `PsionicProficiencyPatch`.

5. **Power points**: Tabletop SK has no power point pool by default, but `Power Reserve` and `Mental Power` blade skills grant them. The PW power point system may need to be generalized for SK to optionally have a PP pool.

6. **Module naming**: The project is currently named `PsychicWarrior`. With SK added, consider renaming to `PsionicsUnleashed` or `PUMod` to reflect multi-class scope — affects Info.json, DLL name, and all UMM references.
