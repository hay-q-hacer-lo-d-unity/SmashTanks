using UnityEngine;

public class TankScript : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;  // assign in Inspector
    public Transform firePoint;          // empty GameObject at tank’s barrel

    [Header("Shot Settings")]
    public float speedMultiplier = 5f;   // scales velocity by distance
    public float maxSpeed = 20f;         // cap velocity if cursor is very far

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click to shoot
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

        Vector2 dir = (cursorPosition - (Vector2)firePoint.position).normalized;
        float distance = Vector2.Distance(cursorPosition, firePoint.position);

        float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dir * speed;
        }

        // Pass the tank collider to the projectile so it won't collide with it
        Collider2D tankCollider = GetComponent<Collider2D>();
        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetOwner(tankCollider);
        }
    }

}

