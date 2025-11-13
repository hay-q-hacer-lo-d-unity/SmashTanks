using System;
using UnityEngine;

namespace Tank
{
    public static class SkillsUtils
    {
        public static float CalculateJuggernautDamage(float baseDamage, float totalDamageReceived, IncreaseType increaseType)
        {
            const float maxBonus = SmashTanksConstants.Juggernaut.MaxBonus;
            const float softCap = SmashTanksConstants.Juggernaut.SoftCap;
            const float baseScaledSoftCap = SmashTanksConstants.Juggernaut.BaseScaledSoftCap;
            const float baseScaledMaxFactor = SmashTanksConstants.Juggernaut.BaseScaledMaxFactor;
            const float multiplier = SmashTanksConstants.Juggernaut.Multiplier;
            const float baseScaledMultiplier = SmashTanksConstants.Juggernaut.BaseScaledMultiplier;
            const float scalingFactor = SmashTanksConstants.Juggernaut.ScalingFactor;
            const float logMaxBonus = SmashTanksConstants.Juggernaut.LOGMaxBonus;
            const float logSoftCap = SmashTanksConstants.Juggernaut.LOGSoftCap;
            const float logBaseScaledSoftCap = SmashTanksConstants.Juggernaut.LOGBaseScaledSoftCap;
            const float logMultiplier = SmashTanksConstants.Juggernaut.LOGMultiplier;
            const float logScalingFactor = SmashTanksConstants.Juggernaut.LOGScalingFactor;

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
                    // componente constante + componente dependiente de baseDamage
                    (multiplier + baseDamage * scalingFactor) * totalDamageReceived,
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