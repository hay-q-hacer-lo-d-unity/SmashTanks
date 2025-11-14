using System;
using Manager;
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

        private readonly HashSet<TankScript> _damagedTanks = new();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherTank = collision.collider.GetComponent<TankScript>();
            if (!otherTank) return;

            if (_damagedTanks.Contains(otherTank)) return;

            var damage = damageMultiplier * collision.relativeVelocity.magnitude * rb.mass;
            GameManagerScript.Instance.ApplyDamage(otherTank.OwnerId, damage);

            _damagedTanks.Add(otherTank); ;
        }
    }
}
