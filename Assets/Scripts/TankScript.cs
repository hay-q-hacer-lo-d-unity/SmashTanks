using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] public Rigidbody2D rb;
    public float forceMultiplier;
    public float maxForce;
    
    [Header("Tank Settings")]
    [SerializeField] private float tankMass = 1f;  // custom mass per tank


    [FormerlySerializedAs("TrajectoryDrawerScript")] [Header("Trajectory")]
    public TrajectoryDrawerScript trajectoryDrawerScript;


    public IAction currentMode;
    void Start()
    {
        currentMode = new MissileAction(
            projectilePrefab,
            speedMultiplier,
            maxSpeed,
            aimPoint,
            firePoint,
            rb
            );
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.mass = tankMass;  // assign mass at runtime
        aimPoint.SetParent(transform);
    }
    
    void Update()
    {
        // dibujar trayectoria en tiempo real
        UpdateTrajectory();

        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;
        // disparar al hacer click izquierdo
        if (Input.GetMouseButtonDown(0) && currentMode != null)
        {
            currentMode.Execute(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    
    public void SetAction(IAction newAction)
    {
        currentMode = newAction;
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

