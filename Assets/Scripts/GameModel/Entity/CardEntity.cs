using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Unity.VisualScripting;

public interface ICardEntity
{
    Guid Identity { get; }
    Option<Guid> OriginCardInstanceGuid { get; }
    string CardDataId { get; }

    CardType Type { get; }
    CardRarity Rarity { get; }
    IEnumerable<CardTheme> Themes { get; }
    int Cost { get; }
    int Power { get; }

    IMainTargetSelectable MainSelectable { get; }
    IEnumerable<ISubTargetSelectable> SubSelectables { get; }

    Dictionary<GameTiming, List<ICardEffect>> Effects { get; }
    IEnumerable<ICardPropertyEntity> Properties { get; }
    IEnumerable<ICardStatusEntity> StatusList { get; }

    IEnumerable<ICardPropertyEntity> AllProperties { get; }

    IPlayerEntity Owner { get; }

    int EvalCost(IGameplayStatusWatcher gameWatcher);
    int EvalPower(IGameplayStatusWatcher gameWatcher);

    bool TryUpdateCardsOnTiming(IGameplayStatusWatcher gameWatcher, GameTiming timing, out IGameEvent gameEvent);

    ICardEntity Clone(IPlayerEntity cloneOwner, IEnumerable<ICardStatusEntity> cardStatuses);
    void AddNewStatus(ICardStatusEntity status);
}

public class CardEntity : ICardEntity
{
    private Guid _indentity;
    private Option<Guid> _originCardInstanceGuid;
    private string _cardDataId;
    private CardType _type;
    private CardRarity _rarity;
    private CardTheme[] _themes;
    private int _cost;
    private int _power;    
    private IMainTargetSelectable _mainSelectable;
    private List<ISubTargetSelectable> _subSelectables;
    private Dictionary<GameTiming, List<ICardEffect>> _effects;
    private List<ICardPropertyEntity> _properties;
    private List<ICardStatusEntity> _statusList;
    private IPlayerEntity _owner;

    public Guid Identity => _indentity;
    public Option<Guid> OriginCardInstanceGuid => _originCardInstanceGuid;
    public string CardDataId => _cardDataId;
    public CardType Type => _type;
    public CardRarity Rarity => _rarity;
    public IEnumerable<CardTheme> Themes => _themes;
    public int Cost => _cost;
    public int Power => _power;
    public IMainTargetSelectable MainSelectable => _mainSelectable;
    public IEnumerable<ISubTargetSelectable> SubSelectables => _subSelectables;
    public Dictionary<GameTiming, List<ICardEffect>> Effects => _effects;
    public IEnumerable<ICardPropertyEntity> Properties => _properties;
    public IEnumerable<ICardStatusEntity> StatusList => _statusList;
    public IEnumerable<ICardPropertyEntity> AllProperties => 
        Properties.Concat(StatusList.SelectMany(s => s.Properties));
    public IPlayerEntity Owner => _owner;
    public bool IsDummy => this == DummyCard;

    public static ICardEntity DummyCard = new CardEntity(
        indentity: Guid.Empty,
        originCardInstanceGuid: Option.None<Guid>(),
        cardDataId: string.Empty,
        type: CardType.None,
        rarity: CardRarity.None,
        themes: new CardTheme[0],
        cost: 0,
        power: 0,
        mainSelectable: new NoneSelectable(),
        subSelectables: new List<ISubTargetSelectable>(),
        effects: new Dictionary<GameTiming, List<ICardEffect>>(),
        properties: new List<ICardPropertyEntity>(),
        statusList: new List<CardStatusEntity>(),
        owner: PlayerEntity.DummyPlayer
    );
    
    private CardEntity(
        Guid indentity,
        Option<Guid> originCardInstanceGuid,
        string cardDataId,
        CardType type,
        CardRarity rarity,
        IEnumerable<CardTheme> themes,
        int cost,
        int power,
        IMainTargetSelectable mainSelectable,
        IEnumerable<ISubTargetSelectable> subSelectables,
        Dictionary<GameTiming, List<ICardEffect>> effects,
        IEnumerable<ICardPropertyEntity> properties,
        IEnumerable<ICardStatusEntity> statusList,
        IPlayerEntity owner
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
        _owner = owner;
    }

    public static ICardEntity CreateFromInstance(CardInstance cardInstance, IPlayerEntity owner)
    {
        return new CardEntity(
            indentity: Guid.NewGuid(),
            originCardInstanceGuid: cardInstance.InstanceGuid.Some(),
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
            statusList: new List<CardStatusEntity>(),
            owner: owner
        );
    }

    public static ICardEntity CreateFromData(CardData cardData, IPlayerEntity owner, IEnumerable<ICardStatusEntity> cardStatuses)
    {
        return new CardEntity(
            indentity: Guid.NewGuid(),
            originCardInstanceGuid: Option.None<Guid>(),
            cardDataId: cardData.ID,
            type: cardData.Type,
            rarity: cardData.Rarity,
            themes: cardData.Themes,
            cost: cardData.Cost,
            power: cardData.Power,
            mainSelectable: cardData.MainSelectable,
            subSelectables: cardData.SubSelectables,
            effects: cardData.CardEffects.ToDictionary(
                    pair => pair.Timing,
                    pair => pair.Effects.ToList()
                ),
            properties: cardData.PropertyDatas.Select(p => p.CreateEntity()),
            statusList: new List<CardStatusEntity>(),
            owner: owner
        );
    }

    public ICardEntity Clone(IPlayerEntity cloneOwner, IEnumerable<ICardStatusEntity> cardStatuses)
    {
        return new CardEntity(
            indentity: Guid.NewGuid(),
            originCardInstanceGuid: Option.None<Guid>(),
            cardDataId: _cardDataId,
            type: _type,
            rarity: _rarity,
            themes: _themes,
            cost: _cost,
            power: _power,
            mainSelectable: _mainSelectable,
            subSelectables: _subSelectables,
            effects: _effects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()),
            properties: _properties.ToList(),
            statusList: cardStatuses.ToList(),
            owner: cloneOwner
        );
    }

    public void AddNewStatus(ICardStatusEntity status)
    {
        _statusList.Add(status);
    }

    public int EvalCost(IGameplayStatusWatcher gameWatcher)
    {
        var cost = Cost;
        foreach(var property in AllProperties.Where(p => p.Property == CardProperty.CostAdjust))
        {
            cost += property.Eval(gameWatcher);
        } 

        return cost;
    }

    public int EvalPower(IGameplayStatusWatcher gameWatcher)
    {
        var power = Power;
        foreach(var property in AllProperties.Where(p => p.Property == CardProperty.PowerAdjust))
        {
            power += property.Eval(gameWatcher);
        } 

        return power;
    }

    public bool TryUpdateCardsOnTiming(IGameplayStatusWatcher gameWatcher, GameTiming timing, out IGameEvent gameEvent)
    {
        foreach(var cardStatus in StatusList)
        {
            cardStatus.UpdateTiming(gameWatcher, timing);
        }
        
        // TODO: return event of property expired
        gameEvent = NoneEvent.Instance;
        return false;
    }
}

public static class CardEntityExtensions
{
    public static bool IsConsumable(this ICardEntity card)
    {
        return card.HasProperty(CardProperty.Consumable);
    }
    public static bool IsDisposable(this ICardEntity card)
    {
        return card.HasProperty(CardProperty.Dispose) || card.HasProperty(CardProperty.AutoDispose);
    }

    public static bool HasProperty(this ICardEntity card, CardProperty property)
    { 
        return card.AllProperties.Any(p => p.Property == property);
    }
}