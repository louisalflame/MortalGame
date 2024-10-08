using System.Collections.Generic;
using System.Linq;

public interface IGraveyardEntity
{
    IReadOnlyCollection<CardEntity> Cards { get; }
    CardCollectionInfo CardCollectionInfo { get; }
    void AddCard(CardEntity card);
    void AddCards(IEnumerable<CardEntity> cards);
    IReadOnlyCollection<CardEntity> PopAllCards();
}
public class GraveyardEntity : IGraveyardEntity
{
    private List<CardEntity> _cards;
    public IReadOnlyCollection<CardEntity> Cards => _cards;

    public CardCollectionInfo CardCollectionInfo =>
        _cards.Select(c => new CardInfo(c)).ToArray().ToCardCollectionInfo();
        
    public GraveyardEntity()
    {
        _cards = new List<CardEntity>();
    }

    public void AddCard(CardEntity card)
    {
        _cards.Add(card);
    }
    public void AddCards(IEnumerable<CardEntity> cards)
    {
        _cards.AddRange(cards);
    }

    public IReadOnlyCollection<CardEntity> PopAllCards()
    {
        var cards = _cards.ToList();
        _cards = new List<CardEntity>();
        return cards;
    }
}