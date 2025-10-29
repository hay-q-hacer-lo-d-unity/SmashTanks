using System;
using Tank;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IAction
{
    void Execute(Vector3 origin, Vector3 target);

    String GetName();

    bool LocksCannon();
}

public class MissileAction : IAction
{
    private readonly GameObject _projectilePrefab;
    private readonly float _speedMultiplier;
    private readonly float _maxSpeed;
    private readonly Transform _aimPoint;
    private readonly Transform _firePoint;
    private Rigidbody2D _rb;
    public MissileAction(
        GameObject projectilePrefab,
        float speedMultiplier,
        float maxSpeed,
        Transform aimPoint,
        Transform firePoint,
        Rigidbody2D rb
        )
    {
        _projectilePrefab = projectilePrefab;
        _speedMultiplier = speedMultiplier;
        _maxSpeed = maxSpeed;
        _aimPoint = aimPoint;
        _firePoint = firePoint;
        _rb = rb;
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
        if (rb)
        {
            rb.linearVelocity = dir * speed;
        }

        var tankCollider = _firePoint.GetComponentInParent<Collider2D>();
        var projectileScript = proj.GetComponent<IProjectile>();
        projectileScript?.SetOwner(tankCollider);
    }

    public string GetName()
    {
        return "Shoot";
    }

    public bool LocksCannon()
    {
        return false;
    }
}

public class JumpAction : IAction
{
    private readonly Transform _aimPoint;
    private readonly Rigidbody2D _rb;
    private readonly float _forceMultiplier;
    public JumpAction(
        float forceMultiplier,
        Transform  aimPoint,
        Rigidbody2D rb
    )
    {
        _forceMultiplier = forceMultiplier;
        _aimPoint = aimPoint;
        _rb = rb;
    }
    public void Execute(Vector3 origin, Vector3 target)
    {
        var cursorPosition = new Vector2(target.x, target.y);

        var dir = (cursorPosition - (Vector2)_aimPoint.position).normalized;
        var distance = Vector2.Distance(cursorPosition, _aimPoint.position);
        var clampedDistance = Mathf.Clamp(distance, 0f, 5f);

        var force = dir * (clampedDistance * _forceMultiplier);

        _rb.AddForce(force, ForceMode2D.Impulse);
    }

    public string GetName()
    {
        return "Jump";
    }

    public bool LocksCannon()
    {
        return true;
    }
}

public class CrashAction : IAction
{
    private readonly Transform _aimPoint;
    private readonly Rigidbody2D _rb;
    private readonly float _forceMultiplier;
    private readonly float _damageMultiplier;
    public CrashAction(
        float forceMultiplier,
        Transform  aimPoint,
        Rigidbody2D rb,
        float damageMultiplier
    )
    {
        _forceMultiplier = forceMultiplier;
        _aimPoint = aimPoint;
        _rb = rb;
        _damageMultiplier = damageMultiplier;
    }
    public void Execute(Vector3 origin, Vector3 target)
    {
        var cursorPosition = new Vector2(target.x, target.y);

        var dir = (cursorPosition - (Vector2)_aimPoint.position).normalized;
        var distance = Vector2.Distance(cursorPosition, _aimPoint.position);
        var clampedDistance = Mathf.Clamp(distance, 0f, 5f);

        var force = dir * (clampedDistance * _forceMultiplier);
        

        _rb.AddForce(force, ForceMode2D.Impulse);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherTank = collision.collider.GetComponent<TankScript>();
        if (otherTank)
        {
            var damage = _damageMultiplier * collision.relativeVelocity.magnitude * _rb.mass;
            otherTank.ApplyDamage(damage);
        }
    }

    public string GetName()
    {
        return "Crash";
    }

    public bool LocksCannon()
    {
        return true;
    }
}