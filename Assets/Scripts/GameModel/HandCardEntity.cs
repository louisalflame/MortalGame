using System.Collections.Generic;
using System.Linq;

public interface IHandCardEntity
{
    int MaxCount { get; }
    IReadOnlyCollection<CardEntity> Cards { get; }
    IReadOnlyCollection<CardInfo> CardInfos { get; }
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
    public IReadOnlyCollection<CardInfo> CardInfos =>
        _cards.Select(c => new CardInfo(c)).ToArray();

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
        var recycleCards = _cards.ToList();
        _cards = new List<CardEntity>();
        return recycleCards;
    }
}

