# Changelog

## [0.1.1] - Unreleased

### Fixed
- Psionic Weapon: bonus damage now correctly scales with manifester level (1d6 → 2d6 → 3d6 → 4d6) instead of always dealing 1d6
- Greater Psionic Weapon: same scaling fix as Psionic Weapon
- Psionic Weapon / Greater Psionic Weapon: multiple separate dice rolls now combined into a single Xd6 roll in the combat log
- Rapid Metabolism / Intuitive Fighting / Psionic Endowment: buffs now apply immediately when the feat is taken while already psionically focused (previously required toggling focus off and back on)
- Dervish Trance: attack bonus now shows in the character sheet attack tooltip and correctly fires on the same rule as the Two-Weapon Fighting penalty
- Dervish Trance: attack bonus now correctly only applies when wielding two weapons (secondary hand has a weapon, not a shield, and grip is not two-handed)
- Secondary path: maneuver menu (expandable action bar button) now correctly appears when a path is selected as a secondary path at level 9
- Secondary path: expanded maneuver is now correctly granted at level 9 alongside the secondary path selection (fixes split/corrupted maneuver menu icon and missing expanded maneuver option)
- All paths: selecting the same path as both primary and secondary is now correctly prevented (greyed out in the secondary selection if already chosen as primary)
- All paths: expandable maneuver button now shows a unified icon instead of a diagonal split (both maneuver and expanded maneuver now use the same icon)

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