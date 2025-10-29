using System.Collections.Generic;
using Manager;
using TMPro;
using UI.ability;
using UI.stat;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatsManagerScript : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private LegendScript legend;

        [Header("Stats")]
        [SerializeField] private List<StatRowScript> statRows;
        [SerializeField] private int totalPoints = 5;

        [Header("Abilities")]
        [SerializeField] private List<AbilityButtonScript> abilityButtons;
        [SerializeField] private int totalAbilities = 2;

        private StatGroup _statGroup;
        private AbilityGroup _abilityGroup;

        private void Awake()
        {
            _statGroup = new StatGroup(statRows, totalPoints, legend, this);
            _abilityGroup = new AbilityGroup(abilityButtons, totalAbilities, legend, this);

            confirmButton.onClick.AddListener(OnConfirm);
        }

        private void Start()
        {
            _statGroup.Initialize();
            _abilityGroup.Initialize();
        }

        private void OnConfirm()
        {
            var skillset = new Skillset(_statGroup.GetMap(), _abilityGroup.GetMap());
            GameManagerScript.Instance.ConfirmTank(skillset);

            _statGroup.Reset();
            _abilityGroup.Reset();
        }

        // Called by StatGroup to update remaining points UI
        public void UpdatePointsUI(int remaining)
        {
            if (pointsText)
                pointsText.text = remaining.ToString();
        }
    }

    public record Skillset(Dictionary<string, int> StatsMap, Dictionary<string, bool> AbilitiesMap)
    {
        public Dictionary<string, int> StatsMap { get; } = StatsMap;
        public Dictionary<string, bool> AbilitiesMap { get; } = AbilitiesMap;
    }
}