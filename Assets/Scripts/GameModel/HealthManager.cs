using UnityEngine;

public class HealthManager
{
    public int Hp;
    public int MaxHp;

    public HealthManager TakeDamage(int amount, out int deltaHp)
    {
        var newHp = Mathf.Clamp(Hp - amount, 0, MaxHp);
        deltaHp = Hp - newHp;

        return new HealthManager()
        {
            Hp = newHp,
            MaxHp = MaxHp,
        };
    }
}
