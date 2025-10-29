namespace UI.stat
{ 
    public class Stat
    {
        public string Name { get; }
        public int Value { get; private set; }

        public event System.Action<int> OnValueChanged;

        public Stat(string name, int initialValue = 1)
        {
            Name = name;
            Value = initialValue;
        }

        public void Increase()
        {
            Value++;
            OnValueChanged?.Invoke(Value);
        }

        public void Decrease()
        {
            Value--;
            OnValueChanged?.Invoke(Value);
        }
    
        public void Reset()
        {
            Value = 1;
            OnValueChanged?.Invoke(Value);
        }

    }

}