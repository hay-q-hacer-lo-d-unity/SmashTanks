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

        public void HalfLineTrajectory()
        {
            if (!_drawer) return;
            var cursor = GetMouseWorld();
            _drawer.DrawHalfLine(_tank.FirePoint.position, cursor);
        }
        
        public void SegmentTrajectory()
        {
            if (!_drawer) return;
            var cursor = GetMouseWorld();
            _drawer.DrawSegment(_tank.FirePoint.position, cursor);
        }

        public void CircularArea(ICircularAreaAction action)
        {
            var cursor = GetMouseWorld();
            _drawer.DrawCircle(cursor, action.Radius);
        }

        public void ParabolicTrajectory(IAction action)
        {
            if (!_drawer) return;

            var velocity = action switch
            {
                BouncyMissileAction => CalculateBouncyMissileVelocity(),
                Actions.Missile => CalculateMissileVelocity(),
                Jump or Crash => CalculateJumpVelocity(),
                _ => Vector2.zero
            };

            var origin = action switch
            {
                Actions.Missile or BouncyMissileAction=> _tank.FirePoint.position,
                Jump or Crash => _tank.AimPoint.position,
                _ => Vector3.zero
            };

            _drawer.DrawParabola(origin, velocity, _tank.Stats.accuracy);
        }
        
        public void GaleTrajectory()
        {
            if (!_drawer) return;
            var cursor = GetMouseWorld();
            var dir = (cursor - (Vector2)_tank.FirePoint.position).normalized;
            var target = _tank.FirePoint.position + (Vector3)dir * SmashTanksConstants.GALE_DISTANCE;
            _drawer.DrawGaleZone(_tank.FirePoint.position, target);
        }
        
        public void Hide() => _drawer?.ClearParabola();

        private Vector2 CalculateMissileVelocity()
        {
            var cursor = GetMouseWorld();
            return TankPhysicsHelper.CalculateMissileSpeed(_tank.Stats.missileMaxSpeed, _tank.FirePoint.position, cursor);
        }
        
        private Vector2 CalculateBouncyMissileVelocity()
        {
            var cursor = GetMouseWorld();
            return TankPhysicsHelper.CalculateMissileSpeed(_tank.Stats.bouncyMissileMaxSpeed, _tank.FirePoint.position, cursor);
        }

        private Vector2 CalculateJumpVelocity()
        {
            var cursor = GetMouseWorld();
            var force = TankPhysicsHelper.CalculateJumpForce(_tank.Stats.maxForce, _tank.AimPoint.position, cursor);
            
            return force / _tank.Rb.mass;
        }
        
        private static Vector2 GetMouseWorld()
        {
            var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(mouseWorld.x, mouseWorld.y);
        }
    }
}
