using Tank;
using UnityEngine;

namespace Actions
{
    public static class ActionFactory
    {
        public static IAction Create(string actionId, TankScript tank)
        {
            if (!tank)
            {
                Debug.LogWarning("ActionFactory: Tank reference is null. Cannot create action.");
                return null;
            }

            switch (actionId.ToLowerInvariant())
            {
                case "missile":       return CreateMissileAction(tank);
                case "jump":        return CreateJumpAction(tank);
                case "crash":       return CreateCrashAction(tank);
                case "beam":        return CreateBeamAction(tank);
                case "teleport":    return CreateTeleportAction(tank);
                case "gale":        return CreateGaleAction(tank);
                case "bouncy missile":      return CreateBouncyMissileAction(tank);
                default:
                    Debug.LogWarning($"ActionFactory: Unknown action ID '{actionId}'.");
                    return null;
            }
        }

        private static IAction CreateMissileAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Missile(
                tank.MissilePrefab,
                stats.missileMaxSpeed,
                tank.FirePoint,
                tank.Rb,
                tank.Collider,
                stats.damage
            );
        }

        private static IAction CreateBouncyMissileAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new BouncyMissileAction(
                tank.BouncyMissilePrefab,
                stats.bouncyMissileMaxSpeed,
                tank.FirePoint,
                tank.Rb,
                tank.Collider,
                stats.damage
            );
        }

        private static IAction CreateJumpAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Jump(stats.maxForce, tank.AimPoint, tank.Rb);
        }

        private static IAction CreateCrashAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Crash(stats.maxForce, tank.AimPoint, tank.Rb, stats.damage);
        }

        private static IAction CreateBeamAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Beam(tank.BeamPrefab, tank.FirePoint, stats.intellect, tank);
        }

        private static IAction CreateTeleportAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Teleport(tank, stats.intellect);
        }

        private static IAction CreateGaleAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Gale(tank.GalePrefab, stats.intellect, tank.FirePoint, tank);
        }
    }
}
