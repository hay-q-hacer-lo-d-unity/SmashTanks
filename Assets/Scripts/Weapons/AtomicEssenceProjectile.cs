using System.Collections.Generic;
using Manager;
using Tank;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class AtomicEssenceProjectile : Projectile
    {

        private void Awake()
        {
            ProjectileCollider = GetComponent<Collider2D>();
            Rb = GetComponent<Rigidbody2D>();

            // Must be trigger to pass through objects
            ProjectileCollider.isTrigger = true;
        }

        private void Update()
        {
            if (Rb.linearVelocity.sqrMagnitude <= 0.01f) return;

            var angle = Mathf.Atan2(Rb.linearVelocity.y, Rb.linearVelocity.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore owner
            if (other == OwnerCollider) return;

            var tank = other.GetComponent<TankScript>();
            if (!tank) return;

            ApplyDamage(tank);
        }

        private void ApplyDamage(TankScript tank)
        {
            GameManagerScript.Instance.ApplyDamage(tank.OwnerId, Damage);

            // Optional: sound / VFX per hit
            SoundsScript.Play2DSound(
                SoundsScript.Instance.atomicHit,
                0.7f
            );
        }
    }
}