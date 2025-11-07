using System;
using Tank;

namespace Actions
{
    using System.Collections.Generic;
    using UnityEngine;

    public class CrashHandlerScript : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float damageMultiplier;

        private void Start()
        {
            Destroy(this, 30f);
        }

        // Keep track of tanks we've already damaged
        private readonly HashSet<TankScript> damagedTanks = new HashSet<TankScript>();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherTank = collision.collider.GetComponent<TankScript>();
            if (!otherTank) return;

            // Only apply damage once per tank
            if (damagedTanks.Contains(otherTank)) return;

            var damage = damageMultiplier * collision.relativeVelocity.magnitude * rb.mass;
            otherTank.ApplyDamage(damage);

            // Record that we've hit this tank
            damagedTanks.Add(otherTank); ;
        }
    }
}
