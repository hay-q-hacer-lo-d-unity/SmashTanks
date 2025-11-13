using System;
using System.Collections.Generic;
using SkillsetUI;

namespace Tank
{
    /// <summary>
    /// Maps a <see cref="Skillset"/> to numerical tank parameters and abilities,
    /// applying per-level multipliers from <see cref="SmashTanksConstants"/>.
    /// </summary>
    public class SkillsetMapper
    {
        /// <summary>
        /// Creates a new skillset mapper that converts skill levels to gameplay stats.
        /// </summary>
        /// <param name="skillset">The skillset object containing stats and abilities.</param>
        public SkillsetMapper(Skillset skillset)
        {
            if (skillset?.StatsMap == null || skillset.AbilitiesMap == null)
                throw new ArgumentNullException(nameof(skillset), "Skillset data cannot be null.");

            MapAccuracy(skillset.StatsMap["Accuracy"]);
            MapMass(skillset.StatsMap["Mass"]);
            MapJump(skillset.StatsMap["Jump Force"]);
            MapMending(skillset.StatsMap["Mending"]);
            MapDamage(skillset.StatsMap["Damage"]);
            MapMagickaRegen(skillset.StatsMap["Magicka Regeneration"]);
            MapIntellect(skillset.StatsMap["Intellect"]);
            MapAbilities(skillset.AbilitiesMap);
        }

        // ============================================================
        // ==================== JUMP SETTINGS =========================
        // ============================================================

        private const float MaxForceMultiplier = SmashTanksConstants.Jump.MaxForceMultiplierPerLevel;
        public float MaxForce = SmashTanksConstants.Jump.MaxForceMultiplierPerLevel;

        private void MapJump(int level) => MapStat(ref MaxForce, level, MaxForceMultiplier);

        // ============================================================
        // ================= MASS & HEALTH SETTINGS ==================
        // ============================================================

        public float Mass = SmashTanksConstants.Stats.BaseMass;
        public float Health = SmashTanksConstants.Stats.BaseHealth;
        private const float MassMultiplier = SmashTanksConstants.Stats.MassMultiplierPerLevel;

        private void MapMass(int level)
        {
            MapStat(ref Mass, level, MassMultiplier);
            MapStat(ref Health, level, MassMultiplier);
        }

        // ============================================================
        // =================== ACCURACY SETTINGS ======================
        // ============================================================

        public float Accuracy = SmashTanksConstants.Stats.BaseAccuracy;
        private const float AccuracyMultiplier = SmashTanksConstants.Stats.AccuracyMultiplierPerLevel;
        private void MapAccuracy(int level) => MapStat(ref Accuracy, level, AccuracyMultiplier);

        // ============================================================
        // =================== MENDING SETTINGS =======================
        // ============================================================

        public float MendingRate = SmashTanksConstants.Stats.BaseMendingRateAbsolute;
        private const float MendingRateMultiplier = SmashTanksConstants.Stats.MendingRateMultiplierPerLevel;
        private void MapMending(int level) => MapStat(ref MendingRate, level, MendingRateMultiplier);

        // ============================================================
        // ==================== DAMAGE SETTINGS =======================
        // ============================================================

        public float Damage = SmashTanksConstants.Stats.BaseDamage;
        private void MapDamage(int level) => Damage = level;

        // ============================================================
        // ================= MAGICKA SETTINGS =========================
        // ============================================================

        /// <summary>
        /// The tank’s maximum magicka capacity. Currently not scaled per level.
        /// </summary>
        public float MaxMagicka = SmashTanksConstants.Stats.BaseMagicka;

        /*
        private const float MaxMagickaMultiplier = SmashTanksConstants.MAX_MAGICKA_MULTIPLIER_PER_LEVEL;
        private void MapMagicka(int level) => MapStat(ref MaxMagicka, level, MaxMagickaMultiplier);
        */

        // ============================================================
        // ================ MAGICKA REGEN SETTINGS ====================
        // ============================================================

        public float MagickaRegenRate = SmashTanksConstants.Stats.MagickaRegenerationRate;
        private const float MagickaRegenRateMultiplier = SmashTanksConstants.Stats.MagickaRegenerationRateMultiplierPerLevel;
        private void MapMagickaRegen(int level) => MapStat(ref MagickaRegenRate, level, MagickaRegenRateMultiplier);

        // ============================================================
        // ==================== INTELLECT SETTINGS ====================
        // ============================================================

        public float Intellect = SmashTanksConstants.Stats.BaseIntellect;
        private void MapIntellect(int level) => Intellect = level;

        // ============================================================
        // ====================== ABILITIES ===========================
        // ============================================================

        public bool Juggernaut;

        /// <summary>
        /// Maps unlocked abilities from the player's skillset.
        /// </summary>
        /// <param name="abilities">A dictionary of ability names and their unlock states.</param>
        private void MapAbilities(IReadOnlyDictionary<string, bool> abilities)
        {
            foreach (var (key, value) in abilities)
            {
                switch (key)
                {
                    case "Juggernaut":
                        Juggernaut = value;
                        break;

                    // Future abilities can be added here.
                }
            }
        }

        // ============================================================
        // ===================== HELPER METHOD ========================
        // ============================================================

        /// <summary>
        /// Applies a level-based multiplier to a base stat value.
        /// </summary>
        /// <param name="stat">Reference to the stat value being modified.</param>
        /// <param name="level">The level of the corresponding skill.</param>
        /// <param name="multiplier">The growth multiplier applied per level.</param>
        /// <param name="compound">
        /// If true, applies compound growth (exponential).
        /// Otherwise, applies linear scaling.
        /// </param>
        private static void MapStat(ref float stat, int level, float multiplier, bool compound = SmashTanksConstants.Config.UseCompoundIncrease)
        {
            stat = compound
                ? stat * (float)Math.Pow(1 + multiplier, level)
                : stat + stat * multiplier * level;
        }
    }
}
