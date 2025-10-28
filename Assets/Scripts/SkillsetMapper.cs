using System;
using UI;

public class SkillsetMapper
{
    // Jump
    public float forceMultiplier;
    public float forceMultiplierMultiplier;
    public float maxForce;

    // Mass
    public float mass = 10;
    public float massMultiplier = 0.2f;
    public float health = 10;


    // Accuracy
    public float accuracy = 0.5f;
    public float accuracyMultiplier = 0.05f;


    public SkillsetMapper(Skillset skillset)
    {
        MapAccuracy(skillset._statsMap["Accuracy"]);
        MapMass(skillset._statsMap["Mass"]);
        MapJump(skillset._statsMap["Jump Force"]);
    }

    private void MapJump(int level, bool compound = false)
    {
        MapStat(ref forceMultiplier, level, forceMultiplierMultiplier, compound);
        MapStat(ref maxForce, level, forceMultiplierMultiplier, compound);
    }
    
    private void MapMass(int level, bool compound = false)
    {
        MapStat(ref mass, level, massMultiplier, compound);
        MapStat(ref health, level, massMultiplier, compound);
    }

    private void MapAccuracy(int level, bool compound = false)
    {
        MapStat(ref accuracy, level, accuracyMultiplier, compound);
    }

    private static void MapStat(ref float stat, int level, float multiplier, bool compound = false)
    {
        stat = compound
            ? stat * (float)Math.Pow(1 + multiplier, level)
            : stat + stat * multiplier * level;
    }
}