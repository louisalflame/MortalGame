using System;
using System.Collections.Generic;
using System.Linq;
using Optional;

public interface IGraveyardEntity : ICardColletionZone
{
    IReadOnlyCollection<ICardEntity> PopAllCards();
}
public class GraveyardEntity : CardColletionZone, IGraveyardEntity
{        
    public GraveyardEntity() : base()
    {
    }

    public IReadOnlyCollection<ICardEntity> PopAllCards()
    {
        var cards = _cards.ToList();
        _cards = new List<ICardEntity>();
        return cards;
    }
}