using Actions;
using Manager;
using UnityEngine;

using UI;

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

        [Header("Stats")]
        [SerializeField] private TankStats stats = new();

        private Rigidbody2D _rb;
        private TurnManagerScript _turnManager;
        private IAction _currentAction;
        private IAction _confirmedAction;
        private TankHealth _health;
        private TankTrajectoryHandler _trajectoryHandler;
        private TankInputHandler _inputHandler;

        private bool _canActThisTurn;

        public int OwnerId { get; private set; }
        public Rigidbody2D Rb => _rb;
        public TankStats Stats => stats;
        public Transform FirePoint => firePoint;
        public Transform AimPoint => aimPoint;
        public GameObject ProjectilePrefab => projectilePrefab;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _turnManager = FindObjectOfType<TurnManagerScript>();

            _trajectoryHandler = new TankTrajectoryHandler(this, trajectoryDrawer);
            _inputHandler = new TankInputHandler(this, _turnManager);

            _currentAction = ActionFactory.Create(ActionType.Missile, this);
        }

        private void Update()
        {
            if (!_canActThisTurn || !_inputHandler.CanAct()) return;
            _trajectoryHandler.UpdateTrajectory(_currentAction);

            if (!_inputHandler.TryGetActionTarget(out var target)) return;
            _confirmedAction = _currentAction;
            _turnManager.RegisterAction(OwnerId, _currentAction.GetName(), firePoint.position, target);
        }

        public void Initialize(Skillset skillset)
        {
            if (stats == null)
            {
                Debug.LogWarning("TankStats not assigned; creating new instance.");
                stats = new TankStats();
            }

            if (_rb == null) _rb = GetComponent<Rigidbody2D>();

            stats.ApplySkillset(skillset);

            _rb.mass = stats.mass;

            if (healthBarPrefab == null)
            {
                Debug.LogError($"Tank {OwnerId}: Missing healthBarPrefab reference!");
                return;
            }

            _health = new TankHealth(this, healthBarPrefab, stats.maxHealth);
            _health.SetHealth(stats.maxHealth);
        }


        public void SetOwnerId(int id) => OwnerId = id;

        public void SetAction(IAction newAction)
        {
            if (newAction == null) return;
            // Debug.Log($"Selected {newAction.GetName()} for tank {OwnerId}");
            _currentAction = newAction;

            if (newAction.LocksCannon())
                canonOrbitAndAim.LockCannonPosition();
            else
                canonOrbitAndAim.UnlockCannonPosition();
        }

        public void ExecuteAction(PlayerAction action)
        {
            _confirmedAction?.Execute(action.Origin, action.Target);
        }

        public void ApplyDamage(float damage)
        {
            _health?.ApplyDamage(damage);
        }

        public void SetControlEnabled(bool newEnabled)
        {
            _canActThisTurn = newEnabled;
            if (!newEnabled) _trajectoryHandler.Hide();
            // else Debug.Log($"Tank {OwnerId} has control!");
        }
        
        public void ApplyTurnStartEffects()
        {
            _health?.Heal(stats.mendingRate);

            if (stats.juggernaut) stats.damage = SkillsUtils.CalculateJuggernautDamage(
                stats.baseDamage,
                _health?.TotalDamageReceived ?? 0f,
                IncreaseType.Linear
                );
        }
    }
}
