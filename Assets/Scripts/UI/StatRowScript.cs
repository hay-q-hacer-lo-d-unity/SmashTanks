namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class StatRowScript : Tooltipable
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text valueTMP;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;

        public string StatName => name;
        public Stat Stat => _stat;

        private Stat _stat;

        public void Initialize(Stat stat, StatsManagerScript manager, LegendScript legend)
        {
            _stat = stat;
            Legend = legend;

            stat.OnValueChanged += UpdateUI;
            UpdateUI(stat.Value);

            increaseButton.onClick.AddListener(() => manager.TryIncreaseStat(stat));
            decreaseButton.onClick.AddListener(() => manager.TryDecreaseStat(stat));
        }

        internal void UpdateUI(int value)
        {
            valueTMP.text = value.ToString();
        }

        public void SetButtonsInteractable(bool canIncrease, bool canDecrease)
        {
            increaseButton.interactable = canIncrease;
            decreaseButton.interactable = canDecrease;
        }
    }
}
