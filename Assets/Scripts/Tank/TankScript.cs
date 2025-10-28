using Actions;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tank
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankScript : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private TrajectoryDrawerScript trajectoryDrawer;
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private CanonOrbitAndAim canonOrbitAndAim;

        [Header("Shot Settings")]
        [SerializeField] private float speedMultiplier = 5f;
        [SerializeField] private float maxSpeed = 20f;

        [Header("Jump Settings")]
        [SerializeField] private float forceMultiplier;
        [SerializeField] private float maxForce;

        [Header("Tank Settings")]
        [SerializeField] private float tankMass;
        [SerializeField] private float maxHealth;
        [SerializeField] public float accuracy;

        [Header("Owner Info")]
        [SerializeField]
        public int ownerId;

        private Rigidbody2D _rb;
        private TurnManagerScript _turnManager;
        private IAction _currentAction;
        private TankHealth _health;

        // === Properties for external access ===
        public int OwnerId => ownerId;
        public Rigidbody2D Rb => _rb;
        public Transform AimPoint => aimPoint;
        public Transform FirePoint => firePoint;
        public GameObject ProjectilePrefab => projectilePrefab;
        public float SpeedMultiplier => speedMultiplier;
        public float MaxSpeed => maxSpeed;
        public float ForceMultiplier => forceMultiplier;
        public float Accuracy => accuracy;
        public IAction CurrentAction => _currentAction;

        private void Start()
        {
            if (aimPoint)
                aimPoint.SetParent(transform, true);

            // Default action
            _currentAction = ActionFactory.Create(ActionType.Missile, this);
        }

        private void Update()
        {
            if (!CanAct()) return;

            UpdateTrajectory();

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _turnManager.RegisterAction(ownerId, _currentAction.GetName(), cursorPos);
                SetCanMove(false);
            }
        }

        private bool CanAct()
        {
            if (EventSystem.current?.IsPointerOverGameObject() == true) return false;
            if (!_turnManager || !_turnManager.IsPlanningPhase()) return false;
            if (_turnManager.HasAction(ownerId)) return false;
            return true;
        }

        private void UpdateTrajectory()
        {
            if (!trajectoryDrawer || !firePoint) return;

            Vector2 initialVelocity = _currentAction switch
            {
                MissileAction => CalculateMissileVelocity(),
                JumpAction => CalculateJumpVelocity(),
                CrashAction => CalculateJumpVelocity(),
                _ => Vector2.zero
            };

            Vector3 origin = _currentAction switch
            {
                MissileAction => firePoint.position,
                JumpAction => aimPoint.position,
                CrashAction => aimPoint.position,
                _ => aimPoint.position
            };

            trajectoryDrawer.DrawParabola(origin, initialVelocity, accuracy);
            if (!canonOrbitAndAim.canMove)
                SetCanMove(true);
        }

        private Vector2 CalculateMissileVelocity()
        {
            Vector2 cursor = GetMouseWorld();
            Vector2 dir = (cursor - (Vector2)aimPoint.position).normalized;
            float distance = Vector2.Distance(cursor, firePoint.position);
            float speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);
            return dir * speed;
        }

        private Vector2 CalculateJumpVelocity()
        {
            Vector2 cursor = GetMouseWorld();
            Vector2 dir = (cursor - (Vector2)aimPoint.position).normalized;
            float distance = Vector2.Distance(cursor, aimPoint.position);
            float clampedDistance = Mathf.Clamp(distance, 0f, 5f);
            Vector2 force = dir * (clampedDistance * forceMultiplier);
            return force / _rb.mass;
        }

        private Vector2 GetMouseWorld()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(mouseWorld.x, mouseWorld.y);
        }

        // === Public API ===

        public void Initialize(Skillset skillset)
        {
            SkillsetMapper mapper = new(skillset);
            tankMass = mapper.mass;
            maxHealth = mapper.health;
            forceMultiplier = mapper.forceMultiplier;
            maxForce = mapper.maxForce;
            accuracy = mapper.accuracy;
            
            _rb = GetComponent<Rigidbody2D>();
            _turnManager = FindObjectOfType<TurnManagerScript>();

            _rb.mass = tankMass;

            _health = new TankHealth(this, healthBarPrefab, maxHealth);
            _health.SetHealth(maxHealth);
        }

        public void SetOwnerId(int id) => ownerId = id;

        public void SetAction(IAction newAction)
        {
            if (newAction == null) return;
            Debug.Log($"Selected {newAction.GetName()} for tank {ownerId}");
            _currentAction = newAction;

            if (newAction.LocksCannon())
                canonOrbitAndAim.LockCannonPosition();
            else
                canonOrbitAndAim.UnlockCannonPosition();
        }

        public void ExecuteAction(PlayerAction action)
        {
            _currentAction?.Execute(action.Target);
        }

        public void HideTrajectory() => trajectoryDrawer?.ClearParabola();

        public void SetCanMove(bool canMove)
        {
            if (canonOrbitAndAim)
                canonOrbitAndAim.canMove = canMove;
        }

        // ✅ Ahora delega el daño al sistema TankHealth
        public void ApplyDamage(float damage)
        {
            _health?.ApplyDamage(damage);
        }
    }
}
