using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.stat
{
    public class StatRowScript : Tooltipable
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text valueTMP;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;

        public string StatName => name;
        public Stat Stat => _stat;

        private Stat _stat;
        private StatGroup _group;

        public void Initialize(Stat stat, StatGroup group, LegendScript legend)
        {
            _stat = stat;
            _group = group;
            Legend = legend;

            // Update UI whenever the stat value changes
            stat.OnValueChanged += UpdateUI;
            UpdateUI(stat.Value);

            // Connect buttons to the group's logic
            increaseButton.onClick.AddListener(() => _group.TryIncrease(stat));
            decreaseButton.onClick.AddListener(() => _group.TryDecrease(stat));
        }

        private void UpdateUI(int value)
        {
            if (valueTMP)
                valueTMP.text = value.ToString();
        }

        public void SetButtonsInteractable(bool canIncrease, bool canDecrease)
        {
            if (increaseButton)
                increaseButton.interactable = canIncrease;
            if (decreaseButton)
                decreaseButton.interactable = canDecrease;
        }
    }
}