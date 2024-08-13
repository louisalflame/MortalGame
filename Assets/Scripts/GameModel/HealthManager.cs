using UnityEngine;

public class HealthManager
{
    public int Hp;
    public int MaxHp;
    public int Shield;

    public HealthManager TakeDamage(int amount, GameContext context, out int deltaHp, out int deltaShield)
    {
        deltaShield = Mathf.Min(Shield, amount);
        var newShield = Shield - deltaShield;
        var healthSuffer = amount - deltaShield;

        var newHp = Mathf.Clamp(Hp - healthSuffer, 0, MaxHp);
        deltaHp = Hp - newHp;

        return new HealthManager()
        {
            Hp = newHp,
            MaxHp = MaxHp,
            Shield = newShield
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
            Shield = Shield
        };
    }
    public HealthManager GetShield(int amount, GameContext context, out int deltaShield)
    {
        var newShield = Mathf.Max(Shield + amount, 0);
        deltaShield = newShield - Shield;

        return new HealthManager()
        {
            Hp = Hp,
            MaxHp = MaxHp,
            Shield = newShield
        };
    }
}
