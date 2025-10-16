using Fusion;
using UnityEngine;

public class ShooterScript : NetworkBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;

    private NetworkRunner runner;

    public override void Spawned() 
    {
        runner = Runner;
    }

    /// <summary>
    /// Dispara un proyectil hacia una dirección objetivo.
    /// Este método lo llama el TankScript.ExecuteAction().
    /// </summary>
    public void FireProjectile(Vector2 targetDirection)
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("[ShooterScript] Falta prefab o firePoint asignado");
            return;
        }

        Vector3 spawnPos = firePoint.position;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, targetDirection);

        Debug.Log($"[ShooterScript] FireProjectile() | IsServer={runner.IsServer} | HasStateAuth={Object.HasStateAuthority}");
        var projectileObj = runner.Spawn(projectilePrefab, spawnPos, rot);

        if (projectileObj.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = targetDirection.normalized * projectileSpeed;
            Debug.Log($"🚀 [ShooterScript] Proyectil disparado con vel {rb.linearVelocity}");
        }
    }
}