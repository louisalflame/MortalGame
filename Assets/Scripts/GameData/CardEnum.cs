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
    TangSect,   // 唐門
    Emei,       // 峨嵋
    Songshan,   // 嵩山
    BeggarClan, // 丐幫
    DianCang    // 點蒼
}

public enum CardTiming
{
    None = 0,
    OnPlayCard,
    TurnEnd,
}

public enum CardProperty
{
    None = 0,
    EffectTimes,
    RecycleTimes,
    PowerAdjust,
    CostAdjust,
    InitialPriority,
    Preserved,
    Sealed,
    Consumable,
    Dispose,
    AutoDispose,
}