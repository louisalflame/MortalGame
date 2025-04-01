using System;
 
public interface IReactionEffect
{
    
}

public class EffectiveDamageEffect : IReactionEffect
{
    public ITargetCharacterCollectionValue Targets;
    public IIntegerValue Value;
}