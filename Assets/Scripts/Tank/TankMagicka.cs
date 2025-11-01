using UnityEngine;

namespace Tank
{
    public class TankMagicka
    {
        private readonly TankScript _tank;
        private readonly BarScript _bar;
        private readonly float _maxMagicka;
        private float _currentMagicka;
        
        public TankMagicka(TankScript tank, GameObject barPrefab, float maxMagicka)
        {
            _tank = tank;
            _maxMagicka = maxMagicka;

            if (!barPrefab) return;

            var canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in scene!");
                return;
            }

            var b = Object.Instantiate(barPrefab, canvas.transform);
            _bar = b.GetComponent<BarScript>();
        }

        public void Update()
        {
            if (!_bar || !_tank) return;

            var screenPos = Camera.main.WorldToScreenPoint(_tank.transform.position + Vector3.up * 2f);
            _bar.transform.position = screenPos;
        }
        
        public void DestroyBar()
        {
            if (_bar != null)
                Object.Destroy(_bar.gameObject);
        }

        
        public void SetMagicka(float value)
        {
            _currentMagicka = Mathf.Clamp(value, 0, _maxMagicka);
            _bar?.Set(_currentMagicka, _maxMagicka);
        }
        
        public void Spend(float dmg)
        {
            _currentMagicka = Mathf.Max(0, _currentMagicka - dmg);
            _bar?.Set(_currentMagicka, _maxMagicka);
        }
        
        public void Regenerate(float amount) => SetMagicka(_currentMagicka + amount);
    }
}