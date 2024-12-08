using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISelectedCardEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    bool TryEnqueueCard(ICardEntity card);
    bool TryDequeueCard(out ICardEntity card);
}

public class SelectedCardEntity : ISelectedCardEntity
{
    public IReadOnlyCollection<ICardEntity> Cards => _cards;

    private int _maxCount;
    private Queue<ICardEntity> _cards;

    public SelectedCardEntity(int selectedCardMaxCount, IEnumerable<ICardEntity> cards)
    {
        _maxCount = selectedCardMaxCount;
        _cards = new Queue<ICardEntity>(cards); 
    }
 
    public bool TryEnqueueCard(ICardEntity card)
    {
        if (Cards.Count < _maxCount)
        {
            _cards.Enqueue(card);
            return true;
        }
        return false;
    }

    public bool TryDequeueCard(out ICardEntity card)
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

