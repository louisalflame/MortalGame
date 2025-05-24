using Rayark.Mast;
using UnityEngine;

public interface ICharacterBuffPropertyEntity
{
    CharacterBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

public class EffectAttributePropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.EffectAttribute;

    private readonly EffectAttributeType _type;
    private readonly IIntegerValue _value;

    public EffectAttributePropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return _value.Eval(gameWatcher, triggerSource, SystemAction.Instance);
    }
}

public class MaxHealthPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxHealth;

    public MaxHealthPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class MaxEnergyPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxEnergy;

    public MaxEnergyPropertyCharacterBuffEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}