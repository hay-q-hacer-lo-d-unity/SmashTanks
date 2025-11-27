using Tank;
using UnityEngine;

namespace Weapons
{
    public class JuggernautProjectile : Projectile
    {
        private void Update()
        {
            if (Rb.linearVelocity.sqrMagnitude <= 0.01f) return;

            var angle = Mathf.Atan2(Rb.linearVelocity.y, Rb.linearVelocity.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var tank = collision.gameObject.GetComponent<TankScript>();
            if (tank != null) tank.ApplyDamage(Damage);
            Destroy(gameObject);
        }
    }
}