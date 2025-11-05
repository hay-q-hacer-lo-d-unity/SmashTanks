using UnityEngine;

namespace Tank
{
    public class TankMagicka : TankBarBase
    {
        public TankMagicka(TankScript tank, GameObject prefab, float maxMagicka)
            : base(tank, prefab, maxMagicka, Vector3.up * 2f) { }

        public void Spend(float amount)
        {
            CurrentValue = Mathf.Max(0, CurrentValue - amount);
            Bar?.Set(CurrentValue, MaxValue);
        }

        public void Regenerate(float amount)
        {
            SetValue(CurrentValue + amount);
        }
    }
}