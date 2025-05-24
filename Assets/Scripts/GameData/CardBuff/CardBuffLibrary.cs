using System;
using System.Collections.Generic;
using UnityEngine;

public class CardBuffLibrary
{
    private readonly Dictionary<string, CardBuffData> _buffs;

    public CardBuffLibrary(IReadOnlyDictionary<string, CardBuffData> cardBuffs)
    {
        _buffs = new Dictionary<string, CardBuffData>(cardBuffs);
    }

    public ICardBuffLifeTimeData GetCardBuffLifeTime(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"CardBuff ID[{buffId}] not found in library.");
            return null;
        }

        return _buffs[buffId].LifeTimeData;
    }

    public IReactionSessionData[] GetCardBuffSessions(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"CardBuff ID[{buffId}] not found in library.");
            return Array.Empty<IReactionSessionData>();
        }

        return _buffs[buffId].Sessions.ToArray();
    }

    public ICardBuffPropertyData[] GetCardBuffProperties(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"CardBuff ID[{buffId}] not found in library.");
            return Array.Empty<ICardBuffPropertyData>();
        }

        return _buffs[buffId].PropertyDatas.ToArray();
    }
}
