using UnityEngine;

public interface IPlayerBuffEffect
{
    
}

public class EffectiveDamageBuffEffect : IPlayerBuffEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}