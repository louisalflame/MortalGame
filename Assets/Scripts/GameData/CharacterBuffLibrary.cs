using System;
using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Collections;
using UnityEngine;

public class CharacterBuffLibrary
{
    private readonly Dictionary<string, CharacterBuffData> _buffs;

    public CharacterBuffLibrary(IReadOnlyDictionary<string, CharacterBuffData> buffs)
    {
        _buffs = new Dictionary<string, CharacterBuffData>(buffs);
    }

    public Option<ConditionalCharacterBuffEffect[]> GetBuffEffects(string buffId, GameTiming gameTiming)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return Option.None<ConditionalCharacterBuffEffect[]>();
        }

        return LinqEnumerableExtensions.FirstOrNone(
                _buffs[buffId].BuffEffects
                    .Where(pair => pair.Timing == gameTiming)
                    .Select(pair => pair.ConditionEffects)
            );
    }

    public IReactionSessionData[] GetBuffSessions(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return null;
        }

        return _buffs[buffId].Sessions.ToArray();
    }

    public IReactionSessionData GetBuffSessionData(string buffId, string sessionId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return null;
        }

        var data = _buffs[buffId].Sessions.First(s => s.Id == sessionId);
        if (data == null)
        {
            Debug.LogError($"Session ID {sessionId} not found in buff ID {buffId}.");
            return null;
        }

        return data;
    }

    public ICharacterBuffLifeTimeData GetBuffLifeTime(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return null;
        }

        return _buffs[buffId].LifeTimeData;
    }

    public ICharacterBuffPropertyData[] GetBuffProperties(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return Array.Empty<ICharacterBuffPropertyData>();
        }

        return _buffs[buffId].PropertyDatas.ToArray();
    }
}
