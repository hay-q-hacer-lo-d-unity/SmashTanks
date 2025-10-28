namespace Tank
{
    using UnityEngine;

    public class TankHealth
    {
        private readonly TankScript _tank;
        private readonly HealthBarScript _bar;
        private readonly float _maxHealth;

        private float _currentHealth;

        public TankHealth(TankScript tank, GameObject healthBarPrefab, float maxHealth)
        {
            _tank = tank;
            _maxHealth = maxHealth;

            if (!healthBarPrefab) return;
            var hb = Object.Instantiate(healthBarPrefab, _tank.transform.position + Vector3.up * 1.5f, Quaternion.identity, _tank.transform);
            _bar = hb.GetComponent<HealthBarScript>();
        }

        public void SetHealth(float value)
        {
            Debug.Log("Set health to: "+value);
            _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
            _bar?.SetHealth(_currentHealth, _maxHealth);
        }

        public void ApplyDamage(float dmg)
        {
            _currentHealth = Mathf.Max(0, _currentHealth - dmg);
            _bar?.SetHealth(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                Object.Destroy(_tank.gameObject);
            }
        }
    }

}