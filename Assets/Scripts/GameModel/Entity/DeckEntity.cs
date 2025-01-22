using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDeckEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    bool PopCard(out ICardEntity card);
    void EnqueueCardsThenShuffle(IEnumerable<ICardEntity> cards);
}
public class DeckEntity : IDeckEntity
{
    private List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
    
    public DeckEntity(IEnumerable<CardInstance> cards, IPlayerEntity owner)
    {
        _cards = new List<ICardEntity>();
        EnqueueCardsThenShuffle(
            cards.Select(c => CardEntity.Create(c, owner)));
    }

    public bool PopCard(out ICardEntity card)
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
    public void EnqueueCardsThenShuffle(IEnumerable<ICardEntity> cards)
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
