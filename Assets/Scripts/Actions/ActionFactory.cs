using Tank;
using UnityEngine;

namespace Actions
{
    public static class ActionFactory
    {
        public static IAction Create(ActionType type, TankScript tank)
        {
            if (tank != null)
                return type switch
                {
                    ActionType.Missile => CreateMissileAction(tank),
                    ActionType.Jump => CreateJumpAction(tank),
                    ActionType.Crash => CreateCrashAction(tank),
                    ActionType.Beam => CreateBeamAction(tank),
                    _ => null
                };
            Debug.LogWarning("ActionFactory: Tank reference is null. Cannot create action.");
            return null;

        }

        private static IAction CreateMissileAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new MissileAction(
                tank.ProjectilePrefab,
                stats.speedMultiplier,
                stats.maxSpeed,
                tank.FirePoint,
                stats.damage,
                stats.explosionRadius,
                stats.explosionForce
            );
        }

        private static IAction CreateJumpAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new JumpAction(
                stats.forceMultiplier,
                tank.AimPoint,
                tank.Rb
            );
        }

        private static IAction CreateCrashAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new CrashAction(
                stats.forceMultiplier,
                tank.AimPoint,
                tank.Rb,
                0.01f
            );
        }
        
        private static IAction CreateBeamAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new BeamAction(
                tank.FirePoint,
                stats.intellect
            );
        }
    }

    public enum ActionType
    {
        Missile,
        Jump,
        Crash,
        Beam
    }
}
