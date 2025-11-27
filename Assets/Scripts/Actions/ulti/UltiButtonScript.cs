using System.Collections.Generic;
using SkillsetUI;
using Tank;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Actions.ulti
{
    public class UltiButtonScript : ActionButtonScript
    {
        [SerializeField] private float requiredValue;
        public string requirement;
        
        public override void Initialize(ActionSelectorScript selector, TankScript tank, LegendScript legend)
        {
            base.Initialize(selector, tank, legend);
            if (!Tank) return;
            gameObject.SetActive(Tank.Stats.Abilities.GetValueOrDefault(id, false));
        }
        public override void UpdateState()
        {
            if (!Tank) return;
            if (!Tank.Stats.Abilities.GetValueOrDefault(id, false)) gameObject.SetActive(false);
            Button.interactable = IsReady();
        }

        private bool IsReady() => Tank.UltiCurrentValues.GetValueOrDefault(id, 0f) >= requiredValue;

        private string GetRequirement() => $"{requirement}: {Tank.UltiCurrentValues.GetValueOrDefault(id, 0f):F2}/{requiredValue}";
        
        public override void ShowTooltip() => Legend?.Show(
            displayName,
            description,
            iconImage ? iconImage.sprite : null,
            0,
            cooldown,
            requirement: GetRequirement()
        );
    }
}
