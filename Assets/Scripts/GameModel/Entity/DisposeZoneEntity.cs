using System.Collections.Generic;
using System.Linq;

// Cards in the dispose zone are removed from the Deck forever in this game.
public interface IDisposeZoneEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    void AddCard(ICardEntity card);
    void AddCards(IEnumerable<ICardEntity> cards);
}

public class DisposeZoneEntity : IDisposeZoneEntity
{
    private List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
        
    public DisposeZoneEntity()
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
}