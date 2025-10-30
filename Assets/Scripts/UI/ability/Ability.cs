using UI.skill;

namespace UI.ability
{
    public class Ability : Skill<bool>
    {
        public Ability(string name, bool active = false) : base(name, active) { }

        public void Toggle() => Value = !Value;

        public override void Reset() => Value = false;
    }

}