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
public class PenetrateDamageEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class AdditionalAttackEffect : ICardEffect 
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class EffectiveAttackEffect : ICardEffect
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
public class GainEnergyEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class LoseEnegyEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class DrawCardEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class DiscardCardEffect : ICardEffect
{
}
[Serializable]
public class ConsumeCardEffect : ICardEffect
{
}
[Serializable]
public class EternalConsumeCardEffect : ICardEffect
{
}
[Serializable]
public class SealCardEffect : ICardEffect
{
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
    public ITargetPlayerValue Targets;
    public string BuffId;
    public IIntegerValue Level;
}
[Serializable]
public class RemoveBuffEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public string BuffId;
}