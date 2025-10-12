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

    MainTargetSelectLogic MainSelect { get; }
    IEnumerable<SubTargetSelectLogic> SubSelects { get; }

    IEnumerable<ICardEffect> Effects { get; }
    IReadOnlyDictionary<GameTiming, List<ICardEffect>> TriggeredEffects { get; }
    IEnumerable<ICardPropertyEntity> Properties { get; }
    ICardBuffManager BuffManager { get; }

    int OriginCost { get; }
    int OriginPower { get; }

    ICardEntity Clone();
}

public class CardEntity : ICardEntity
{
    private readonly Guid _indentity;
    private readonly Option<Guid> _originCardInstanceGuid;
    private readonly string _cardDataId;
    private readonly CardType _type;
    private readonly CardRarity _rarity;
    private readonly CardTheme[] _themes;
    private readonly int _cost;
    private readonly int _power;
    private readonly MainTargetSelectLogic _mainSelect;
    private readonly IReadOnlyList<SubTargetSelectLogic> _subSelects;
    private readonly IReadOnlyList<ICardEffect> _effects;
    private readonly IReadOnlyDictionary<GameTiming, List<ICardEffect>> _triggeredEffects;
    private readonly IReadOnlyList<ICardPropertyEntity> _properties;
    private readonly ICardBuffManager _buffManager;

    public Guid Identity => _indentity;
    public Option<Guid> OriginCardInstanceGuid => _originCardInstanceGuid;
    public string CardDataId => _cardDataId;
    public CardType Type => _type;
    public CardRarity Rarity => _rarity;
    public int OriginCost => _cost;
    public int OriginPower => _power;
    public IEnumerable<CardTheme> Themes => _themes;
    public MainTargetSelectLogic MainSelect => _mainSelect;
    public IEnumerable<SubTargetSelectLogic> SubSelects => _subSelects;
    public IEnumerable<ICardEffect> Effects => _effects;
    public IReadOnlyDictionary<GameTiming, List<ICardEffect>> TriggeredEffects => _triggeredEffects;
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
        mainSelect: new (),
        subSelects: new List<SubTargetSelectLogic>(),
        effects: new List<ICardEffect>(),
        triggeredEffects: new Dictionary<GameTiming, List<ICardEffect>>(),
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
        MainTargetSelectLogic mainSelect,
        IEnumerable<SubTargetSelectLogic> subSelects,
        List<ICardEffect> effects,
        Dictionary<GameTiming, List<ICardEffect>> triggeredEffects,
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
        _mainSelect = mainSelect;
        _subSelects = subSelects.ToList();
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
            mainSelect: cardInstance.MainSelect,
            subSelects: cardInstance.SubSelects,
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
            mainSelect: cardData.MainSelect,
            subSelects: cardData.SubSelects,
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
            mainSelect: _mainSelect,
            subSelects: _subSelects,
            effects: _effects.ToList(),
            triggeredEffects: _triggeredEffects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()),
            properties: _properties.ToList()
        );
    }
}

public static class CardEntityExtensions
{
    public static Option<ICardEntity> GetCard(this IGameplayStatusWatcher gameplayWatcher, Guid identity)
    {
        var allyCardOpt = gameplayWatcher.GameStatus.Ally.CardManager.GetCard(identity);
        if (allyCardOpt.HasValue)
            return allyCardOpt;
        var enemyCardOpt = gameplayWatcher.GameStatus.Enemy.CardManager.GetCard(identity);
        if (enemyCardOpt.HasValue)
            return enemyCardOpt;
        return Option.None<ICardEntity>();
    }

    public static Option<IPlayerEntity> Owner(this ICardEntity card, IGameplayStatusWatcher gameplayWatcher)
    {
        var gameStatus = gameplayWatcher.GameStatus;
        var allyCardOpt = gameStatus.Ally.CardManager.GetCard(card.Identity);
        if (allyCardOpt.HasValue)
            return (gameStatus.Ally as IPlayerEntity).Some();
        var enemyCardOpt = gameStatus.Enemy.CardManager.GetCard(card.Identity);
        if (enemyCardOpt.HasValue)
            return (gameStatus.Enemy as IPlayerEntity).Some();
        return Option.None<IPlayerEntity>();
    }
    public static Faction Faction(this ICardEntity card, IGameplayStatusWatcher gameplayWatcher)
    {
        return card.Owner(gameplayWatcher).ValueOr(PlayerEntity.DummyPlayer).Faction;
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