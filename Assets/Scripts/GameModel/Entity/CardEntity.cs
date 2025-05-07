using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    List<ICardEffect> Effects { get; }
    Dictionary<TriggerTiming, List<ICardEffect>> TriggeredEffects { get; }
    IEnumerable<ICardPropertyEntity> Properties { get; }
    IEnumerable<ICardBuffEntity> BuffList { get; }

    IEnumerable<ICardPropertyEntity> AllProperties { get; }

    int EvalCost(IGameplayStatusWatcher gameWatcher);
    int EvalPower(IGameplayStatusWatcher gameWatcher);

    ICardEntity Clone(IEnumerable<ICardBuffEntity> cardBuffs);
    void AddNewStatus(ICardBuffEntity status);
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
    private List<ICardEffect> _effects;
    private Dictionary<TriggerTiming, List<ICardEffect>> _triggeredEffects;
    private List<ICardPropertyEntity> _properties;
    private List<ICardBuffEntity> _buffList;

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
    public List<ICardEffect> Effects => _effects;
    public Dictionary<TriggerTiming, List<ICardEffect>> TriggeredEffects => _triggeredEffects;
    public IEnumerable<ICardPropertyEntity> Properties => _properties;
    public IEnumerable<ICardBuffEntity> BuffList => _buffList;
    public IEnumerable<ICardPropertyEntity> AllProperties => 
        Properties.Concat(BuffList.SelectMany(s => s.Properties));
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
        effects: new List<ICardEffect>(),
        triggeredEffects: new Dictionary<TriggerTiming, List<ICardEffect>>(),
        properties: new List<ICardPropertyEntity>(),
        buffList: new List<CardBuffEntity>()
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
        List<ICardEffect> effects,
        Dictionary<TriggerTiming, List<ICardEffect>> triggeredEffects,
        IEnumerable<ICardPropertyEntity> properties,
        IEnumerable<ICardBuffEntity> buffList
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
        _effects = effects.ToList();
        _triggeredEffects = triggeredEffects.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToList()
        );
        _properties = properties.ToList();
        _buffList = buffList.ToList();
    }

    public static ICardEntity CreateFromInstance(CardInstance cardInstance)
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
            effects: cardInstance.Effects.ToList(),
            triggeredEffects: cardInstance.TriggeredEffects.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.ToList()
                ),
            properties: cardInstance.PropertyDatas.Select(p => p.CreateEntity()),
            buffList: new List<CardBuffEntity>()
        );
    }

    public static ICardEntity CreateFromData(CardData cardData, IEnumerable<ICardBuffEntity> cardBuffs)
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
            effects: cardData.Effects.ToList(),
            triggeredEffects: cardData.TriggeredEffects.ToDictionary(
                    pair => pair.Timing,
                    pair => pair.Effects.ToList()
                ),
            properties: cardData.PropertyDatas.Select(p => p.CreateEntity()),
            buffList: cardBuffs.ToList()
        );
    }

    public ICardEntity Clone(IEnumerable<ICardBuffEntity> cardBuffs)
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
            effects: _effects.ToList(),
            triggeredEffects: _triggeredEffects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()),
            properties: _properties.ToList(),
            buffList: cardBuffs.ToList()
        );
    }

    public void AddNewStatus(ICardBuffEntity status)
    {
        _buffList.Add(status);
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
}

public static class CardEntityExtensions
{
    public static Option<IPlayerEntity> Owner(this ICardEntity card, IGameplayStatusWatcher watcher)
    {
        var allyCardOpt = watcher.GameStatus.Ally.CardManager.GetCard(card.Identity);
        if (allyCardOpt.HasValue)
            return (watcher.GameStatus.Ally as IPlayerEntity).Some();
        var enemyCardOpt = watcher.GameStatus.Enemy.CardManager.GetCard(card.Identity);
        if (enemyCardOpt.HasValue)
            return (watcher.GameStatus.Enemy as IPlayerEntity).Some();
        return Option.None<IPlayerEntity>();
    }
    public static Faction Faction(this ICardEntity card, IGameplayStatusWatcher watcher)
    {
        return card.Owner(watcher).ValueOr(PlayerEntity.DummyPlayer).Faction;
    }

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