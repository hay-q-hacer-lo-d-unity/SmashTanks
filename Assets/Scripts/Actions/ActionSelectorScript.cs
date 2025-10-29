using Tank;
using UnityEngine;

namespace Actions
{
    public class ActionSelectorScript : MonoBehaviour
    {
        private TankScript _tank;
    
        public void SetTank(TankScript newTank) => this._tank = newTank;

        public void SelectMissile() => SetAction(ActionType.Missile);
        public void SelectJump() => SetAction(ActionType.Jump);
        public void SelectCrash() => SetAction(ActionType.Crash);

        private void SetAction(ActionType type)
        {
            if (!_tank) return;
            _tank.SetAction(ActionFactory.Create(type, _tank));
        }
    }
}
