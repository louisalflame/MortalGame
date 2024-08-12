using System;
 
public interface ICardEffect
{
    
}

[Serializable]
public class DamageEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class ShieldEffect : ICardEffect
{
    public IIntegerValue Value;
}

[Serializable]
public class HealEffect : ICardEffect
{
    public IIntegerValue Value;
}

[Serializable]
public class DrawEffect : ICardEffect
{
    public IIntegerValue Value;
}