using System;
using UnityEngine;


public enum CardType
{
    None = 0,
    Attack,     // 捅人
    Defense,    // 備揍     
    Speech,     // 嘴攻
    Sneak,      // 暗器
    Special,    // 絕招
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
    BattleBegin,
    TurnStart,
    OnDrawCard,
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
    // Consumable means the card will be removed from the battle but added again next battle.
    Consumable          = 1 << 7,
    // Dispose means the card will be removed from the game forever.
    Dispose             = 1 << 8,
    AutoDispose         = 1 << 9,
    AppendEffect        = 1 << 10,
}

public enum CardCollectionType
{
    None = 0,
    Deck,
    HandCard,
    Graveyard,
    ExclusionZone,
    DisposeZone,
}

public enum GameKeyWord
{
    None = 0,
    Attack,
    Defense,
    Speech,
    Sneak,
    Special,
    Item,
    TangSect,
    Emei,
    Songshan,
    BeggarClan,
    DianCang,
    BattleBegin,
    TurnStart,
    OnDrawCard,
    OnPlayCard,
    TurnEnd,
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
    AppendEffect,
}