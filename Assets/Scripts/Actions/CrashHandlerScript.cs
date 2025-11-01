using Tank;

namespace Actions
{
    using UnityEngine;

    public class CrashHandlerScript : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float damageMultiplier;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherTank = collision.collider.GetComponent<TankScript>();
            if (!otherTank) return;

            var damage = damageMultiplier * collision.relativeVelocity.magnitude * rb.mass;
            Debug.Log("Crash! Dealing " + damage + " damage to " + otherTank.name);
            otherTank.ApplyDamage(damage);

            Destroy(this); // remove after first crash, optional
        }
    }

}