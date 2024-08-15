using UnityEngine;

public class HealthManager
{
    public int Hp;
    public int MaxHp;
    public int Dp;

    public HealthManager TakeDamage(int amount, GameContext context, out int deltaHp, out int deltaDp)
    {
        deltaDp = Mathf.Min(Dp, amount);
        var newShield = Dp - deltaDp;
        var healthSuffer = amount - deltaDp;

        var newHp = Mathf.Clamp(Hp - healthSuffer, 0, MaxHp);
        deltaHp = Hp - newHp;

        return new HealthManager()
        {
            Hp = newHp,
            MaxHp = MaxHp,
            Dp = newShield
        };
    }
    public HealthManager TakePenetrateDamage(int amount, GameContext context, out int deltaHp)
    {
        var newHp = Mathf.Clamp(Hp - amount, 0, MaxHp);
        deltaHp = Hp - newHp;

        return new HealthManager()
        {
            Hp = newHp,
            MaxHp = MaxHp,
            Dp = Dp
        };
    }
    public HealthManager TakeAdditionalDamage(int amount, GameContext context, out int deltaHp, out int deltaDp)
    {
        deltaDp = Mathf.Min(Dp, amount);
        var newShield = Dp - deltaDp;
        var healthSuffer = amount - deltaDp;

        var newHp = Mathf.Clamp(Hp - healthSuffer, 0, MaxHp);
        deltaHp = Hp - newHp;

        return new HealthManager()
        {
            Hp = newHp,
            MaxHp = MaxHp,
            Dp = newShield
        };
    }
    public HealthManager TakeEffectiveDamage(int amount, GameContext context, out int deltaHp)
    {
        var newHp = Mathf.Clamp(Hp - amount, 0, MaxHp);
        deltaHp = Hp - newHp;

        return new HealthManager()
        {
            Hp = newHp,
            MaxHp = MaxHp,
            Dp = Dp
        };
    }

    public HealthManager GetHeal(int amount, GameContext context, out int deltaHp)
    {
        var newHp = Mathf.Clamp(Hp + amount, Hp, MaxHp);
        deltaHp = newHp - Hp;

        return new HealthManager()
        {
            Hp = newHp,
            MaxHp = MaxHp,
            Dp = Dp
        };
    }
    public HealthManager GetShield(int amount, GameContext context, out int deltaShield)
    {
        var newShield = Mathf.Max(Dp + amount, 0);
        deltaShield = newShield - Dp;

        return new HealthManager()
        {
            Hp = Hp,
            MaxHp = MaxHp,
            Dp = newShield
        };
    }
}
