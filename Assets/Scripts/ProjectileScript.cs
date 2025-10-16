using Fusion;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Rigidbody2D _rb;

    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public float damage = 2f;
    public GameObject explosionEffectPrefab;

    private Collider2D _ownerCollider;
    private float lifeTime = 5f;
    private float timer = 0f;

    public void SetOwner(Collider2D owner)
    {
        _ownerCollider = owner;
        Collider2D projectileCollider = GetComponent<Collider2D>();
        if (_ownerCollider && projectileCollider)
        {
            Physics2D.IgnoreCollision(_ownerCollider, projectileCollider);
        }
    }

    public override void Spawned()
    {
        _rb = GetComponent<Rigidbody2D>();
        timer = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg + 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        timer += Runner.DeltaTime;
        if (timer >= lifeTime && Object.HasStateAuthority)
        {
            Explode();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        Explode();
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            // Instancia solo efectos visuales (no en red)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D col in colliders)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null && rb != _rb)
            {
                Vector2 direction = rb.position - (Vector2)transform.position;
                float distance = direction.magnitude;
                float normalizedDistance = Mathf.Clamp01(distance / explosionRadius);
                float attenuation = 1f - normalizedDistance * normalizedDistance;

                float force = explosionForce * attenuation;
                float damageAmount = damage * attenuation;

                TankScript tank = col.GetComponent<TankScript>();
                if (tank != null)
                {
                    tank.ApplyDamage(damageAmount);
                    Debug.Log($"[Projectile] Hit {tank.name} → Damage: {damageAmount:F2}");
                }

                rb.AddForce(direction.normalized * force);
            }
        }

        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
