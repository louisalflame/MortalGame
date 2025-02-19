using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardStatusEntity
{
    string CardStatusDataID { get; }
    Dictionary<CardTiming, List<ICardEffect>> Effects { get; }
    List<ICardPropertyEntity> Properties { get; }
    ICardStatusLifeTimeEntity LifeTime { get; }

    void UpdateTiming(GameContextManager contextManager, CardTiming timing);
}

public class CardStatusEntity : ICardStatusEntity
{
    public string CardStatusDataID { get; private set; }

    public Dictionary<CardTiming, List<ICardEffect>> Effects { get; private set; }

    public List<ICardPropertyEntity> Properties { get; private set; }

    public ICardStatusLifeTimeEntity LifeTime { get; private set; }

    private CardStatusEntity(
        string cardStatusDataID,
        Dictionary<CardTiming, List<ICardEffect>> effects,
        List<ICardPropertyEntity> properties,
        ICardStatusLifeTimeEntity lifeTime)
    {
        CardStatusDataID = cardStatusDataID;
        Effects = effects;
        Properties = properties;
        LifeTime = lifeTime;
    }

    public static CardStatusEntity CreateEntity(CardStatusData data)
    {
        return new CardStatusEntity(
            data.ID,
            data.Effects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()),
            data.PropertyDatas.Select(p => p.CreateEntity()).ToList(),
            data.LifeTimeData.CreateEntity()
        );
    }

    public void UpdateTiming(GameContextManager contextManager, CardTiming timing)
    {
        foreach (var property in Properties)
        {
            property.UpdateTiming(contextManager, timing);
        }

        LifeTime.UpdateTiming(contextManager, timing);
    }
}
