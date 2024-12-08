using System;
using System.Collections.Generic;
using System.Linq;

public interface ICardEntity
{
    Guid Indentity { get; }
    Guid OriginCardInstanceGuid { get; }
    string CardDataId { get; }

    CardType Type { get; }
    CardRarity Rarity { get; }
    IEnumerable<CardTheme> Themes { get; }
    int Cost { get; }
    int Power { get; }

    IMainTargetSelectable MainSelectable { get; }
    IEnumerable<ISubTargetSelectable> SubSelectables { get; }

    Dictionary<CardTiming, List<ICardEffect>> Effects { get; }
    IEnumerable<ICardPropertyEntity> Properties { get; }
    IEnumerable<CardStatusEntity> StatusList { get; }

    IEnumerable<ICardPropertyEntity> AllProperties { get; }

    int EvalCost(GameContext gameContext);
    int EvalPower(GameContext gameContext);

    bool TryUpdateCardsOnTiming(GameContextManager contextManager, CardTiming timing, out IGameEvent gameEvent);
}

public class CardEntity : ICardEntity
{
    private Guid _indentity;
    private Guid _originCardInstanceGuid;
    private string _cardDataId;
    private CardType _type;
    private CardRarity _rarity;
    private CardTheme[] _themes;
    private int _cost;
    private int _power;    
    private IMainTargetSelectable _mainSelectable;
    private List<ISubTargetSelectable> _subSelectables;
    private Dictionary<CardTiming, List<ICardEffect>> _effects;
    private List<ICardPropertyEntity> _properties;
    private List<CardStatusEntity> _statusList;

    public Guid Indentity => _indentity;
    public Guid OriginCardInstanceGuid => _originCardInstanceGuid;
    public string CardDataId => _cardDataId;
    public CardType Type => _type;
    public CardRarity Rarity => _rarity;
    public IEnumerable<CardTheme> Themes => _themes;
    public int Cost => _cost;
    public int Power => _power;
    public IMainTargetSelectable MainSelectable => _mainSelectable;
    public IEnumerable<ISubTargetSelectable> SubSelectables => _subSelectables;
    public Dictionary<CardTiming, List<ICardEffect>> Effects => _effects;
    public IEnumerable<ICardPropertyEntity> Properties => _properties;
    public IEnumerable<CardStatusEntity> StatusList => _statusList;

    public IEnumerable<ICardPropertyEntity> AllProperties => 
        Properties.Concat(StatusList.SelectMany(s => s.Properties));

    public bool IsDummy => this == DummyCard;
    public static ICardEntity DummyCard = new CardEntity(
        indentity: Guid.Empty,
        originCardInstanceGuid: Guid.Empty,
        cardDataId: string.Empty,
        type: CardType.None,
        rarity: CardRarity.None,
        themes: new CardTheme[0],
        cost: 0,
        power: 0,
        mainSelectable: new NoneSelectable(),
        subSelectables: new List<ISubTargetSelectable>(),
        effects: new Dictionary<CardTiming, List<ICardEffect>>(),
        properties: new List<ICardPropertyEntity>(),
        statusList: new List<CardStatusEntity>()
    );
    
    private CardEntity(
        Guid indentity,
        Guid originCardInstanceGuid,
        string cardDataId,
        CardType type,
        CardRarity rarity,
        IEnumerable<CardTheme> themes,
        int cost,
        int power,
        IMainTargetSelectable mainSelectable,
        IEnumerable<ISubTargetSelectable> subSelectables,
        Dictionary<CardTiming, List<ICardEffect>> effects,
        IEnumerable<ICardPropertyEntity> properties,
        IEnumerable<CardStatusEntity> statusList
    )
    {
        _indentity = indentity;
        _originCardInstanceGuid = originCardInstanceGuid;
        _cardDataId = cardDataId;
        _type = type;
        _rarity = rarity;
        _themes = themes.ToArray();
        _cost = cost;
        _power = power;
        _mainSelectable = mainSelectable;
        _subSelectables = subSelectables.ToList();
        _effects = effects.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToList()
        );
        _properties = properties.ToList();
        _statusList = statusList.ToList();
    }

    public static CardEntity Create(CardInstance cardInstance)
    {
        return new CardEntity(
            indentity: Guid.NewGuid(),
            originCardInstanceGuid: cardInstance.InstanceGuid,
            cardDataId: cardInstance.CardDataId,
            type: cardInstance.Type,
            rarity: cardInstance.Rarity,
            themes: cardInstance.Themes,
            cost: cardInstance.Cost,
            power: cardInstance.Power,
            mainSelectable: cardInstance.MainSelectable,
            subSelectables: cardInstance.SubSelectables,
            effects: cardInstance.Effects.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.ToList()
                ),
            properties: cardInstance.PropertyDatas.Select(p => p.CreateEntity()),
            statusList: new List<CardStatusEntity>()
        );
    }

    public int EvalCost(GameContext gameContext)
    {
        var cost = Cost;
        foreach(var property in AllProperties.Where(p => p.Property == CardProperty.CostAdjust))
        {
            cost = property.Value.Eval(cost);
        } 

        return cost;
    }

    public int EvalPower(GameContext gameContext)
    {
        var power = Power;
        foreach(var property in AllProperties.Where(p => p.Property == CardProperty.PowerAdjust))
        {
            power = property.Value.Eval(power);
        } 

        return power;
    }

    public bool TryUpdateCardsOnTiming(GameContextManager contextManager, CardTiming timing, out IGameEvent gameEvent)
    {
        foreach(var property in AllProperties)
        {
            property.UseCount.UpdateTiming(contextManager, timing);
        }
        
        // TODO: return event of property expired
        gameEvent = NoneEvent.Instance;
        return false;
    }
}