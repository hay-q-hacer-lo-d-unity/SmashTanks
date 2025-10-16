using UnityEngine;

public interface IAction
{
    void Execute(Vector3 target);
}

public class MissileAction : IAction
{
    private ShooterScript shooter;
    private float speedMultiplier;
    private float maxSpeed;
    private Transform aimPoint;
    private Transform firePoint;

    public MissileAction(
        ShooterScript shooter,
        float speedMultiplier,
        float maxSpeed,
        Transform aimPoint,
        Transform firePoint
    )
    {
        this.shooter = shooter;
        this.speedMultiplier = speedMultiplier;
        this.maxSpeed = maxSpeed;
        this.aimPoint = aimPoint;
        this.firePoint = firePoint;
    }

    public void Execute(Vector3 target)
    {
        Debug.Log($"[MissileAction] Execute llamado | Target: {target}");
        
        if (shooter == null || firePoint == null)
        {
            Debug.LogWarning($"[MissileAction] Falta ShooterScript ({shooter != null}) o firePoint ({firePoint != null}) asignado");
            return;
        }

        Vector2 cursorPosition = new Vector2(target.x, target.y);
        Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;

        Debug.Log($"[MissileAction] Disparando misil | Dir: {dir} | Shooter: {shooter.name}");
        // Mandamos solo la dirección, Shooter se encarga del resto
        shooter.FireProjectile(dir);
    }
}

public class JumpAction : IAction
{
    private Transform aimPoint;
    private Rigidbody2D rb;
    private float forceMultiplier;

    public JumpAction(
        float forceMultiplier,
        Transform aimPoint,
        Rigidbody2D rb
    )
    {
        this.forceMultiplier = forceMultiplier;
        this.aimPoint = aimPoint;
        this.rb = rb;
    }

    public void Execute(Vector3 target)
    {
        Vector2 cursorPosition = new Vector2(target.x, target.y);
        Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;
        float distance = Vector2.Distance(cursorPosition, aimPoint.position);
        float clampedDistance = Mathf.Clamp(distance, 0f, 5f);

        Vector2 force = dir * (clampedDistance * forceMultiplier);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
