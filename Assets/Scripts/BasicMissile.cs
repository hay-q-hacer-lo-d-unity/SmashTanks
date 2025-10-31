using System.Collections;
using Tank;
using UnityEngine;

public class BasicMissile : MonoBehaviour, IProjectile
{
    private Rigidbody2D _thisRb;

    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 1000f;
    public float damage = 2f;
    public GameObject explosionEffectPrefab;

    private Collider2D _ownerCollider;
    private Coroutine _reenableCollisionRoutine;

    public void SetOwner(Collider2D owner)
    {
        _ownerCollider = owner;
        var projectileCollider = GetComponent<Collider2D>();

        if (!_ownerCollider || !projectileCollider) return;

        // Get all colliders on the owner's hierarchy
        var ownerColliders = _ownerCollider.GetComponentsInChildren<Collider2D>();

        // Ignore collisions with all of them
        foreach (var col in ownerColliders)
        {
            Debug.Log($"Ignoring collision between {col.name} and projectile.");
            Physics2D.IgnoreCollision(col, projectileCollider, true);
        }

        // Stop any existing coroutine
        if (_reenableCollisionRoutine != null) StopCoroutine(_reenableCollisionRoutine);

        _reenableCollisionRoutine = StartCoroutine(
            ReenableCollisionAfterDelay(ownerColliders, projectileCollider, 0.25f)
        );
    }

    private IEnumerator ReenableCollisionAfterDelay(Collider2D[] ownerColliders, Collider2D projectile, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (projectile)
        {
            foreach (var col in ownerColliders)
            {
                if (col != null)
                    Physics2D.IgnoreCollision(col, projectile, false);
            }
        }

        _reenableCollisionRoutine = null;
    }

    private void OnDestroy()
    {
        // If destroyed early, stop coroutine to prevent warnings/errors
        if (_reenableCollisionRoutine != null) StopCoroutine(_reenableCollisionRoutine);
    }
    
    public void SetStats(float newExplosionRadius, float newExplosionForce, float newDamage)
    {
        Debug.Log($"newExplosionRadius: {newExplosionRadius}\nnewExplosionForce: {newExplosionForce}\nnewDamage: {newDamage}");
        explosionRadius = newExplosionRadius;
        explosionForce = newExplosionForce;
        damage = newDamage;
    }

    private void Start() => _thisRb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        if (!(_thisRb.linearVelocity.sqrMagnitude > 0.01f)) return;
        var angle = Mathf.Atan2(_thisRb.linearVelocity.y, _thisRb.linearVelocity.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnCollisionEnter2D()
    {
        Explode();
    }

    public void Explode()
    {
        if (explosionEffectPrefab != null) Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var col in colliders)
        {
            var rb = col.GetComponent<Rigidbody2D>();
            if (rb == null || rb == _thisRb) continue;
            var direction = rb.position - (Vector2)transform.position;
            var distance = direction.magnitude;
            var normalizedDistance = Mathf.Clamp01(distance / explosionRadius);
            var attenuation = 1f - normalizedDistance * normalizedDistance;

            var force = explosionForce * attenuation;
            var damageAmount = damage * attenuation;
            var tank = col.GetComponent<TankScript>();
            if (tank != null) tank.ApplyDamage(damageAmount);
            rb.AddForce(direction.normalized * force);
        }
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}