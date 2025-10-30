using System.Collections.Generic;
using System.Linq;

namespace UI.skill
{
    public abstract class SkillGroup<TSkill, TValue>
        where TSkill : Skill<TValue>
    {
        protected readonly List<TSkill> Skills = new();
        public IReadOnlyDictionary<string, TValue> Map => Skills.ToDictionary(s => s.Name, s => s.Value);

        public abstract void Initialize();
        public virtual void Reset()
        {
            foreach (var skill in Skills)
                skill.Reset();
        }
    }

}