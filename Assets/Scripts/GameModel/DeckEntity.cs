using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDeckEntity
{
    IReadOnlyCollection<CardEntity> Cards { get; }
    IReadOnlyCollection<CardInfo> CardInfos { get; }
    bool PopCard(out CardEntity card);
    void EnqueueCardsThenShuffle(IReadOnlyCollection<CardEntity> cards);
}
public class DeckEntity : IDeckEntity
{
    private List<CardEntity> _cards;
    public IReadOnlyCollection<CardEntity> Cards => _cards;

    public IReadOnlyCollection<CardInfo> CardInfos =>
        Cards.Select(c => new CardInfo(c)).ToArray();
    
    public DeckEntity(IReadOnlyCollection<CardEntity> cards)
    {
        _cards = new List<CardEntity>();
        EnqueueCardsThenShuffle(cards);
    }

    public bool PopCard(out CardEntity card)
    {
        if (Cards.Count == 0)
        {
            card = null;
            return false;
        }
        card = Cards.ElementAt(0);
        _cards = Cards.Skip(1).ToList();
        return true;
    }
    public void EnqueueCardsThenShuffle(IReadOnlyCollection<CardEntity> cards)
    {
        _cards.AddRange(cards);
        
        for (int i = 0; i < _cards.Count; i++)
        {
            var temp = _cards[i];
            var randomIndex = Random.Range(0, _cards.Count);
            _cards[i] = _cards[randomIndex];
            _cards[randomIndex] = temp;
        }
    }
}
