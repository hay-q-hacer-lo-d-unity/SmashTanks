using System;
using Tank;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
        
        int Cooldown { get;  }

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

    #region Stat Scaled Action
    /// <summary>
    /// Base class for actions that scale with a given stat (e.g., damage, intellect).
    /// Handles mapper logic automatically.
    /// </summary>
    public abstract class StatScaledAction : IAction
    {
        protected readonly float StatLevel;
        protected readonly Func<float, float> Mapper;

        protected StatScaledAction(float statLevel, Func<float, float> mapper)
        {
            StatLevel = statLevel;
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Executes the action by first mapping the stat level, then performing the behavior.
        /// </summary>
        public void Execute(Vector3 origin, Vector3 target)
        {
            var scaledValue = Mapper.Invoke(StatLevel);
            Perform(origin, target, scaledValue);
        }

        /// <summary>
        /// Performs the actual action with the mapped stat value (e.g., damage or intellect scaling).
        /// </summary>
        protected abstract void Perform(Vector3 origin, Vector3 target, float scaledValue);

        public abstract string GetName();
        public abstract AimType AimType();
        public abstract int Cooldown { get; }
        public virtual bool LocksCannon() => false;
    }
    #endregion
    
    #region Damage Scaled Action
    /// <summary>
    /// Base class for actions that scale with the tank's damage stat.
    /// </summary>
    public abstract class DamageScaledAction : StatScaledAction
    {
        protected DamageScaledAction(float damageLevel, Func<float, float> damageMapper)
            : base(damageLevel, damageMapper) { }

        // Optionally: you can override this if all physical actions have consistent rules
        public override bool LocksCannon() => false;
    }

    #endregion
    
    #region Missile
    /// <summary>
    /// Launches a missile projectile toward a target point.
    /// </summary>
    public class Missile : DamageScaledAction
    {
        private readonly GameObject _projectilePrefab;
        private readonly float _maxSpeed;
        private readonly Transform _firePoint;
        private readonly Rigidbody2D _tankRb;
        private readonly Collider2D _tankCollider;

        public Missile(
            GameObject projectilePrefab,
            float maxSpeed,
            Transform firePoint,
            Rigidbody2D tankRb,
            Collider2D tankCollider,
            float damageLevel
        ) : base(damageLevel, StatMapper.MapMissileDamage)
        {
            _projectilePrefab = projectilePrefab;
            _maxSpeed = maxSpeed;
            _firePoint = firePoint;
            _tankRb = tankRb;
            _tankCollider = tankCollider;
        }

        protected override void Perform(Vector3 origin, Vector3 target, float damage)
        {
            if (!_projectilePrefab || !_firePoint) return;

            var direction = (target - origin).normalized;
            var initialSpeed = TankPhysicsHelper.CalculateMissileSpeed(_maxSpeed, origin, target);

            var projectile = Object.Instantiate(_projectilePrefab, origin, Quaternion.identity);
            if (!projectile.TryGetComponent<ExplosiveProjectile>(out var proj)) return;

            proj.Initialize(
                _tankCollider,
                initialSpeed,
                SmashTanksConstants.MISSILE_EXPLOSION_RADIUS,
                SmashTanksConstants.MISSILE_EXPLOSION_FORCE,
                damage
            );

            _tankRb.AddForce(-direction * SmashTanksConstants.MISSILE_RECOIL_FORCE, ForceMode2D.Impulse);
        }

        public override string GetName() => "Shoot";
        public override AimType AimType() => Actions.AimType.Parabolic;
        public override int Cooldown => 0;
    }

    #endregion

    #region Bouncy Missile Action
    /// <summary>
    /// Launches a bouncy missile projectile toward a target point.
    /// </summary>
    public class BouncyMissileAction : DamageScaledAction
    {
        private readonly GameObject _projectilePrefab;
        private readonly float _maxSpeed;
        private readonly Transform _firePoint;
        private readonly Rigidbody2D _tankRb;
        private readonly Collider2D _tankCollider;

        public BouncyMissileAction(
            GameObject projectilePrefab,
            float maxSpeed,
            Transform firePoint,
            Rigidbody2D tankRb,
            Collider2D tankCollider,
            float damageLevel
        ) : base(damageLevel, StatMapper.MapBouncyMissileDamage)
        {
            _projectilePrefab = projectilePrefab;
            _maxSpeed = maxSpeed;
            _firePoint = firePoint;
            _tankRb = tankRb;
            _tankCollider = tankCollider;
        }
        
        protected override void Perform(Vector3 origin, Vector3 target, float damage)
        {
            if (!_projectilePrefab || !_firePoint) return;

            var direction = (target - origin).normalized;
            var initialSpeed = TankPhysicsHelper.CalculateMissileSpeed(_maxSpeed, origin, target);

            var projectile = Object.Instantiate(_projectilePrefab, origin, Quaternion.identity);
            if (!projectile.TryGetComponent<ExplosiveProjectile>(out var proj)) return;
            proj.Initialize(
                _tankCollider, 
                initialSpeed, 
                SmashTanksConstants.BOUNCY_MISSILE_EXPLOSION_RADIUS, 
                SmashTanksConstants.BOUNCY_MISSILE_EXPLOSION_FORCE, 
                damage
                );
            _tankRb.AddForce(-direction * SmashTanksConstants.BOUNCY_MISSILE_RECOIL_FORCE, ForceMode2D.Impulse);
        }

        public override string GetName() => "Bouncy";

        public override AimType AimType() => Actions.AimType.Parabolic;
        
        public override int Cooldown => 1;
        public new bool LocksCannon() => false;
    }
    
    #endregion

    #region Jump
    /// <summary>
    /// Makes the tank jump toward a target location.
    /// </summary>
    public class Jump : IAction
    {
        private readonly Transform _aimPoint;
        private readonly Rigidbody2D _rb;
        private readonly float _maxForce;
        int IAction.Cooldown => 0;

        

        /// <summary>
        /// Initializes a new instance of the <see cref="Jump"/> class.
        /// </summary>
        public Jump(float maxForce, Transform aimPoint, Rigidbody2D rb)
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

    #region Crash
    /// <summary>
    /// Makes the tank jump toward a target location and applies collision damage when it impacts another object.
    /// </summary>
    public class Crash : DamageScaledAction
    {
        private readonly Transform _aimPoint;
        private readonly Rigidbody2D _rb;
        private readonly float _maxForce;
        public override int Cooldown => 2;


        /// <summary>
        /// Initializes a new instance of the <see cref="Crash"/> class.
        /// </summary>
        public Crash(
            float maxForce,
            Transform aimPoint,
            Rigidbody2D rb,
            float damageLevel
            ) : base(damageLevel, StatMapper.MapCrashDamageMultiplier)
        {
            _maxForce = maxForce;
            _aimPoint = aimPoint;
            _rb = rb;
        }

        /// <inheritdoc />
        protected override void Perform(Vector3 origin, Vector3 target, float damageMultiplier)
        {
            var force = TankPhysicsHelper.CalculateJumpForce(_maxForce, _aimPoint.position, target);
            _rb.AddForce(force, ForceMode2D.Impulse);

            // Attach a temporary crash handler to process collision-based damage.
            var handler = _rb.gameObject.AddComponent<CrashHandlerScript>();
            handler.rb = _rb;
            handler.damageMultiplier = damageMultiplier;
        }

        /// <inheritdoc />
        public override string GetName() => "Crash";

        /// <inheritdoc />
        public override AimType AimType() => Actions.AimType.Parabolic;
        
        public new bool LocksCannon() => false;
    }
    #endregion

    #region Intellect Scaled Action
    /// <summary>
    /// Base class for actions that use magicka and scale with intellect.
    /// </summary>
    public abstract class IntellectScaledAction : StatScaledAction
    {
        protected readonly TankScript Tank;

        protected IntellectScaledAction(
            TankScript tank, 
            float intellectLevel,
            Func<float, float> intellectMapper
            )
            : base(intellectLevel, intellectMapper)
        {
            Tank = tank;
        }

        /// <summary>
        /// The magicka cost to perform this action.
        /// </summary>
        protected abstract float MagickaCost { get; }
        
        /// <summary>
        /// Executes the magical action, consuming magicka before performing.
        /// </summary>
        public new void Execute(Vector3 origin, Vector3 target)
        {
            if (!Tank) return;

            Tank.SpendMagicka(MagickaCost);
            base.Execute(origin, target);
        }
        public new virtual bool LocksCannon() => false;
    }
    #endregion

    #region Beam
    /// <summary>
    /// Fires a magical energy beam from the tank toward the target.
    /// </summary>
    public class Beam : IntellectScaledAction
    {
        private readonly GameObject _beamPrefab;
        private readonly Transform _firePoint;
        public override int Cooldown => 3;
        protected override float MagickaCost => SmashTanksConstants.BEAM_MAGICKA_COST;

        public Beam(GameObject beamPrefab, Transform firePoint, float intellectLevel, TankScript tank)
            : base(tank, intellectLevel, StatMapper.MapBeamDamage)
        {
            _beamPrefab = beamPrefab;
            _firePoint = firePoint;
        }

        protected override void Perform(Vector3 origin, Vector3 target, float damage)
        {
            if (!_beamPrefab || !_firePoint) return;

            var beam = Object.Instantiate(_beamPrefab, origin, Quaternion.identity);
            if (!beam.TryGetComponent<BeamScript>(out var beamScript)) return;

            beamScript.Initialize(damage, (target - origin).normalized);
        }

        public override string GetName() => "Beam";
        public override AimType AimType() => Actions.AimType.HalfLine;
    }
    #endregion
    
    #region Teleport

    /// <summary>
    /// Teleports the tank to a location near the target.
    /// The higher the intellect, the more accurate the teleport.
    /// </summary>
    public class Teleport : IntellectScaledAction, ICircularAreaAction
    {
        public float Radius { get; }

        protected override float MagickaCost => SmashTanksConstants.TELEPORT_MAGICKA_COST;
        public override int Cooldown => 2;


        public Teleport(TankScript tank, float intellectLevel)
            : base(tank, intellectLevel, StatMapper.MapTeleportRadius)
        {
            Radius = StatMapper.MapTeleportRadius(intellectLevel);
        }

        protected override void Perform(Vector3 origin, Vector3 target, float radius)
        {
            var randomOffset = Random.insideUnitCircle * Radius;
            var teleportDestination = new Vector2(target.x + randomOffset.x, target.y + randomOffset.y);
            Tank.transform.position = teleportDestination;
        }

        public override string GetName() => "Teleport";
        public override AimType AimType() => Actions.AimType.CircularArea;
    }


    #endregion
    
    #region Gale
    /// <summary>
    /// Creates a moving wind force that pushes all rigidbodies it touches.
    /// </summary>
    public class Gale : IntellectScaledAction
    {
        private readonly GameObject _galePrefab;
        private readonly Transform _firePoint;
        private readonly float _force;
        public override int Cooldown => 3;


        protected override float MagickaCost => SmashTanksConstants.GALE_MAGICKA_COST;

        public Gale(GameObject galePrefab, float intellectLevel, Transform firePoint, TankScript tank)
            : base(tank, intellectLevel, StatMapper.MapGaleForce)
        {
            _galePrefab = galePrefab;
            _firePoint = firePoint;
        }

        protected override void Perform(Vector3 origin, Vector3 target, float force)
        {
            if (!_firePoint) return;

            var direction = (target - origin).normalized;
            var gale = Object.Instantiate(_galePrefab, origin, Quaternion.identity);

            if (!gale.TryGetComponent<GaleScript>(out var galeScript)) return;
            galeScript.Initialize(direction, force);
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
