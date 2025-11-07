using UnityEditor;

namespace Tank
{
    using UnityEngine;

    public static class TankPhysicsHelper
    {
        public static Vector2 CalculateJumpForce(float maxForce, Vector2 origin, Vector2 target)
        {
            var cursorPosition = new Vector2(target.x, target.y);

            var dir = (cursorPosition - origin).normalized;
            var distance = Vector2.Distance(cursorPosition, origin);
            var clampedDistance = Mathf.Clamp(distance, 0f, 10f) / 10f;

            var forceMagnitude = Mathf.Clamp(clampedDistance * maxForce, 0, maxForce);
            var force = dir * forceMagnitude;
            return force;
        }
        
        public static Vector2 CalculateMissileSpeed(float maxSpeed, Vector2 origin, Vector2 target)
        {
            var cursorPosition = new Vector2(target.x, target.y);

            var dir = (cursorPosition - origin).normalized;
            var distance = Vector2.Distance(cursorPosition, origin);
            var clampedDistance = Mathf.Clamp(distance, 0f, 10f) / 10f;

            var speedMagnitude = Mathf.Clamp(clampedDistance * maxSpeed, 0, maxSpeed);
            var speed = dir * speedMagnitude;
            return speed;
        }
    }
}