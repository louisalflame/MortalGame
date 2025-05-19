using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;


public interface IPlayerCardManager
{
    IDeckEntity Deck { get; }
    IHandCardEntity HandCard { get; }
    IGraveyardEntity Graveyard { get; }
    IExclusionZoneEntity ExclusionZone { get; }
    IDisposeZoneEntity DisposeZone { get; }
    ICardColletionZone GetCardCollectionZone(CardCollectionType type);

    IEnumerable<IGameEvent> ClearHandOnTurnEnd(IGameplayStatusWatcher gameWatcher);

    Option<ICardEntity> GetCard(Guid cardIdentity);
    bool TryDiscardCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    bool TryConsumeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    bool TryDisposeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    void AddNewCard(ICardEntity card, CardCollectionType cloneDestination);
    
    void UpdateTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
}

public class PlayerCardManager : IPlayerCardManager
{
    public IHandCardEntity HandCard { get; }
    public IDeckEntity Deck { get; }
    public IGraveyardEntity Graveyard { get; }
    public IExclusionZoneEntity ExclusionZone { get; }
    public IDisposeZoneEntity DisposeZone { get; }

    public PlayerCardManager(
        int handCardMaxCount,
        IEnumerable<CardInstance> cardInstances)
    {
        Deck = new DeckEntity(cardInstances);
        HandCard = new HandCardEntity(handCardMaxCount);
        Graveyard = new GraveyardEntity();
        ExclusionZone = new ExclusionZoneEntity();
        DisposeZone = new DisposeZoneEntity();
    }

    public ICardColletionZone GetCardCollectionZone(CardCollectionType type)
    {
        switch (type)
        {
            case CardCollectionType.Deck:
                return Deck;
            case CardCollectionType.HandCard:
                return HandCard;
            case CardCollectionType.Graveyard:
                return Graveyard;
            case CardCollectionType.ExclusionZone:
                return ExclusionZone;
            case CardCollectionType.DisposeZone:
                return DisposeZone;
            default:
                return CardColletionZone.Dummy;
        }
    }

    public IEnumerable<IGameEvent> ClearHandOnTurnEnd(IGameplayStatusWatcher gameWatcher)
    {
        var events = new List<IGameEvent>();

        var nonePreservedCards = HandCard.ClearHand();
        var excludeCards = nonePreservedCards.Where(c => c.HasProperty(CardProperty.AutoDispose));
        ExclusionZone.AddCards(excludeCards);

        var recycleCards = nonePreservedCards.Except(excludeCards);
        Graveyard.AddCards(recycleCards);
        events.Add(new RecycleHandCardEvent()
        {
            Faction = this.Owner(gameWatcher).ValueOr(PlayerEntity.DummyPlayer).Faction,
            RecycledCardInfos = recycleCards.Select(c => new CardInfo(c, gameWatcher)).ToArray(),
            ExcludedCardInfos = excludeCards.Select(c => new CardInfo(c, gameWatcher)).ToArray(),
            HandCardInfo = HandCard.ToCardCollectionInfo(gameWatcher),
            GraveyardInfo = Graveyard.ToCardCollectionInfo(gameWatcher),
            ExclusionZoneInfo = ExclusionZone.ToCardCollectionInfo(gameWatcher),
            DisposeZoneInfo = DisposeZone.ToCardCollectionInfo(gameWatcher),
        });

        return events;
    }

    public Option<ICardEntity> GetCard(Guid cardIdentity)
    {
        if (HandCard.TryGetCard(cardIdentity, out var card))
        {
            return Option.Some(card);
        }
        else if (Deck.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else if (Graveyard.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else if (ExclusionZone.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else if (DisposeZone.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else
        {
            return Option.None<ICardEntity>();
        }
    }

    public bool TryDiscardCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination)
    {
        CardCollectionType GetDiscardDestinationZone(ICardEntity card)
        {
            if (card.IsConsumable())
                return CardCollectionType.ExclusionZone;
            else if (card.IsDisposable())
                return CardCollectionType.DisposeZone;
            else
                return CardCollectionType.Graveyard;
        }

        card = null;
        if (HandCard.TryGetCard(cardIdentity, out var handCard))
        {
            card = handCard;
            start = HandCard;
            destination = GetCardCollectionZone(GetDiscardDestinationZone(handCard));

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(cardIdentity, out var deckCard))
        {
            card = deckCard;
            start = Deck;
            destination = GetCardCollectionZone(GetDiscardDestinationZone(deckCard));

            start.RemoveCard(deckCard);
            destination.AddCard(deckCard);
            return true;
        }

        start = CardColletionZone.Dummy;
        destination = CardColletionZone.Dummy;
        return false;
    }
    public bool TryConsumeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination)
    {
        CardCollectionType GetConsumeDestinationZone(ICardEntity card)
        {
            if (card.IsDisposable())
                return CardCollectionType.DisposeZone;
            else
                return CardCollectionType.ExclusionZone;
        }

        card = null;
        if (HandCard.TryGetCard(cardIdentity, out var handCard))
        {
            card = handCard;
            start = HandCard;
            destination = GetCardCollectionZone(GetConsumeDestinationZone(handCard));

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(cardIdentity, out var deckCard))
        {
            card = deckCard;
            start = Deck;
            destination = GetCardCollectionZone(GetConsumeDestinationZone(deckCard));

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Graveyard.TryGetCard(cardIdentity, out var graveyardCard))
        {
            card = graveyardCard;
            start = Graveyard;
            destination = GetCardCollectionZone(GetConsumeDestinationZone(graveyardCard));

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }

        start = CardColletionZone.Dummy;
        destination = CardColletionZone.Dummy;
        return false;
    }
    public bool TryDisposeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination)
    {
        destination = DisposeZone;
        card = null;
        if (HandCard.TryGetCard(cardIdentity, out var handCard))
        {
            card = handCard;
            start = HandCard;

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(cardIdentity, out var deckCard))
        {
            card = deckCard;
            start = Deck;

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Graveyard.TryGetCard(cardIdentity, out var graveyardCard))
        {
            card = graveyardCard;
            start = Graveyard;

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (ExclusionZone.TryGetCard(cardIdentity, out var exclusionCard))
        {
            card = exclusionCard;
            start = ExclusionZone;

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }

        start = CardColletionZone.Dummy;
        return false;
    }

    public void AddNewCard(ICardEntity newCard, CardCollectionType cloneDestination)
    {
        switch (cloneDestination)
        {
            case CardCollectionType.Deck:
                Deck.EnqueueCardsThenShuffle(new[] { newCard });
                break;
            case CardCollectionType.HandCard:
                HandCard.AddCard(newCard);
                break;
            case CardCollectionType.Graveyard:
                Graveyard.AddCard(newCard);
                break;
            case CardCollectionType.ExclusionZone:
                ExclusionZone.AddCard(newCard);
                break;
            case CardCollectionType.DisposeZone:
                DisposeZone.AddCard(newCard);
                break;
            default:
                break;
        }
    }

    public void UpdateTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        foreach (var card in HandCard.Cards
            .Concat(Deck.Cards)
            .Concat(Graveyard.Cards)
            .Concat(ExclusionZone.Cards)
            .Concat(DisposeZone.Cards))
        {
            card.BuffManager.UpdateTiming(gameWatcher, timing);
        }
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    {
        foreach (var card in HandCard.Cards
            .Concat(Deck.Cards)
            .Concat(Graveyard.Cards)
            .Concat(ExclusionZone.Cards)
            .Concat(DisposeZone.Cards))
        {
            card.BuffManager.UpdateIntent(gameWatcher, intent);
        }
    }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    {
        foreach (var card in HandCard.Cards
            .Concat(Deck.Cards)
            .Concat(Graveyard.Cards)
            .Concat(ExclusionZone.Cards)
            .Concat(DisposeZone.Cards))
        {
            card.BuffManager.UpdateResult(gameWatcher, result);
        }
    }
}

public static class PlayerCardManagerExtensions
{
    public static Option<IPlayerEntity> Owner(this IPlayerCardManager cardManager, IGameplayStatusWatcher watcher)
    {
        if (watcher.GameStatus.Ally.CardManager == cardManager)
            return (watcher.GameStatus.Ally as IPlayerEntity).Some();
        if (watcher.GameStatus.Enemy.CardManager == cardManager)
            return (watcher.GameStatus.Enemy as IPlayerEntity).Some();
        return Option.None<IPlayerEntity>();
    }
}
