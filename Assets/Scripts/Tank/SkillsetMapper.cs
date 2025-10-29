using System;
using System.Collections.Generic;
using UI;

namespace Tank
{
    public class SkillsetMapper
    {
        public SkillsetMapper(Skillset skillset)
        {
            MapAccuracy(skillset.StatsMap["Accuracy"]);
            MapMass(skillset.StatsMap["Mass"]);
            MapJump(skillset.StatsMap["Jump Force"]);
            MapMending(skillset.StatsMap["Mending"]);
            MapDamage(skillset.StatsMap["Damage"]);
            MapAbilities(skillset.AbilitiesMap);
        }

        /// ===== JUMP =====
        public float ForceMultiplier = SmashTanksConstants.BASE_JUMP_FORCE_MULTIPLIER;
        private const float ForceMultiplierMultiplier = SmashTanksConstants.JUMP_FORCE_MULTIPLIER_MULTIPLIER_PER_LEVEL;
        public float MaxForce = SmashTanksConstants.BASE_MAX_JUMP_FORCE;
        private void MapJump(int level, bool compound = false)
        {
            MapStat(ref ForceMultiplier, level, ForceMultiplierMultiplier, compound);
            MapStat(ref MaxForce, level, ForceMultiplierMultiplier, compound);
        }
    
        /// ===== MASS (& HEALTH) =====
        public float Mass = SmashTanksConstants.BASE_MASS;
        private const float MassMultiplier = SmashTanksConstants.MASS_MULTIPLIER_PER_LEVEL;
        public float Health = SmashTanksConstants.BASE_HEALTH;
        private void MapMass(int level, bool compound = false)
        {
            MapStat(ref Mass, level, MassMultiplier, compound);
            MapStat(ref Health, level, MassMultiplier, compound);
        }

        /// ==== ACCURACY =====
        public float Accuracy = SmashTanksConstants.BASE_ACCURACY;
        private const float AccuracyMultiplier = SmashTanksConstants.ACCURACY_MULTIPLIER_PER_LEVEL;
        private void MapAccuracy(int level, bool compound = false)
        {
            MapStat(ref Accuracy, level, AccuracyMultiplier, compound);
        }

        /// ==== MENDING =====
        public float MendingRate = SmashTanksConstants.BASE_MENDING_RATE;
        private const float MendingRateMultiplier = SmashTanksConstants.MENDING_RATE_MULTIPLIER_PER_LEVEL;
        private void MapMending(int level, bool compound = false)
        {
            MapStat(ref MendingRate, level, MendingRateMultiplier, compound);
        }
        
        
        /// ===== DAMAGE =====
        public float Damage = SmashTanksConstants.BASE_DAMAGE;
        private const float DamageMultiplier = SmashTanksConstants.DAMAGE_MULTIPLIER_PER_LEVEL;

        private void MapDamage(int level, bool compound = false)
        {
            MapStat(ref Damage, level, DamageMultiplier, compound);
        }
        
        private static void MapStat(ref float stat, int level, float multiplier, bool compound = false)
        {
            stat = compound
                ? stat * (float)Math.Pow(1 + multiplier, level)
                : stat + stat * multiplier * level;
        }
        
        /// ===== ABILITIES =====
        public bool Juggernaut;
        private void MapAbilities(Dictionary<string, bool> abilities)
        {
            foreach (var ability in abilities)
            {
                switch (ability.Key)
                {
                    case "Juggernaut":
                        Juggernaut = ability.Value;
                        break;
                    case "Shield":
                        // 
                        break;
                }
            }
        }
    }
}