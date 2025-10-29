namespace UI
{
    public class Ability
    {
        public string Name { get; }
        public bool Active { get; internal set; }

        public event System.Action<bool> OnValueChanged;

        public Ability(string name, bool active = false)
        {
            Name = name;
            Active = active;
        }

        public void Toggle()
        {
            Active = !Active;
            OnValueChanged?.Invoke(Active);
        }
    }
}