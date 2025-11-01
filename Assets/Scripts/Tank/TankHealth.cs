using Unity.VisualScripting;

namespace Tank
{
    using UnityEngine;

    public class TankHealth
    {
        private readonly TankScript _tank;
        private readonly BarScript _bar;
        private readonly float _maxHealth;
        private float _currentHealth;
        
        public float TotalDamageReceived { get; private set; } = 0f;

        public TankHealth(TankScript tank, GameObject healthBarPrefab, float maxHealth)
        {
            _tank = tank;
            _maxHealth = maxHealth;

            if (!healthBarPrefab) return;
            
            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in scene!");
                return;
            }

            var b = Object.Instantiate(healthBarPrefab, canvas.transform);
            _bar = b.GetComponent<BarScript>();
        }
        
        public void DestroyBar()
        {
            if (_bar != null)
                Object.Destroy(_bar.gameObject);
        }


        public void Update()
        {
            if (!_bar || !_tank) return;

            var screenPos = Camera.main.WorldToScreenPoint(_tank.transform.position + Vector3.up * 1.5f);
            _bar.transform.position = screenPos;
        }

        
        public void SetHealth(float value)
        {
            _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
            _bar?.Set(_currentHealth, _maxHealth);
        }
        
        public void ApplyDamage(float dmg)
        {
            _currentHealth = Mathf.Max(0, _currentHealth - dmg);
            _bar?.Set(_currentHealth, _maxHealth);
            TotalDamageReceived += dmg;
            if (!(_currentHealth <= 0)) return;
            _tank.Kill();
        }
        
        public void Heal(float amount)
        {
            SetHealth(_currentHealth + amount);
        }
    }
}