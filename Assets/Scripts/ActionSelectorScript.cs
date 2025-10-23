using UnityEngine;

public class ActionSelectorScript : MonoBehaviour
{
    public TankScript tank;

    public void SelectMissile()
    {
        tank.SetAction(new MissileAction(
            tank.projectilePrefab,
            tank.speedMultiplier,
            tank.maxSpeed,
            tank.aimPoint,
            tank.firePoint,
            tank.rb
            ));
    }

    public void SelectJump()
    {
        tank.SetAction(new JumpAction(
            tank.forceMultiplier,
            tank.aimPoint,
            tank.rb
            ));
    }
    
    public void SelectCrash()
    {
        tank.SetAction(new CrashAction(
            tank.forceMultiplier,
            tank.aimPoint,
            tank.rb,
            0.1f
            ));
    }
}