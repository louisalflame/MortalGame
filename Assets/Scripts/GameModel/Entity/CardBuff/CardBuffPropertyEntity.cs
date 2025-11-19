using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardBuffPropertyEntity
{
    CardProperty Property { get; }
    IEnumerable<string> Keywords { get; }

    int Eval(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit, ITriggerSource triggerSource);

    ICardBuffPropertyEntity Clone();
}

public class SealedCardBuffPropertyEntity : ICardBuffPropertyEntity
{
    public CardProperty Property => CardProperty.Sealed;
    public IEnumerable<string> Keywords => Property.ToString().WrapAsEnumerable();

    public SealedCardBuffPropertyEntity() { }
    public int Eval(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit, ITriggerSource triggerSource) => 0;

    public ICardBuffPropertyEntity Clone() => new SealedCardBuffPropertyEntity();
}

public class PowerCardBuffPropertyEntity : ICardBuffPropertyEntity
{
    public CardProperty Property => CardProperty.PowerAddition;
    public IEnumerable<string> Keywords => Enumerable.Empty<string>();

    private readonly IIntegerValue _value;

    public PowerCardBuffPropertyEntity(IIntegerValue value)
    {
        _value = value;
    }
    public int Eval(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit, ITriggerSource triggerSource)
    {
        return _value.Eval(gameWatcher, triggerSource, new CardBuffPropertyLookAction(this));
    }

    public ICardBuffPropertyEntity Clone() => new PowerCardBuffPropertyEntity(_value);
}