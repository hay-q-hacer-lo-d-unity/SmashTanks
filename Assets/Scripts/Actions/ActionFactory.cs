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
                case "action_missile":        return CreateMissileAction      (tank);
                case "action_jump":           return CreateJumpAction         (tank);
                case "action_crash":          return CreateCrashAction        (tank);
                case "action_beam":           return CreateBeamAction         (tank);
                case "action_teleport":       return CreateTeleportAction     (tank);
                case "action_gale":           return CreateGaleAction         (tank);
                case "action_bouncy_missile": return CreateBouncyMissileAction(tank);
                case "juggernaut":            return CreateJuggernautAction   (tank);
                default: 
                    Debug.LogWarning($"ActionFactory: Unknown action ID '{actionId}'.");
                    return null;
            }
        }

        private static Missile CreateMissileAction(TankScript tank)
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

        private static BouncyMissile CreateBouncyMissileAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new BouncyMissile(
                tank.BouncyMissilePrefab,
                stats.bouncyMissileMaxSpeed,
                tank.FirePoint,
                tank.Rb,
                tank.Collider,
                stats.damage
            );
        }

        private static Jump CreateJumpAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Jump(stats.maxForce, tank.Center, tank.Rb);
        }

        private static Crash CreateCrashAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Crash(stats.maxForce, tank.Center, tank.Rb, stats.damage);
        }

        private static Beam CreateBeamAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Beam(tank.BeamPrefab, tank.FirePoint, stats.intellect, tank);
        }

        private static Teleport CreateTeleportAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Teleport(tank, stats.intellect);
        }

        private static Gale CreateGaleAction(TankScript tank)
        {
            var stats = tank.Stats;
            return new Gale(tank.GalePrefab, stats.intellect, tank.FirePoint, tank);
        }
        
        private static JuggernautUlti CreateJuggernautAction(TankScript tank)
        {
            return new JuggernautUlti(
                tank.UltiCurrentValues["Juggernaut"],
                tank.JuggernautProjectilePrefab,
                tank,
                tank.Stats.juggernautShotMaxSpeed,
                tank.FirePoint,
                tank.Rb,
                tank.Collider
                );
        }
    }
}
