using UnityEngine;

namespace Tank
{
    public abstract class TankBarBase
    {
        protected readonly TankScript Tank;
        protected readonly BarScript Bar;
        protected readonly float MaxValue;
        protected float CurrentValue;
        protected readonly Vector3 Offset;

        protected TankBarBase(TankScript tank, GameObject prefab, float maxValue, Vector3 offset)
        {
            Tank = tank;
            MaxValue = maxValue;
            CurrentValue = maxValue;
            Offset = offset;

            if (prefab == null)
            {
                Debug.LogError($"{GetType().Name}: Prefab is missing!");
                return;
            }

            // ✅ Always attach to the correct canvas
            var uiCanvas = GameObject.Find("UserInterface")?.GetComponent<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError($"{GetType().Name}: No 'UserInterface' canvas found in the scene!");
                return;
            }

            // ✅ Instantiate and parent under UI
            var instance = Object.Instantiate(prefab, uiCanvas.transform);
            instance.transform.SetAsLastSibling(); // ensures proper draw order

            Bar = instance.GetComponent<BarScript>();
            if (Bar == null)
            {
                Debug.LogError($"{GetType().Name}: Prefab is missing BarScript component!");
                return;
            }

            UpdateBarPosition();
            Bar.Set(CurrentValue, MaxValue);

            Debug.Log($"{tank.name} -> {GetType().Name} bar created successfully!");
        }

        public virtual void Update()
        {
            if (Bar == null || Tank == null)
                return;

            UpdateBarPosition();
        }

        protected void UpdateBarPosition()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(Tank.transform.position + Offset);
            Bar.transform.position = screenPos;
        }

        public virtual void DestroyBar()
        {
            if (Bar != null)
                Bar.DestroyBar();
        }

        public virtual void SetValue(float value)
        {
            CurrentValue = Mathf.Clamp(value, 0, MaxValue);
            Bar?.Set(CurrentValue, MaxValue);
        }

        public float GetValue() => CurrentValue;
    }
}
