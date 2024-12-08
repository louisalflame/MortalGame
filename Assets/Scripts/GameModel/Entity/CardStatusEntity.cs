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

public abstract class CardStatusEntity : ICardStatusEntity
{
    public string CardStatusDataID { get; private set; }

    public Dictionary<CardTiming, List<ICardEffect>> Effects { get; private set; }

    public List<ICardPropertyEntity> Properties { get; private set; }

    public ICardStatusLifeTimeEntity LifeTime { get; private set; }

    public CardStatusEntity(
        string cardStatusDataID,
        IReadOnlyDictionary<CardTiming, List<ICardEffect>> effects,
        IReadOnlyCollection<ICardPropertyEntity> properties,
        ICardStatusLifeTimeEntity lifeTime)
    {
        CardStatusDataID = cardStatusDataID;

        Effects = effects.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToList()
        );
        Properties = properties.ToList();
        LifeTime = lifeTime;
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
