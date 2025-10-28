using System;
using Tank;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IAction
{
    void Execute(Vector3 target);

    String GetName();

    bool LocksCannon();
}

public class MissileAction : IAction
{
    private GameObject projectilePrefab;
    private float speedMultiplier;
    private float maxSpeed;
    private Transform aimPoint;
    private Transform firePoint;
    private Rigidbody2D rb;
    public MissileAction(
        GameObject projectilePrefab,
        float speedMultiplier,
        float maxSpeed,
        Transform aimPoint,
        Transform firePoint,
        Rigidbody2D rb
        )
    {
        this.projectilePrefab = projectilePrefab;
        this.speedMultiplier = speedMultiplier;
        this.maxSpeed = maxSpeed;
        this.aimPoint = aimPoint;
        this.firePoint = firePoint;
        this.rb = rb;
    }
    
    public void Execute(Vector3 target)
    {
        if (!projectilePrefab || !firePoint) return;

        Vector2 cursorPosition = new Vector2(target.x, target.y);

        Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;
        float distance = Vector2.Distance(cursorPosition, firePoint.position);

        float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);

        GameObject proj = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb_ = proj.GetComponent<Rigidbody2D>();
        if (rb_)
        {
            rb_.linearVelocity = dir * speed;
        }

        Collider2D tankCollider = firePoint.GetComponentInParent<Collider2D>();
        IProjectile projectileScript = proj.GetComponent<IProjectile>();
        if (projectileScript != null)
        {
            projectileScript.SetOwner(tankCollider);
        }
    }

    public string GetName()
    {
        return "Shoot";
    }

    public bool LocksCannon()
    {
        return false;
    }
}

public class JumpAction : IAction
{
    private Transform aimPoint;
    private Rigidbody2D rb;
    float forceMultiplier;
    public JumpAction(
        float forceMultiplier,
        Transform  aimPoint,
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

    public string GetName()
    {
        return "Jump";
    }

    public bool LocksCannon()
    {
        return true;
    }
}

public class CrashAction : IAction
{
    private Transform aimPoint;
    private Rigidbody2D rb;
    float forceMultiplier;
    private float damageMultiplier;
    public CrashAction(
        float forceMultiplier,
        Transform  aimPoint,
        Rigidbody2D rb,
        float damageMultiplier
    )
    {
        this.forceMultiplier = forceMultiplier;
        this.aimPoint = aimPoint;
        this.rb = rb;
        this.damageMultiplier = damageMultiplier;
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TankScript otherTank = collision.collider.GetComponent<TankScript>();
        if (otherTank)
        {
            float damage = damageMultiplier * collision.relativeVelocity.magnitude * rb.mass;
            otherTank.ApplyDamage(damage);
        }
    }

    public string GetName()
    {
        return "Crash";
    }

    public bool LocksCannon()
    {
        return true;
    }
}