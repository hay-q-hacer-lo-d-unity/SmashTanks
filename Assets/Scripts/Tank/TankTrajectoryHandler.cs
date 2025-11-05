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

        public void UpdateLinearTrajectory()
        {
            if (!_drawer) return;
            var cursor = GetMouseWorld();
            _drawer.DrawLine(_tank.FirePoint.position, cursor);
        }

        public void UpdateTrajectory(IAction action)
        {
            if (!_drawer) return;

            var velocity = action switch
            {
                MissileAction => CalculateMissileVelocity(),
                JumpAction or CrashAction => CalculateJumpVelocity(),
                _ => Vector2.zero
            };

            var origin = action switch
            {
                MissileAction => _tank.FirePoint.position,
                JumpAction or CrashAction => _tank.AimPoint.position,
                _ => Vector3.zero
            };

            _drawer.DrawParabola(origin, velocity, _tank.Stats.accuracy);
        }

        private Vector2 CalculateMissileVelocity()
        {
            var cursor = GetMouseWorld();
            var dir = (cursor - (Vector2)_tank.AimPoint.position).normalized;
            var distance = Vector2.Distance(cursor, _tank.FirePoint.position);
            var speed = Mathf.Clamp(distance * _tank.Stats.speedMultiplier, 0, _tank.Stats.maxSpeed);
            return dir * speed;
        }

        private Vector2 CalculateJumpVelocity()
        {
            var cursor = GetMouseWorld();
            var force = TankPhysicsHelper.CalculateJumpForce(_tank.Stats.maxForce, _tank.AimPoint.position, cursor);
            
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
