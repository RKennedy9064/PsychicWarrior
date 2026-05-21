# PsychicWarrior — WotR Mod

A Pathfinder: Wrath of the Righteous mod that adds the **Psychic Warrior** class, based on the Pathfinder 1e *Psionic Unleashed* rules. The class blends martial combat with psionic powers, using a focus-based resource system instead of spell slots.

---

## Overview

The Psychic Warrior is a full BAB martial class that manifests psychic powers through a unique resource called **Psionic Focus**. Powers are organized into a spellbook (0–6th level), and the class gains **Psychic Warrior Paths** that grant bonus feats and thematic power access at 1st, 3rd, and 7th level.

### Class Features

- **Psionic Focus** — Spend a swift action to gain focus; expend focus to activate certain powers and feats
- **Talents** — 0-level powers chosen at character creation
- **Manifesting** — Powers known scaling with level; uses a Wisdom-based power point economy adapted to WotR's spellbook system
- **Bonus Feats** — Combat or psionic feats at 1st and every even level
- **Paths** — 12 distinct warrior paths, each granting a unique feature at 1st, 3rd, and 7th path level
- **Eternal Warrior** — Level 20 capstone

---

## WotR Mechanical Adaptations

Several tabletop rules had to be adapted or replaced due to engine limitations:

| Tabletop Rule | WotR Adaptation |
|---|---|
| Power point economy | Replaced with a spellbook per-day slot system; manifester level still drives scaling |
| Focus as move/swift action with skill check | Simplified: focus is applied as a buff, no check required |
| `AddEffectFastHealing` fires both `HealRate` and `Heal` independently | Fast healing powers use `AddBuffActions(newRound: HealTarget(...))` to avoid double-ticking |
| `CopyFrom` on blueprints | Only copies display properties, not mechanical components — all powers re-implement their mechanics directly |
| `UnitCondition.ImmuneToAttackOfOpportunity` | Prevents a unit from *making* AoOs, not from *provoking* them — Up the Walls uses a Harmony patch on `UnitCombatEngagementController.ProvokeAttackOfOpportunity` instead |
| Conditional scaling bonus weapon damage | `AddInitiatorAttackWithWeaponTrigger` + `DealDamage` always shows as a separate combat log entry; no engine component supports conditional scaled dice inline with base weapon damage |
| Force damage type | No explicit Force enum exists in WotR; represented as `DamageEnergyType.Magic` |
| Temp HP from Vigor | Matches *False Life* scaling — applies `FalseLifeBuff` directly rather than a custom implementation |
| Telekinetic Punch | Added a Will save (not in original talent description) to match the tabletop power it references |

---

## Implemented Content

### Powers

**0-Level Talents**
- Burst — bonus land speed
- Deceleration — slow an enemy
- Empty Mind — +2 Will save
- Minor Precognition — insight bonus to AC
- Precognition (Defensive) — deflection bonus to AC
- Telekinetic Punch — ranged touch attack, Will save for damage
- Valor — morale bonus to saves vs fear

**1st Level**
- Biofeedback — damage reduction
- Expansion — enlarge person effect
- Compression — reduce person effect
- Force Screen — shield bonus to AC
- Inertial Armor — armor bonus to AC
- Metaphysical Claw — enhance natural weapons
- Metaphysical Weapon — enhance a weapon
- Thicken Skin — natural armor bonus
- Vigor — temporary hit points (matches False Life)

**2nd Level**
- Animal Affinity — ability score boost
- Body Adjustment — heal HP
- Body Purification — remove ability damage
- Concealing Amorpha — 20% concealment
- Detect Hostile Intent — sense enemies
- Hustle — bonus move action
- Psionic Lion's Charge — full attack on a charge
- Strength of My Enemy — steal strength from foe

**3rd Level**
- Concealing Amorpha (Greater) — 50% concealment
- Evade Burst — evasion for one round
- Graft Weapon — bond a weapon to your hand
- Keen Edge (Psionic) — keen weapon
- Mental Barrier — deflection bonus to AC
- Physical Acceleration — haste effect
- Dimension Slide — short-range teleport
- Ubiquitous Vision — all-around vision
- Vampiric Blade — drain HP on hit

**4th Level**
- Battle Transformation — polymorph
- Dimension Door — teleport
- Energy Adaptation — resist energy
- Freedom of Movement
- Inertial Barrier — DR 10/—
- Steadfast Perception — true seeing lite
- Weapon of Energy — add energy damage to weapon
- Zealous Fury — rage-like combat boost

**5th Level**
- Adapt Body — environmental adaptation
- True Metabolism — fast healing
- True Seeing

**6th Level**
- Body of Iron — iron golem transformation
- Disintegrate (Psionic)
- Mind Blank (Personal, Psionic)
- Oak Body — wooden golem transformation

### Feats

**Psionic Combat Feats**
- Psionic Weapon — 1d6–4d6 force damage on melee attacks while focused
- Greater Psionic Weapon — additional 1d6–4d6 force damage on melee attacks (stacks)
- Psionic Fist — 1d6–4d6 force damage on unarmed/natural attacks while focused
- Greater Psionic Fist — additional scaling damage (stacks)
- Psionic Shot — 1d6–4d6 force damage on ranged attacks while focused
- Greater Psionic Shot — additional scaling damage (stacks)
- Deep Impact — spend focus to ignore target's armor on a melee touch attack
- Psionic Critical — spend focus to auto-confirm a critical hit
- Aligned Attack — spend focus to bypass alignment DR
- Wounding Attack — spend focus to deal Constitution damage
- Fell Shot — spend focus for a ranged touch attack
- Unavoidable Strike — spend focus for a melee touch attack

**Psionic Utility Feats**
- Gain Psionic Focus — prerequisite for all psionic feats
- Psionic Meditation — regain focus as a swift action while stationary
- Psionic Body — +2 HP per psionic feat
- Psionic Dodge — +1 dodge AC while focused
- Psionic Endowment — +1 to DC of manifested powers while focused
- Speed of Thought — +10 ft movement while focused
- Up the Walls — no attacks of opportunity when moving while focused
- Combat Manifestation — reduce concentration penalties when manifesting in combat
- Rapid Metabolism — fast healing 1 per round
- Reckless Offense — trade AC for attack bonus
- Intuitive Fighting — use Wisdom instead of Dexterity for combat maneuvers
- Critical Refocus — regain focus on a confirmed critical hit

### Paths

| Path | Theme |
|---|---|
| Weaponmaster | Weapon focus, martial mastery |
| Brawler | Unarmed and grapple |
| Archer | Ranged combat |
| Ascetic | Monk-like discipline |
| Assassin's | Stealth and sneak attack |
| Dervish | Two-weapon and mobility |
| Feral Warrior | Natural weapons and beast affinity |
| Gladiator | Intimidation and performance |
| Infiltrator | Skills and deception |
| Interceptor | Defense and counter-attacks |
| Mind Knight | Mental discipline and defense |
| Survivor | Resilience and healing |

Twisting Paths pathweaving allows multipath feat selection at later levels.

### Harmony Patches

- **UpTheWallsPatch** — Suppresses movement-triggered AoOs for units with the Up the Walls buff by prefixing `UnitCombatEngagementController.ProvokeAttackOfOpportunity`
- **PsionicProficiencyPatch** — Tracks psionic feats so Psionic Body can count them; enables the Psionic Meditation swift-action refocus window

---

## Build & Deploy

### Prerequisites

- [Unity Mod Manager](https://www.nexusmods.com/site/mods/21) installed and patched into *Pathfinder: Wrath of the Righteous*
- Visual Studio 2022 or the .NET SDK (net472 target)
- Game installed at the default Steam path: `C:\Program Files (x86)\Steam\steamapps\common\Pathfinder Second Adventure`

If your game is installed elsewhere, update the `<WrathPath>` property in [PsychicWarrior.csproj](PsychicWarrior.csproj).

### Building

```
dotnet build
```

The PostBuild target automatically copies three files to `$(WrathPath)\Mods\PsychicWarrior\`:

- `PsychicWarrior.dll` — the mod assembly
- `Info.json` — UMM manifest
- `BlueprintCore.dll` — BlueprintCore runtime dependency

### Running

Launch the game normally through Steam. UMM will load the mod on startup. Errors are written to `Player.log` (in the game's `%AppData%` folder) and prefixed with `[PsychicWarrior]`.
