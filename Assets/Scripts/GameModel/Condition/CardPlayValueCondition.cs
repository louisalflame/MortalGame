using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public interface ICardPlayValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, CardPlaySource cardPlay);
}
[Serializable]
public class CardPlayPositionCondition : ICardPlayValueCondition
{
    public enum OrderType
    {
        MostNew,
        MostOld
    }

    public OrderType Order;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, CardPlaySource cardPlay)
    {
        switch (Order)
        { 
            case OrderType.MostNew:
                return cardPlay.HandCardIndex == cardPlay.HandCardsCount - 1;
            case OrderType.MostOld:
                return cardPlay.HandCardIndex == 0;
            default:
                return false;
        }
    }
}
[Serializable]
public class CardPlayCardCondition : ICardPlayValueCondition
{
    [ShowInInspector]
    [HorizontalGroup("1")]
    public List<ICardValueCondition> Conditions = new();

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, IActionUnit actionUnit, CardPlaySource cardPlay)
    {
        return Conditions.All(c => c.Eval(gameWatcher, source, actionUnit, cardPlay.Card));
    }
}