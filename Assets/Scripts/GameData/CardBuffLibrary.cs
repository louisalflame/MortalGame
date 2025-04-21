using System.Collections.Generic;
using UnityEngine;

public class CardBuffLibrary
{
    private readonly Dictionary<string, CardBuffData> _cardBuffs;

    public CardBuffLibrary(IReadOnlyDictionary<string, CardBuffData> cardBuffs)
    {
        _cardBuffs = new Dictionary<string, CardBuffData>(cardBuffs);
    }

    public CardBuffData GetCardBuffData(string cardBuffId)
    {
        return _cardBuffs[cardBuffId];
    }
}
