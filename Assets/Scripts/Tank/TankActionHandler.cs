namespace Tank
{
    using UnityEngine;

    public class TankActionHandler
    {
        private readonly TankScript _tank;
        private readonly GameObject _projectilePrefab;
        private readonly Transform _firePoint;
        private readonly Transform _aimPoint;
        private readonly Rigidbody2D _rb;

        public IAction DefaultAction { get; }

        public TankActionHandler(TankScript tank, GameObject projectilePrefab, Transform firePoint, Transform aimPoint, Rigidbody2D rb)
        {
            _tank = tank;
            _projectilePrefab = projectilePrefab;
            _firePoint = firePoint;
            _aimPoint = aimPoint;
            _rb = rb;

            DefaultAction = new MissileAction(_projectilePrefab, 5f, 20f, _aimPoint, _firePoint, _rb);
        }
    }

}