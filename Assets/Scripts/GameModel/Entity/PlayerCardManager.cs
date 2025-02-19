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

    IEnumerable<IGameEvent> ClearHandOnTurnEnd(GameContextManager contextManager);
    
    IEnumerable<IGameEvent> UpdateCardsOnTiming(GameContextManager contextManager, CardTiming timing);

    Option<ICardEntity> GetCard(Guid cardIdentity);
    bool TryDiscardCard(Guid cardIdentity, out ICardEntity card);
    bool TryConsumeCard(Guid cardIdentity, out ICardEntity card);
    bool TryDisposeCard(Guid cardIdentity, out ICardEntity card);
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
        IEnumerable<CardInstance> cardInstances,
        IPlayerEntity owner)
    {
        Deck = new DeckEntity(cardInstances, owner);
        HandCard = new HandCardEntity(handCardMaxCount);
        Graveyard = new GraveyardEntity();
        ExclusionZone = new ExclusionZoneEntity();
        DisposeZone = new DisposeZoneEntity();
    }

    public IEnumerable<IGameEvent> ClearHandOnTurnEnd(GameContextManager contextManager)
    {
        var events = new List<IGameEvent>();

        var nonePreservedCards = HandCard.ClearHand();
        var excludeCards = nonePreservedCards.Where(c => c.HasProperty(CardProperty.AutoDispose));
        ExclusionZone.AddCards(excludeCards);

        var recycleCards = nonePreservedCards.Except(excludeCards);
        Graveyard.AddCards(recycleCards);
        events.Add(new RecycleHandCardEvent(){
            Faction = contextManager.Context.ExecutePlayer.Faction,
            RecycledCardInfos = recycleCards.Select(c => new CardInfo(c, contextManager.Context)).ToArray(),
            ExcludedCardInfos = excludeCards.Select(c => new CardInfo(c, contextManager.Context)).ToArray(),
            HandCardInfo = HandCard.Cards.ToCardCollectionInfo(contextManager.Context),
            GraveyardInfo = Graveyard.Cards.ToCardCollectionInfo(contextManager.Context),
            ExclusionZoneInfo = ExclusionZone.Cards.ToCardCollectionInfo(contextManager.Context),
            DisposeZoneInfo = DisposeZone.Cards.ToCardCollectionInfo(contextManager.Context),
        });

        return events;
    }

    public IEnumerable<IGameEvent> UpdateCardsOnTiming(GameContextManager contextManager, CardTiming timing)
    {
        var events = new List<IGameEvent>();

        foreach(var card in HandCard.Cards)
        {
            if(card.TryUpdateCardsOnTiming(contextManager, timing, out var cardEvent))
            {
                events.Add(cardEvent);
            }
        }

        return events;
    }

    public Option<ICardEntity> GetCard(Guid cardIdentity)
    {
        if (HandCard.TryGetCard(cardIdentity, out var card))
        {
            return Option.Some(card);
        }
        else if(Deck.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else if(Graveyard.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else if(ExclusionZone.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else if(DisposeZone.TryGetCard(cardIdentity, out card))
        {
            return Option.Some(card);
        }
        else
        {
            return Option.None<ICardEntity>();
        }        
    }

    public bool TryDiscardCard(Guid cardIdentity, out ICardEntity card)
    {
        ICardColletionZone GetDiscardDestinationZone(ICardEntity card)
        {
            if(card.IsConsumable())
                return ExclusionZone;
            else if(card.IsDisposable())
                return DisposeZone;
            else
                return Graveyard;
        }

        card = null;
        if (HandCard.TryGetCard(cardIdentity, out var handCard))
        {
            card = handCard;
            HandCard.RemoveCard(handCard);
            GetDiscardDestinationZone(handCard).AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(cardIdentity, out var deckCard))
        {
            card = deckCard;
            Deck.RemoveCard(deckCard);
            GetDiscardDestinationZone(deckCard).AddCard(deckCard);
            return true;
        }

        return false;
    }
    public bool TryConsumeCard(Guid cardIdentity, out ICardEntity card)
    {
        ICardColletionZone GetConsumeDestinationZone(ICardEntity card)
        {
            if(card.IsDisposable())
                return DisposeZone;
            else
                return ExclusionZone;
        }

        card = null;
        if (HandCard.TryGetCard(cardIdentity, out var handCard))
        {
            card = handCard;
            HandCard.RemoveCard(handCard);
            GetConsumeDestinationZone(handCard).AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(cardIdentity, out var deckCard))
        {
            card = deckCard;
            Deck.RemoveCard(deckCard);
            GetConsumeDestinationZone(deckCard).AddCard(deckCard);
            return true;
        }
        else if (Graveyard.TryGetCard(cardIdentity, out var graveyardCard))
        {
            card = graveyardCard;
            Graveyard.RemoveCard(graveyardCard);
            GetConsumeDestinationZone(graveyardCard).AddCard(graveyardCard);
            return true;
        }

        return false;
    }
    public bool TryDisposeCard(Guid cardIdentity, out ICardEntity card)
    {
        card = null;
        if (HandCard.TryGetCard(cardIdentity, out var handCard))
        {
            card = handCard;
            HandCard.RemoveCard(handCard);
            DisposeZone.AddCard(handCard);
            return true;
        }
        else if (Deck.TryGetCard(cardIdentity, out var deckCard))
        {
            card = deckCard;
            Deck.RemoveCard(deckCard);
            DisposeZone.AddCard(deckCard);
            return true;
        }
        else if (Graveyard.TryGetCard(cardIdentity, out var graveyardCard))
        {
            card = graveyardCard;
            Graveyard.RemoveCard(graveyardCard);
            DisposeZone.AddCard(graveyardCard);
            return true;
        }
        else if (ExclusionZone.TryGetCard(cardIdentity, out var exclusionCard))
        {
            card = exclusionCard;
            ExclusionZone.RemoveCard(exclusionCard);
            DisposeZone.AddCard(exclusionCard);
            return true;
        }

        return false;
    }
}
