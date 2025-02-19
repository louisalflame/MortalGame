using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Optional;
using UnityEngine;

public interface IDeckEntity : ICardColletionZone
{
    bool PopCard(out ICardEntity card);
    void EnqueueCardsThenShuffle(IEnumerable<ICardEntity> cards);
}
public class DeckEntity : CardColletionZone, IDeckEntity
{    
    public DeckEntity(IEnumerable<CardInstance> cards, IPlayerEntity owner) : base()
    {
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
            var randomIndex = UnityEngine.Random.Range(0, _cards.Count);
            _cards[i] = _cards[randomIndex];
            _cards[randomIndex] = temp;
        }
    }
}
