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

    IMainTargetSelectable MainSelectable { get; }
    IEnumerable<ISubTargetSelectable> SubSelectables { get; }

    List<ICardEffect> Effects { get; }
    Dictionary<TriggerTiming, List<ICardEffect>> TriggeredEffects { get; }
    IEnumerable<ICardPropertyEntity> Properties { get; }
    ICardBuffManager BuffManager { get; }

    int OriginCost { get; }
    int OriginPower { get; }

    ICardEntity Clone();
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
    private ICardBuffManager _buffManager;

    public Guid Identity => _indentity;
    public Option<Guid> OriginCardInstanceGuid => _originCardInstanceGuid;
    public string CardDataId => _cardDataId;
    public CardType Type => _type;
    public CardRarity Rarity => _rarity;
    public int OriginCost => _cost;
    public int OriginPower => _power;
    public IEnumerable<CardTheme> Themes => _themes;
    public IMainTargetSelectable MainSelectable => _mainSelectable;
    public IEnumerable<ISubTargetSelectable> SubSelectables => _subSelectables;
    public List<ICardEffect> Effects => _effects;
    public Dictionary<TriggerTiming, List<ICardEffect>> TriggeredEffects => _triggeredEffects;
    public IEnumerable<ICardPropertyEntity> Properties => _properties;
    public ICardBuffManager BuffManager => _buffManager;
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
        properties: new List<ICardPropertyEntity>()
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
        IEnumerable<ICardPropertyEntity> properties
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
        _buffManager = new CardBuffManager();
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
            properties: cardInstance.PropertyDatas.Select(p => p.CreateEntity())
        );
    }

    public static ICardEntity RuntimeCreateFromData(CardData cardData)
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
            properties: cardData.PropertyDatas.Select(p => p.CreateEntity())
        );
    }

    public ICardEntity Clone()
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
            properties: _properties.ToList()
        );
    }

    public int EvalCost(IGameplayStatusWatcher gameWatcher)
    {
        var cost = _cost;
        var cardTrigger = new CardTrigger(this);
        var systemAction = new SystemAction();
        foreach (var property in Properties.Where(p => p.Property == CardProperty.CostAddition))
        {
            cost += property.Eval(gameWatcher, cardTrigger);
        }
        foreach (var buff in BuffManager.Buffs)
        {
            foreach (var property in buff.Properties.Where(p => p.Property == CardProperty.CostAddition))
            {
                cost += property.Eval(gameWatcher, cardTrigger);
            }
        }

        return cost;
    }
}

public static class CardEntityExtensions
{
    public static Option<IPlayerEntity> Owner(this ICardEntity card, GameStatus gameStatus)
    {
        var allyCardOpt = gameStatus.Ally.CardManager.GetCard(card.Identity);
        if (allyCardOpt.HasValue)
            return (gameStatus.Ally as IPlayerEntity).Some();
        var enemyCardOpt = gameStatus.Enemy.CardManager.GetCard(card.Identity);
        if (enemyCardOpt.HasValue)
            return (gameStatus.Enemy as IPlayerEntity).Some();
        return Option.None<IPlayerEntity>();
    }
    public static Faction Faction(this ICardEntity card, GameStatus gameStatus)
    {
        return card.Owner(gameStatus).ValueOr(PlayerEntity.DummyPlayer).Faction;
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
        return
            card.Properties.Any(p => p.Property == property) ||
            card.BuffManager.Buffs.Any(b => b.Properties.Any(p => p.Property == property));
    }
    
    public static int GetCardProperty(
        this ICardEntity card, IGameplayStatusWatcher watcher, CardProperty targetProperty)
    {
        var value = 0;

        var cardTrigger = new CardTrigger(card);
        foreach (var property in card.Properties.Where(p => p.Property == targetProperty))
        {
            value += property.Eval(watcher, cardTrigger);
        }

        foreach (var buff in card.BuffManager.Buffs)
        {
            var cardBuffTrigger = new CardBuffTrigger(buff);
            foreach (var property in buff.Properties.Where(p => p.Property == targetProperty))
            {
                value += property.Eval(watcher, cardBuffTrigger);
            }
        }

        return value;
    }
}