using System;
using UnityEngine;

namespace Tank
{
    public static class SkillsUtils
    {
        public static float CalculateJuggernautDamage(float baseDamage, float totalDamageReceived, IncreaseType increaseType)
        {
            const float maxBonus = SmashTanksConstants.JUGGERNAUT_MAX_BONUS;
            const float softCap = SmashTanksConstants.JUGGERNAUT_SOFT_CAP;
            const float baseScaledSoftCap = SmashTanksConstants.JUGGERNAUT_BASE_SCALED_SOFT_CAP;
            const float baseScaledMaxFactor = SmashTanksConstants.JUGGERNAUT_BASE_SCALED_MAX_FACTOR;
            const float multiplier = SmashTanksConstants.JUGGERNAUT_MULTIPLIER;
            const float baseScaledMultiplier = SmashTanksConstants.JUGGERNAUT_BASE_SCALED_MULTIPLIER;
            const float scalingFactor = SmashTanksConstants.JUGGERNAUT_SCALING_FACTOR;
            const float logMaxBonus = SmashTanksConstants.JUGGERNAUT_LOG_MAX_BONUS;
            const float logSoftCap = SmashTanksConstants.JUGGERNAUT_LOG_SOFT_CAP;
            const float logBaseScaledSoftCap = SmashTanksConstants.JUGGERNAUT_LOG_BASE_SCALED_SOFT_CAP;
            const float logMultiplier = SmashTanksConstants.JUGGERNAUT_LOG_MULTIPLIER;
            const float logScalingFactor = SmashTanksConstants.JUGGERNAUT_LOG_SCALING_FACTOR;

            return baseDamage + increaseType switch
            {
                IncreaseType.Asymptotic => 
                    maxBonus * totalDamageReceived / (totalDamageReceived + softCap),
                
                IncreaseType.AsymptoticBaseScaled => 
                    baseDamage * baseScaledMaxFactor * totalDamageReceived / (totalDamageReceived + baseScaledSoftCap),
                
                IncreaseType.Linear => 
                    multiplier * totalDamageReceived,
                
                IncreaseType.LinearBaseScaled =>
                    baseDamage * baseScaledMultiplier * totalDamageReceived,
                
                IncreaseType.LinearHybrid => 
                    (multiplier + baseDamage * scalingFactor) * totalDamageReceived,
                  // componente constante + componente dependiente de baseDamage
                IncreaseType.Logarithmic =>
                    logMaxBonus * Mathf.Log(1 + totalDamageReceived / logSoftCap),

               IncreaseType.LogarithmicBaseScaled =>
                   baseDamage * logMaxBonus * Mathf.Log(1 + totalDamageReceived / logBaseScaledSoftCap),

               IncreaseType.LogarithmicHybrid =>
                   (logMultiplier + baseDamage * logScalingFactor) * Mathf.Log(1 + totalDamageReceived),
                
                _ => throw new ArgumentOutOfRangeException(nameof(increaseType), increaseType, null)
            };
        }
    }
    public enum IncreaseType
    {
        Asymptotic,
        AsymptoticBaseScaled,
        Linear,
        LinearBaseScaled,
        LinearHybrid,
        Logarithmic,
        LogarithmicBaseScaled,
        LogarithmicHybrid
    }
}