using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBuffLibrary
{
    private readonly Dictionary<string, PlayerBuffData> _buffs;

    public PlayerBuffLibrary(IReadOnlyDictionary<string, PlayerBuffData> buffs)
    {
        _buffs = new Dictionary<string, PlayerBuffData>(buffs);
    }

    public Dictionary<BuffTiming, IPlayerBuffEffect[]>  GetBuffEffects(string buffId)
    {
        return _buffs[buffId].Effects.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToArray());
    }
}
