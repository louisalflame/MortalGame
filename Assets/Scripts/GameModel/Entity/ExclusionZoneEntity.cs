using System.Collections.Generic;
using System.Linq;

public interface IExclusionZoneEntity
{
    IReadOnlyCollection<ICardEntity> Cards { get; }
    void AddCard(ICardEntity card);
    void AddCards(IEnumerable<ICardEntity> cards);
}

public class ExclusionZoneEntity : IExclusionZoneEntity
{
    private List<ICardEntity> _cards;
    public IReadOnlyCollection<ICardEntity> Cards => _cards;
        
    public ExclusionZoneEntity()
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