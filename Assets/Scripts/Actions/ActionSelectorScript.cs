using Actions;
using Tank;
using UnityEngine;

using UnityEngine;

public class ActionSelectorScript : MonoBehaviour
{
    [SerializeField] private TankScript tank;

    public void SelectMissile() => SetAction(ActionType.Missile);
    public void SelectJump() => SetAction(ActionType.Jump);
    public void SelectCrash() => SetAction(ActionType.Crash);

    private void SetAction(ActionType type)
    {
        if (!tank) return;
        tank.SetAction(ActionFactory.Create(type, tank));
    }
}
