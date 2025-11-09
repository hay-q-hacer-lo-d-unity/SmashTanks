using Tank;
using UnityEngine;

namespace Weapons
{
    public class BouncyMissile : ExplosiveProjectile
    {
        [SerializeField] private float fuseTime = SmashTanksConstants.BOUNCY_MISSILE_FUSE_TIME;

        private void Start() => Invoke(nameof(Explode), fuseTime);

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.TryGetComponent<TankScript>(out _))
                Explode();
        }
    }
}