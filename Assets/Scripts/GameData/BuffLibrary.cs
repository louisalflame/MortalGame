using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffLibrary
{
    private readonly Dictionary<string, BuffData> _buffs;

    public BuffLibrary(IReadOnlyDictionary<string, BuffData> buffs)
    {
        _buffs = new Dictionary<string, BuffData>(buffs);
    }

    public Dictionary<BuffTiming, IBuffEffect[]>  GetBuffEffects(string buffId)
    {
        return _buffs[buffId].Effects.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.ToArray());
    }
}
