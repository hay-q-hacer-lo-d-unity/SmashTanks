using System;
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

            var (origin, velocity) = action switch
            {
                AtomicEssenceUlti => (_tank.FirePoint.position, CalculateAtomicEssenceProjectileVelocity()),
                
                JuggernautUlti => (_tank.FirePoint.position, CalculateJuggernautProjectileVelocity()),

                BouncyMissile => (_tank.FirePoint.position, CalculateBouncyMissileVelocity()),

                Actions.Missile => (_tank.FirePoint.position, CalculateMissileVelocity()),

                // AIMPOINT actions
                Jump or Crash => (_tank.Center.position, CalculateJumpVelocity()),

                _ => (Vector3.zero, Vector2.zero)
            };

            const SmashTanksConstants.Config.AccuracyMode mode = SmashTanksConstants.Config.TrajectoryAccuracyMode;
            switch (mode)
            {
                case SmashTanksConstants.Config.AccuracyMode.TimeBased:
                    var time = 
                        SmashTanksConstants.Stats.AccuracyBaseTime + 
                        (_tank.Stats.accuracy - 1) * SmashTanksConstants.Stats.AccuracyTimeIncreasePerLevel;
                    _drawer.DrawParabola_ByTime(origin, velocity, time);
                    break;
                case SmashTanksConstants.Config.AccuracyMode.LengthBased:
                    var length = 
                        SmashTanksConstants.Stats.AccuracyBaseLength + 
                        (_tank.Stats.accuracy - 1) * SmashTanksConstants.Stats.AccuracyLengthIncreasePerLevel;
                    _drawer.DrawParabola_ByArcLength(origin, velocity, length);
                    break;
                case SmashTanksConstants.Config.AccuracyMode.PointsDistanceBased:
                    var distance = 
                        SmashTanksConstants.Stats.AccuracyBasePointsDistance + 
                        (_tank.Stats.accuracy - 1) * SmashTanksConstants.Stats.AccuracyPointsDistanceIncreasePerLevel;
                    _drawer.DrawParabola_ByDistanceBetweenPoints(origin, velocity, distance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        public void GaleTrajectory()
        {
            if (!_drawer) return;
            var cursor = GetMouseWorld();
            var dir = (cursor - (Vector2)_tank.FirePoint.position).normalized;
            var target = _tank.FirePoint.position + (Vector3)dir * SmashTanksConstants.Gale.Distance;
            _drawer.DrawGaleZone(_tank.FirePoint.position, target);
        }
        
        public void Hide() => _drawer?.ClearParabola();

        private Vector2 CalculateMissileVelocity()
        {
            var cursor = GetMouseWorld();
            return TankPhysicsHelper.CalculateProjectileSpeed(_tank.Stats.missileMaxSpeed, _tank.FirePoint.position, cursor);
        }
        
        private Vector2 CalculateBouncyMissileVelocity()
        {
            var cursor = GetMouseWorld();
            return TankPhysicsHelper.CalculateProjectileSpeed(_tank.Stats.bouncyMissileMaxSpeed, _tank.FirePoint.position, cursor);
        }
        
        private Vector2 CalculateJuggernautProjectileVelocity()
        {
            var cursor = GetMouseWorld();
            return TankPhysicsHelper.CalculateProjectileSpeed(_tank.Stats.juggernautShotMaxSpeed, _tank.FirePoint.position, cursor);
        }
        
        private Vector2 CalculateAtomicEssenceProjectileVelocity()
        {
            var cursor = GetMouseWorld();
            return TankPhysicsHelper.CalculateProjectileSpeed(_tank.Stats.atomicEssenceShotMaxSpeed, _tank.FirePoint.position, cursor);
        }

        private Vector2 CalculateJumpVelocity()
        {
            var cursor = GetMouseWorld();
            var force = TankPhysicsHelper.CalculateJumpForce(_tank.Stats.maxForce, _tank.Center.position, cursor);
            
            return force / _tank.Rb.mass;
        }
        
        private static Vector2 GetMouseWorld()
        {
            var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(mouseWorld.x, mouseWorld.y);
        }
    }
}
