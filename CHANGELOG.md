# Changelog

## [0.1.4] - Unreleased

### Added
- Advanced Weaponmaster Path feat: while in Weaponmaster Trance, your trance competence bonus also applies to weapon damage rolls (same manufactured-weapon criteria as the attack bonus; natural attacks excluded). Prerequisites: Weaponmaster path, BAB +6, psychic warrior level 10.

## [0.1.3] - 2026-05-28

### Fixed
- Mind Knight Path: Call Weaponry weapon picks at 3rd, 7th, 11th, 15th, and 19th level no longer appear for non-Mind Knight psychic warriors; picks are now granted via a Mind Knight-owned sub-progression and correctly prompt the player during level-up

## [0.1.2] - 2026-05-27

### Added
- Mind Knight Path: Call Weaponry feature — at 3rd, 7th, 11th, 15th, and 19th level, choose a weapon to psionically call to your primary hand at will
  - Supports every weapon the character is proficient with; exotic weapons (e.g. Elven Curved Blade) appear automatically once the matching exotic weapon proficiency is taken
  - Only one called weapon may be active at a time; switching calls dismisses the previous weapon
  - First weapon choice granted at 1st level (when the path is taken); additional choices at 3rd, 7th, 11th, 15th, and 19th level (six total)
  - Enhancement bonus scales with psychic warrior level: +1 at 3rd, +2 at 7th, +3 at 11th, +4 at 15th, +5 at 19th
  - Called weapon is locked in the primary hand slot while active and cannot be unequipped
- Mind Knight Path: Called weapon now displays an astral glow visual effect (borrowed from Brilliant Energy FX) while active
- Mind Knight Trance: attack roll bonus now only applies when attacking with the currently called weapon (primary hand); secondary hand attacks and attacks without an active called weapon are unaffected. Initiative bonus remains unconditional.

### Fixed
- All paths: trance toggle and maneuver menu now correctly appear at 3rd level (previously appeared immediately at 1st level when the path was selected)
- Mind Knight Path: Call Weaponry first weapon selection now correctly appears immediately when the path is chosen at 1st level — the path is now itself a feature selection, which causes the engine to cascade into the weapon pick automatically
- Mind Knight Path: Called weapon now correctly appears in the primary hand slot when a Call Weaponry toggle is activated (previously the buff and slot lock applied but no weapon appeared)
- Mind Knight Path: Called weapon no longer appears as unidentified when summoned
- Mind Knight Path: Additional weapon selections at 3rd, 7th, 11th, 15th, and 19th level now correctly prompt the player during level-up (previously the selections were silently granted without showing the pick UI)
- Mind Knight Path: Called weapon enhancement bonus now correctly applies to both attack and damage rolls (previously only applied to damage; now uses a proper weapon enchantment so the bonus appears on the weapon itself)
- Mind Knight Path: Called weapon enhancement bonus now automatically upgrades when the character gains a psychic warrior level while the weapon is active (no longer requires toggling the weapon off and on after leveling)

## [0.1.1] - 2026-05-25

### Fixed
- Critical Refocus: scoring a critical hit now correctly regains psionic focus for the attacker (previously had no effect)
- Martial Power: on-hit maneuver effects and the once-per-round cooldown now correctly apply to the attacker (previously had no effect)
- Martial Power: maneuver buffs now last 2 rounds so they are active at the start of your next turn
- Aligned Attack: extra 2d6 damage now correctly bypasses all alignment-based damage reduction (Good/Evil/Law/Chaos)
- Psionic Critical: extra 1d8 damage now inherits the weapon's material and enhancement for damage reduction bypass, matching the weapon attack it extends
- Aligned Attack / Psionic Critical: when dual-wielding, each hand's extra damage now correctly uses that hand's weapon properties (previously always used the primary hand, negating all damage if the enemy was immune to that material)
- Psionic Weapon: bonus damage now correctly scales with manifester level (1d6 → 2d6 → 3d6 → 4d6) instead of always dealing 1d6
- Greater Psionic Weapon: same scaling fix as Psionic Weapon
- Psionic Weapon / Greater Psionic Weapon: multiple separate dice rolls now combined into a single Xd6 roll in the combat log
- Rapid Metabolism / Intuitive Fighting / Psionic Endowment: buffs now apply immediately when the feat is taken while already psionically focused (previously required toggling focus off and back on)
- Dervish Trance: attack bonus now shows in the character sheet attack tooltip and correctly fires on the same rule as the Two-Weapon Fighting penalty
- Dervish Trance: attack bonus now correctly only applies when wielding two weapons (secondary hand has a weapon, not a shield, and grip is not two-handed)
- Dervish Trance: attack bonus no longer applies when unarmed or using natural weapons in the secondary hand
- Secondary path: maneuver menu (expandable action bar button) now correctly appears when a path is selected as a secondary path at level 9
- Secondary path: expanded maneuver is now correctly granted at level 9 alongside the secondary path selection (fixes split/corrupted maneuver menu icon and missing expanded maneuver option)
- All paths: selecting the same path as both primary and secondary is now correctly prevented (greyed out in the secondary selection if already chosen as primary)
- All paths: expandable maneuver button now shows a unified icon instead of a diagonal split (both maneuver and expanded maneuver now use the same icon)
- Weaponmaster Trance: attack bonus now correctly applies to all manufactured weapon attacks
- Feral Warrior Trance: attack bonus now correctly applies to natural weapon and unarmed attacks
- Martial Power: each variant now has a distinct icon matching its path maneuver (instead of all showing the same hammer icon)

## [0.1.0] - 2026-05-24

### Fixed
- All path maneuvers: maneuver menu now only appears at 3rd level (when Expanded Path Maneuver is granted), not at 1st level
- Archer Trance: attack bonus now only applies to ranged and thrown weapons (not natural weapons)
- Feral Warrior Trance: attack bonus now only applies to natural weapon attacks
- Weaponmaster Trance: attack bonus now only applies to manufactured melee weapons (not natural weapons)
- All path trances: trance bonuses now scale with psychic warrior level starting at 3rd level, as per tabletop text ("beginning at 3rd level, +1 every four levels")
  - Weaponmaster, Archer, Feral Warrior, Ascetic: attack/AC bonus now +1 at 3rd, scaling to +5 at 19th
  - Mind Knight: Initiative and attack bonuses now scale identically
  - Interceptor: attack and damage bonuses now scale identically
  - Assassin's, Gladiator: bonuses start at +2 at 3rd level, scaling to +6 at 19th
  - Infiltrator: Persuasion stays +1 higher than damage at all levels, both scaling from 3rd
- Dervish Trance: attack bonus now correctly requires two weapons equipped
- Dervish Trance: attack bonus now scales correctly starting at 3rd level (+1 at 3rd, +2 at 7th, +3 at 11th, +4 at 15th, +5 at 19th)