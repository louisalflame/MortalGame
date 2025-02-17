using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

public interface IGraveyardEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    Option<ICardEntity> GetCard(Guid cardIdentity);
    void AddCard(ICardEntity card);
    void AddCards(IEnumerable<ICardEntity> cards);
    IReadOnlyCollection<ICardEntity> PopAllCards();
}
public class GraveyardEntity : IGraveyardEntity
{
    private List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
        
    public GraveyardEntity()
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

    public IReadOnlyCollection<ICardEntity> PopAllCards()
    {
        var cards = _cards.ToList();
        _cards = new List<ICardEntity>();
        return cards;
    }
}