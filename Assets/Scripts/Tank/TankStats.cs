using UI;
using UnityEngine;

namespace Tank
{
    [System.Serializable]
    public class TankStats
    {
        [Header("Shot Settings")] 
        public float missileMaxSpeed = SmashTanksConstants.MISSILE_MAX_SPEED;
        public float bouncyMissileMaxSpeed = SmashTanksConstants.BOUNCY_MISSILE_MAX_SPEED;

        [Header("Jump Settings")]
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
        public float explosionRadius;
        public float explosionForce;
        
        [Header("Abilities")]
        public bool juggernaut;
        

        public void ApplySkillset(Skillset skillset)
        {
            explosionRadius = SmashTanksConstants.EXPLOSION_RADIUS;
            explosionForce = SmashTanksConstants.EXPLOSION_FORCE;
            SkillsetMapper mapper = new(skillset);
            mass = mapper.Mass;
            maxHealth = mapper.Health;
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