using UnityEngine;

namespace Weapons
{
    public interface IProjectile
    {
        void SetOwner(Collider2D owner);
        void SetStats(float explosionRadius, float explosionForce, float damage);
        void Explode();
        GameObject gameObject { get; }
    }
}
