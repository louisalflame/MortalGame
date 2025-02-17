using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

public interface IHandCardEntity
{
    int MaxCount { get; }
    IReadOnlyCollection<ICardEntity> Cards { get; }
    Option<ICardEntity> GetCard(Guid cardIdentity);
    void AddCard(ICardEntity card);
    bool RemoveCard(ICardEntity card);
    IReadOnlyCollection<ICardEntity> ClearHand();
}

public class HandCardEntity : IHandCardEntity
{
    private int _maxCount;
    private List<ICardEntity> _cards;

    public int MaxCount => _maxCount;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;

    public HandCardEntity(int maxCount)
    {
        _maxCount = maxCount;
        _cards = new List<ICardEntity>();
    }

    public Option<ICardEntity> GetCard(Guid cardIdentity)
    {
        var card = _cards.FirstOrDefault(c => c.Identity == cardIdentity);
        return card.SomeNotNull();
    }

    public void AddCard(ICardEntity card)
    {
        _cards.Add(card);
    }

    public bool RemoveCard(ICardEntity card)
    {
        var isRemoved = _cards.Remove(card);
        return isRemoved;
    }

    public IReadOnlyCollection<ICardEntity> ClearHand()
    {
        var preservedCards = _cards.Where(c => c.HasProperty(CardProperty.Preserved)).ToList();
        var recycleCards = _cards.Except(preservedCards).ToList();
        _cards = preservedCards;
        return recycleCards;
    }
}

