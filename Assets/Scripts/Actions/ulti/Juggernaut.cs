namespace Actions.ulti
{
    [System.Serializable]
    public class Juggernaut : Ulti
    {
        public override void InitializeValues()
        {
            RequiredValue = SmashTanksConstants.JuggernautUlti.RequiredTdr;
        }

        public override string GetRequirement() => "Total damage received: " + CurrentValue + " / " + RequiredValue;
    }
}