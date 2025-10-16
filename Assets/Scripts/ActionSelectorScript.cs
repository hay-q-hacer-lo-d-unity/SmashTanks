using UnityEngine;

public class ActionSelectorScript : MonoBehaviour
{
    public TankScript tank;

    public void SetTank(TankScript newTank)
    {
        tank = newTank;
    }
    
    public void SelectMissile()
    {
        tank.SetAction(new MissileAction(
            tank.shooter,
            tank.speedMultiplier,
            tank.maxSpeed,
            tank.aimPoint,
            tank.firePoint
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
}