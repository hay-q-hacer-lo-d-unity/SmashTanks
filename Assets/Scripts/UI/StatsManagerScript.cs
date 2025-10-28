using System.Linq;
using Manager;
using UnityEngine.Rendering;
using UnityEngine.UI;


namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class StatsManagerScript : MonoBehaviour
    {
        [SerializeField] public GameObject panel;
        
        [Header("Stats")]
        [SerializeField] private List<StatRowScript> statRows; 
        [SerializeField] private int totalPoints;
        private int _remainingPoints;
        [SerializeField] private TMP_Text pointsText;
        public Dictionary<string, int> StatsMap;
        

        [Header("Abilities")]
        [SerializeField] private List<AbilityButtonScript> abilityButtons;
        [SerializeField] private int totalAbilities;
        private int _remainingAbilities;
        public Dictionary<string, bool> AbilitiesMap;
        
        [Header("References")]
        [SerializeField] private LegendScript legend;
        [SerializeField] public Button confirmButton;

        private readonly List<Stat> _stats = new();
        private List<Skillset> _skillsets;
        
        private void Start()
        {
            _remainingPoints = totalPoints;
            UpdatePointsUI();

            foreach (var row in statRows)
            {
                if (!row) continue;
                var stat = new Stat(row.StatName);
                _stats.Add(stat);

                row.Initialize(stat, this, legend);
                UpdateStatRowButtons(row, stat);
            }
            StatsMap = _stats.ToDictionary(stat => stat.Name, _ => 1);

            _remainingAbilities = totalAbilities;
            foreach (var button in abilityButtons)
            {
                if (!button) continue;
                var ability = new Ability(button.name);
                Debug.Log(legend);
                button.Initialize(ability, this, legend);
            }
            AbilitiesMap = abilityButtons.ToDictionary(button => button.name, _ => false);
            
            confirmButton.onClick.AddListener(() =>
            {
                GameManagerScript.Instance.ConfirmTank(new Skillset(StatsMap, AbilitiesMap));

                ClearSkillset();
            });
        }

        private void ClearSkillset()
        {
            _remainingAbilities = totalAbilities;
            _remainingPoints = totalPoints;

            foreach (var stat in _stats)
            {
                stat.Reset();
            }

            ClearAbilitiesMap();
            ClearStatsMap();
            UpdatePointsUI();
            UpdateAllStatRows();
        }

        
        private void ClearStatsMap()
        {
            StatsMap = _stats.ToDictionary(stat => stat.Name, _ => 1);
        }
        private void ClearAbilitiesMap()
        {
            AbilitiesMap = abilityButtons.ToDictionary(button => button.name, _ => false);
        }
        
        private void UpdatePointsUI()
        {
            if (pointsText)
                pointsText.text = _remainingPoints.ToString();
        }

        // Called by StatRowScript
        public bool TryIncreaseStat(Stat stat)
        {
            if (_remainingPoints <= 0) return false;
            StatsMap[stat.Name] = stat.Value + 1;
            stat.Increase();
            _remainingPoints--;
            UpdatePointsUI();
            UpdateAllStatRows();
            return true;
        }

        public bool TryDecreaseStat(Stat stat)
        {
            if (stat.Value <= 1) return false;
            StatsMap[stat.Name] = stat.Value - 1;
            stat.Decrease();
            _remainingPoints++;
            UpdatePointsUI();
            UpdateAllStatRows();
            return true;
        }

        private void UpdateAllStatRows()
        {
            foreach (var row in statRows.Where(row => row))
            {
                UpdateStatRowButtons(row, row.Stat);
            }
        }

        private void UpdateStatRowButtons(StatRowScript row, Stat stat)
        {
            row.SetButtonsInteractable(
                canIncrease: _remainingPoints > 0,
                canDecrease: stat.Value > 1
            );
        }
        
        public bool TryAbility(Ability ability)
        {
            if (ability.Active)
            {
                ability.Toggle();
                _remainingAbilities++;
                AbilitiesMap[ability.Name] = false;
                return true;
            }
            if (_remainingAbilities <= 0) return false;
            ability.Toggle();
            _remainingAbilities--;
            AbilitiesMap[ability.Name] = true;
            return true;
        }
    }

    public record Skillset
    {
        public Dictionary<string, int> _statsMap { private set; get; }
        public Dictionary<string, bool> _abilitiesMap {private set; get; }

        public Skillset(Dictionary<string, int> statsMap, Dictionary<string, bool> abilities)
        {
            _statsMap = statsMap;
            _abilitiesMap = abilities;
        }

        public Skillset()
        {
            _statsMap = new Dictionary<string, int>();
            _abilitiesMap = new Dictionary<string, bool>();
        }
    }
}
