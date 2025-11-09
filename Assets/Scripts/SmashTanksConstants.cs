
public static class SmashTanksConstants
{
    public const float BASE_DAMAGE = 2f;
    public const float DAMAGE_MULTIPLIER_PER_LEVEL = 0.15f;
    public const float EXPLOSION_FORCE = 25f;
    public const float EXPLOSION_RADIUS = 5f;
    
    public const float BASE_HEALTH = 15f;
    public const float BASE_MASS = 10f;
    public const float MASS_MULTIPLIER_PER_LEVEL = 0.2f;
    
    public const float BASE_MAGICKA = 20f;
    public const float MAX_MAGICKA_MULTIPLIER_PER_LEVEL = 0.2f;
    
    public const float MAGICKA_REGENERATION_RATE = 1f;
    public const float MAGICKA_REGENERATION_RATE_MULTIPLIER_PER_LEVEL = 0.1f;
    
    public const float BASE_INTELLECT = 1f;
    
    public const float BASE_ACCURACY = 0.5f;
    public const float ACCURACY_MULTIPLIER_PER_LEVEL = 0.4f;
    
    public const float BASE_MENDING_RATE_ABSOLUTE = 0.5f;
    public const float BASE_MENDING_RATE_RELATIVE = BASE_HEALTH * 0.05f;
    public const float MENDING_RATE_MULTIPLIER_PER_LEVEL = 0.1f;
    
    public const float JUMP_FORCE_MULTIPLIER_PER_LEVEL = 0.3f;
    public const float BASE_MAX_JUMP_FORCE = 100f;
    
    // Asymptotic (absolute)
    public const float JUGGERNAUT_MAX_BONUS = 10f;             // absolute max bonus (non-base-scaled)
    public const float JUGGERNAUT_SOFT_CAP = 50f;             // controls saturation speed

    // Asymptotic (base-scaled)
    public const float JUGGERNAUT_BASE_SCALED_MAX_FACTOR = 0.5f; // at large TDR, add up to 50% of baseDamage
    public const float JUGGERNAUT_BASE_SCALED_SOFT_CAP = 50f;

    // Linear
    public const float JUGGERNAUT_MULTIPLIER = 0.02f;         // additive per TDR (non-base-scaled)
    public const float JUGGERNAUT_SCALING_FACTOR = 0.002f;    // per-point *baseDamage* contribution for hybrid
    public const float JUGGERNAUT_BASE_SCALED_MULTIPLIER = 0.002f; // base-scaled linear multiplier
    
    // Logarithmic
    public const float JUGGERNAUT_LOG_MAX_BONUS = 5f;
    public const float JUGGERNAUT_LOG_SOFT_CAP = 15f;
    public const float JUGGERNAUT_LOG_BASE_SCALED_SOFT_CAP = 40f;
    public const float JUGGERNAUT_LOG_MULTIPLIER = 0.12f;
    public const float JUGGERNAUT_LOG_SCALING_FACTOR = 0.01f;
    
    public const float MISSILE_MAX_SPEED = 20f;

    public const float BEAM_DAMAGE_PER_INTELLECT = 0.5f;
    
    public const float BEAM_MAGICKA_COST = 10f;
    public const float TELEPORT_MAGICKA_COST = 5f;
    public const float GALE_MAGICKA_COST = 7f;
    public const float GALE_RADIUS = 3f;
    public const float GALE_SPEED = 25f;
    public const float GALE_DISTANCE = 25f;
    
    public const float GALE_BASE_FORCE = 100f;
    public const float GALE_DOUBLING_LEVEL = 10f;
    public const float GALE_FORCE_MULTIPLIER_PER_INTELLECT = 1f / GALE_DOUBLING_LEVEL;
    public const float GALE_INCREASE_PER_INTELLECT = GALE_BASE_FORCE * GALE_FORCE_MULTIPLIER_PER_INTELLECT;

    public const float GALE_FORCE_PER_INTELLECT = 50f;
    
    public const float MISSILE_RECOIL_FORCE = 50f;
    public const float BOUNCY_MISSILE_RECOIL_FORCE = 25f;
    public const float BOUNCY_MISSILE_FUSE_TIME = 8f;
    
    public const int STATPOINTS = 25;
    public const bool USE_COMPOUND_INCREASE = false;
    public const float GRAVITY = -9.81f;
}


