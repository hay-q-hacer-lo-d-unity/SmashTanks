using System;
using System.Collections.Generic;
using System.Linq;
using Actions;
using Manager;
using UnityEngine;
using UI;
using UnityEngine.Serialization;

namespace Tank
{
    /// <summary>
    /// Handles all gameplay behavior of a tank, including actions, health, magicka,
    /// trajectory preview, and interaction with the turn-based system.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class TankScript : MonoBehaviour
    {
        #region Serialized Fields

        [FormerlySerializedAs("projectilePrefab")]
        [Header("References")]
        [Tooltip("Prefab of the projectile used for missile-type attacks.")]
        [SerializeField] private GameObject missilePrefab;
        
        [SerializeField] private GameObject bouncyMissilePrefab;

        [Tooltip("Prefab used for the beam attack")]
        [SerializeField] private GameObject beamPrefab;
        
        [Tooltip("Prefab used for the gale spell")]
        [SerializeField] private GameObject galePrefab;

        [Tooltip("Transform point where projectiles and effects are spawned.")]
        [SerializeField] private Transform firePoint;

        [Tooltip("Transform used for aiming direction.")]
        [SerializeField] private Transform aimPoint;

        [Tooltip("Component responsible for drawing the projectile trajectory.")]
        [SerializeField] private TrajectoryDrawerScript trajectoryDrawer;

        [Tooltip("Prefab used to instantiate the tank's health bar UI.")]
        [SerializeField] private GameObject healthBarPrefab;

        [Tooltip("Prefab used to instantiate the tank's magicka (mana) bar UI.")]
        [SerializeField] private GameObject magickaBarPrefab;

        [Tooltip("Component that controls cannon rotation and aiming lock state.")]
        [SerializeField] private CannonOrbitAndAim cannonOrbitAndAim;

        [Header("Stats")]
        [Tooltip("Base stats for this tank, including health, magicka, and damage.")]
        [SerializeField] private TankStats stats = new();

        #endregion

        #region Private Fields

        private TurnManagerScript _turnManager;
        private IAction _currentAction;
        private IAction _confirmedAction;
        private TankHealth _health;
        private TankMagicka _magicka;
        private TankTrajectoryHandler _trajectoryHandler;
        private TankInputHandler _inputHandler;

        private bool _canActThisTurn;
        public bool IsDead { get; private set; }

        #endregion

        #region Public Properties

        /// <summary> Unique identifier of the player who owns this tank. </summary>
        public int OwnerId { get; private set; }

        /// <summary> Rigidbody2D component controlling tank physics. </summary>
        public Rigidbody2D Rb { get; private set; }

        /// <summary> Collider2D component of the tank. </summary>
        public Collider2D Collider { get; private set; }

        /// <summary> Tank stats reference (mass, health, magicka, damage, etc.). </summary>
        public TankStats Stats => stats;

        /// <summary> Firing point transform (projectile spawn position). </summary>
        public Transform FirePoint => firePoint;

        /// <summary> Aiming reference transform (used for trajectory targeting). </summary>
        public Transform AimPoint => aimPoint;

        /// <summary> Prefab for missile-based actions. </summary>
        public GameObject MissilePrefab => missilePrefab;
        
        /// <summary> Prefab for bouncy projectile-based actions. </summary>
        public GameObject BouncyMissilePrefab => bouncyMissilePrefab;

        /// <summary> Prefab for beam-based actions. </summary>
        public GameObject BeamPrefab => beamPrefab;
        
        /// <summary> Prefab for gale-based actions. </summary>
        public GameObject GalePrefab => galePrefab;

        /// <summary> Current magicka value. </summary>
        public float Magicka => _magicka?.GetValue() ?? 0f;
        
        public Dictionary<string, int> currentCooldowns = new();

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            IsDead = false;
            Rb = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            _turnManager = FindAnyObjectByType<TurnManagerScript>();

            _trajectoryHandler = new TankTrajectoryHandler(this, trajectoryDrawer);
            _inputHandler = new TankInputHandler(this, _turnManager);

            // Default to missile action at turn start
            _currentAction = ActionFactory.Create(ActionType.Shoot, this);
        }

        private void Update()
        {
            UpdateBars();

            // Skip input if not this tank's turn or input is disabled
            if (!_canActThisTurn || !_inputHandler.CanAct()) return;

            UpdateTrajectory();
            TryRegisterAction();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the tank with a skillset, applying all stat modifiers,
        /// setting up health and magicka bars, and ensuring base values are ready.
        /// </summary>
        /// <param name="skillset">Skillset defining the tank’s stat bonuses and traits.</param>
        public void Initialize(Skillset skillset)
        {
            if (stats == null)
            {
                Debug.LogWarning("TankStats not assigned; creating new instance.");
                stats = new TankStats();
            }

            Rb ??= GetComponent<Rigidbody2D>();
            stats.ApplySkillset(skillset);
            Rb.mass = stats.mass;

            if (healthBarPrefab == null)
            {
                Debug.LogError($"Tank {OwnerId}: Missing healthBarPrefab reference!");
                return;
            }

            _health = new TankHealth(this, healthBarPrefab, stats.maxHealth);
            _health.SetValue(stats.maxHealth);

            _magicka = new TankMagicka(this, magickaBarPrefab, stats.maxMagicka);
            _magicka.SetValue(stats.maxMagicka);
        }

        #endregion

        #region Turn and Action Logic

        /// <summary> Assigns a player ID to this tank. </summary>
        public void SetOwnerId(int id) => OwnerId = id;

        /// <summary>
        /// Changes the current selected action (e.g., missile, beam, etc.)
        /// and adjusts cannon lock state depending on the action type.
        /// </summary>
        public void SetAction(IAction newAction)
        {
            if (newAction == null) return;

            _currentAction = newAction;

            if (newAction.LocksCannon())
                cannonOrbitAndAim.LockCannonPosition();
            else
                cannonOrbitAndAim.UnlockCannonPosition();
        }

        /// <summary>
        /// Executes a confirmed player action (fired from the TurnManager during the Action phase).
        /// </summary>
        /// <param name="action">Action data including origin, target, and type.</param>
        public void ExecuteAction(PlayerAction action)
        {
            _confirmedAction?.Execute(action.Origin, action.Target);
            currentCooldowns[_currentAction.GetName()] = _currentAction.Cooldown;
        }

        /// <summary>
        /// Enables or disables player control for this tank during the planning phase.
        /// </summary>
        public void SetControlEnabled(bool newEnabled)
        {
            _canActThisTurn = newEnabled;
            if (!newEnabled)
                _trajectoryHandler.Hide();
        }

        /// <summary>
        /// Applies per-turn passive effects like healing, magicka regen,
        /// or stat recalculations for special traits (e.g., Juggernaut).
        /// </summary>
        public void ApplyTurnStartEffects()
        {
            foreach (var key in new List<string>(currentCooldowns.Keys).Where(key => currentCooldowns[key] > 0))
            {
                currentCooldowns[key]--;
            }
            _health?.Heal(stats.mendingRate);
            _magicka?.Regenerate(stats.magickaRegenRate);

            if (stats.juggernaut)
            {
                stats.damage = SkillsUtils.CalculateJuggernautDamage(
                    stats.baseDamage,
                    _health?.TotalDamageReceived ?? 0f,
                    IncreaseType.LinearHybrid
                );
            }
        }

        #endregion

        #region Combat and Resources

        /// <summary> Applies damage to this tank, reducing health accordingly. </summary>
        public void ApplyDamage(float damage) => _health?.ApplyDamage(damage);

        /// <summary> Consumes a portion of the tank’s magicka. </summary>
        public void SpendMagicka(float amount) => _magicka?.Spend(amount);

        #endregion

        #region Death Handling

        /// <summary>
        /// Handles death of the tank: destroys UI bars, marks as dead, and removes the GameObject.
        /// </summary>
        public void Kill()
        {
            _health?.DestroyBar();
            _magicka?.DestroyBar();
            IsDead = true;
            Destroy(gameObject);
        }

        #endregion

        #region Internal Helpers

        /// <summary> Updates health and magicka bars each frame. </summary>
        private void UpdateBars()
        {
            _health?.Update();
            _magicka?.Update();
        }

        /// <summary> Updates the trajectory preview depending on the current action. </summary>
        private void UpdateTrajectory()
        {
            switch (_currentAction.AimType())
            {
                case AimType.HalfLine:
                    _trajectoryHandler.HalfLineTrajectory();
                    break;
                case AimType.Parabolic:
                    _trajectoryHandler.ParabolicTrajectory(_currentAction);
                    break;
                case AimType.CircularArea:
                    _trajectoryHandler.CircularArea(_currentAction as ICircularAreaAction);
                    break;
                case AimType.Point:
                    break;
                case AimType.Segment:
                    _trajectoryHandler.SegmentTrajectory();
                    break;
                case AimType.GaleZone:
                    _trajectoryHandler.GaleTrajectory();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Checks player input and, if an action target is selected, registers the action
        /// with the <see cref="TurnManagerScript"/> for later execution.
        /// </summary>
        private void TryRegisterAction()
        {
            if (_inputHandler.TryGetSkipTurn())
            {
                _confirmedAction = null;
                _turnManager.RegisterAction(OwnerId, "Idle", Vector2.zero, Vector2.zero);
                return;
            }
            if (!_inputHandler.TryGetActionTarget(out var target)) 
                return;

            _confirmedAction = _currentAction;
            _turnManager.RegisterAction(OwnerId, _currentAction.GetName(), firePoint.position, target);
        }


        #endregion
    }
}
