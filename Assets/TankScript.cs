using DefaultNamespace;
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

    [Header("Trajectory")]
    public TrajectoryDrawerScript TrajectoryDrawerScript;
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
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 initialVelocity = CalculateInitialVelocity();

        // crear proyectil
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // aplicar velocidad
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = initialVelocity; // en 2D se usa "velocity"
        }
    }
    Vector2 CalculateInitialVelocity()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

        Vector2 dir = (cursorPosition - (Vector2)firePoint.position).normalized;
        float distance = Vector2.Distance(cursorPosition, firePoint.position);
        float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);

        return dir * speed;
    }

    void UpdateTrajectory()
    {
        if (TrajectoryDrawerScript == null || firePoint == null) return;

        Vector2 initialVelocity = CalculateInitialVelocity();
        TrajectoryDrawerScript.DrawParabola(firePoint.position, initialVelocity);
    }
}

