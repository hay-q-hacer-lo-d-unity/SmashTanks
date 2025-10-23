using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class StatsManagerScript : MonoBehaviour
    {
        [SerializeField] private List<StatRowScript> statRows; 
        [SerializeField] private int totalPoints;
        [SerializeField] private TMP_Text pointsText;
        [SerializeField] private StatLegendScript statLegend;

        private int remainingPoints;
        private readonly List<Stat> stats = new();

        private void Start()
        {
            remainingPoints = totalPoints;
            UpdatePointsUI();

            foreach (var row in statRows)
            {
                if (!row) continue;
                var stat = new Stat(row.StatName);
                stats.Add(stat);

                row.Initialize(stat, this, statLegend);
                UpdateRowButtons(row, stat);
            }
        }

        private void UpdatePointsUI()
        {
            if (pointsText)
                pointsText.text = remainingPoints.ToString();
        }

        // Called by StatRowScript
        public bool TryIncrease(Stat stat)
        {
            if (remainingPoints <= 0) return false;

            stat.Increase();
            remainingPoints--;
            UpdatePointsUI();
            UpdateAllRowButtons();
            return true;
        }

        public bool TryDecrease(Stat stat)
        {
            if (stat.Value <= 1) return false;

            stat.Decrease();
            remainingPoints++;
            UpdatePointsUI();
            UpdateAllRowButtons();
            return true;
        }

        private void UpdateAllRowButtons()
        {
            foreach (var row in statRows.Where(row => row))
            {
                UpdateRowButtons(row, row.Stat);
            }
        }

        private void UpdateRowButtons(StatRowScript row, Stat stat)
        {
            row.SetButtonsInteractable(
                canIncrease: remainingPoints > 0,
                canDecrease: stat.Value > 1
            );
        }
    }
}

