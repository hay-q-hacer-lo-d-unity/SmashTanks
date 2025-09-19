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

    [Header("Jump Settings")] 
    [SerializeField] private Rigidbody2D rb;
    public float forceMultiplier;
    public float maxForce;
    
    [Header("Tank Settings")]
    [SerializeField] private float tankMass = 1f;  // custom mass per tank


    [FormerlySerializedAs("TrajectoryDrawerScript")] [Header("Trajectory")]
    public TrajectoryDrawerScript trajectoryDrawerScript;
    
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.mass = tankMass;  // assign mass at runtime
        aimPoint.SetParent(transform);
    }
    
    void Update()
    {
        // dibujar trayectoria en tiempo real
        UpdateTrajectory();

        // disparar al hacer click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetMouseButton(1))
        {
            Jump();
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
            if (rb)
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

    void Jump()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

        // Direction from aimPoint to cursor
        Vector2 dir = (cursorPosition - (Vector2)aimPoint.position).normalized;

        // Distance between aimPoint and cursor
        float distance = Vector2.Distance(cursorPosition, aimPoint.position);

        // Clamp distance so force doesn’t grow infinitely
        float clampedDistance = Mathf.Clamp(distance, 0f, 10f);

        // Compute force
        Vector2 force = dir * (clampedDistance * forceMultiplier);

        // Apply force -> Unity takes Rigidbody2D.mass into account
        rb.AddForce(force, ForceMode2D.Impulse);
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

