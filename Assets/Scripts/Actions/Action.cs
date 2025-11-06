using System;
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
        /// Indicates whether this action has falloff effects over distance.
        /// </summary>
        bool HasFalloff();

        /// <summary>
        /// Indicates whether this action locks the tank's cannon while executing.
        /// </summary>
        bool LocksCannon();
    }

    #region Missile Action
    /// <summary>
    /// Launches a missile projectile toward a target point.
    /// </summary>
    public class MissileAction : IAction
    {
        private readonly GameObject _projectilePrefab;
        private readonly float _speedMultiplier;
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
            float speedMultiplier,
            float maxSpeed,
            Transform firePoint,
            float damage,
            float explosionRadius,
            float explosionForce)
        {
            _projectilePrefab = projectilePrefab;
            _speedMultiplier = speedMultiplier;
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
            var distance = Vector2.Distance(target, origin);
            var speed = Mathf.Clamp(distance * _speedMultiplier, 0, _maxSpeed);

            var projectile = Object.Instantiate(_projectilePrefab, origin, Quaternion.identity);
            if (projectile.TryGetComponent<Rigidbody2D>(out var rb))
                rb.linearVelocity = direction * speed;

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
        public bool HasFalloff() => true;

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
        public bool HasFalloff() => true;

        /// <inheritdoc />
        public bool LocksCannon() => false;
    }
    #endregion

    #region Crash Action
    /// <summary>
    /// Propels the tank forward and applies collision damage when it impacts another object.
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
        public bool HasFalloff() => true;

        /// <inheritdoc />
        public bool LocksCannon() => false;
    }
    #endregion

    #region Beam Action
    /// <summary>
    /// Fires a magical energy beam from the tank toward the target.
    /// </summary>
    public class BeamAction : IAction
    {
        private readonly GameObject _beamPrefab;
        private readonly Transform _firePoint;
        private readonly float _damage;
        private readonly TankScript _tank;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeamAction"/> class.
        /// </summary>
        /// <param name="beamPrefab">Prefab of the beam projectile.</param>
        /// <param name="firePoint">Transform from which the beam is fired.</param>
        /// <param name="intellect">Intellect stat used to calculate beam damage.</param>
        /// <param name="tank">The tank executing the beam action.</param>
        public BeamAction(GameObject beamPrefab, Transform firePoint, float intellect, TankScript tank)
        {
            _beamPrefab = beamPrefab;
            _firePoint = firePoint;
            _damage = intellect * SmashTanksConstants.BEAM_DAMAGE_PER_INTELLECT;
            _tank = tank;
        }

        /// <inheritdoc />
        public void Execute(Vector3 origin, Vector3 target)
        {
            if (!_beamPrefab || !_firePoint) return;

            var beam = Object.Instantiate(_beamPrefab, origin, Quaternion.identity);
            _tank.SpendMagicka(SmashTanksConstants.BEAM_MAGICKA_COST);

            if (!beam.TryGetComponent<BeamScript>(out var beamScript)) return;
            beamScript.SetStats(_damage);
            beam.transform.up = (target - origin).normalized;
        }

        /// <inheritdoc />
        public string GetName() => "Beam";

        /// <inheritdoc />
        public bool HasFalloff() => false;

        /// <inheritdoc />
        public bool LocksCannon() => false;
    }
    #endregion
}
