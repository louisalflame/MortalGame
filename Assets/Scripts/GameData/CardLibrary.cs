using System.Collections.Generic;
using UnityEngine;

public class CardLibrary
{
    private readonly Dictionary<string, CardData> _cards;

    public CardLibrary(IReadOnlyDictionary<string, CardData> cards)
    {
        _cards = new Dictionary<string, CardData>(cards);
    }
}
