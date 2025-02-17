using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

// Cards in the exclusion zone are removed from the Deck, but return next Battle.
public interface IExclusionZoneEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    Option<ICardEntity> GetCard(Guid cardIdentity);
    void AddCard(ICardEntity card);
    void AddCards(IEnumerable<ICardEntity> cards);
}

public class ExclusionZoneEntity : IExclusionZoneEntity
{
    private List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
        
    public ExclusionZoneEntity()
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