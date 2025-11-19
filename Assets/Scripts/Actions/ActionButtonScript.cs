using System.Collections.Generic;
using SkillsetUI;
using Tank;
using UnityEngine;
using UnityEngine.UI;

namespace Actions
{
    [RequireComponent(typeof(Button))]
    public class ActionButtonScript : Tooltipable
    {
        private float _magickaCost;
        public int cooldown;
        private Button _button;
        private TankScript _tank;
        private ActionSelectorScript _selector;
        [SerializeField] private Image iconImage;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);
        }

        public void Initialize(ActionSelectorScript selector, TankScript tank, LegendScript legend)
        {
            Legend = legend;
            _selector = selector;
            _tank = tank;
        }
        
        

        private void OnButtonClick()
        {
            if (_tank == null || _selector == null) return;
            _button.Select();
            _selector.SelectAction(name);
        }

        public void UpdateState()
        {
            if (!_tank) return;

            var magicka = _tank.Magicka;
            _magickaCost = _tank.MagickaCosts.GetValueOrDefault(name, 0);
            var cooldowns = _tank.CurrentCooldowns;

            var hasMagicka = magicka >= _magickaCost;
            var notOnCooldown = true;

            if (cooldowns.TryGetValue(name, out var remainingCooldown))
                notOnCooldown = remainingCooldown < 0;

            _button.interactable = hasMagicka && notOnCooldown;
        }
        
        public override void ShowTooltip()
        {
            Legend?.Show(
                name,
                description,
                iconImage ? iconImage.sprite : null,
                _magickaCost,
                cooldown
                );
        }
    }
}