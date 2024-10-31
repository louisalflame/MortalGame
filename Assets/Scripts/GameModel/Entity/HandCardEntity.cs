using System.Collections.Generic;
using System.Linq;

public interface IHandCardEntity
{
    int MaxCount { get; }
    IReadOnlyCollection<CardEntity> Cards { get; }
    void AddCard(CardEntity card);
    bool RemoveCard(CardEntity card);
    IReadOnlyCollection<CardEntity> ClearHand();
}

public class HandCardEntity : IHandCardEntity
{
    private int _maxCount;
    private List<CardEntity> _cards;

    public int MaxCount => _maxCount;
    public IReadOnlyCollection<CardEntity> Cards => _cards;

    public HandCardEntity(int maxCount)
    {
        _maxCount = maxCount;
        _cards = new List<CardEntity>();
    }

    public void AddCard(CardEntity card)
    {
        _cards.Add(card);
    }

    public bool RemoveCard(CardEntity card)
    {
        var isRemoved = _cards.Remove(card);
        return isRemoved;
    }

    public IReadOnlyCollection<CardEntity> ClearHand()
    {
        var preservedCards = _cards.Where(c => c.HasProperty(CardProperty.Preserved)).ToList();
        var recycleCards = _cards.Except(preservedCards).ToList();
        _cards = preservedCards;
        return recycleCards;
    }
}

