using System;
using Tank;
using UnityEngine;
using Weapons;
using Object = UnityEngine.Object;

namespace Actions
{
    public interface IAction
    {
        void Execute(Vector3 origin, Vector3 target);

        string GetName();

        bool HasFalloff();

        bool LocksCannon();
    }

    public class MissileAction : IAction
    {
        private readonly GameObject _projectilePrefab;
        private readonly float _speedMultiplier;
        private readonly float _maxSpeed;
        private readonly Transform _firePoint;
        private readonly float _damage;
        private readonly float _explosionRadius;
        private readonly float _explosionForce;
        public MissileAction(
            GameObject projectilePrefab,
            float speedMultiplier,
            float maxSpeed,
            Transform firePoint,
            float damage,
            float explosionRadius,
            float explosionForce
        )
        {
            _projectilePrefab = projectilePrefab;
            _speedMultiplier = speedMultiplier;
            _maxSpeed = maxSpeed;
            _firePoint = firePoint;
            _damage = damage;
            _explosionRadius = explosionRadius;
            _explosionForce = explosionForce;
        }
    
        public void Execute(Vector3 origin, Vector3 target)
        {
            if (!_projectilePrefab || !_firePoint) return;

            var cursorPosition = new Vector2(target.x, target.y);
            var originPosition = new Vector2(origin.x, origin.y);
        
            var dir = (cursorPosition - originPosition).normalized;
            var distance = Vector2.Distance(cursorPosition, origin);

            var speed = Mathf.Clamp(distance * _speedMultiplier, 0, _maxSpeed);

            var proj = Object.Instantiate(_projectilePrefab, origin, Quaternion.identity);

            var rb = proj.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = dir * speed;

            var tankCollider = _firePoint.GetComponentInParent<Collider2D>();
            var projectileScript = proj.GetComponent<IProjectile>();
            projectileScript?.SetOwner(tankCollider);
            projectileScript?.SetStats(_explosionRadius, _explosionForce, _damage);
        }

        public string GetName() => "Shoot";
        public bool HasFalloff() => true;

        public bool LocksCannon() => false;
    }

    public class JumpAction : IAction
    {
        private readonly Transform _aimPoint;
        private readonly Rigidbody2D _rb;
        private readonly float _forceMultiplier;
        private readonly float _maxForce;
        public JumpAction(
            float maxForce,
            float forceMultiplier,
            Transform  aimPoint,
            Rigidbody2D rb
        )
        {
            _forceMultiplier = forceMultiplier;
            _aimPoint = aimPoint;
            _rb = rb;
            _maxForce = maxForce;
        }
        public void Execute(Vector3 origin, Vector3 target)
        {
            var force = TankPhysicsHelper.CalculateJumpForce(_maxForce, _aimPoint.position, target);

            _rb.AddForce(force, ForceMode2D.Impulse);
        }

        public string GetName() => "Jump";
        
        public bool HasFalloff() => true;

        public bool LocksCannon() => true;
    }

    public class CrashAction : IAction
    {
        private readonly Transform _aimPoint;
        private readonly Rigidbody2D _rb;
        private readonly float _maxForce;
        private readonly float _damageMultiplier;

        public CrashAction(
            float maxForce,
            Transform aimPoint,
            Rigidbody2D rb,
            float damageMultiplier
        )
        {
            _maxForce = maxForce;
            _aimPoint = aimPoint;
            _rb = rb;
            _damageMultiplier = damageMultiplier;
        }

        public void Execute(Vector3 origin, Vector3 target)
        {
            var force = TankPhysicsHelper.CalculateJumpForce(_maxForce, _aimPoint.position, target);

            _rb.AddForce(force, ForceMode2D.Impulse);

            // Add temporary CrashHandler to detect collisions
            var handler = _rb.gameObject.AddComponent<CrashHandlerScript>();
            handler.rb = _rb;
            handler.damageMultiplier = _damageMultiplier;
        }

        public string GetName() => "Crash";
        public bool HasFalloff() => true;


        public bool LocksCannon() => true;
    }
    
    public class BeamAction : IAction
    {
        private readonly GameObject _beamPrefab;
        private readonly Transform _firePoint;
        private readonly float _damage;
        private readonly TankScript _tank;

        public BeamAction(GameObject beamPrefab, Transform firePoint, float intellect, TankScript tank)
        {
            _beamPrefab = beamPrefab;
            _firePoint = firePoint;
            _damage = intellect * SmashTanksConstants.BEAM_DAMAGE_PER_INTELLECT;
            _tank = tank;
        }

        public void Execute(Vector3 origin, Vector3 target)
        {
            if (!_beamPrefab || !_firePoint) return;

            var beam = Object.Instantiate(_beamPrefab, origin, Quaternion.identity);
            _tank.SpendMagicka(SmashTanksConstants.BEAM_MAGICKA_COST);
            
            var beamScript = beam.GetComponent<BeamScript>();
            if (!beamScript) return;
            beamScript.SetStats(_damage);

            var direction = (target - origin).normalized;
            beam.transform.up = direction;
        }

        public string GetName() => "Beam";
        public bool HasFalloff() => false;

        public bool LocksCannon() => false;
    }
}