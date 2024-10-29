using System.Collections.Generic;
using System.Linq;

public interface IExclusionZoneEntity
{
    IReadOnlyCollection<CardEntity> Cards { get; }
    CardCollectionInfo CardCollectionInfo { get; }
    void AddCard(CardEntity card);
    void AddCards(IEnumerable<CardEntity> cards);
}

public class ExclusionZoneEntity : IExclusionZoneEntity
{
    private List<CardEntity> _cards;
    public IReadOnlyCollection<CardEntity> Cards => _cards;

    public CardCollectionInfo CardCollectionInfo =>
        _cards.Select(c => new CardInfo(c)).ToArray().ToCardCollectionInfo();
        
    public ExclusionZoneEntity()
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
}