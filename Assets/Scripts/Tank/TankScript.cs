using Actions;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tank
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankScript : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private GameObject projectilePrefab;

        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private TrajectoryDrawerScript trajectoryDrawer;
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private CanonOrbitAndAim canonOrbitAndAim;

        [Header("Shot Settings")] [SerializeField]
        private float speedMultiplier = 5f;

        [SerializeField] private float maxSpeed = 20f;

        [Header("Jump Settings")] [SerializeField]
        private float forceMultiplier;

        [SerializeField] private float maxForce;

        [Header("Health Settings")] [SerializeField]
        private float tankMass;

        [SerializeField] private float maxHealth;

        [Header("Accuracy Settings")] [SerializeField]
        public float accuracy;

        [Header("Owner Info")] [SerializeField]
        public int ownerId;

        private Rigidbody2D _rb;
        private TurnManagerScript _turnManager;
        private IAction _currentAction;
        private TankHealth _health;
        private bool _canActThisTurn = false; // NEW — control flag for sequential planning

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
            if (aimPoint) aimPoint.SetParent(transform, true);

            _currentAction = ActionFactory.Create(ActionType.Missile, this);
        }

        private void Update()
        {
            if (!_canActThisTurn) return; // only active on your turn
            if (!CanAct()) return;

            UpdateTrajectory();

            if (!Input.GetMouseButtonDown(0)) return;
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _turnManager.RegisterAction(
                ownerId,
                _currentAction.GetName(),
                firePoint.position,
                cursorPos
                );
        }

        private bool CanAct()
        {
            if (EventSystem.current?.IsPointerOverGameObject() == true) return false;
            if (!_turnManager || !_turnManager.IsPlanningPhase()) return false;
            return !_turnManager.HasAction(ownerId);
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
        }

        private Vector2 CalculateMissileVelocity()
        {
            var cursor = GetMouseWorld();
            var dir = (cursor - (Vector2)aimPoint.position).normalized;
            var distance = Vector2.Distance(cursor, firePoint.position);
            var speed = Mathf.Clamp(distance * speedMultiplier, 0, maxSpeed);
            return dir * speed;
        }

        private Vector2 CalculateJumpVelocity()
        {
            var cursor = GetMouseWorld();
            var dir = (cursor - (Vector2)aimPoint.position).normalized;
            var distance = Vector2.Distance(cursor, aimPoint.position);
            var clampedDistance = Mathf.Clamp(distance, 0f, 5f);
            var force = dir * (clampedDistance * forceMultiplier);
            return force / _rb.mass;
        }

        private Vector2 GetMouseWorld()
        {
            var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(mouseWorld.x, mouseWorld.y);
        }

        // === Public API ===

        public void Initialize(Skillset skillset)
        {
            SkillsetMapper mapper = new(skillset);
            tankMass = mapper.Mass;
            maxHealth = mapper.Health;
            forceMultiplier = mapper.ForceMultiplier;
            maxForce = mapper.MaxForce;
            accuracy = mapper.Accuracy;

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
            _currentAction?.Execute(action.Origin, action.Target);
        }

        public void HideTrajectory() => trajectoryDrawer?.ClearParabola();

        private void SetCanMove(bool canMove)
        {
            if (canonOrbitAndAim)
                canonOrbitAndAim.canMove = canMove;
        }

        public void ApplyDamage(float damage)
        {
            _health?.ApplyDamage(damage);
        }

        // === NEW METHODS FOR SEQUENTIAL TURN CONTROL ===

        public void SetControlEnabled(bool newEnabled)
        {
            _canActThisTurn = newEnabled;

            if (!newEnabled)
            {
                HideTrajectory();
            }
            else
            {
                Debug.Log($"Tanque {ownerId} tiene el control ahora!");
            }
        }
    }
}
