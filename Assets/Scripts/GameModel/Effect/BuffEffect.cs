using UnityEngine;

public interface IBuffEffect
{
    
}

public class EffectiveDamageBuffEffect : IBuffEffect
{
    public ITargetPlayerCollectionValue Targets;
    public IIntegerValue Value;
}