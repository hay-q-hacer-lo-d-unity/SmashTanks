using UnityEngine;

namespace Tank
{
    public class TankHealth : TankBarBase
    {
        public float TotalDamageReceived { get; private set; }

        public TankHealth(TankScript tank, GameObject prefab, float maxHealth)
            : base(tank, prefab, maxHealth, Vector3.up * 1.5f) { }

        public void ApplyDamage(float dmg)
        {
            CurrentValue = Mathf.Max(0, CurrentValue - dmg);
            Bar?.Set(CurrentValue, MaxValue);
            TotalDamageReceived += dmg;

            if (CurrentValue <= 0)
                Tank.Kill();
        }

        public void Heal(float amount)
        {
            SetValue(CurrentValue + amount);
        }
    }
}