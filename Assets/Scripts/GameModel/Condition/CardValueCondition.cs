using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Sirenix.OdinInspector;
using UnityEngine;

public interface ICardValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, ICardEntity card);
}

[Serializable]
public class CardEqualCondition : ICardValueCondition
{
    [HorizontalGroup("1")]
    public ITargetCardValue CompareCard;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, ICardEntity card)
    {
        return CompareCard
            .Eval(gameWatcher, source, actionUnit)
            .Match(
                compareCard => card.Identity == compareCard.Identity,
                () => false);
    }
}
[Serializable]
public class CardTypesCondition : ICardValueCondition
{
    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<CardType> CardTypes = new();

    public SetConditionType Condition;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, ICardEntity card)
    {
        return Condition switch
        {
            SetConditionType.AnyInside => CardTypes.Any(type => type == card.Type),
            SetConditionType.AllInside => CardTypes.All(type => type == card.Type),
            SetConditionType.AnyOutside => CardTypes.Any(type => type != card.Type),
            SetConditionType.AllOutside => CardTypes.All(type => type != card.Type),
            _ => false
        };
    }
}