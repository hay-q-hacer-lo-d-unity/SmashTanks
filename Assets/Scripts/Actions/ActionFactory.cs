using Tank;
using UnityEngine;

namespace Actions
{
    public static class ActionFactory
    {
        public static IAction Create(ActionType type, TankScript tank)
        {
            if (tank) return type switch
                {
                    ActionType.Shoot => CreateMissileAction(tank),
                    ActionType.Jump => CreateJumpAction(tank),
                    ActionType.Crash => CreateCrashAction(tank),
                    ActionType.Beam => CreateBeamAction(tank),
                    ActionType.Teleport => CreateTeleportAction(tank),
                    ActionType.Gale => CreateGaleAction(tank),
                    ActionType.Bouncy => CreateBouncyMissileAction(tank),
                    _ => null
                };
            Debug.LogWarning("ActionFactory: Tank reference is null. Cannot create action.");
            return null;

        }

        private static IAction CreateMissileAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new MissileAction(
                tank.MissilePrefab,
                stats.missileMaxSpeed,
                tank.FirePoint,
                tank.Rb,
                tank.Collider,
                stats.damage,
                stats.explosionRadius,
                stats.explosionForce
            );
        }
        
        private static IAction CreateBouncyMissileAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new BouncyMissileAction(
                tank.BouncyMissilePrefab,
                stats.missileMaxSpeed,
                tank.FirePoint,
                tank.Rb,
                tank.Collider,
                stats.damage,
                stats.explosionRadius,
                stats.explosionForce
            );
        }

        private static IAction CreateJumpAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new JumpAction(
                stats.maxForce,
                tank.AimPoint,
                tank.Rb
            );
        }

        private static IAction CreateCrashAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new CrashAction(
                stats.maxForce,
                tank.AimPoint,
                tank.Rb,
                0.025f
            );
        }
        
        private static IAction CreateBeamAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new BeamAction(
                tank.BeamPrefab,
                tank.FirePoint,
                stats.intellect,
                tank
            );
        }
        
        private static IAction CreateTeleportAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new TeleportAction(
                tank,
                stats.intellect
            );
        }
        
        private static IAction CreateGaleAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new GaleAction(
                tank.GalePrefab,
                stats.intellect,
                tank.FirePoint,
                tank
            );
        }
    }

    public enum ActionType
    {
        Shoot,
        Jump,
        Crash,
        Beam,
        Teleport,
        Gale,
        Bouncy
    }
}
