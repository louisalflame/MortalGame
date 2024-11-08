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

public enum CardProperty : int
{
    None                = 0,
    EffectTimes         = 1,
    RecycleTimes        = 1 << 1,
    PowerAdjust         = 1 << 2, 
    CostAdjust          = 1 << 3,
    InitialPriority     = 1 << 4,
    Preserved           = 1 << 5,
    Sealed              = 1 << 6,
    Consumable          = 1 << 7,
    Dispose             = 1 << 8,
    AutoDispose         = 1 << 9,
    AppendEffect        = 1 << 10,
}