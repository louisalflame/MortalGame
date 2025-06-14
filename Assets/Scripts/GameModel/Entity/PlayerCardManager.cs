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

    bool TryPlayCard(ICardEntity card, out int handCardIndex, out int handCardsCount);
    void EndPlayCard();
    IEnumerable<IGameEvent> ClearHandOnTurnEnd(IGameplayStatusWatcher gameWatcher);

    Option<ICardEntity> GetCard(Guid cardIdentity);
    bool TryDiscardCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    bool TryConsumeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    bool TryDisposeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    CreateCardResult CreateNewCard(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit,
        CardData cardData,
        CardCollectionType cloneDestination,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas);
    CloneCardResult CloneNewCard(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit,
        ICardEntity originCard,
        CardCollectionType cloneDestination,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas);
    
    IEnumerable<ICardEntity> Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit);
}

public class PlayerCardManager : IPlayerCardManager
{
    public IHandCardEntity HandCard { get; }
    public IDeckEntity Deck { get; }
    public IGraveyardEntity Graveyard { get; }
    public IExclusionZoneEntity ExclusionZone { get; }
    public IDisposeZoneEntity DisposeZone { get; }
    public Option<ICardEntity> PlayingCard { get; private set; }

    public PlayerCardManager(
        int handCardMaxCount,
        IEnumerable<CardInstance> cardInstances)
    {
        Deck = new DeckEntity(cardInstances);
        HandCard = new HandCardEntity(handCardMaxCount);
        Graveyard = new GraveyardEntity();
        ExclusionZone = new ExclusionZoneEntity();
        DisposeZone = new DisposeZoneEntity();
        PlayingCard = Option.None<ICardEntity>();
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

    public bool TryPlayCard(ICardEntity card, out int index, out int cardsCount)
    {
        cardsCount = HandCard.Cards.Count;
        index = HandCard.Cards.ToList().IndexOf(card);
        if (index < 0)
        {
            return false;
        }
        else
        {
            HandCard.RemoveCard(card);
            PlayingCard = Option.Some(card);
            return true;
        }
    }
    public void EndPlayCard()
    {
        PlayingCard.MatchSome(
            card =>
            { 
                ICardColletionZone destination =
                    (card.HasProperty(CardProperty.Dispose) || card.HasProperty(CardProperty.AutoDispose)) ?
                    ExclusionZone : Graveyard;
                destination.AddCard(card);
            }
        );
        PlayingCard = Option.None<ICardEntity>();                    
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
            RecycledCardInfos = recycleCards.Select(c => c.ToInfo(gameWatcher)).ToArray(),
            ExcludedCardInfos = excludeCards.Select(c => c.ToInfo(gameWatcher)).ToArray(),
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
        else if (PlayingCard.Match(
            card => card.Identity == cardIdentity, 
            () => false))
        { 
            return PlayingCard;
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

    public CreateCardResult CreateNewCard(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit,
        CardData cardData,
        CardCollectionType cloneDestination,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas)
    {
        var newCard = CardEntity.RuntimeCreateFromData(cardData);

        var addCardBuffResults = addCardBuffDatas
            .Select(addCardBuffData => newCard.BuffManager
                .AddBuff(
                    cardBuffLibrary,
                    gameWatcher,
                    trigger,
                    actionUnit,
                    addCardBuffData.CardBuffId,
                    addCardBuffData.Level.Eval(gameWatcher, trigger, actionUnit)))
            .ToArray();
        
        _AddCard(newCard, cloneDestination);

        return new CreateCardResult
        {
            Card = newCard,
            Zone = GetCardCollectionZone(cloneDestination),
            AddBuffs = addCardBuffResults
        };
    }
    public CloneCardResult CloneNewCard(
        IGameplayStatusWatcher gameWatcher,
        ITriggerSource trigger,
        IActionUnit actionUnit,
        ICardEntity originCard,
        CardCollectionType cloneDestination,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas)
    {
        var cloneCard = originCard.Clone();

        var addCardBuffResults = addCardBuffDatas
            .Select(addCardBuffData => cloneCard.BuffManager
                .AddBuff(
                    cardBuffLibrary,
                    gameWatcher,
                    trigger,
                    actionUnit,
                    addCardBuffData.CardBuffId,
                    addCardBuffData.Level.Eval(gameWatcher, trigger, actionUnit)))
            .ToArray();

        _AddCard(cloneCard, cloneDestination);

        return new CloneCardResult
        {
            Card = cloneCard,
            Zone = GetCardCollectionZone(cloneDestination),
            AddBuffs = addCardBuffResults
        };
    }
    private void _AddCard(ICardEntity card, CardCollectionType type)
    {
        switch (type)
        {
            case CardCollectionType.Deck:
                Deck.EnqueueCardsThenShuffle(new[] { card });
                break;
            case CardCollectionType.HandCard:
                HandCard.AddCard(card);
                break;
            case CardCollectionType.Graveyard:
                Graveyard.AddCard(card);
                break;
            case CardCollectionType.ExclusionZone:
                ExclusionZone.AddCard(card);
                break;
            case CardCollectionType.DisposeZone:
                DisposeZone.AddCard(card);
                break;
        }
    }

    public IEnumerable<ICardEntity> Update(IGameplayStatusWatcher gameWatcher, IActionUnit actionUnit)
    {
        foreach (var card in HandCard.Cards
            .Concat(Deck.Cards)
            .Concat(Graveyard.Cards)
            .Concat(ExclusionZone.Cards)
            .Concat(DisposeZone.Cards))
        {
            if (card.BuffManager.Update(gameWatcher, actionUnit))
            { 
                yield return card;
            }
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
