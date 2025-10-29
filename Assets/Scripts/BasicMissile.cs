using Tank;
using UnityEngine;

public class BasicMissile : MonoBehaviour, IProjectile
{
    private Rigidbody2D _rb;

    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public float damage = 2f;
    public GameObject explosionEffectPrefab;

    private Collider2D _ownerCollider;

    public void SetOwner(Collider2D owner)
    {
        _ownerCollider = owner;
        Collider2D projectileCollider = GetComponent<Collider2D>();
        if (_ownerCollider && projectileCollider)
        {
            Physics2D.IgnoreCollision(_ownerCollider, projectileCollider);
        }
    }
    
    public void SetStats(float newExplosionRadius, float newExplosionForce, float newDamage)
    {
        explosionRadius = newExplosionRadius;
        explosionForce = newExplosionForce;
        damage = newDamage;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg + 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    public void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var col in colliders)
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null && rb != _rb)
            {
                Vector2 direction = rb.position - (Vector2)transform.position;
                float distance = direction.magnitude;
                float normalizedDistance = Mathf.Clamp01(distance / explosionRadius);
                float attenuation = 1f - normalizedDistance * normalizedDistance;
                Debug.Log("Damage: " + damage);
                Debug.Log("Attenuation: " + attenuation);

                float force = explosionForce * attenuation;
                float damageAmount = damage * attenuation;
                TankScript tank = col.GetComponent<TankScript>();
                if (tank != null)
                {
                    tank.ApplyDamage(damageAmount);
                }
                rb.AddForce(direction.normalized * force);
            }
        }
        Debug.Log("Explode");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}