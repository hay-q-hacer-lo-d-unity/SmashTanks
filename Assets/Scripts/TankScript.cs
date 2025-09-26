using System.Net.Mime;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TankScript : MonoBehaviour
{
    [Header("References")] public GameObject projectilePrefab; // assign in Inspector
    public Transform firePoint; // empty GameObject at tank’s barrel
    public Transform aimPoint;

    [Header("Shot Settings")] public float speedMultiplier = 5f; // scales velocity by distance
    public float maxSpeed = 20f; // cap velocity if cursor is very far

    [Header("Jump Settings")] [SerializeField]
    public Rigidbody2D rb;
    public float forceMultiplier;
    public float maxForce;
    
    [Header("Tank Settings")] [SerializeField]
    public float tankMass; // custom mass per tank
    public float maxHealth;
    public float health;
    
    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab;   // assign prefab in Inspector
    private HealthBarScript healthBarScript;     // assign in Inspector (World Space Canvas prefab)
    
    [Header("Weapons")] [SerializeField]
    public Missile missileWeapon = new Missile(2f, 500f, 20);

    public int ownerId;
    
    private TurnManagerScript turnManager;
    public void SetOwnerId(int id)
    {
        ownerId = id;
    }

    public CanonOrbitAndAim CanonOrbitAndAim;
    
    public void SetCanMove(bool canMove)
    {
        if (CanonOrbitAndAim != null)
            CanonOrbitAndAim.canMove = canMove;
    }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

[FormerlySerializedAs("TrajectoryDrawerScript")] [Header("Trajectory")]
    public TrajectoryDrawerScript trajectoryDrawerScript;


    public IAction currentMode;
    void Start()
    {
        turnManager = FindObjectOfType<TurnManagerScript>();
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
        health = tankMass;
        maxHealth = tankMass;
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            hb.transform.SetParent(transform, worldPositionStays: true);

            healthBarScript = hb.GetComponent<HealthBarScript>();
            healthBarScript.SetHealth(health, maxHealth);
        }
        aimPoint.SetParent(transform);
    }
    
    void Update()
    {
        // dibujar trayectoria en tiempo real
        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;
        // disparar al hacer click izquierdo
        if (turnManager != null && turnManager.IsPlanningPhase() && !HasRegisteredAction())
        {
            UpdateTrajectory();
            if(!CanonOrbitAndAim.canMove) SetCanMove(true);
            if (Input.GetMouseButtonDown(0) && currentMode != null)
            {
                // calcular hacia dónde apunta el mouse
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

                // registrar acción de disparo
                turnManager.RegisterAction(ownerId, "Shoot", cursorPosition);
                SetCanMove(false);
            }
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
        
        public void ExecuteAction(PlayerAction action)
        {
            currentMode.Execute(action.target);
        }
        
        private bool HasRegisteredAction()
        {
            return turnManager.HasAction(ownerId);
        }

}

