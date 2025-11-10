using System.Collections.Generic;
using System.Linq;
using SkillsetUI.skill;

namespace SkillsetUI.stat
{
    public class StatGroup : SkillGroup<Stat, int>
    {
        private readonly List<StatRowScript> _rows;
        private readonly int _totalPoints;
        private readonly LegendScript _legend;
        private readonly SkillsManagerScript _manager;

        private int _remainingPoints;

        public StatGroup(List<StatRowScript> rows, int totalPoints, LegendScript legend, SkillsManagerScript manager)
        {
            _rows = rows;
            _totalPoints = totalPoints;
            _legend = legend;
            _manager = manager;
        }

        public override void Initialize()
        {
            _remainingPoints = _totalPoints;
            Skills.Clear();

            foreach (var row in _rows.Where(r => r))
            {
                var stat = new Stat(row.StatName);
                Skills.Add(stat);
                row.Initialize(stat, this, _legend);
            }

            UpdateUI();
            UpdateButtons();
        }

        public bool TryIncrease(Stat stat)
        {
            if (_remainingPoints <= 0) return false;
            stat.Increase();
            _remainingPoints--;
            UpdateUI();
            UpdateButtons();
            return true;
        }

        public bool TryDecrease(Stat stat)
        {
            if (stat.Value <= 1) return false;
            stat.Decrease();
            _remainingPoints++;
            UpdateUI();
            UpdateButtons();
            return true;
        }

        private void UpdateUI() => _manager.UpdateStatpointsUI(_remainingPoints);

        private void UpdateButtons()
        {
            foreach (var row in _rows.Where(r => r))
                row.SetButtonsInteractable(_remainingPoints > 0, row.Stat.Value > 1);
        }

        public override void Reset()
        {
            _remainingPoints = _totalPoints;
            base.Reset();
            UpdateUI();
            UpdateButtons();
        }
    }
}
