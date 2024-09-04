using System;
using UnityEngine;

public interface IHealthManager
{
    int Hp { get; }
    int MaxHp { get; }
    int Dp { get; }
    TakeDamageResult TakeDamage(int amount, GameContext context);
    TakeDamageResult TakePenetrateDamage(int amount, GameContext context);
    TakeDamageResult TakeAdditionalDamage(int amount, GameContext context);
    TakeDamageResult TakeEffectiveDamage(int amount, GameContext context);
    GetHealResult GetHeal(int amount, GameContext context);
    GetShieldResult GetShield(int amount, GameContext context);
}
public class HealthManager : IHealthManager
{
    private int _hp;
    private int _maxHp;
    private int _dp;

    public int Hp => _hp;
    public int MaxHp => _maxHp;
    public int Dp => _dp;

    public HealthManager(int currentHealth, int maxHealth)
    {
        _maxHp = currentHealth;
        _hp = maxHealth;
        _dp = 0;
    }

    public TakeDamageResult TakeDamage(int amount, GameContext context)
    {
        var deltaDp = _AcceptArmorDamage(amount, out var damageRemain);
        var deltaHp = _AcceptHealthDamage(damageRemain, out var damageOver);

        return new TakeDamageResult()
        {
            Type = DamageType.Normal,
            DamagePoint = amount,
            DeltaHp = deltaHp,
            DeltaDp = deltaDp,
            OverHp = damageOver
        };
    }
    public TakeDamageResult TakePenetrateDamage(int amount, GameContext context)
    {
        var deltaHp = _AcceptHealthDamage(amount, out var damageOver);

        return new TakeDamageResult()
        {
            Type = DamageType.Penetrate,
            DamagePoint = amount,
            DeltaHp = deltaHp,
            DeltaDp = 0,
            OverHp = damageOver
        };
    }
    public TakeDamageResult TakeAdditionalDamage(int amount, GameContext context)
    {
        var deltaDp = _AcceptArmorDamage(amount, out var damageRemain);
        var deltaHp = _AcceptHealthDamage(damageRemain, out var damageOver);

        return new TakeDamageResult()
        {
            Type = DamageType.Additional,
            DamagePoint = amount,
            DeltaHp = deltaHp,
            DeltaDp = deltaDp,
            OverHp = damageOver
        };
    }
    public TakeDamageResult TakeEffectiveDamage(int amount, GameContext context)
    { 
        var deltaHp = _AcceptHealthDamage(amount, out var damageOver);

        return new TakeDamageResult()
        {
            Type = DamageType.Effective,
            DamagePoint = amount,
            DeltaHp = deltaHp,
            DeltaDp = 0,
            OverHp = damageOver
        };
    }

    public GetHealResult GetHeal(int amount, GameContext context)
    {
        var deltaHp = _AcceptHealthHeal(amount, out var hpOver);

        return new GetHealResult()
        {
            HealPoint = amount,
            DeltaHp = deltaHp,
            OverHp = hpOver
        };
    }
    public GetShieldResult GetShield(int amount, GameContext context)
    {
        var deltaDp = _AcceptArmorGain(amount, out var dpOver);

        return new GetShieldResult()
        {
            ShieldPoint = amount,
            DeltaDp = deltaDp,
            OverDp = dpOver
        };
    }

    private int _AcceptArmorDamage(int amount, out int damageRemain)
    { 
        var originDp = _dp;
        _dp = Mathf.Clamp(_dp - amount, 0, originDp);
        var deltaDp = originDp - _dp;
        damageRemain = Mathf.Max(amount - deltaDp, 0);

        return deltaDp;
    }
    private int _AcceptHealthDamage(int amount, out int damageRemain)
    { 
        var originHp = _hp;
        _hp = Mathf.Clamp(_hp - amount, 0, originHp);
        var deltaHp = originHp - _hp;
        damageRemain = Mathf.Max(amount - deltaHp, 0);

        return deltaHp;
    }

    private int _AcceptArmorGain(int amount, out int dpOver)
    {
        var originDp = _dp;
        _dp = Mathf.Clamp(_dp + amount, originDp, _maxHp);
        var deltaDp = _dp - originDp;
        dpOver = Mathf.Max(amount - deltaDp, 0);

        return deltaDp;
    }
    private int _AcceptHealthHeal(int amount, out int hpOver)
    {
        var originHp = _hp;
        _hp = Mathf.Clamp(_hp + amount, originHp, _maxHp);
        var deltaHp = _hp - originHp;
        hpOver = Mathf.Max(amount - deltaHp, 0);

        return deltaHp;
    }
}
