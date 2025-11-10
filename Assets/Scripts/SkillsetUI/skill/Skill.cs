namespace SkillsetUI.skill
{
    public abstract class Skill<T>
    {
        public string Name { get; }
        private T _value;
        public T Value
        {
            get => _value;
            protected set
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        public event System.Action<T> OnValueChanged;

        protected Skill(string name, T initialValue)
        {
            Name = name;
            _value = initialValue;
        }

        public abstract void Reset();
    }
}