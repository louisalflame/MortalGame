using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISelectedCardEntity
{
    IReadOnlyCollection<CardEntity> Cards { get; }
    bool TryEnqueueCard(CardEntity card);
    bool TryDequeueCard(out CardEntity card);
}

public class SelectedCardEntity : ISelectedCardEntity
{
    public IReadOnlyCollection<CardEntity> Cards => _cards;

    private int _maxCount;
    private Queue<CardEntity> _cards;

    public SelectedCardEntity(int selectedCardMaxCount, IEnumerable<CardEntity> cards)
    {
        _maxCount = selectedCardMaxCount;
        _cards = new Queue<CardEntity>(cards); 
    }
 
    public bool TryEnqueueCard(CardEntity card)
    {
        if (Cards.Count < _maxCount)
        {
            _cards.Enqueue(card);
            return true;
        }
        return false;
    }

    public bool TryDequeueCard(out CardEntity card)
    { 
        if (Cards.Count > 0)
        {
            card = _cards.Dequeue();
            return true;
        }
        card = null;
        return false;
    }
}

