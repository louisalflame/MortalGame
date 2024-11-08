using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor.PackageManager;
using UnityEngine;


public class CardEntity
{
    public Guid Indentity { get; private set; }
    public Guid OriginCardInstanceGuid { get; private set; }
    public string CardDataId { get; private set; }
    public string Title { get; private set; }
    public string Info { get; private set; }

    public CardType Type { get; private set; }
    public CardRarity Rarity { get; private set; }
    public CardTheme[] Themes = new CardTheme[0];
    public int Cost { get; private set; }
    public int Power { get; private set; }
    
    public List<ITargetSelectable> Selectables;

    public Dictionary<CardTiming, List<ICardEffect>> Effects { get; private set; }

    public List<ICardPropertyEntity> Properties { get; private set; }
    public List<ICardPropertyEntity> AppendProperties { get; private set; }
    public IEnumerable<ICardPropertyEntity> AllProperties => Properties.Concat(AppendProperties);


    public bool IsDummy => this == DummyCard;
    public static CardEntity DummyCard = new CardEntity()
    {
        Indentity = Guid.Empty,
    };

    public static CardEntity Create(CardInstance cardInstance)
    {
        return new CardEntity(){
            Indentity = Guid.NewGuid(),
            CardDataId = cardInstance.CardDataId,
            Title = cardInstance.TitleKey,
            Info = cardInstance.InfoKey,
            Type = cardInstance.Type,
            Rarity = cardInstance.Rarity,
            Themes = cardInstance.Themes.ToArray(),
            Cost = cardInstance.Cost,
            Power = cardInstance.Power,
            Selectables = cardInstance.Selectables.ToList(),
            Effects = cardInstance.Effects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()
            ),
            Properties = cardInstance.PropertyDatas.Select(p => p.CreateEntity()).ToList(),
            AppendProperties = new List<ICardPropertyEntity>(),
            OriginCardInstanceGuid = cardInstance.InstanceGuid,
        };
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
            property.Lifetime.UpdateTiming(contextManager, timing);
        }
        
        Properties = Properties.Where(p => !p.Lifetime.IsExpired).ToList();
        AppendProperties = AppendProperties.Where(p => !p.Lifetime.IsExpired).ToList();

        // TODO: return event of property expired
        gameEvent = NoneEvent.Instance;
        return false;
    }
}