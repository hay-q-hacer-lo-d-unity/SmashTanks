using UnityEngine;

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DamageOnCrashScript : MonoBehaviour
{
    
    [Header("Damage settings")]
    [Tooltip("Multiplier for final damage value (tweak to taste).")]
    [SerializeField] private float damageScale = 0.1f;

    [Tooltip("Minimum impact speed (along collision normal) to consider as damaging collision.")]
    [SerializeField] private float minImpactSpeed = 1f;
    

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.collider.attachedRigidbody;
        if (!otherRb) return;

        ContactPoint2D contact = collision.GetContact(0);
        
        float relativeAlongNormal = Vector2.Dot(collision.relativeVelocity, contact.normal);
        float impactSpeed = Mathf.Abs(relativeAlongNormal); // positive scalar

        if (impactSpeed < minImpactSpeed) return; // too soft, ignore

        float m1 = rb.mass;

        float energy = 0.5f * m1 * impactSpeed;

        float damage = energy * damageScale;


        var otherTank = collision.collider.GetComponent<TankScript>();
        if (otherTank != null)
        {
            otherTank.ApplyDamage(damage);
        }
    }
}
