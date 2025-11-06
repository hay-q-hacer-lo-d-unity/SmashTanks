using UnityEngine;

namespace Tank
{
    public abstract class TankBarBase
    {
        protected readonly TankScript Tank;
        protected readonly BarScript Bar;
        protected readonly float MaxValue;
        protected float CurrentValue;
        private readonly Vector3 _offset;

        protected TankBarBase(TankScript tank, GameObject prefab, float maxValue, Vector3 offset)
        {
            Tank = tank;
            MaxValue = maxValue;
            CurrentValue = maxValue;
            _offset = offset;

            if (prefab == null)
            {
                Debug.LogError($"{GetType().Name}: Prefab is missing!");
                return;
            }

            var uiCanvas = GameObject.Find("UserInterface")?.GetComponent<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError($"{GetType().Name}: No 'UserInterface' canvas found in the scene!");
                return;
            }

            var instance = Object.Instantiate(prefab, uiCanvas.transform);
            instance.transform.SetAsLastSibling();

            Bar = instance.GetComponent<BarScript>();
            if (Bar == null) return;

            UpdateBarPosition();
            Bar.Set(CurrentValue, MaxValue); 
        }

        public virtual void Update()
        {
            if (!Bar || !Tank) return;
            UpdateBarPosition();
        }

        private void UpdateBarPosition()
        {
            var screenPos = Camera.main.WorldToScreenPoint(Tank.transform.position + _offset);
            Bar.transform.position = screenPos;
        }

        public virtual void DestroyBar()
        {
            if (Bar != null) Bar.DestroyBar();
        }

        public virtual void SetValue(float value)
        {
            CurrentValue = Mathf.Clamp(value, 0, MaxValue);
            Bar?.Set(CurrentValue, MaxValue);
        }

        public float GetValue() => CurrentValue;
    }
}
