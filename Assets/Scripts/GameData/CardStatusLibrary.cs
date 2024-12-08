using System.Collections.Generic;
using UnityEngine;

public class CardStatusLibrary
{
    private readonly Dictionary<string, CardStatusData> _cardStatuses;

    public CardStatusLibrary(IReadOnlyDictionary<string, CardStatusData> cardStatuses)
    {
        _cardStatuses = new Dictionary<string, CardStatusData>(cardStatuses);
    }

    public CardStatusData GetCardStatusData(string cardStatusId)
    {
        return _cardStatuses[cardStatusId];
    }
}
