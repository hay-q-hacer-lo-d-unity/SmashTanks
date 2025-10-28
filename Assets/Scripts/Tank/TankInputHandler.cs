namespace Tank
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class TankInputHandler
    {
        private readonly TankScript _tank;
        private readonly TrajectoryDrawerScript _trajectoryDrawer;
        private readonly CanonOrbitAndAim _canon;
        private readonly TurnManagerScript _turnManager;

        public TankInputHandler(TankScript tank, TrajectoryDrawerScript trajectoryDrawer, CanonOrbitAndAim canon, TurnManagerScript turnManager)
        {
            _tank = tank;
            _trajectoryDrawer = trajectoryDrawer;
            _canon = canon;
            _turnManager = turnManager;
        }

        public void UpdateInput()
        {
            if (_turnManager == null || !_turnManager.IsPlanningPhase()) return;
            if (_turnManager.HasAction(_tank.ownerId)) return;
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;

            UpdateTrajectory();
            if (!_canon.canMove) _canon.canMove = true;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 cursorPosition = new(mouseWorld.x, mouseWorld.y);

                _turnManager.RegisterAction(_tank.ownerId, _tank.CurrentAction.GetName(), cursorPosition);
                _canon.canMove = false;
            }
        }

        private void UpdateTrajectory()
        {
            if (!_trajectoryDrawer) return;

            Vector2 initialVelocity = TankPhysicsHelper.CalculateInitialVelocity(_tank, _tank.CurrentAction.GetName());
            Vector3 origin = _tank.CurrentAction.GetName() == "Shoot"
                ? _tank.transform.Find("FirePoint").position
                : _tank.transform.Find("AimPoint").position;

            _trajectoryDrawer.DrawParabola(origin, initialVelocity, _tank.accuracy);
        }
    }

}