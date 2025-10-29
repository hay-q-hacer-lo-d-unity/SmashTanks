using UnityEngine;
using UnityEngine.EventSystems;
using Manager;

namespace Tank
{
    public class TankInputHandler
    {
        private readonly TankScript _tank;
        private readonly TurnManagerScript _turnManager;

        public TankInputHandler(TankScript tank, TurnManagerScript turnManager)
        {
            _tank = tank;
            _turnManager = turnManager;
        }

        public bool CanAct()
        {
            if (EventSystem.current?.IsPointerOverGameObject() == true) return false;
            if (!_turnManager || !_turnManager.IsPlanningPhase()) return false;
            return !_turnManager.HasAction(_tank.OwnerId);
        }

        public bool TryGetActionTarget(out Vector2 target)
        {
            target = Vector2.zero;
            if (!Input.GetMouseButtonDown(0)) return false;
            target = GetMouseWorld();
            return true;
        }

        private static Vector2 GetMouseWorld()
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector2(worldPos.x, worldPos.y);
        }
    }
}