using UnityEngine;

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
        // convert mouse to world position
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

        // direction and distance
        Vector2 dir = (cursorPosition - (Vector2)firePoint.position).normalized;
        float distance = Vector2.Distance(cursorPosition, firePoint.position);

        // compute initial speed
        float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);

        // spawn projectile
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // apply velocity
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * speed;
        }
    }
}

