using System.Collections;
using Tank;
using UnityEngine;

namespace Weapons
{
    /// <summary>
    /// Represents a basic missile projectile that explodes on impact,
    /// dealing damage and applying explosion force to nearby objects.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class BasicMissile : MonoBehaviour, IProjectile
    {
        private Rigidbody2D _rb;

        [Header("Explosion Settings")]
        [Tooltip("Radius of the explosion area.")]
        public float explosionRadius;

        [Tooltip("Force applied to nearby objects within the explosion radius.")]
        public float explosionForce;

        [Tooltip("Base damage dealt by the explosion.")]
        public float damage;

        [Tooltip("Prefab of the explosion effect visual.")]
        public GameObject explosionEffectPrefab;

        private Collider2D _ownerCollider;
        private Coroutine _reenableCollisionRoutine;

        #region IProjectile Implementation

        /// <summary>
        /// Assigns an owner collider to prevent self-collision.
        /// </summary>
        /// <param name="owner">The collider of the tank or entity that fired this missile.</param>
        public void SetOwner(Collider2D owner)
        {
            _ownerCollider = owner;
            var projectileCollider = GetComponent<Collider2D>();

            if (!_ownerCollider || !projectileCollider) return;

            var ownerColliders = _ownerCollider.GetComponentsInChildren<Collider2D>();

            // Ignore collisions with all owner colliders.
            foreach (var col in ownerColliders)
                Physics2D.IgnoreCollision(col, projectileCollider, true);

            // Restart collision reenable coroutine if needed.
            if (_reenableCollisionRoutine != null)
                StopCoroutine(_reenableCollisionRoutine);

            _reenableCollisionRoutine = StartCoroutine(
                ReenableCollisionAfterDelay(ownerColliders, projectileCollider, 0.25f)
            );
        }

        /// <summary>
        /// Sets the missile's explosion parameters.
        /// </summary>
        public void SetStats(float newExplosionRadius, float newExplosionForce, float newDamage)
        {
            explosionRadius = newExplosionRadius;
            explosionForce = newExplosionForce;
            damage = newDamage;
        }

        #endregion

        #region Unity Callbacks

        private void Start() => _rb = GetComponent<Rigidbody2D>();

        private void Update()
        {
            if (_rb.linearVelocity.sqrMagnitude <= 0.01f) return;

            var angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnCollisionEnter2D() => Explode();

        private void OnDestroy()
        {
            if (_reenableCollisionRoutine != null)
                StopCoroutine(_reenableCollisionRoutine);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }

        #endregion

        #region Explosion Logic

        /// <summary>
        /// Handles the explosion effect, applying forces and damage to nearby objects.
        /// </summary>
        public void Explode()
        {
            if (explosionEffectPrefab)
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

            var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (var col in colliders)
            {
                if (col.attachedRigidbody == null || col.attachedRigidbody == _rb)
                    continue;

                var rb = col.attachedRigidbody;
                var direction = rb.position - (Vector2)transform.position;
                var distance = direction.magnitude;
                var normalizedDistance = Mathf.Clamp01(distance / explosionRadius);

                // Quadratic falloff for smoother attenuation
                var attenuation = 1f - normalizedDistance * normalizedDistance;
                var force = explosionForce * attenuation;
                var damageAmount = damage * attenuation;

                if (col.TryGetComponent<TankScript>(out var tank))
                    tank.ApplyDamage(damageAmount);

                rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Waits for a delay before re-enabling collision between the missile and its owner.
        /// </summary>
        private IEnumerator ReenableCollisionAfterDelay(Collider2D[] ownerColliders, Collider2D projectile, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (projectile)
            {
                foreach (var col in ownerColliders)
                {
                    if (col)
                        Physics2D.IgnoreCollision(col, projectile, false);
                }
            }

            _reenableCollisionRoutine = null;
        }

        #endregion
    }
}
