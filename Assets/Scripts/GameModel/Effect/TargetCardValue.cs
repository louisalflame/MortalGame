using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ITargetCardValue
{
    Option<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

[Serializable]
public class NoneCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return Option.None<ICardEntity>();
    }
}
[Serializable]
public class SelectedCard : ITargetCardValue
{
    public Option<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return gameWatcher.GameContext.SelectedCard.Some();
    }
}

public interface ITargetCardCollectionValue
{
    IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source);
}

[Serializable]
public class SingleCardCollection : ITargetCardCollectionValue
{
    public ITargetCardValue TargetCard;

    public IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return  TargetCard.Eval(gameWatcher, source).ToEnumerable().ToList();
    }
}
[Serializable]
public class AllyHandCards : ITargetCardCollectionValue
{
    public IReadOnlyCollection<ICardEntity> Eval(IGameplayStatusWatcher gameWatcher, IActionSource source)
    {
        return source switch
        {
            CardSource cardSource   => cardSource.Card.Owner(gameWatcher).Match(
                                        player  => player.CardManager.HandCard.Cards,
                                        ()      => Array.Empty<ICardEntity>()),
            SystemSource _          => Array.Empty<ICardEntity>(),
            _                       => Array.Empty<ICardEntity>()
        };
    }
}

