using Tank;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;

namespace Actions
{
    /// <summary>
    /// Defines a general action that can be executed by the tank.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Executes the action using a given origin and target position.
        /// </summary>
        /// <param name="origin">The starting position of the action.</param>
        /// <param name="target">The target position of the action.</param>
        void Execute(Vector3 origin, Vector3 target);

        /// <summary>
        /// Returns the name of the action.
        /// </summary>
        string GetName();

        /// <summary>
        /// Indicates how the action aims thus how the trajectory should be drawn.
        /// </summary>
        AimType AimType();

        /// <summary>
        /// Indicates whether this action locks the tank's cannon while executing.
        /// </summary>
        bool LocksCannon();
    }
    
    #region Circular Area Action

    /// <summary>
    /// Action that targets a circular area.
    /// </summary>
    public interface ICircularAreaAction : IAction
    {
        /// <summary>
        /// Gets the radius of the circular area.
        /// </summary>
        float Radius { get; }
    }

    #endregion

    #region Missile Action
    /// <summary>
    /// Launches a missile projectile toward a target point.
    /// </summary>
    public class MissileAction : IAction
    {
        private readonly GameObject _projectilePrefab;
        private readonly float _maxSpeed;
        private readonly Transform _firePoint;
        private readonly float _damage;
        private readonly float _explosionRadius;
        private readonly float _explosionForce;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissileAction"/> class.
        /// </summary>
        public MissileAction(
            GameObject projectilePrefab,
            float maxSpeed,
            Transform firePoint,
            float damage,
            float explosionRadius,
            float explosionForce
            )
        {
            _projectilePrefab = projectilePrefab;
            _maxSpeed = maxSpeed;
            _firePoint = firePoint;
            _damage = damage;
            _explosionRadius = explosionRadius;
            _explosionForce = explosionForce;
        }

        /// <inheritdoc />
        public void Execute(Vector3 origin, Vector3 target)
        {
            if (!_projectilePrefab || !_firePoint) return;

            var direction = (target - origin).normalized;
            var speed = TankPhysicsHelper.CalculateMissileSpeed(_maxSpeed, origin, target);

            var projectile = Object.Instantiate(_projectilePrefab, origin, Quaternion.identity);
            if (projectile.TryGetComponent<Rigidbody2D>(out var rb))
                rb.linearVelocity = speed;

            var tankRb = _firePoint.GetComponentInParent<Rigidbody2D>();
            if (tankRb) tankRb.AddForce(-direction * SmashTanksConstants.MISSILE_RECOIL_FORCE, ForceMode2D.Impulse);

            var tankCollider = _firePoint.GetComponentInParent<Collider2D>();
            if (!tankCollider || !projectile.TryGetComponent<IProjectile>(out var projectileScript)) return;
            projectileScript.SetOwner(tankCollider);
            projectileScript.SetStats(_explosionRadius, _explosionForce, _damage);
        }

        /// <inheritdoc />
        public string GetName() => "Shoot";

        /// <inheritdoc />
        public AimType AimType() => Actions.AimType.Parabolic;

        /// <inheritdoc />
        public bool LocksCannon() => false;
    }
    #endregion

    #region Jump Action
    /// <summary>
    /// Makes the tank jump toward a target location.
    /// </summary>
    public class JumpAction : IAction
    {
        private readonly Transform _aimPoint;
        private readonly Rigidbody2D _rb;
        private readonly float _maxForce;

        /// <summary>
        /// Initializes a new instance of the <see cref="JumpAction"/> class.
        /// </summary>
        public JumpAction(float maxForce, Transform aimPoint, Rigidbody2D rb)
        {
            _maxForce = maxForce;
            _aimPoint = aimPoint;
            _rb = rb;
        }

        /// <inheritdoc />
        public void Execute(Vector3 origin, Vector3 target)
        {
            var force = TankPhysicsHelper.CalculateJumpForce(_maxForce, _aimPoint.position, target);
            _rb.AddForce(force, ForceMode2D.Impulse);
        }

        /// <inheritdoc />
        public string GetName() => "Jump";

        /// <inheritdoc />
        public AimType AimType() => Actions.AimType.Parabolic;

        /// <inheritdoc />
        public bool LocksCannon() => false;
    }
    #endregion

    #region Crash Action
    /// <summary>
    /// Makes the tank jump toward a target location and applies collision damage when it impacts another object.
    /// </summary>
    public class CrashAction : IAction
    {
        private readonly Transform _aimPoint;
        private readonly Rigidbody2D _rb;
        private readonly float _maxForce;
        private readonly float _damageMultiplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrashAction"/> class.
        /// </summary>
        public CrashAction(float maxForce, Transform aimPoint, Rigidbody2D rb, float damageMultiplier)
        {
            _maxForce = maxForce;
            _aimPoint = aimPoint;
            _rb = rb;
            _damageMultiplier = damageMultiplier;
        }

        /// <inheritdoc />
        public void Execute(Vector3 origin, Vector3 target)
        {
            var force = TankPhysicsHelper.CalculateJumpForce(_maxForce, _aimPoint.position, target);
            _rb.AddForce(force, ForceMode2D.Impulse);

            // Attach a temporary crash handler to process collision-based damage.
            var handler = _rb.gameObject.AddComponent<CrashHandlerScript>();
            handler.rb = _rb;
            handler.damageMultiplier = _damageMultiplier;
        }

        /// <inheritdoc />
        public string GetName() => "Crash";

        /// <inheritdoc />
        public AimType AimType() => Actions.AimType.Parabolic;
        
        /// <inheritdoc />
        public bool LocksCannon() => false;
    }
    #endregion

    #region Magic
    /// <summary>
    /// Base class for actions that use magicka and scale with intellect.
    /// </summary>
    public abstract class Magic : IAction
    {
        protected readonly TankScript Tank;
        protected readonly float Intellect;

        protected Magic(TankScript tank, float intellect)
        {
            Tank = tank;
            Intellect = intellect;
        }

        /// <summary>
        /// The magicka cost to perform this action.
        /// </summary>
        protected abstract float MagickaCost { get; }

        /// <summary>
        /// Executes the magical action’s effect.
        /// </summary>
        protected abstract void Cast(Vector3 origin, Vector3 target);

        /// <inheritdoc/>
        public void Execute(Vector3 origin, Vector3 target)
        {
            if (!Tank) return;

            Tank.SpendMagicka(MagickaCost);
            Cast(origin, target);
        }

        public abstract string GetName();
        public abstract AimType AimType();
        public virtual bool LocksCannon() => false;
    }
    #endregion

    #region Beam Action
    /// <summary>
    /// Fires a magical energy beam from the tank toward the target.
    /// </summary>
    public class BeamAction : Magic
    {
        private readonly GameObject _beamPrefab;
        private readonly Transform _firePoint;
        private readonly float _damage;

        protected override float MagickaCost => SmashTanksConstants.BEAM_MAGICKA_COST;

        public BeamAction(GameObject beamPrefab, Transform firePoint, float intellect, TankScript tank)
            : base(tank, intellect)
        {
            _beamPrefab = beamPrefab;
            _firePoint = firePoint;
            _damage = intellect * SmashTanksConstants.BEAM_DAMAGE_PER_INTELLECT;
        }

        protected override void Cast(Vector3 origin, Vector3 target)
        {
            if (!_beamPrefab || !_firePoint) return;

            var beam = Object.Instantiate(_beamPrefab, origin, Quaternion.identity);
            if (beam.TryGetComponent<BeamScript>(out var beamScript))
                beamScript.SetStats(_damage);

            beam.transform.up = (target - origin).normalized;
        }

        public override string GetName() => "Beam";
        public override AimType AimType() => Actions.AimType.HalfLine;
    }
    #endregion
    
    #region Teleport Action

    /// <summary>
    /// Teleports the tank to a location near the target.
    /// The higher the intellect, the more accurate the teleport.
    /// </summary>
    public class TeleportAction : Magic, ICircularAreaAction
    {
        public float Radius { get; }

        protected override float MagickaCost => SmashTanksConstants.TELEPORT_MAGICKA_COST;

        public TeleportAction(TankScript tank, float intellect)
            : base(tank, intellect)
        {
            const float minRadius = 2f;
            const float maxRadius = 20f;
            const float decayRate = (maxRadius - minRadius) / (SmashTanksConstants.STATPOINTS + 1);
            Radius = Mathf.Clamp(maxRadius - intellect * decayRate, minRadius, maxRadius);
        }

        protected override void Cast(Vector3 origin, Vector3 target)
        {
            var randomOffset = Random.insideUnitCircle * Radius;
            var teleportDestination = new Vector2(target.x + randomOffset.x, target.y + randomOffset.y);
            Tank.transform.position = teleportDestination;
        }

        public override string GetName() => "Teleport";
        public override AimType AimType() => Actions.AimType.CircularArea;
    }


    #endregion
    
    #region Gale Action
    /// <summary>
    /// Creates a moving wind force that pushes all rigidbodies it touches.
    /// </summary>
    public class GaleAction : Magic
    {
        private readonly GameObject _galePrefab;
        private readonly Transform _firePoint;
        private readonly float _force = SmashTanksConstants.GALE_BASE_FORCE;

        protected override float MagickaCost => SmashTanksConstants.GALE_MAGICKA_COST;

        public GaleAction(GameObject galePrefab, float intellect, Transform firePoint, TankScript tank)
            : base(tank, intellect)
        {
            _galePrefab = galePrefab;
            _firePoint = firePoint;
            _force += intellect * SmashTanksConstants.GALE_FORCE_MULTIPLIER_PER_INTELLECT;
        }

        protected override void Cast(Vector3 origin, Vector3 target)
        {
            if (!_firePoint) return;

            var direction = (target - origin).normalized;
            var gale = Object.Instantiate(_galePrefab, origin, Quaternion.identity);

            if (!gale.TryGetComponent<GaleScript>(out var galeScript)) return;
            var ownerCollider = _firePoint.GetComponentInParent<Collider2D>();
            galeScript.Initialize(direction, _force, ownerCollider);
        }

        public override string GetName() => "Gale";
        public override AimType AimType() => Actions.AimType.GaleZone;
    }
    #endregion

    
    
    public enum AimType
    {
        Segment,
        HalfLine,
        Parabolic,
        Point,
        CircularArea,
        GaleZone
    }
}
