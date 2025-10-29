using System.Collections.Generic;
using System.Linq;
using UI.ability;
using UnityEngine;

namespace UI
{
    public class AbilityGroup
    {
        private readonly List<AbilityButtonScript> _buttons;
        private readonly int _totalAbilities;
        private readonly LegendScript _legend;
        private readonly StatsManagerScript _manager;

        private int _remainingAbilities;
        private Dictionary<string, bool> _map;

        public AbilityGroup(List<AbilityButtonScript> buttons, int totalAbilities, LegendScript legend, StatsManagerScript manager)
        {
            _buttons = buttons;
            _totalAbilities = totalAbilities;
            _legend = legend;
            _manager = manager;
        }

        public void Initialize()
        {
            _remainingAbilities = _totalAbilities;
            foreach (var button in _buttons.Where(b => b))
            {
                var ability = new Ability(button.name);
                button.Initialize(ability, this, _legend);
            }
            _map = _buttons.ToDictionary(b => b.name, _ => false);
        }

        public bool TryToggle(Ability ability)
        {
            if (ability.Active)
            {
                ability.Toggle();
                _remainingAbilities++;
                _map[ability.Name] = false;
                return true;
            }

            if (_remainingAbilities <= 0) return false;
            ability.Toggle();
            _remainingAbilities--;
            _map[ability.Name] = true;
            return true;
        }

        public void Reset()
        {
            _remainingAbilities = _totalAbilities;
            _map = _buttons.ToDictionary(b => b.name, _ => false);
        }

        public Dictionary<string, bool> GetMap() => new(_map);
    }
}