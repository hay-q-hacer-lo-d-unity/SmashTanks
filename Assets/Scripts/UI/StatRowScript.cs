using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class StatRowScript : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text valueTMP;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;

        [Header("Stat Info")]
        [SerializeField] private string statName = "New Stat";
        [TextArea] [SerializeField] private string description = "Stat description";

        public string StatName => statName;
        public Stat Stat => stat;

        private Stat stat;
        private StatsManagerScript manager;
        private StatLegendScript legend;

        public void Initialize(Stat stat, StatsManagerScript manager, StatLegendScript legend)
        {
            this.stat = stat;
            this.manager = manager;
            this.legend = legend;

            stat.OnValueChanged += UpdateUI;
            UpdateUI(stat.Value);

            increaseButton.onClick.AddListener(() => manager.TryIncrease(stat));
            decreaseButton.onClick.AddListener(() => manager.TryDecrease(stat));
        }

        private void UpdateUI(int value)
        {
            valueTMP.text = value.ToString();
        }

        public void SetButtonsInteractable(bool canIncrease, bool canDecrease)
        {
            increaseButton.interactable = canIncrease;
            decreaseButton.interactable = canDecrease;
        }

        public void ShowTooltip()
        {
            var backgroundImage = GetComponentInChildren<Image>();
            legend?.Show(statName, description, backgroundImage.sprite);
        }

        public void HideTooltip()
        {
            legend?.Hide();
        }
    }
}
