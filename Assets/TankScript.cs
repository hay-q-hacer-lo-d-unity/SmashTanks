using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class TankScript : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;  // assign in Inspector
    public Transform firePoint;          // empty GameObject at tank’s barrel
    public Transform aimPoint;

    [Header("Shot Settings")]
    public float speedMultiplier = 5f;   // scales velocity by distance
    public float maxSpeed = 20f;         // cap velocity if cursor is very far

    [FormerlySerializedAs("TrajectoryDrawerScript")] [Header("Trajectory")]
    public TrajectoryDrawerScript trajectoryDrawerScript;
    void Update()
    {
        // dibujar trayectoria en tiempo real
        UpdateTrajectory();

        // disparar al hacer click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
        {
            if (!projectilePrefab || !firePoint) return;

            Vector2 initialVelocity = CalculateInitialVelocity();

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

            Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;
            float distance = Vector2.Distance(cursorPosition, firePoint.position);

            float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // aplicar velocidad
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * speed; // en 2D se usa "velocity"
            }
            // Pass the tank collider to the projectile so it won't collide with it
                    Collider2D tankCollider = GetComponent<Collider2D>();
                    Projectile projectileScript = proj.GetComponent<Projectile>();
                    if (projectileScript)
                    {
                        projectileScript.SetOwner(tankCollider);
                    }
        }

        Vector2 CalculateInitialVelocity()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

            Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;
            float distance = Vector2.Distance(cursorPosition, firePoint.position);
            float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);

            return dir * speed;
        }

        void UpdateTrajectory()
            {
                if (!trajectoryDrawerScript || !firePoint) return;
                Vector2 initialVelocity = CalculateInitialVelocity();
                trajectoryDrawerScript.DrawParabola(firePoint.position, initialVelocity);
            }
}

