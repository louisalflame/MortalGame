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
public class EffectAttackEffect : ICardEffect
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
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class ConsumeCardEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class EternalConsumeCardEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}
[Serializable]
public class SealCardEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}

[Serializable]
public class AddBuffEffect : ICardEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}