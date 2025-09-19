using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rb;

    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public GameObject explosionEffectPrefab;

    private Collider2D _ownerCollider;

    public void SetOwner(Collider2D owner)
    {
        _ownerCollider = owner;
        Collider2D projectileCollider = GetComponent<Collider2D>();
        if (_ownerCollider != null && projectileCollider != null)
        {
            Physics2D.IgnoreCollision(_ownerCollider, projectileCollider);
        }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
        {
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
                float force = Mathf.Lerp(explosionForce, 0, distance / explosionRadius);
                rb.AddForce(direction.normalized * force);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}