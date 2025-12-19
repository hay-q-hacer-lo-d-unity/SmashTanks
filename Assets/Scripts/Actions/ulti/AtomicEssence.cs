namespace Actions.ulti
{
    [System.Serializable]
    public class AtomicEssence : Ulti
    {
        public override void InitializeValues()
        {
            RequiredValue = SmashTanksConstants.AtomicEssenceUlti.RequiredMagickaSaved;
        }

        public override string GetRequirement() => $"Total magicka used: {CurrentValue} / {RequiredValue}";
    }

}