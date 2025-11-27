using System.Collections;
using UnityEngine;

namespace Weapons
{
    public abstract class Projectile : MonoBehaviour
    {
        protected Collider2D ProjectileCollider;
        
        protected Collider2D OwnerCollider;

        protected Coroutine ReenableCollisionRoutine;
        
        protected Rigidbody2D Rb;
        
        protected float Damage { get; set; }

        public void Initialize(Collider2D owner, Vector2 speed, float damage)
        {
            ProjectileCollider = GetComponent<Collider2D>();
            Rb = GetComponent<Rigidbody2D>();
            Rb.linearVelocity = speed;
            Damage = damage;
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