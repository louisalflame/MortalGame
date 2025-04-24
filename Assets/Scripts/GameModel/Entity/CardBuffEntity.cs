using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ICardBuffEntity
{
    string CardBuffDataID { get; }
    Dictionary<TriggerTiming, List<ICardEffect>> Effects { get; }
    List<ICardPropertyEntity> Properties { get; }
    
    bool IsExpired();
    void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing);
    void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent);
    void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result);
}

public class CardBuffEntity : ICardBuffEntity
{
    public string CardBuffDataID { get; private set; }

    public Dictionary<TriggerTiming, List<ICardEffect>> Effects { get; private set; }

    public List<ICardPropertyEntity> Properties { get; private set; }

    public ICardBuffLifeTimeEntity LifeTime { get; private set; }

    private CardBuffEntity(
        string cardBuffDataID,
        Dictionary<TriggerTiming, List<ICardEffect>> effects,
        List<ICardPropertyEntity> properties,
        ICardBuffLifeTimeEntity lifeTime)
    {
        CardBuffDataID = cardBuffDataID;
        Effects = effects;
        Properties = properties;
        LifeTime = lifeTime;
    }

    public static CardBuffEntity CreateEntity(CardBuffData data)
    {
        return new CardBuffEntity(
            data.ID,
            data.Effects.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.ToList()),
            data.PropertyDatas.Select(p => p.CreateEntity()).ToList(),
            data.LifeTimeData.CreateEntity()
        );
    }

    public bool IsExpired()
    {
        return LifeTime.IsExpired();
    }

    public void UpdateByTiming(IGameplayStatusWatcher gameWatcher, UpdateTiming timing)
    {
        foreach(var property in Properties)
        {
            property.UpdateByTiming(gameWatcher, timing);
        }

        LifeTime.UpdateByTiming(gameWatcher, timing);
    }

    public void UpdateIntent(IGameplayStatusWatcher gameWatcher, IIntentAction intent)
    { }

    public void UpdateResult(IGameplayStatusWatcher gameWatcher, IResultAction result)
    { }
}
