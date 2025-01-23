using System.Collections.Generic;
using System.Linq;
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
}
