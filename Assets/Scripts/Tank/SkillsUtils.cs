using UnityEngine;

namespace Tank
{
    public static class SkillsUtils
    {
        public static float CalculateJuggernautDamage(float baseDamage, float totalDamageReceived, IncreaseType increaseType)
        {
            return increaseType switch
            {
                IncreaseType.Asymptotic => 
                    baseDamage + 
                    SmashTanksConstants.JUGGERNAUT_MAX_BONUS *
                    totalDamageReceived / (totalDamageReceived + SmashTanksConstants.JUGGERNAUT_SOFT_CAP),
                IncreaseType.Linear => 
                    baseDamage +
                    SmashTanksConstants.JUGGERNAUT_MULTIPLIER * totalDamageReceived,
                IncreaseType.Exponential => 
                    baseDamage * 
                    Mathf.Pow(1.1f, totalDamageReceived),
                _ => baseDamage
            };
        }
    }

    public enum IncreaseType
    {
        Asymptotic,
        Linear,
        Exponential
    }
}