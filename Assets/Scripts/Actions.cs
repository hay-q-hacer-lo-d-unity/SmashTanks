using UnityEngine;

public interface IAction
{
    void Execute(Transform aimPoint, Transform firePoint, Rigidbody2D rb,
        GameObject projectilePrefab, float speedMultiplier,
        float maxSpeed, float forceMultiplier);
}

public class MissileAction : IAction
{
    
    public void Execute(Transform aimPoint, Transform firePoint, Rigidbody2D rb,
        GameObject projectilePrefab, float speedMultiplier,
        float maxSpeed, float forceMultiplier)
    {
        if (!projectilePrefab || !firePoint) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

        Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;
        float distance = Vector2.Distance(cursorPosition, firePoint.position);

        float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);

        GameObject proj = Object.Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb_ = proj.GetComponent<Rigidbody2D>();
        if (rb_ != null)
        {
            rb_.linearVelocity = dir * speed;
        }

        Collider2D tankCollider = firePoint.GetComponentInParent<Collider2D>();
        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript)
        {
            projectileScript.SetOwner(tankCollider);
        }
    }
}

public class JumpAction : IAction
{
    public void Execute(Transform aimPoint, Transform firePoint, Rigidbody2D rb,
        GameObject projectilePrefab, float speedMultiplier,
        float maxSpeed, float forceMultiplier)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

        Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;
        float distance = Vector2.Distance(cursorPosition, aimPoint.position);
        float clampedDistance = Mathf.Clamp(distance, 0f, 5f);

        Vector2 force = dir * (clampedDistance * forceMultiplier);

        rb.AddForce(force, ForceMode2D.Impulse);
    }
}