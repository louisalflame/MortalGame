using UnityEngine;

public class PowerManager
{
    public int Power;

    public int EvaluateDamagePoint(int damagePoint, GameContext context)
    {
        return damagePoint + Power;
    }
}