using System;
using UnityEngine;

namespace Actions
{
    public static class StatMapper
    {
        public static float MapMissileDamage(float damageLevel) => MapByIncrease(
            SmashTanksConstants.Missile.BaseDamage,
            damageLevel, 
            SmashTanksConstants.Missile.DamageIncreasePerDamage
            );
        
        public static float MapBouncyMissileDamage(float damageLevel) => MapByIncrease(
            SmashTanksConstants.BouncyMissile.BaseDamage,
            damageLevel, 
            SmashTanksConstants.BouncyMissile.DamageIncreasePerDamage
            );
        
        public static float MapCrash(float damageLevel) => MapByIncrease(
            SmashTanksConstants.Crash.BaseDamageMultiplier,
            damageLevel,
            SmashTanksConstants.Crash.DamageMultiplierIncreasePerDamage
            );

        public static float MapGaleForce(float intellectLevel) =>
            MapByIncrease(
                SmashTanksConstants.Gale.BaseForce,
                intellectLevel,
                SmashTanksConstants.Gale.ForceIncreasePerIntellect
            );

        public static float MapBeamDamage(float intellectLevel) => MapByIncrease(
            SmashTanksConstants.Beam.BaseDamage,
            intellectLevel,
            SmashTanksConstants.Beam.DamagePerIntellect
            );

        public static float MapTeleportRadius(float intellectLevel) => 
            SmashTanksConstants.Teleport.MaxRadius - intellectLevel * SmashTanksConstants.Teleport.DecayRate;
        

        private static float MapStat(float stat, float level, float multiplier, bool compound = SmashTanksConstants.Config.UseCompoundIncrease)
        {
            return compound
                ? stat * (float)Math.Pow(1 + multiplier, level)
                : stat + stat * multiplier * level;
        }

        private static float MapByIncrease(float baseline, float level, float increasePerLevel) =>
            baseline + increasePerLevel * level;
    }
}