using System.Net.Mime;
using DefaultNamespace;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class TankScript : NetworkBehaviour
{
    [Header("References")] 
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Transform aimPoint;

    [Header("Shot Settings")] 
    public float speedMultiplier = 5f;
    public float maxSpeed = 20f;
    public ShooterScript shooter;

    [Header("Jump Settings")] 
    [SerializeField] public Rigidbody2D rb;
    public float forceMultiplier;
    public float maxForce;
    
    [Header("Tank Settings")] 
    [SerializeField] public float tankMass;
    public float maxHealth;
    [Networked] public float health { get; set; }

    [Header("Health Bar Settings")]
    public GameObject healthBarPrefab;
    private HealthBarScript healthBarScript;

    [Header("Weapons")] 
    [SerializeField] public Missile missileWeapon = new Missile(2f, 500f, 20);

    [Networked] public int ownerId { get; set; }
    
    private TurnManagerScript turnManager;

    public CanonOrbitAndAim CanonOrbitAndAim;

    [FormerlySerializedAs("TrajectoryDrawerScript")] [Header("Trajectory")]
    public TrajectoryDrawerScript trajectoryDrawerScript;

    public IAction currentMode;

    public override void Spawned()
    {
        turnManager = FindObjectOfType<TurnManagerScript>();

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.mass = tankMass;
        
        if (shooter == null) shooter = GetComponent<ShooterScript>();

        maxHealth = tankMass;
        if (Object.HasStateAuthority)
            health = tankMass; // solo el host inicializa el valor

        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            hb.transform.SetParent(transform, worldPositionStays: true);

            healthBarScript = hb.GetComponent<HealthBarScript>();
            healthBarScript.SetHealth(health, maxHealth);
        }

        if (aimPoint != null)
            aimPoint.SetParent(transform);

        currentMode = new MissileAction(
            shooter,
            speedMultiplier,
            maxSpeed,
            aimPoint,
            firePoint
        );

        // Registrar este tanque con el TurnManager
        if (turnManager != null && Object.HasStateAuthority)
        {
            turnManager.RegisterTank(this);
            Debug.Log($"[Tank] Registrado en TurnManager");
        }

        if (Object.HasInputAuthority)
            Debug.Log($"[Tank {Object.InputAuthority.PlayerId}] Control local activo ✅");
        else
            Debug.Log($"[Tank {Object.InputAuthority.PlayerId}] Control remoto 🌐");
    }

    public override void FixedUpdateNetwork()
    {
        // Actualiza la barra de vida en todos los clientes
        if (healthBarScript != null)
            healthBarScript.SetHealth(health, maxHealth);

        if (!Object.HasInputAuthority)
            return;

        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        if (turnManager != null && turnManager.IsPlanningPhase() && !HasRegisteredAction())
        {
            UpdateTrajectory();

            if (!CanonOrbitAndAim.canMove)
                SetCanMove(true);

            if (Input.GetMouseButtonDown(0) && currentMode != null)
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 cursorPosition = new Vector2(mouseWorld.x, mouseWorld.y);

                Debug.Log($"[Tank {ownerId}] Registrando acción de disparo (RPC)");

                // Llamamos al RPC del TurnManager para registrar la acción en red
                turnManager.RPC_RegisterAction(ownerId, "Shoot", cursorPosition);
                SetCanMove(false);
            }
        }
    }

    public void SetOwnerId(int id)
    {
        ownerId = id;
    }

    public void SetCanMove(bool canMove)
    {
        if (CanonOrbitAndAim != null)
            CanonOrbitAndAim.canMove = canMove;
    }

    public void ApplyDamage(float damage)
    {
        if (!Object.HasStateAuthority)
            return;

        health -= damage;
        if (health <= 0)
        {
            Debug.Log($"[Tank {ownerId}] fue destruido");
            Runner.Despawn(Object);
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
    
    public void SetAction(IAction newAction)
    {
        currentMode = newAction;
    }
        
    public void ExecuteAction(PlayerAction action)
    {
        Debug.Log($"[TankScript] ExecuteAction llamado para Tank {ownerId} | Acción: {action.actionType} | Target: {action.target}");
        
        if (currentMode != null)
        {
            Debug.Log($"[TankScript] currentMode NO es null, ejecutando...");
            currentMode.Execute(action.target);
        }
        else 
        {
            Debug.LogWarning($"[Tank {ownerId}] No hay acción asignada para ejecutar");
        }
    }
        
    private bool HasRegisteredAction()
    {
        return turnManager.HasAction(ownerId);
    }
}
