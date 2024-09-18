using System;
using UnityEngine;


public enum CardType
{
    None = 0,
    Attack,
    Defense,
    Speech,
    Sneak,
    Special,
    Item
}

public enum CardRarity
{
    None = 0,
    Common,
    Rare,
    Epic,
    Legendary
}

public enum CardTheme
{
    None = 0,
    TangSect,
    Emei,
    Songshan,
    BeggarClan,
    DianCang
}

public enum CardTiming
{
    None = 0,
    OnPlayCard,
}

public enum CardProperty
{
    None = 0,
    OnPlayExtraTimes,
    InitialPriority,
    Preserved,
    Sealed,
}