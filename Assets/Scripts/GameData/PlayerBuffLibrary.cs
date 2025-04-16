using System.Collections.Generic;
using System.Linq;
using Optional;
using Optional.Collections;
using UnityEngine;

public class PlayerBuffLibrary
{
    private readonly Dictionary<string, PlayerBuffData> _buffs;

    public PlayerBuffLibrary(IReadOnlyDictionary<string, PlayerBuffData> buffs)
    {
        _buffs = new Dictionary<string, PlayerBuffData>(buffs);
    }

    public Option<ConditionalPlayerBuffEffect[]> GetBuffEffects(string buffId, GameTiming gameTiming)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return Option.None<ConditionalPlayerBuffEffect[]>();
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

    public IPlayerBuffLifeTimeData GetBuffLifeTime(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return null;
        }

        return _buffs[buffId].LifeTimeData;
    }

    public IPlayerBuffPropertyData[] GetBuffProperties(string buffId)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return new IPlayerBuffPropertyData[0];
        }

        return _buffs[buffId].PropertyDatas.ToArray();
    }
}
