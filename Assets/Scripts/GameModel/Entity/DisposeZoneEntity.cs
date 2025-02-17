using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

// Cards in the dispose zone are removed from the Deck forever in this game.
public interface IDisposeZoneEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    Option<ICardEntity> GetCard(Guid cardIdentity);
    void AddCard(ICardEntity card);
    void AddCards(IEnumerable<ICardEntity> cards);
}

public class DisposeZoneEntity : IDisposeZoneEntity
{
    private List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
        
    public DisposeZoneEntity()
    {
        _cards = new List<ICardEntity>();
    }

    public Option<ICardEntity> GetCard(Guid cardIdentity)
    {
        var card = Cards.FirstOrDefault(c => c.Identity == cardIdentity);
        return card.SomeNotNull();
    }

    public void AddCard(ICardEntity card)
    {
        _cards.Add(card);
    }
    public void AddCards(IEnumerable<ICardEntity> cards)
    {
        _cards.AddRange(cards);
    }
}