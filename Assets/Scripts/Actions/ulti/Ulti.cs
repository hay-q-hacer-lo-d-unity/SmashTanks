using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Actions.ulti
{
    [System.Serializable]
    public class Ulti
    {
        public Sprite icon;
        public string name;
        public string requirement;
        [TextArea(5, 20)] [SerializeField] public string description;

        protected float RequiredValue;
        protected float CurrentValue;

        public bool IsReady() => CurrentValue >= RequiredValue;

        public void IncreaseValue(float amount) => CurrentValue += amount;
        public void DecreaseValue(float amount) => CurrentValue -= amount;
        public void ResetValue() => CurrentValue = 0f;

        // Subclasses MUST set RequiredValue here
        public virtual void InitializeValues()
        {
        }

        public virtual string GetRequirement() => $"{requirement}: {CurrentValue}/{RequiredValue}";
    }
}