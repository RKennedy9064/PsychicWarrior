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
            catch (System.Exception e)
            {
                log.Error($"[PsychicWarrior] {name} failed: {e}");
                UnityEngine.Debug.LogError($"[PsychicWarrior] {name} failed: {e}");
            }
        }

        // Keep Debug.LogError on Configure failures (above) — silent failures
        // led to a hard-to-find GUID collision; surfacing them to Player.log
        // is cheap insurance for future blueprint additions.

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

        // ── Phase 8: 2nd-level powers ──────────────────────────────────────────
        Run(nameof(Powers.BodyAdjustment),           Powers.BodyAdjustment.Configure,            logger);
        Run(nameof(Powers.BodyPurification),         Powers.BodyPurification.Configure,          logger);
        Run(nameof(Powers.StrengthOfMyEnemy),        Powers.StrengthOfMyEnemy.Configure,         logger);
        Run(nameof(Powers.AnimalAffinity),           Powers.AnimalAffinity.Configure,            logger);
        Run(nameof(Powers.DetectHostileIntent),      Powers.DetectHostileIntent.Configure,       logger);
        Run(nameof(Powers.Hustle),                   Powers.Hustle.Configure,                    logger);

        // ── Phase 14: 5th-level powers ─────────────────────────────────────────
        Run(nameof(Powers.TrueMetabolism),           Powers.TrueMetabolism.Configure,            logger);
        Run(nameof(Powers.AdaptBody),                Powers.AdaptBody.Configure,                 logger);
        Run(nameof(Powers.TrueSeeing),               Powers.TrueSeeing.Configure,                logger);

        // ── Phase 14: 6th-level powers ─────────────────────────────────────────
        Run(nameof(Powers.DisintegratePsionic),      Powers.DisintegratePsionic.Configure,       logger);
        Run(nameof(Powers.MindBlankPersonalPsionic),Powers.MindBlankPersonalPsionic.Configure, logger);
        Run(nameof(Powers.OakBody),                  Powers.OakBody.Configure,                   logger);

        // ── Phase 14: 4th-level powers ─────────────────────────────────────────
        Run(nameof(Powers.DimensionDoor),            Powers.DimensionDoor.Configure,             logger);
        Run(nameof(Powers.FreedomOfMovement),        Powers.FreedomOfMovement.Configure,         logger);
        Run(nameof(Powers.WeaponOfEnergy),           Powers.WeaponOfEnergy.Configure,            logger);
        Run(nameof(Powers.SteadfastPerception),      Powers.SteadfastPerception.Configure,       logger);

        // ── Phase 9: 3rd-level powers ──────────────────────────────────────────
        Run(nameof(Powers.VampiricBlade),            Powers.VampiricBlade.Configure,             logger);
        Run(nameof(Powers.MentalBarrier),            Powers.MentalBarrier.Configure,             logger);
        Run(nameof(Powers.ConcealingAmorphaGreater), Powers.ConcealingAmorphaGreater.Configure,  logger);
        Run(nameof(Powers.GraftWeapon),              Powers.GraftWeapon.Configure,               logger);
        Run(nameof(Powers.KeenEdgePsionic),          Powers.KeenEdgePsionic.Configure,           logger);

        // ── Phase 4: Class features (paths first, skill bonuses reference path GUIDs) ──
        Run(nameof(Features.PsychicWarriorProficiencies), Features.PsychicWarriorProficiencies.Configure, logger);
        Run(nameof(Features.PsychicWarriorBonusFeat),     Features.PsychicWarriorBonusFeat.Configure,     logger);

        // Individual paths must be configured before PathSelection (which references their GUIDs)
        // and before PathSkillBonus (which references WeaponmasterPath/BrawlerPath GUIDs).
        Run(nameof(Features.Paths.WeaponmasterPath),      Features.Paths.WeaponmasterPath.Configure,      logger);
        Run(nameof(Features.Paths.BrawlerPath),           Features.Paths.BrawlerPath.Configure,           logger);
        Run(nameof(Features.Paths.ArcherPath),            Features.Paths.ArcherPath.Configure,            logger);
        Run(nameof(Features.Paths.AsceticPath),           Features.Paths.AsceticPath.Configure,           logger);
        Run(nameof(Features.Paths.AssassinsPath),         Features.Paths.AssassinsPath.Configure,         logger);
        Run(nameof(Features.Paths.DervishPath),           Features.Paths.DervishPath.Configure,           logger);
        Run(nameof(Features.Paths.FeralWarriorPath),      Features.Paths.FeralWarriorPath.Configure,      logger);
        Run(nameof(Features.Paths.GladiatorPath),         Features.Paths.GladiatorPath.Configure,         logger);
        Run(nameof(Features.Paths.InfiltratorPath),       Features.Paths.InfiltratorPath.Configure,       logger);
        Run(nameof(Features.Paths.InterceptorPath),       Features.Paths.InterceptorPath.Configure,       logger);
        Run(nameof(Features.Paths.MindKnightPath),        Features.Paths.MindKnightPath.Configure,        logger);
        Run(nameof(Features.Paths.SurvivorPath),          Features.Paths.SurvivorPath.Configure,          logger);
        Run(nameof(Features.Paths.PsychicWarriorPathSelection), Features.Paths.PsychicWarriorPathSelection.Configure, logger);
        Run(nameof(Features.PathSkillBonus),         Features.PathSkillBonus.Configure,          logger);
        Run(nameof(Features.Paths.TwistingPathsPathweaving), Features.Paths.TwistingPathsPathweaving.Configure, logger);

        // ── Phase 5: Feats ─────────────────────────────────────────────────────
        Run(nameof(Feats.PsionicMeditation),         Feats.PsionicMeditation.Configure,          logger);
        Run(nameof(Feats.PsionicWeapon),             Feats.PsionicWeapon.Configure,              logger);

        // ── Phase 6b: Psionic Feats ────────────────────────────────────────────
        // SpeedOfThought/PsionicDodge inject components into PsionicFocusBuff — must run after Focus.
        Run(nameof(Feats.PsionicBody),               Feats.PsionicBody.Configure,                logger);
        Run(nameof(Feats.SpeedOfThought),            Feats.SpeedOfThought.Configure,             logger);
        Run(nameof(Feats.PsionicDodge),              Feats.PsionicDodge.Configure,               logger);
        Run(nameof(Feats.CriticalRefocus),           Feats.CriticalRefocus.Configure,            logger);
        Run(nameof(Feats.PsionicFist),               Feats.PsionicFist.Configure,                logger);
        Run(nameof(Feats.PsionicShot),               Feats.PsionicShot.Configure,                logger);
        // Greater feats require base feat GUIDs and register with PsionicProficiencyPatch
        Run(nameof(Feats.GreaterPsionicWeapon),      Feats.GreaterPsionicWeapon.Configure,       logger);
        Run(nameof(Feats.GreaterPsionicFist),        Feats.GreaterPsionicFist.Configure,         logger);
        Run(nameof(Feats.GreaterPsionicShot),        Feats.GreaterPsionicShot.Configure,         logger);
        // Tier 2 feats
        Run(nameof(Feats.RapidMetabolism),           Feats.RapidMetabolism.Configure,            logger);
        Run(nameof(Feats.CombatManifestation),       Feats.CombatManifestation.Configure,        logger);
        Run(nameof(Feats.DeepImpact),                Feats.DeepImpact.Configure,                 logger);
        Run(nameof(Feats.UpTheWalls),                Feats.UpTheWalls.Configure,                 logger);
        Run(nameof(Feats.PsionicEndowment),          Feats.PsionicEndowment.Configure,           logger);

        // ── Phase 15: Level 20 Capstone ───────────────────────────────────────
        Run(nameof(Features.EternalWarrior),             Features.EternalWarrior.Configure,             logger);

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
