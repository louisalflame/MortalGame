using Rayark.Mast;
using UnityEngine;

public interface IPlayerBuffPropertyEntity
{
    PlayerBuffProperty Property { get; }
}
public interface IPlayerBuffIntegerPropertyEntity : IPlayerBuffPropertyEntity
{
    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}
public interface IPlayerBuffRatioPropertyEntity : IPlayerBuffPropertyEntity
{
    float Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

public class AllCardPowerPlayerBuffPropertyEntity : IPlayerBuffIntegerPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.AllCardPower;
    private readonly IIntegerValue _value;

    public AllCardPowerPlayerBuffPropertyEntity(IIntegerValue value)
    { 
        _value = value;
    }
    
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
        => _value.Eval(gameWatcher, triggerSource, SystemAction.Instance);
}

public class AllCardCostPlayerBuffPropertyEntity : IPlayerBuffIntegerPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.AllCardCost;
    private readonly IIntegerValue _value;

    public AllCardCostPlayerBuffPropertyEntity(IIntegerValue value)
    {
        _value = value;
    }

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
        => _value.Eval(gameWatcher, triggerSource, SystemAction.Instance);
}
public class NormalDamageAdditionPlayerBuffPropertyEntity : IPlayerBuffIntegerPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.NormalDamageAddition;
    private readonly IIntegerValue _value;

    public NormalDamageAdditionPlayerBuffPropertyEntity(IIntegerValue value)
    {
        _value = value;
    }

    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
        => _value.Eval(gameWatcher, triggerSource, SystemAction.Instance);
}
public class NormalDamageRatioPlayerBuffPropertyEntity : IPlayerBuffRatioPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.NormalDamageRatio;
    private readonly float _value;

    public NormalDamageRatioPlayerBuffPropertyEntity(float value)
    { 
        _value = value;
    }
    
    public float Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
        => _value;
}
public class MaxHealthPlayerBuffPropertyEntity : IPlayerBuffIntegerPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.MaxHealth;
    private readonly IIntegerValue _value;

    public MaxHealthPlayerBuffPropertyEntity(IIntegerValue value)
    { 
        _value = value;
    }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
        => _value.Eval(gameWatcher, triggerSource, SystemAction.Instance);
}
public class MaxEnergyPlayerBuffPropertyEntity : IPlayerBuffIntegerPropertyEntity
{
    public PlayerBuffProperty Property => PlayerBuffProperty.MaxEnergy;
    private readonly IIntegerValue _value;

    public MaxEnergyPlayerBuffPropertyEntity(IIntegerValue value)
    { 
        _value = value;
    }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
        => _value.Eval(gameWatcher, triggerSource, SystemAction.Instance);
}