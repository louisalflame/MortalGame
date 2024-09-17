using UnityEngine;

public interface IBuffEffect
{
    
}

public class EffectiveDamageBuffEffect : IBuffEffect
{
    public ITargetPlayerValue Targets;
    public IIntegerValue Value;
}