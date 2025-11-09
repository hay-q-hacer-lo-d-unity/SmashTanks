using System.Collections;
using Tank;
using UnityEngine;

namespace Weapons
{
    public abstract class ExplosiveProjectile : MonoBehaviour
    {
        [Tooltip("Radius of the explosion area.")]
        protected float ExplosionRadius { get; set; }

        [Tooltip("Force applied to nearby objects within the explosion radius.")]
        protected float ExplosionForce { get; set; }

        [Tooltip("Base damage dealt by the explosion.")]
        protected float Damage { get; set; }
        
        protected Collider2D ProjectileCollider;
        
        protected Collider2D OwnerCollider;

        protected Coroutine ReenableCollisionRoutine;


        protected Rigidbody2D Rb;

        
        [Tooltip("Prefab of the explosion effect visual.")]
        protected GameObject ExplosionEffectPrefab { get; set; }
        

        public void Initialize(Collider2D owner, Vector2 speed, float explosionRadius, float explosionForce, float damage)
        {
            ProjectileCollider = GetComponent<Collider2D>();
            Rb = GetComponent<Rigidbody2D>();
            Rb.linearVelocity = speed;
            SetStats(explosionRadius, explosionForce, damage);
            SetOwner(owner);
        }

        private void SetOwner(Collider2D owner)
        {
            OwnerCollider = owner;
            
            // Ignore collisions with owner collider.
            if (!ProjectileCollider) return;
            Physics2D.IgnoreCollision(owner, ProjectileCollider, true);

            // Restart collision reenable coroutine if needed.
            if (ReenableCollisionRoutine != null) StopCoroutine(ReenableCollisionRoutine);

            ReenableCollisionRoutine = StartCoroutine(ReenableCollisionAfterDelay(OwnerCollider, ProjectileCollider, 0.25f));
        }

        private void SetStats(float explosionRadius, float explosionForce, float damage)
        {
            ExplosionRadius = explosionRadius;
            ExplosionForce = explosionForce;
            Damage = damage;
        }
        
        private void OnDestroy()
        {
            if (ReenableCollisionRoutine != null) StopCoroutine(ReenableCollisionRoutine);
        }

        
        /// <summary>
        /// Handles the explosion effect, applying forces and damage to nearby objects.
        /// </summary>
        public void Explode()
        {
            if (ExplosionEffectPrefab)
                Instantiate(ExplosionEffectPrefab, transform.position, Quaternion.identity);

            var colliders = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius);

            foreach (var col in colliders)
            {
                if (col.attachedRigidbody == null || col.attachedRigidbody == Rb)
                    continue;

                var rb = col.attachedRigidbody;
                var direction = rb.position - (Vector2)transform.position;
                var distance = direction.magnitude;
                var normalizedDistance = Mathf.Clamp01(distance / ExplosionRadius);

                // Quadratic falloff for smoother attenuation
                var attenuation = 1f - normalizedDistance * normalizedDistance;
                var force = ExplosionForce * attenuation;
                var damageAmount = Damage * attenuation;

                if (col.TryGetComponent<TankScript>(out var tank))
                    tank.ApplyDamage(damageAmount);

                rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
        
        private IEnumerator ReenableCollisionAfterDelay(Collider2D owner, Collider2D projectile, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (projectile) Physics2D.IgnoreCollision(owner, projectile, false);

            ReenableCollisionRoutine = null;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
        }
    }
}
