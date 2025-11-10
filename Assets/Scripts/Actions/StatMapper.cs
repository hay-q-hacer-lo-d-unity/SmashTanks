using System;
using UnityEngine;

namespace Actions
{
    public class StatMapper
    {
        public static float MapMissileDamage(float damageLevel) => MapByIncrease(
            SmashTanksConstants.MISSILE_BASE_DAMAGE,
            damageLevel, 
            SmashTanksConstants.MISSILE_DAMAGE_INCREASE_PER_DAMAGE
            );
        
        public static float MapBouncyMissileDamage(float damageLevel) => MapByIncrease(
            SmashTanksConstants.BOUNCY_MISSILE_BASE_DAMAGE,
            damageLevel, 
            SmashTanksConstants.BOUNCY_MISSILE_DAMAGE_INCREASE_PER_DAMAGE
            );
        
        public static float MapCrashDamageMultiplier(float damageLevel) => MapByIncrease(
            SmashTanksConstants.CRASH_BASE_DAMAGE_MULTIPLIER,
            damageLevel,
            SmashTanksConstants.CRASH_DAMAGE_MULTIPLIER_INCREASE_PER_DAMAGE
            );

        public static float MapGaleForce(float intellectLevel) =>
            MapByIncrease(
                SmashTanksConstants.GALE_BASE_FORCE,
                intellectLevel,
                SmashTanksConstants.GALE_FORCE_INCREASE_PER_INTELLECT
            );

        public static float MapBeamDamage(float intellectLevel) => MapByIncrease(
            SmashTanksConstants.BEAM_BASE_DAMAGE,
            intellectLevel,
            SmashTanksConstants.BEAM_DAMAGE_PER_INTELLECT
            );

        public static float MapTeleportRadius(float intellectLevel)
        {
            const float minRadius = 2f;
            const float maxRadius = 20f;
            const float decayRate = (maxRadius - minRadius) / (SmashTanksConstants.STATPOINTS + 1);
            return maxRadius - intellectLevel * decayRate;
        }

        private static float MapStat(float stat, float level, float multiplier, bool compound = SmashTanksConstants.USE_COMPOUND_INCREASE)
        {
            return compound
                ? stat * (float)Math.Pow(1 + multiplier, level)
                : stat + stat * multiplier * level;
        }

        private static float MapByIncrease(float baseline, float level, float increasePerLevel) =>
            baseline + increasePerLevel * level;
    }
}