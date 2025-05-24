using Rayark.Mast;
using UnityEngine;

public interface IPlayerBuffPropertyEntity
{
    PlayerBuffProperty Property { get; }
    
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

public class EffectAttributePlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.EffectAttribute;
    public EffectAttributeType AttributeType => _type;

    private readonly EffectAttributeType _type;
    private readonly IIntegerValue _value;

    public EffectAttributePlayerBuffPropertyEntity(EffectAttributeType type, IIntegerValue value)
    {
        _value = value;
        _type = type;
    }

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return _value.Eval(gameWatcher, triggerSource, SystemAction.Instance);
    }
}

public class MaxHealthPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.MaxHealth;

    public MaxHealthPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class MaxEnergyPlayerBuffPropertyEntity : IPlayerBuffPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.MaxEnergy;

    public MaxEnergyPlayerBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}