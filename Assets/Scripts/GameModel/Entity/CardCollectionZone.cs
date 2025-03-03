using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface ICardColletionZone
{
    CardCollectionType Type { get; }
    IReadOnlyCollection<ICardEntity> Cards { get; }
    bool TryGetCard(Guid cardIdentity, out ICardEntity card);
    void AddCard(ICardEntity card);
    void AddCards(IEnumerable<ICardEntity> cards);
    bool RemoveCard(ICardEntity card);
}

public abstract class CardColletionZone : ICardColletionZone
{
    private CardCollectionType _type;
    protected List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
    public CardCollectionType Type => _type;

    public static ICardColletionZone Dummy => new DummyCardCollectionZone();
    
    public CardColletionZone(CardCollectionType type)
    {
        _type = type;
        _cards = new List<ICardEntity>();
    }

    public bool TryGetCard(Guid cardIdentity, out ICardEntity card)
    {
        card = _cards.FirstOrDefault(c => c.Identity == cardIdentity);
        return card != null;
    }

    public void AddCard(ICardEntity card)
    {
        _cards.Add(card);
    }
    public void AddCards(IEnumerable<ICardEntity> cards)
    {
        _cards.AddRange(cards);
    }
    public bool RemoveCard(ICardEntity card)
    {
        var isRemoved = _cards.Remove(card);
        return isRemoved;
    }
}

public class DummyCardCollectionZone : CardColletionZone
{
    public DummyCardCollectionZone() : base(CardCollectionType.None)
    {
    }
}