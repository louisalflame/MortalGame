using System.Collections.Generic;
using System.Linq;

public interface IGraveyardEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    void AddCard(ICardEntity card);
    void AddCards(IEnumerable<ICardEntity> cards);
    IReadOnlyCollection<ICardEntity> PopAllCards();
}
public class GraveyardEntity : IGraveyardEntity
{
    private List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
        
    public GraveyardEntity()
    {
        _cards = new List<ICardEntity>();
    }

    public void AddCard(ICardEntity card)
    {
        _cards.Add(card);
    }
    public void AddCards(IEnumerable<ICardEntity> cards)
    {
        _cards.AddRange(cards);
    }

    public IReadOnlyCollection<ICardEntity> PopAllCards()
    {
        var cards = _cards.ToList();
        _cards = new List<ICardEntity>();
        return cards;
    }
}