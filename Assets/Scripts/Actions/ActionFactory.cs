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
                    _ => null
                };
            Debug.LogWarning("ActionFactory: Tank reference is null. Cannot create action.");
            return null;

        }

        private static IAction CreateMissileAction(TankScript tank) =>
            new MissileAction(
                tank.ProjectilePrefab,
                tank.SpeedMultiplier,
                tank.MaxSpeed,
                tank.AimPoint,
                tank.FirePoint,
                tank.Rb
            );

        private static IAction CreateJumpAction(TankScript tank) =>
            new JumpAction(
                tank.ForceMultiplier,
                tank.AimPoint,
                tank.Rb
            );

        private static IAction CreateCrashAction(TankScript tank) =>
            new CrashAction(
                tank.ForceMultiplier,
                tank.AimPoint,
                tank.Rb,
                0.1f
            );
    }

    public enum ActionType
    {
        Missile,
        Jump,
        Crash
    }
}