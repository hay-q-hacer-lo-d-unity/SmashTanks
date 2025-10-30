using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ability
{
    public class AbilityButtonScript : Tooltipable
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject backgroundImage;

        public string AbilityName => name;

        private Ability _ability;
        private AbilityGroup _group;

        public void Initialize(Ability ability, AbilityGroup group, LegendScript legend)
        {
            _ability = ability;
            _group = group;
            Legend = legend;

            // Subscribe to ability state changes
            _ability.OnValueChanged += UpdateVisual;
            UpdateVisual(_ability.Value);

            button.onClick.AddListener(() => _group.TryToggle(_ability));
        }

        private void UpdateVisual(bool isActive)
        {
            if (backgroundImage)
                backgroundImage.SetActive(isActive);
        }

        public new void ShowTooltip()
        {
            var backgrounds = GetComponentsInChildren<Image>();
            Legend?.Show(name, description, backgrounds.Last().sprite);
        }
    }
}