using System;
 
public interface ICardEffect
{
    
}

[Serializable]
public class DamageEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class PenetrateDamageEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class AdditionalAttackEffect : ICardEffect 
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class EffectiveAttackEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class ShieldEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class HealEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class GainEnergyEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class LoseEnegyEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class DrawCardEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class DiscardCardEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;
}
[Serializable]
public class CloneTempCardEffect : ICardEffect
{
}
[Serializable]
public class ReserveCardEffect : ICardEffect
{
}

[Serializable]
public class AddBuffEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public string BuffId;
    public IIntegerValue Level;
}
[Serializable]
public class RemoveBuffEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;
    public string BuffId;
}