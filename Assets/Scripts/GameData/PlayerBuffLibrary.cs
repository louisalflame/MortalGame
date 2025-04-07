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

    public Option<ConditionalEffect[]> GetBuffEffects(string buffId, GameTiming gameTiming)
    {
        if (!_buffs.ContainsKey(buffId))
        {
            Debug.LogError($"Buff ID {buffId} not found in library.");
            return Option.None<ConditionalEffect[]>();
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
            return new IReactionSessionData[0];
        }

        return _buffs[buffId].Sessions.ToArray();
    }
}
