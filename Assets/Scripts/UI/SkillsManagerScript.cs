using System.Collections.Generic;
using Manager;
using TMPro;
using UI.ability;
using UI.stat;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillsManagerScript : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text statpointsText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private LegendScript legend;

        [Header("Stats")]
        [SerializeField] private List<StatRowScript> statRows;
        [SerializeField] private int totalStatPoints = SmashTanksConstants.STATPOINTS;

        [Header("Abilities")]
        [SerializeField] private List<AbilityButtonScript> abilityButtons;
        [SerializeField] private int maxActiveAbilities = 2;

        private StatGroup _statGroup;
        private AbilityGroup _abilityGroup;

        private void Start()
        {
            _statGroup = new StatGroup(statRows, totalStatPoints, legend, this);
            _abilityGroup = new AbilityGroup(abilityButtons, maxActiveAbilities, legend);

            _statGroup.Initialize();
            _abilityGroup.Initialize();

            confirmButton.onClick.AddListener(OnConfirm);
        }

        private void OnConfirm()
        {
            var skillset = new Skillset(_statGroup.Map, _abilityGroup.Map);
            GameManagerScript.Instance.ConfirmTank(skillset);

            _statGroup.Reset();
            _abilityGroup.Reset();
        }

        public void UpdateStatpointsUI(int remaining)
        {
            statpointsText.text = remaining.ToString();
        }
    }

    public record Skillset(
        IReadOnlyDictionary<string, int> StatsMap,
        IReadOnlyDictionary<string, bool> AbilitiesMap
    )
    {
        public IReadOnlyDictionary<string, int> StatsMap { get; } = StatsMap;
        public IReadOnlyDictionary<string, bool> AbilitiesMap { get; } = AbilitiesMap;
    }
}