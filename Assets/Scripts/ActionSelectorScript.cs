using UnityEngine;

public class ActionSelectorScript : MonoBehaviour
{
    public TankScript tank;

    public void SelectMissile()
    {
        tank.SetAction(new MissileAction());
    }

    public void SelectJump()
    {
        tank.SetAction(new JumpAction());
    }
}