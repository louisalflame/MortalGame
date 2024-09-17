using System.Collections.Generic;
using UnityEngine;

public class BuffLibrary
{
    private readonly Dictionary<string, BuffData> _buffs;

    public BuffLibrary(IReadOnlyDictionary<string, BuffData> buffs)
    {
        _buffs = new Dictionary<string, BuffData>(buffs);
    }

    public BuffData GetBuffData(string buffId)
    {
        return _buffs[buffId];
    }
}
