using System.Collections.Generic;
using System.Linq;
using SkillsetUI.skill;

namespace SkillsetUI.ability
{
    public class AbilityGroup : SkillGroup<Ability, bool>
    {
        private readonly List<AbilityButtonScript> _buttons;
        private readonly int _maxActiveAbilities;
        private readonly LegendScript _legend;

        private int _remainingAbilities;

        public AbilityGroup(List<AbilityButtonScript> buttons, int maxActiveAbilities, LegendScript legend)
        {
            _buttons = buttons;
            _maxActiveAbilities = maxActiveAbilities;
            _legend = legend;
        }

        public override void Initialize()
        {
            _remainingAbilities = _maxActiveAbilities;
            Skills.Clear();

            foreach (var button in _buttons.Where(b => b))
            {
                var ability = new Ability(button.AbilityName);
                Skills.Add(ability);
                button.Initialize(ability, this, _legend);
            }
        }

        public bool TryToggle(Ability ability)
        {
            if (ability.Value)
            {
                ability.Toggle();
                _remainingAbilities++;
                return true;
            }

            if (_remainingAbilities <= 0)
                return false;

            ability.Toggle();
            _remainingAbilities--;
            return true;
        }

        public override void Reset()
        {
            _remainingAbilities = _maxActiveAbilities;
            base.Reset(); // resets all abilities to inactive
        }
    }
}