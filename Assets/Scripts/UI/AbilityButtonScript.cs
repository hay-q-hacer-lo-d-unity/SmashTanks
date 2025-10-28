using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityButtonScript : Tooltipable
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject backgroundImage;

        private bool _isSelected;
        private Ability _ability;

        public void Initialize(Ability ability, StatsManagerScript manager, LegendScript legend)
        {
            _ability = ability;
            Legend = legend;
            backgroundImage.SetActive(false);

            button.onClick.AddListener(() =>{
                if (!manager.TryAbility(ability)) return;
                _isSelected = !_isSelected;
                Debug.Log(_isSelected);
                UpdateVisual();
            });
        }

        public new void ShowTooltip()
        {
            var backgrounds = GetComponentsInChildren<Image>();
            Legend?.Show(name, description, backgrounds.Last().sprite);
        }

        private void UpdateVisual()
        {
            backgroundImage.SetActive(_isSelected);
        }

        public void SetSelected(bool value)
        {
            _isSelected = value;
            UpdateVisual();
        }
    }
}