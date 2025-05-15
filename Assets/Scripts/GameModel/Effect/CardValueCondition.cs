using System;
using Optional;
using UnityEngine;

public interface ICardValueCondition
{
    bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, Option<ICardEntity> card);
}

[Serializable]
public class CardEqualCondition : ICardValueCondition
{
    public ITargetCardValue CompareCard;

    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, Option<ICardEntity> cardOpt)
    {
        return 
            cardOpt.Match(
                card => CompareCard
                    .Eval(gameWatcher, source)
                    .Match(
                        compareCard => card.Identity == compareCard.Identity,
                        ()          => false),
                () => false);
    }
}
[Serializable]
public class HandCardPositionCondition : ICardValueCondition
{
    public bool Eval(IGameplayStatusWatcher gameWatcher, ITriggerSource source, Option<ICardEntity> cardOpt)
    {
        return false;
    }
}