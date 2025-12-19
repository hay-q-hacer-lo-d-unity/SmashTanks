using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] protected Collider2D projectileCollider;
        
        protected Collider2D OwnerCollider;

        protected Coroutine ReenableCollisionRoutine;
        
        [SerializeField] protected Rigidbody2D rb;
        
        protected float Damage { get; set; }

        public void Initialize(Collider2D owner, Vector2 speed, float damage)
        {
            rb.linearVelocity = speed;
            Damage = damage;
            SetOwner(owner);
        }

        private void FixedUpdate()
        {
            if (!Utils.IsInsideMapBounds(transform.position)) Destroy(gameObject);
        }
        private void SetOwner(Collider2D owner)
        {
            OwnerCollider = owner;
            
            // Ignore collisions with owner collider.
            if (!projectileCollider) return;
            Physics2D.IgnoreCollision(owner, projectileCollider, true);

            // Restart collision reenable coroutine if needed.
            if (ReenableCollisionRoutine != null) StopCoroutine(ReenableCollisionRoutine);

            ReenableCollisionRoutine = StartCoroutine(ReenableCollisionAfterDelay(OwnerCollider, projectileCollider, 0.5f));
        }
        
        private IEnumerator ReenableCollisionAfterDelay(Collider2D owner, Collider2D projectile, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (projectile) Physics2D.IgnoreCollision(owner, projectile, false);

            ReenableCollisionRoutine = null;
        }
        
        private void OnDestroy()
        {
            if (ReenableCollisionRoutine != null) StopCoroutine(ReenableCollisionRoutine);
        }
    }
}