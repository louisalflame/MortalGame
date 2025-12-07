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
    Option<ICardEntity> PlayingCard { get; }
    ICardColletionZone GetCardCollectionZone(CardCollectionType type);

    (bool Success, IDisposable PlayCardDisposable) TryPlayCard(ICardEntity card, out int handCardIndex, out int handCardsCount);
    IEnumerable<IGameEvent> ClearHandOnTurnEnd(IGameplayModel gameWatcher);

    Option<ICardEntity> GetCard(Func<ICardEntity, bool> predicate);
    bool TryDiscardCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    bool TryConsumeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    bool TryDisposeCard(Guid cardIdentity, out ICardEntity card, out ICardColletionZone start, out ICardColletionZone destination);
    CreateCardResult CreateNewCard(
        TriggerContext triggerContext,
        string cardDataId,
        CardCollectionType cloneDestination,
        CardLibrary cardLibrary,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas);
    CloneCardResult CloneNewCard(
        TriggerContext triggerContext,
        ICardEntity originCard,
        CardCollectionType cloneDestination,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas);

    IEnumerable<ICardEntity> Update(TriggerContext triggerContext);
}

public record CardManagerInfo(
    IReadOnlyDictionary<CardCollectionType, CardCollectionInfo> CardZoneInfos,
    Option<CardInfo> PlayingCard);

public class PlayerCardManager : IPlayerCardManager, IDisposable
{
    public IHandCardEntity HandCard { get; }
    public IDeckEntity Deck { get; }
    public IGraveyardEntity Graveyard { get; }
    public IExclusionZoneEntity ExclusionZone { get; }
    public IDisposeZoneEntity DisposeZone { get; }
    public Option<ICardEntity> PlayingCard { get; private set; }

    public PlayerCardManager(
        int handCardMaxCount,
        IEnumerable<CardInstance> cardInstances,
        CardLibrary cardLibrary)
    {
        Deck = new DeckEntity(cardInstances, cardLibrary);
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

    public (bool Success, IDisposable PlayCardDisposable) TryPlayCard(ICardEntity card, out int index, out int cardsCount)
    {
        cardsCount = HandCard.Cards.Count;
        index = HandCard.Cards.ToList().IndexOf(card);
        if (index < 0)
        {
            return (false, this);
        }
        else
        {
            HandCard.RemoveCard(card);
            PlayingCard = Option.Some(card);
            return (true, this);
        }
    }
    public void Dispose()
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

    public IEnumerable<IGameEvent> ClearHandOnTurnEnd(IGameplayModel gameWatcher)
    {
        var events = new List<IGameEvent>();

        var nonePreservedCards = HandCard.ClearHand();
        var excludeCards = nonePreservedCards.Where(c => c.HasProperty(CardProperty.AutoDispose)).ToArray();
        ExclusionZone.AddCards(excludeCards);

        var recycleCards = nonePreservedCards.Except(excludeCards).ToArray();
        Graveyard.AddCards(recycleCards);
        events.Add(new RecycleHandCardEvent(
            Faction: this.Owner(gameWatcher).ValueOr(PlayerEntity.DummyPlayer).Faction,
            RecycledCardInfos: recycleCards.Select(c => c.ToInfo(gameWatcher)).ToArray(),
            ExcludedCardInfos: excludeCards.Select(c => c.ToInfo(gameWatcher)).ToArray(),
            CardManagerInfo: this.ToInfo(gameWatcher)));

        return events;
    }

    public Option<ICardEntity> GetCard(Func<ICardEntity, bool> predicate)
    {
        if (HandCard.TryGetCard(predicate, out var card))
        {
            return Option.Some(card);
        }
        else if (Deck.TryGetCard(predicate, out card))
        {
            return Option.Some(card);
        }
        else if (Graveyard.TryGetCard(predicate, out card))
        {
            return Option.Some(card);
        }
        else if (ExclusionZone.TryGetCard(predicate, out card))
        {
            return Option.Some(card);
        }
        else if (DisposeZone.TryGetCard(predicate, out card))
        {
            return Option.Some(card);
        }
        else if (PlayingCard.Match(
            card => predicate(card),
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
        if (HandCard.TryGetCard(card => card.Identity == cardIdentity, out var handCard))
        {
            card = handCard;
            start = HandCard;
            destination = GetCardCollectionZone(GetDiscardDestinationZone(handCard));

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(card => card.Identity == cardIdentity, out var deckCard))
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
        if (HandCard.TryGetCard(card => card.Identity == cardIdentity, out var handCard))
        {
            card = handCard;
            start = HandCard;
            destination = GetCardCollectionZone(GetConsumeDestinationZone(handCard));

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(card => card.Identity == cardIdentity, out var deckCard))
        {
            card = deckCard;
            start = Deck;
            destination = GetCardCollectionZone(GetConsumeDestinationZone(deckCard));

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Graveyard.TryGetCard(card => card.Identity == cardIdentity, out var graveyardCard))
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
        if (HandCard.TryGetCard(card => card.Identity == cardIdentity, out var handCard))
        {
            card = handCard;
            start = HandCard;

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(card => card.Identity == cardIdentity, out var deckCard))
        {
            card = deckCard;
            start = Deck;

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (Graveyard.TryGetCard(card => card.Identity == cardIdentity, out var graveyardCard))
        {
            card = graveyardCard;
            start = Graveyard;

            start.RemoveCard(handCard);
            destination.AddCard(handCard);
            return true;
        }
        else if (ExclusionZone.TryGetCard(card => card.Identity == cardIdentity, out var exclusionCard))
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
        TriggerContext triggerContext,
        string cardDataId,
        CardCollectionType cloneDestination,
        CardLibrary cardLibrary,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas)
    {
        var newCard = CardEntity.RuntimeCreateFromId(cardDataId, cardLibrary);

        var addCardBuffResults = addCardBuffDatas
            .Select(addCardBuffData => newCard.BuffManager
                .AddBuff(
                    cardBuffLibrary,
                    triggerContext,
                    addCardBuffData.CardBuffId,
                    addCardBuffData.Level.Eval(triggerContext)))
            .ToArray();

        _AddCard(newCard, cloneDestination);

        return new CreateCardResult(
            Card: newCard,
            Zone: GetCardCollectionZone(cloneDestination),
            AddBuffs: addCardBuffResults
        );
    }
    public CloneCardResult CloneNewCard(
        TriggerContext triggerContext,
        ICardEntity originCard,
        CardCollectionType cloneDestination,
        CardBuffLibrary cardBuffLibrary,
        IEnumerable<AddCardBuffData> addCardBuffDatas)
    {
        var cloneCard = originCard.Clone(includeCardBuffs: false, includeCardProperties: false);

        var addCardBuffResults = addCardBuffDatas
            .Select(addCardBuffData => cloneCard.BuffManager
                .AddBuff(
                    cardBuffLibrary,
                    triggerContext,
                    addCardBuffData.CardBuffId,
                    addCardBuffData.Level.Eval(triggerContext)))
            .ToArray();

        _AddCard(cloneCard, cloneDestination);

        return new CloneCardResult(
            OriginCard: originCard,
            Card: cloneCard,
            Zone: GetCardCollectionZone(cloneDestination),
            AddBuffs: addCardBuffResults
        );
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

    public IEnumerable<ICardEntity> Update(TriggerContext triggerContext)
    {
        foreach (var card in HandCard.Cards
            .Concat(Deck.Cards)
            .Concat(Graveyard.Cards)
            .Concat(ExclusionZone.Cards)
            .Concat(DisposeZone.Cards))
        {
            if (card.BuffManager.Update(triggerContext))
            {
                yield return card;
            }
        }
    }
}

public static class PlayerCardManagerExtensions
{
    public static Option<IPlayerEntity> Owner(this IPlayerCardManager cardManager, IGameplayModel watcher)
    {
        if (watcher.GameStatus.Ally.CardManager == cardManager)
            return (watcher.GameStatus.Ally as IPlayerEntity).Some();
        if (watcher.GameStatus.Enemy.CardManager == cardManager)
            return (watcher.GameStatus.Enemy as IPlayerEntity).Some();
        return Option.None<IPlayerEntity>();
    }

    public static CardManagerInfo ToInfo(this IPlayerCardManager cardManager, IGameplayModel gameWatcher)
        => new(
            new Dictionary<CardCollectionType, CardCollectionInfo>
            {
                { CardCollectionType.Deck, cardManager.Deck.ToCardCollectionInfo(gameWatcher) },
                { CardCollectionType.HandCard, cardManager.HandCard.ToCardCollectionInfo(gameWatcher) },
                { CardCollectionType.Graveyard, cardManager.Graveyard.ToCardCollectionInfo(gameWatcher) },
                { CardCollectionType.ExclusionZone, cardManager.ExclusionZone.ToCardCollectionInfo(gameWatcher) },
                { CardCollectionType.DisposeZone, cardManager.DisposeZone.ToCardCollectionInfo(gameWatcher) }
            },
            cardManager.PlayingCard.Map(c => c.ToInfo(gameWatcher)));
}
