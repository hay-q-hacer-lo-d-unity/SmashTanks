
public static class SmashTanksConstants
{

    public static class Stats
    {
        public const float BaseHealth = 16f;
        public const float BaseMass = 8f;
        public const float MassMultiplierPerLevel = 0.2f;

        public const float BaseMagicka = 20f;
        public const float MaxMagickaMultiplierPerLevel = 0.2f;

        public const float MagickaRegenerationRate = 1f;
        public const float MagickaRegenerationRateMultiplierPerLevel = 0.1f;

        public const float BaseIntellect = 1f;
        public const float BaseDamage = 1f;

        public const float BaseAccuracy = 0.5f;
        public const float AccuracyMultiplierPerLevel = 0.4f;

        public const float BaseMendingRateAbsolute = 0.5f;
        public const float BaseMendingRateRelative = BaseHealth * 0.05f;
        public const float MendingRateMultiplierPerLevel = 0.1f;
    }

    public static class Juggernaut
    {
        // Asymptotic (absolute)
        public const float MaxBonus = 10f; // absolute max bonus (non-base-scaled)
        public const float SoftCap = 50f; // controls saturation speed

        // Asymptotic (base-scaled)
        public const float BaseScaledMaxFactor = 0.5f; // at large TDR, add up to 50% of baseDamage
        public const float BaseScaledSoftCap = 50f;

        // Linear
        public const float Multiplier = 0.1f; // additive per TDR (non-base-scaled)
        public const float ScalingFactor = 0.05f; // per-point *baseDamage* contribution for hybrid
        public const float BaseScaledMultiplier = 0.05f; // base-scaled linear multiplier

        // Logarithmic
        public const float LOGMaxBonus = 5f;
        public const float LOGSoftCap = 15f;
        public const float LOGBaseScaledSoftCap = 40f;
        public const float LOGMultiplier = 0.12f;
        public const float LOGScalingFactor = 0.01f;
    }

    #region Weapons / Actions

    public static class Jump
    {
        public const float MaxForceMultiplierPerLevel = 0.2f;
        public const float BaseMaxForce = 100f;
        public const int Cooldown = 0;
    }

    public static class Missile
    {
        public const float BaseDamage = 2f;
        private const float DoublingLevel = 10f;
        private const float DamageMultiplierPerDamage = 1f / DoublingLevel;

        public const float DamageIncreasePerDamage =
            BaseDamage * DamageMultiplierPerDamage;

        public const float ExplosionForce = 25f;
        public const float ExplosionRadius = 5f;
        public const float RecoilForce = 50f;
        public const float MaxInitialSpeed = 20f;
        public const int Cooldown = 0;
    }

    public static class BouncyMissile
    {
        public const float BaseDamage = 3f;
        private const float DoublingLevel = 8f;
        private const float DamageMultiplierPerDamage = 1f / DoublingLevel;

        public const float DamageIncreasePerDamage =
            BaseDamage * DamageMultiplierPerDamage;

        public const float ExplosionForce = 35f;
        public const float ExplosionRadius = 7f;
        public const float RecoilForce = 25f;
        public const float FuseTime = 8f;
        public const float MaxInitialSpeed = 30f;
        public const int Cooldown = 2;
    }
    
    public static class Crash
    {
        public const float BaseDamageMultiplier = 0.025f;
        private const float DoublingLevel = 10f;
        public const float DamageMultiplierPerDamage = 1f / DoublingLevel;
        public const float DamageMultiplierIncreasePerDamage = 0f;
        public const int Cooldown = 2;
    }

    public static class Gale
    {
        public const float BaseForce = 150f;
        private const float DoublingLevel = 6f;
        private const float ForceMultiplierPerIntellect = 1f / DoublingLevel;
        public const float ForceIncreasePerIntellect = BaseForce * ForceMultiplierPerIntellect;
        public const float Radius = 3f;
        public const float Speed = 25f;
        public const float Distance = 25f;
        public const float MagickaCost = 7f;
        public const int Cooldown = 2;
    }

    public static class Beam
    {
        public const float BaseDamage = 0f;
        public const float DamagePerIntellect = 0.5f;
        public const float MagickaCost = 10f;
        public const int Cooldown = 4;
    }

    public static class Teleport
    {
        public const float MagickaCost = 5f;
        private const float MinRadius = 2f;
        public const float MaxRadius = 20f;
        public const float DecayRate = (MaxRadius - MinRadius) / (Config.Statpoints + 1);
        public const int Cooldown = 2;
    }

    #endregion

    public static class Config
    {
        public const int Statpoints = 25;
        public const bool UseCompoundIncrease = false;
    }

    public static class Physics
    {
        public const float Gravity = -9.81f;
    }
}


