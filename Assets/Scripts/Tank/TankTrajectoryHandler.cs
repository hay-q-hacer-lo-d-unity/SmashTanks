using Actions;
using UnityEngine;

namespace Tank
{
    public class TankTrajectoryHandler
    {
        private readonly TankScript _tank;
        private readonly TrajectoryDrawerScript _drawer;

        public TankTrajectoryHandler(TankScript tank, TrajectoryDrawerScript drawer)
        {
            _tank = tank;
            _drawer = drawer;
        }

        public void UpdateTrajectory(IAction action)
        {
            if (_drawer == null) return;

            Vector2 velocity = action switch
            {
                MissileAction => CalculateMissileVelocity(),
                JumpAction or CrashAction => CalculateJumpVelocity(),
                _ => Vector2.zero
            };

            Vector3 origin = action switch
            {
                MissileAction => _tank.FirePoint.position,
                JumpAction or CrashAction => _tank.AimPoint.position,
                _ => _tank.AimPoint.position
            };

            _drawer.DrawParabola(origin, velocity, _tank.Stats.accuracy);
        }

        private Vector2 CalculateMissileVelocity()
        {
            Vector2 cursor = GetMouseWorld();
            Vector2 dir = (cursor - (Vector2)_tank.AimPoint.position).normalized;
            float distance = Vector2.Distance(cursor, _tank.FirePoint.position);
            float speed = Mathf.Clamp(distance * _tank.Stats.speedMultiplier, 0, _tank.Stats.maxSpeed);
            return dir * speed;
        }

        private Vector2 CalculateJumpVelocity()
        {
            Vector2 cursor = GetMouseWorld();
            Vector2 dir = (cursor - (Vector2)_tank.AimPoint.position).normalized;
            float distance = Vector2.Distance(cursor, _tank.AimPoint.position);
            float clamped = Mathf.Clamp(distance, 0f, 5f);
            Vector2 force = dir * (clamped * _tank.Stats.forceMultiplier);
            return force / _tank.Rb.mass;
        }

        public void Hide() => _drawer?.ClearParabola();

        private static Vector2 GetMouseWorld()
        {
            var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(mouseWorld.x, mouseWorld.y);
        }
    }
}
