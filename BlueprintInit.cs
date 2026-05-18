using System.Linq;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;

namespace PsychicWarrior;

[HarmonyPatch(typeof(BlueprintsCache), "Init")]
public static class BlueprintInit
{
    private static bool Initialized;

    [HarmonyPostfix]
    public static void Postfix()
    {
        if (Initialized) return;
        Initialized = true;

        var logger = LogWrapper.Get("PsychicWarrior");

        // Each Configure() call is wrapped so a single failure is logged without
        // aborting the rest of initialization or the class registration.
        static void Run(string name, System.Action action, LogWrapper log)
        {
            try { action(); }
            catch (System.Exception e) { log.Error($"[PsychicWarrior] {name} failed: {e}"); }
        }

        // ── Phase 1: Foundation ────────────────────────────────────────────────
        Run(nameof(Mechanics.Focus),                 Mechanics.Focus.Configure,                  logger);
        Run(nameof(Features.PsionicProficiency),     Features.PsionicProficiency.Configure,      logger);
        Run(nameof(Features.MartialPower),           Features.MartialPower.Configure,            logger);

        // ── Phase 2: 0-level talent abilities (must exist before TalentsSelection) ──
        Run(nameof(Powers.MinorPrecognition),        Powers.MinorPrecognition.Configure,         logger);
        Run(nameof(Powers.Burst),                    Powers.Burst.Configure,                     logger);
        Run(nameof(Powers.EmptyMind),                Powers.EmptyMind.Configure,                 logger);
        Run(nameof(Powers.Valor),                    Powers.Valor.Configure,                     logger);
        Run(nameof(Powers.TelekineticPunch),         Powers.TelekineticPunch.Configure,          logger);
        Run(nameof(Powers.PrecognitionDefensive),    Powers.PrecognitionDefensive.Configure,     logger);
        Run(nameof(Powers.Deceleration),             Powers.Deceleration.Configure,              logger);

        // Talent selection references the 0-level power GUIDs above
        Run(nameof(Features.TalentsSelection),       Features.TalentsSelection.Configure,        logger);

        // ── Phase 3: 1st-level powers ──────────────────────────────────────────
        Run(nameof(Powers.Vigor),                    Powers.Vigor.Configure,                     logger);
        Run(nameof(Powers.ForceScreen),              Powers.ForceScreen.Configure,               logger);
        Run(nameof(Powers.InertialArmor),            Powers.InertialArmor.Configure,             logger);
        Run(nameof(Powers.ThickenSkin),              Powers.ThickenSkin.Configure,               logger);
        Run(nameof(Powers.Biofeedback),              Powers.Biofeedback.Configure,               logger);
        Run(nameof(Powers.MetaphysicalWeapon),       Powers.MetaphysicalWeapon.Configure,        logger);

        // ── Phase 4: Class features (paths first, skill bonuses reference path GUIDs) ──
        Run(nameof(Features.PsychicWarriorProficiencies), Features.PsychicWarriorProficiencies.Configure, logger);
        Run(nameof(Features.PsychicWarriorBonusFeat),     Features.PsychicWarriorBonusFeat.Configure,     logger);

        // Paths must be configured before PathSkillBonus because
        // PathSkillBonus.Configure() adds prerequisites referencing WeaponmasterPath/BrawlerPath.
        Run(nameof(Features.Paths.PsychicWarriorPathSelection), Features.Paths.PsychicWarriorPathSelection.Configure, logger);
        Run(nameof(Features.PathSkillBonus),         Features.PathSkillBonus.Configure,          logger);

        // ── Phase 5: Feats ─────────────────────────────────────────────────────
        Run(nameof(Feats.PsionicMeditation),         Feats.PsionicMeditation.Configure,          logger);
        Run(nameof(Feats.PsionicWeapon),             Feats.PsionicWeapon.Configure,              logger);

        // ── Phase 6: Class definition (must come last) ─────────────────────────
        Run(nameof(Classes.PsychicWarriorSpellbook), Classes.PsychicWarriorSpellbook.Configure,  logger);
        Run(nameof(Classes.PsychicWarriorClass),     Classes.PsychicWarriorClass.Configure,      logger);

        // ── Class registration ─────────────────────────────────────────────────
        try
        {
            var root = BlueprintRoot.Instance;
            var pwClassRef = BlueprintTool.GetRef<BlueprintCharacterClassReference>(Utils.Guids.PsychicWarriorClass);

            if (!root.Progression.m_CharacterClasses.Contains(pwClassRef))
                root.Progression.m_CharacterClasses = [.. root.Progression.m_CharacterClasses, pwClassRef];
        }
        catch (System.Exception e)
        {
            logger.Error($"[PsychicWarrior] Class registration failed: {e}");
        }
    }
}
