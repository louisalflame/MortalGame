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
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class HealEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class DrawEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}