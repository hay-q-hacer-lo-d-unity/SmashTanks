using SkillsetUI.skill;

namespace SkillsetUI.stat
{ 
    public class Stat : Skill<int>
    {
        private const int DefaultValue = 1;

        public Stat(string name, int initialValue = DefaultValue) : base(name, initialValue) { }

        public void Increase() => Value++;
        public void Decrease() => Value--;

        public override void Reset() => Value = DefaultValue;
    }
}