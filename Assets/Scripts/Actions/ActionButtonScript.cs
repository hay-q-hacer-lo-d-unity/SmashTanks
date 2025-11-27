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
        protected Button Button;
        protected TankScript Tank;
        private ActionSelectorScript _selector;
        [SerializeField] protected Image iconImage;

        protected void Awake()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(OnButtonClick);
        }

        public virtual void Initialize(ActionSelectorScript selector, TankScript tank, LegendScript legend)
        {
            Legend = legend;
            _selector = selector;
            Tank = tank;
        }
        
        

        private void OnButtonClick()
        {
            if (Tank == null || _selector == null) return;
            Button.Select();
            _selector.SelectAction(id);
        }

        public virtual void UpdateState()
        {
            if (!Tank) return;

            var magicka = Tank.Magicka;
            _magickaCost = Tank.MagickaCosts.GetValueOrDefault(id, 0);
            var cooldowns = Tank.CurrentCooldowns;

            var hasMagicka = magicka >= _magickaCost;
            var notOnCooldown = true;

            if (cooldowns.TryGetValue(id, out var remainingCooldown))
                notOnCooldown = remainingCooldown < 0;

            Button.interactable = hasMagicka && notOnCooldown;
        }
        
        public override void ShowTooltip()
        {
            Legend?.Show(
                displayName,
                description,
                iconImage ? iconImage.sprite : null,
                _magickaCost,
                cooldown
                );
        }
    }
}