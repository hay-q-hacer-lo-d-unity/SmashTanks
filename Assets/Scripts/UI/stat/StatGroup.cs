using System.Collections.Generic;
using System.Linq;

namespace UI.stat
{
    public class StatGroup
    {
        private readonly List<StatRowScript> _rows;
        private readonly int _totalPoints;
        private readonly LegendScript _legend;
        private readonly StatsManagerScript _manager;

        private readonly List<Stat> _stats = new();
        private int _remainingPoints;
        private Dictionary<string, int> _statsMap;

        public StatGroup(List<StatRowScript> rows, int totalPoints, LegendScript legend, StatsManagerScript manager)
        {
            _rows = rows;
            _totalPoints = totalPoints;
            _legend = legend;
            _manager = manager;
        }

        public void Initialize()
        {
            _remainingPoints = _totalPoints;
            _stats.Clear();

            foreach (var row in _rows.Where(r => r))
            {
                var stat = new Stat(row.StatName);
                _stats.Add(stat);
                row.Initialize(stat, this, _legend);
            }

            RebuildMap();
            UpdateUI();
        }

        private void RebuildMap() => _statsMap = _stats.ToDictionary(s => s.Name, s => s.Value);

        public bool TryIncrease(Stat stat)
        {
            if (_remainingPoints <= 0) return false;
            stat.Increase();
            _remainingPoints--;
            SyncStat(stat);
            return true;
        }

        public bool TryDecrease(Stat stat)
        {
            if (stat.Value <= 1) return false;
            stat.Decrease();
            _remainingPoints++;
            SyncStat(stat);
            return true;
        }

        private void SyncStat(Stat stat)
        {
            _statsMap[stat.Name] = stat.Value;
            UpdateUI();
            UpdateButtons();
        }

        private void UpdateUI() => _manager.UpdatePointsUI(_remainingPoints);

        private void UpdateButtons()
        {
            foreach (var row in _rows.Where(r => r))
                row.SetButtonsInteractable(_remainingPoints > 0, row.Stat.Value > 1);
        }

        public void Reset()
        {
            _remainingPoints = _totalPoints;
            foreach (var stat in _stats) stat.Reset();
            RebuildMap();
            UpdateUI();
            UpdateButtons();
        }

        public Dictionary<string, int> GetMap() => new(_statsMap);
    }
}
