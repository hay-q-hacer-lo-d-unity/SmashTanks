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
        
        int Cooldown { get; }

        /// <summary>
        /// Indicates whether this action locks the tank's cannon while executing.
        /// </summary>
        bool LocksCannon();
    }
    
    public interface IActionWithSound
    {
        AudioClip ExecuteSound { get; }
    }

    
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
    

    /// <summary>
    /// Base class for actions that scale with a given stat (e.g., damage, intellect).
    /// Handles mapper logic automatically.
    /// </summary>
    public abstract class StatScaledAction : IAction
    {
        protected readonly ActionContext Ctx;
        private readonly Func<float, float> _mapper;

        protected StatScaledAction(ActionContext ctx, float statLevel, Func<float, float> mapper)
        {
            Ctx = ctx;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            StatLevel = statLevel;
        }

        protected float StatLevel { get; }

        public void Execute(Vector3 origin, Vector3 target)
        {
            var scaledValue = _mapper(StatLevel);
            Perform(origin, target, scaledValue);
        }

        protected abstract void Perform(Vector3 origin, Vector3 target, float scaledValue);

        public abstract string GetName();
        public abstract AimType AimType();
        public abstract int Cooldown { get; }
        public virtual bool LocksCannon() => false;
    }

    
    
    /// <summary>
    /// Base class for actions that scale with the tank's damage stat.
    /// </summary>
    public abstract class DamageScaledAction : StatScaledAction
    {
        protected DamageScaledAction(ActionContext ctx, float damageLevel, Func<float, float> damageMapper)
            : base(ctx, damageLevel, damageMapper) { }

        public override bool LocksCannon() => false;
    } 
    
    
    /// <summary>
    /// Launches a missile projectile toward a target point.
    /// </summary>
    public class Missile : DamageScaledAction, IActionWithSound
    {
        public AudioClip ExecuteSound => Ctx.ExecuteSound;

        public Missile(ActionContext ctx)
            : base(ctx, ctx.Stats.damage, StatMapper.MapMissileDamage) { }

        protected override void Perform(Vector3 origin, Vector3 target, float damage)
        {
            var direction = (target - origin).normalized;
            var speed = TankPhysicsHelper.CalculateProjectileSpeed(
                Ctx.Stats.missileMaxSpeed, origin, target
            );

            var projectile = Object.Instantiate(
                Ctx.MissilePrefab, origin, Quaternion.identity
            );

            if (!projectile.TryGetComponent(out ExplosiveProjectile proj)) return;

            proj.Initialize(
                Ctx.Collider,
                speed,
                SmashTanksConstants.Missile.ExplosionRadius,
                SmashTanksConstants.Missile.ExplosionForce,
                damage
            );

            Ctx.Rb.AddForce(
                -direction * SmashTanksConstants.Missile.RecoilForce,
                ForceMode2D.Impulse
            );
        }

        public override string GetName() => "Shoot";
        public override AimType AimType() => Actions.AimType.Parabolic;
        public override int Cooldown => SmashTanksConstants.Missile.Cooldown;
    }
    
    
    /// <summary>
    /// Launches a bouncy missile projectile toward a target point.
    /// </summary>
    public class BouncyMissile : DamageScaledAction, IActionWithSound
    {
        public BouncyMissile(ActionContext ctx) 
            : base(ctx, ctx.Stats.damage, StatMapper.MapBouncyMissileDamage) { }
        
        protected override void Perform(Vector3 origin, Vector3 target, float damage)
        {
            var direction = (target - origin).normalized;
            var speed = TankPhysicsHelper.CalculateProjectileSpeed(
                Ctx.Stats.bouncyMissileMaxSpeed, origin, target
            );

            var projectile = Object.Instantiate(Ctx.BouncyMissilePrefab, origin, Quaternion.identity);
            if (!projectile.TryGetComponent<ExplosiveProjectile>(out var proj)) return;
            proj.Initialize(
                Ctx.Collider, 
                speed, 
                SmashTanksConstants.BouncyMissile.ExplosionRadius, 
                SmashTanksConstants.BouncyMissile.ExplosionForce, 
                damage
                );
            Ctx.Rb.AddForce(-direction * SmashTanksConstants.BouncyMissile.RecoilForce, ForceMode2D.Impulse);
        }

        public override string GetName() => "Bouncy";

        public override AimType AimType() => Actions.AimType.Parabolic;
        
        public override int Cooldown => SmashTanksConstants.BouncyMissile.Cooldown;
        public new bool LocksCannon() => false;
        public AudioClip ExecuteSound => Ctx.ExecuteSound;
    }
    
    
    /// <summary>
    /// Makes the tank jump toward a target location.
    /// </summary>
    public class Jump : IAction, IActionWithSound
    {
        private readonly ActionContext _ctx;
        public AudioClip ExecuteSound => _ctx.ExecuteSound;

        public Jump(ActionContext ctx) => _ctx = ctx;

        public void Execute(Vector3 origin, Vector3 target)
        {
            var force = TankPhysicsHelper.CalculateJumpForce(
                _ctx.Stats.maxForce,
                _ctx.Center.position,
                target
            );
            _ctx.Rb.AddForce(force, ForceMode2D.Impulse);
        }

        public string GetName() => "Jump";
        public AimType AimType() => Actions.AimType.Parabolic;
        public int Cooldown => SmashTanksConstants.Jump.Cooldown;
        public bool LocksCannon() => false;
    }
    
    
    /// <summary>
    /// Makes the tank jump toward a target location and applies collision damage when it impacts another object.
    /// </summary>
    public class Crash : DamageScaledAction, IActionWithSound
    {
        public override int Cooldown => SmashTanksConstants.Crash.Cooldown;


        /// <summary>
        /// Initializes a new instance of the <see cref="Crash"/> class.
        /// </summary>
        public Crash(
            ActionContext ctx
            ) : base(ctx, ctx.Stats.damage, StatMapper.MapCrash)
        {
        }

        /// <inheritdoc />
        protected override void Perform(Vector3 origin, Vector3 target, float damageMultiplier)
        {
            var force = TankPhysicsHelper.CalculateJumpForce(Ctx.Stats.maxForce, Ctx.Center.position, target);
            Ctx.Rb.AddForce(force, ForceMode2D.Impulse);

            // Attach a temporary crash handler to process collision-based damage.
            var handler = Ctx.Rb.gameObject.AddComponent<CrashHandlerScript>();
            handler.rb = Ctx.Rb;
            handler.damageMultiplier = damageMultiplier;
        }

        /// <inheritdoc />
        public override string GetName() => "Crash";

        /// <inheritdoc />
        public override AimType AimType() => Actions.AimType.Parabolic;
        
        public new bool LocksCannon() => false;
        public AudioClip ExecuteSound => Ctx.ExecuteSound;
    }

    
    /// <summary>
    /// Base class for actions that use magicka and scale with intellect.
    /// </summary>
    public abstract class IntellectScaledAction : StatScaledAction
    {
        protected IntellectScaledAction(
            ActionContext ctx, 
            float intellectLevel,
            Func<float, float> intellectMapper
            )
            : base(ctx, intellectLevel, intellectMapper) { }

        /// <summary>
        /// The magicka cost to perform this action.
        /// </summary>
        protected abstract float MagickaCost { get; set; }
        
        /// <summary>
        /// Executes the magical action, consuming magicka before performing.
        /// </summary>
        public new void Execute(Vector3 origin, Vector3 target)
        {
            if (!Ctx.Tank) return;

            Ctx.Tank.SpendMagicka(MagickaCost);
            base.Execute(origin, target);
        }
        public new virtual bool LocksCannon() => false;
    }
    

    /// <summary>
    /// Fires a magical energy beam from the tank toward the target.
    /// </summary>
    public class Beam : IntellectScaledAction, IActionWithSound
    {
        public AudioClip ExecuteSound => Ctx.ExecuteSound;

        public Beam(ActionContext ctx)
            : base(ctx, ctx.Stats.intellect, StatMapper.MapBeamDamage)
        {
            MagickaCost = ctx.Tank.MagickaCosts[GetName()];
        }

        protected sealed override float MagickaCost { get; set; }

        protected override void Perform(Vector3 origin, Vector3 target, float damage)
        {
            var beam = Object.Instantiate(Ctx.BeamPrefab, origin, Quaternion.identity);
            if (!beam.TryGetComponent(out BeamScript beamScript)) return;

            beamScript.Initialize(damage, (target - origin).normalized);
        }

        public override string GetName() => "action_beam";
        public override AimType AimType() => Actions.AimType.HalfLine;
        public override int Cooldown => SmashTanksConstants.Beam.Cooldown;
    }
    
    
    /// <summary>
    /// Teleports the tank to a location near the target.
    /// The higher the intellect, the more accurate the teleport.
    /// </summary>
    public class Teleport : IntellectScaledAction, ICircularAreaAction, IActionWithSound
    {
        public AudioClip ExecuteSound => Ctx.ExecuteSound;
        public float Radius { get; }

        protected sealed override float MagickaCost { get; set; }

        public Teleport(ActionContext ctx)
            : base(ctx, ctx.Stats.intellect, StatMapper.MapTeleportRadius)
        {
            Radius = StatMapper.MapTeleportRadius(ctx.Stats.intellect);
            MagickaCost = ctx.Tank.MagickaCosts[GetName()];
        }

        public override string GetName() => "action_teleport";
        public override AimType AimType() => Actions.AimType.CircularArea;
        public override int Cooldown => SmashTanksConstants.Teleport.Cooldown;

        protected override void Perform(Vector3 origin, Vector3 target, float radius)
        {
            var j = 0;
            while (true) {
                for (var i = 0; i < SmashTanksConstants.Teleport.MaxAttempts; i++)
                {
                    var randomOffset = Random.insideUnitCircle * (Radius + j * 1f);
                    var candidate = new Vector2(target.x + randomOffset.x, target.y + randomOffset.y);

                    if (IsInsideSolidObject(candidate)) continue;
                    Ctx.Tank.transform.position = candidate;
                    return;
                }
                j++;
            }
        }

        private static bool IsInsideMapBounds(Vector2 point)
        {
            return point.x is >= SmashTanksConstants.MapBounds.MinX and <= SmashTanksConstants.MapBounds.MaxX &&
                   point.y is >= SmashTanksConstants.MapBounds.MinY and <= SmashTanksConstants.MapBounds.MaxY;
        }

        private static bool IsInsideSolidObject(Vector2 point)
        {
            var collider = Physics2D.OverlapCircle(point, SmashTanksConstants.Teleport.CollisionCheckRadius);
            return collider;
        }
    }


    /// <summary>
    /// Creates a moving wind force that pushes all rigidbodies it touches.
    /// </summary>
    public class Gale : IntellectScaledAction, IActionWithSound
    {
        private readonly float _force;
        public override int Cooldown => SmashTanksConstants.Gale.Cooldown;
        
        protected sealed override float MagickaCost { get; set; }

        public Gale(ActionContext ctx)
            : base(ctx, ctx.Stats.intellect, StatMapper.MapGaleForce) { }

        protected override void Perform(Vector3 origin, Vector3 target, float force)
        {
            var direction = (target - origin).normalized;
            var gale = Object.Instantiate(Ctx.GalePrefab, origin, Quaternion.identity);

            if (!gale.TryGetComponent<GaleScript>(out var galeScript)) return;
            galeScript.Initialize(direction, force);
        }

        public sealed override string GetName() => "action_gale";
        public override AimType AimType() => Actions.AimType.GaleZone;
        public AudioClip ExecuteSound => Ctx.ExecuteSound;
    }
    
    public class JuggernautUlti : IAction, IActionWithSound
    {
        private readonly ActionContext _ctx;
        public AudioClip ExecuteSound => _ctx.ExecuteSound;

        public JuggernautUlti(ActionContext ctx) => _ctx = ctx;

        public void Execute(Vector3 origin, Vector3 target)
        {
            _ctx.Tank.UltiCurrentValues["Juggernaut"] = 0f;

            var direction = (target - origin).normalized;
            var speed = TankPhysicsHelper.CalculateProjectileSpeed(
                _ctx.Stats.juggernautShotMaxSpeed, origin, target
            );

            var projectile = Object.Instantiate(
                _ctx.JuggernautProjectilePrefab, origin, Quaternion.identity
            );

            if (!projectile.TryGetComponent(out JuggernautProjectile proj)) return;

            proj.Initialize(_ctx.Collider, speed, _ctx.Stats.damage);

            _ctx.Rb.AddForce(
                -direction * SmashTanksConstants.JuggernautUlti.RecoilForce,
                ForceMode2D.Impulse
            );
        }

        public string GetName() => "Juggernaut";
        public AimType AimType() => Actions.AimType.Parabolic;
        public int Cooldown => 0;
        public bool LocksCannon() => false;
    }
    
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
