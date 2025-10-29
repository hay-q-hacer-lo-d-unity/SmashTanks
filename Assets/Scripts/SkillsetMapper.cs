using System;
using UI;

public class SkillsetMapper
{
    // Jump
    public float ForceMultiplier = 20f;
    private const float ForceMultiplierMultiplier = 2f;
    public float MaxForce = 200f;

    // Mass
    public float Mass = 10;
    private const float MassMultiplier = 0.2f;
    public float Health = 10;


    // Accuracy
    public float Accuracy = 0.5f;
    private const float AccuracyMultiplier = 0.05f;


    public SkillsetMapper(Skillset skillset)
    {
        MapAccuracy(skillset._statsMap["Accuracy"]);
        MapMass(skillset._statsMap["Mass"]);
        MapJump(skillset._statsMap["Jump Force"]);
    }

    private void MapJump(int level, bool compound = false)
    {
        MapStat(ref ForceMultiplier, level, ForceMultiplierMultiplier, compound);
        MapStat(ref MaxForce, level, ForceMultiplierMultiplier, compound);
    }
    
    private void MapMass(int level, bool compound = false)
    {
        MapStat(ref Mass, level, MassMultiplier, compound);
        MapStat(ref Health, level, MassMultiplier, compound);
    }

    private void MapAccuracy(int level, bool compound = false)
    {
        MapStat(ref Accuracy, level, AccuracyMultiplier, compound);
    }

    private static void MapStat(ref float stat, int level, float multiplier, bool compound = false)
    {
        stat = compound
            ? stat * (float)Math.Pow(1 + multiplier, level)
            : stat + stat * multiplier * level;
    }
}