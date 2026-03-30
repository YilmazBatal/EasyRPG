using System;
using Random = UnityEngine.Random;

public interface IDamageCalculator
{
    int CalculateDMG(int sourceAtk, int targetDef, float critRate, float critDmg, out bool isCrit);
}
public class DamageCalculator : IDamageCalculator
{
    float minPercent = 90f;
    float maxPercent = 111f;
    public int CalculateDMG(int sourceAtk, int targetDef, float critRate, float critDmg, out bool isCrit)
    {
        float baseDmg = sourceAtk * 100.0f / (100.0f + targetDef);

        isCrit = Random.Range(0, 101) <= critRate;
        float newDmg = isCrit ? (baseDmg * critDmg / 100.0f) : baseDmg;

        float deviation = newDmg * Random.Range(minPercent, maxPercent) / 100.0f;

        int finalDamage = (int)Math.Round(deviation);
        return Math.Max(1, finalDamage);
    }
}

