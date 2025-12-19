using System.Collections;
using Actions;
using Manager;
using Tank;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public abstract class ExplosiveProjectile : Projectile
    {
        [Tooltip("Radius of the explosion area.")]
        protected float ExplosionRadius { get; set; }

        [Tooltip("Force applied to nearby objects within the explosion radius.")]
        protected float ExplosionForce { get; set; }
        
        [Tooltip("Prefab of the explosion effect visual.")]
        protected GameObject ExplosionEffectPrefab { get; set; }
        
        [FormerlySerializedAs("ExplosionSound")]
        [Header("Audio")]
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private float explosionVolume = 1f;

        public void Initialize(Collider2D owner, Vector2 speed, float explosionRadius, float explosionForce, float damage)
        {
            base.Initialize(owner, speed, damage);
            SetStats(explosionRadius, explosionForce, damage);
        }

        private void SetStats(float explosionRadius, float explosionForce, float damage)
        {
            ExplosionRadius = explosionRadius;
            ExplosionForce = explosionForce;
            Damage = damage;
        }

        
        /// <summary>
        /// Handles the explosion effect, applying forces and damage to nearby objects.
        /// </summary>
        public void Explode()
        {
            if (ExplosionEffectPrefab) Instantiate(ExplosionEffectPrefab, transform.position, Quaternion.identity);

            if (explosionSound)
                SoundsScript.Play2DSound(
                    explosionSound,
                    explosionVolume
                );
            
            var colliders = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

            foreach (var col in colliders)
            {
                if (col.attachedRigidbody == null || col.attachedRigidbody == base.rb) continue;

                var rb = col.attachedRigidbody;
                var direction = rb.position - (Vector2)transform.position;
                var distance = direction.magnitude;
                var normalizedDistance = Mathf.Clamp01(distance / ExplosionRadius);

                var attenuation = 1f - normalizedDistance * normalizedDistance;
                var force = ExplosionForce * attenuation;
                var damageAmount = Damage * attenuation;

                if (col.TryGetComponent<TankScript>(out var tank))
                    GameManagerScript.Instance.ApplyDamage(tank.OwnerId, damageAmount);

                rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
        }
    }
}
