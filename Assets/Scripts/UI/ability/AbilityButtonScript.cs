using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ability
{
    public class AbilityButtonScript : Tooltipable
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject backgroundImage;

        private Ability _ability;
        private AbilityGroup _group;
        private bool _isSelected;

        public void Initialize(Ability ability, AbilityGroup group, LegendScript legend)
        {
            _ability = ability;
            _group = group;
            Legend = legend;
            _isSelected = false;
            backgroundImage.SetActive(false);

            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (_group.TryToggle(_ability))
            {
                _isSelected = _ability.Active;
                UpdateVisual();
            }
        }

        private void UpdateVisual()
        {
            if (backgroundImage)
                backgroundImage.SetActive(_isSelected);
        }

        public void SetSelected(bool value)
        {
            _isSelected = value;
            _ability.Active = value;
            UpdateVisual();
        }

        public new void ShowTooltip()
        {
            var backgrounds = GetComponentsInChildren<Image>();
            Legend?.Show(name, description, backgrounds.Last().sprite);
        }
    }
}