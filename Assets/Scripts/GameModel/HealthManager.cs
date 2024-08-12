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
}
