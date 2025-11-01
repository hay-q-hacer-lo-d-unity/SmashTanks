using UI;
using UnityEngine;

namespace Tank
{
    [System.Serializable]
    public class TankStats
    {
        [Header("Shot Settings")] public float speedMultiplier = SmashTanksConstants.MISSILE_SPEED_MULTIPLIER;
        public float maxSpeed = SmashTanksConstants.MISSILE_MAX_SPEED;

        [Header("Jump Settings")]
        public float forceMultiplier;
        public float maxForce;

        [Header("Health Settings")]
        public float mass;
        public float maxHealth;
        
        [Header("Magicka Settings")]
        public float maxMagicka;
        
        [Header("Magicka Regeneration Settings")]
        public float magickaRegenRate;
        
        [Header("Intellect Settings")]
        public float intellect;

        [Header("Accuracy Settings")]
        public float accuracy;
        
        [Header("Mending Settings")]
        public float mendingRate;
        
        [Header("Damage Settings")]
        public float damage;
        public float baseDamage;

        [Header("Explosion Settings")] 
        public float explosionRadius = SmashTanksConstants.EXPLOSION_RADIUS;
        public float explosionForce = SmashTanksConstants.EXPLOSION_FORCE;
        
        [Header("Abilities")]
        public bool juggernaut;
        

        public void ApplySkillset(Skillset skillset)
        {
            SkillsetMapper mapper = new(skillset);
            mass = mapper.Mass;
            maxHealth = mapper.Health;
            forceMultiplier = mapper.ForceMultiplier;
            maxForce = mapper.MaxForce;
            accuracy = mapper.Accuracy;
            mendingRate = mapper.MendingRate;
            damage = mapper.Damage;
            baseDamage = mapper.Damage;
            maxMagicka = mapper.MaxMagicka;
            magickaRegenRate = mapper.MagickaRegenRate;
            intellect = mapper.Intellect;
            juggernaut = mapper.Juggernaut;
        }
    }
}