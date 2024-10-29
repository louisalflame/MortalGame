using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPlayerCardManager
{
    IDeckEntity Deck { get; }
    IHandCardEntity HandCard { get; }
    IGraveyardEntity Graveyard { get; }
    IExclusionZoneEntity ExclusionZone { get; }

    IEnumerable<IGameEvent> ClearHandOnTurnEnd(GameContextManager contextManager);
    
    IEnumerable<IGameEvent> UpdateCardsOnTiming(GameContextManager contextManager, CardTiming timing);
}

public class PlayerCardManager : IPlayerCardManager
{
    public IHandCardEntity HandCard { get; }
    public IDeckEntity Deck { get; }
    public IGraveyardEntity Graveyard { get; }
    public IExclusionZoneEntity ExclusionZone { get; }

    public PlayerCardManager(
        int handCardMaxCount,
        IEnumerable<CardInstance> cardInstances)
    {
        Deck = new DeckEntity(cardInstances);
        HandCard = new HandCardEntity(handCardMaxCount);
        Graveyard = new GraveyardEntity();
        ExclusionZone = new ExclusionZoneEntity();
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
            RecycledCardInfos = recycleCards.Select(c => new CardInfo(c)).ToArray(),
            ExcludedCardInfos = excludeCards.Select(c => new CardInfo(c)).ToArray(),
            HandCardInfo = HandCard.CardCollectionInfo,
            GraveyardInfo = Graveyard.CardCollectionInfo
        });

        return events;
    }

    public IEnumerable<IGameEvent> UpdateCardsOnTiming(GameContextManager contextManager, CardTiming timing)
    {
        var events = new List<IGameEvent>();

        foreach(var card in HandCard.Cards)
        {
            foreach(var property in card.Properties)
            {
                property.Lifetime.UpdateTiming(contextManager, timing);
            }
            card.Properties = card.Properties.Where(p => !p.Lifetime.IsExpired).ToList();
        }

        return events;
    }
}
