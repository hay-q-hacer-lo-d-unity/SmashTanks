using UnityEngine;

public interface IProjectile
{
    void SetOwner(Collider2D owner);
    void Explode();
    GameObject gameObject { get; }
}
