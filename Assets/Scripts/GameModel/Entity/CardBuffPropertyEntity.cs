using UnityEngine;

public interface ICardBuffPropertyEntity
{
    CardProperty Property { get; }

    int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource);
}

public class SealedCardBuffPropertyEntity : ICardBuffPropertyEntity
{
    public CardProperty Property => CardProperty.Sealed;

    public SealedCardBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource) => 0;
}

public class PowerCardBuffPropertyEntity : ICardBuffPropertyEntity
{
    public CardProperty Property => CardProperty.PowerAdjust;
    private readonly IIntegerValue _value;

    public PowerCardBuffPropertyEntity(IIntegerValue value)
    {
        _value = value;
    }
    public int Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource triggerSource)
    {
        return _value.Eval(gameWatcher, triggerSource);
    }
}