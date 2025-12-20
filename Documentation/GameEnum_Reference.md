# 遊戲枚舉參考 - MortalGame 卡牌遊戲

## 概述
本文件記錄了 MortalGame 專案中所有核心枚舉類型的定義與用途。這些枚舉分布在 `GameEnum.cs` 和 `CardEnum.cs` 兩個文件中，涵蓋了遊戲的各個核心系統。

## 文件結構
- **GameEnum.cs**: 通用遊戲機制枚舉（戰鬥、效果、目標、時機等）
- **CardEnum.cs**: 卡片專用枚舉（類型、屬性、觸發、收集等）

---

## 1. 卡片系統枚舉 (CardEnum.cs)

### 1.1 卡片基礎分類

#### CardType - 卡片類型
```csharp
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
```
**武俠主題設計**：
- `Attack` - 正面攻擊招式
- `Defense` - 防禦招式
- `Speech` - 言語攻擊（心理戰）
- `Sneak` - 暗器攻擊
- `Special` - 獨門絕招
- `Item` - 道具物品

#### CardRarity - 卡片稀有度
```csharp
public enum CardRarity
{
    None = 0,
    Common,      // 普通
    Rare,        // 稀有
    Epic,        // 史詩
    Legendary    // 傳奇
}
```
**遊戲價值層級**：標準四層稀有度系統，影響卡片獲取難度和強度。

#### CardTheme - 卡片門派主題
```csharp
public enum CardTheme
{
    None = 0,
    TangSect,    // 唐門
    Emei,        // 峨嵋
    Songshan,    // 嵩山
    BeggarClan,  // 丐幫
    DianCang     // 點蒼
}
```
**武俠門派設定**：
- `TangSect` - 唐門（暗器機關）
- `Emei` - 峨嵋派（正統武學）
- `Songshan` - 嵩山派（剛猛武功）
- `BeggarClan` - 丐幫（實用招式）
- `DianCang` - 點蒼派（劍法精妙）

### 1.2 卡片屬性系統

#### CardProperty - 卡片屬性（位運算標記）
```csharp
public enum CardProperty : int
{
    None                = 0,
    EffectRepeat         = 1,           // 效果重複
    Recycle             = 1 << 1,       // 回收
    PowerAddition       = 1 << 2,       // 威力加成
    CostAddition        = 1 << 3,       // 費用加成
    Initialize          = 1 << 4,       // 初始化
    Preserved           = 1 << 5,       // 保留
    Sealed              = 1 << 6,       // 封印
    Consumable          = 1 << 7,       // 消耗（移出戰鬥，下場回歸）
    Dispose             = 1 << 8,       // 廢棄（永久移除）
    AutoDispose         = 1 << 9,       // 自動廢棄
}
```
**設計特點**：
- 使用位運算允許多重屬性組合
- `Consumable` vs `Dispose` 區分臨時與永久移除
- 支援複雜的卡片行為模式

### 1.3 卡片觸發與收集

#### CardTriggeredTiming - 卡片觸發時機
```csharp
public enum CardTriggeredTiming
{
    None = 0,
    Drawed,          // 抽到時
    EffectDrawed,    // 效果抽到時
    Played,          // 打出時
    EffectPlayed,    // 效果打出時
    Preserved,       // 保留時
    Discarded,       // 棄掉時
    EffectDiscarded, // 效果棄掉時
    Initialize,      // 初始化時
}
```
**時機分類**：區分主動觸發和效果觸發，支援複雜的卡片關鍵字效果。

#### CardCollectionType - 卡片區域類型
```csharp
public enum CardCollectionType
{
    None = 0,
    Deck,           // 牌庫
    HandCard,       // 手牌
    Graveyard,      // 墓地
    ExclusionZone,  // 放逐區
    DisposeZone,    // 廢棄區
}
```
**區域管理**：標準卡牌遊戲區域劃分，支援多層次的卡片狀態管理。

#### TargetLogicTag - 目標邏輯標籤
```csharp
public enum TargetLogicTag
{
    None = 0,
    ToEnemy,   // 敵方
    ToAlly,    // 友方
    ToRandom,  // 隨機
}
```
**目標選擇**：簡化的目標邏輯分類。

---

## 2. 通用遊戲機制枚舉 (GameEnum.cs)

### 2.1 數值運算系統

#### ArithmeticType - 運算類型
```csharp
public enum ArithmeticType
{
    None = 0,
    Add,       // 加法
    Multiply,  // 乘法
    Overwrite  // 覆寫
}
```
**用途**：屬性修改、傷害計算、數值變更的基礎運算類型。

#### ArithmeticConditionType - 運算條件
```csharp
public enum ArithmeticConditionType
{
    None = 0,
    Equal,              // 等於
    NotEqual,           // 不等於
    GreaterThan,        // 大於
    LessThan,           // 小於
    GreaterThanOrEqual, // 大於等於
    LessThanOrEqual,    // 小於等於
}
```
**用途**：條件判斷、觸發條件、AI邏輯判定。

#### SetConditionType - 集合條件
```csharp
public enum SetConditionType
{
    None = 0,
    AnyInside,   // 任意在內
    AllInside,   // 全部在內
    AnyOutside,  // 任意在外
    AllOutside,  // 全部在外
}
```
**用途**：集合運算、群體判定、範圍檢查。

#### OrderType - 排序類型
```csharp
public enum OrderType
{
    None = 0,
    Ascending,  // 升序
    Descending, // 降序
    Random      // 隨機
}
```
**用途**：列表排序、優先順序、隨機化處理。

### 2.2 戰鬥系統

#### Faction - 陣營
```csharp
public enum Faction
{
    None = 0,
    Ally,   // 友軍
    Enemy   // 敵軍
}
```
**用途**：區分友敵關係、技能目標判定、AI行為邏輯。

#### DamageType - 傷害類型
```csharp
public enum DamageType
{
    Normal,      // 普通傷害
    Penetrate,   // 穿透傷害
    Additional,  // 額外傷害
    Effective    // 有效傷害
}
```
**傷害機制**：
- `Normal` - 標準傷害，受護甲減免
- `Penetrate` - 穿透傷害，無視護甲
- `Additional` - 額外傷害，疊加計算
- `Effective` - 有效傷害，最終結算值

#### DamageStyle - 傷害風格
```csharp
public enum DamageStyle
{
    None = 1 >> 0,
    FullAttack = 1 >> 1,    // 全力攻擊
    QuickAttack = 1 >> 2,   // 快速攻擊
    ComboAttack = 1 >> 3,   // 連擊
    FollowAttack = 1 >> 4,  // 追擊
    CounterAttack = 1 >> 5, // 反擊
}
```
**武俠戰鬥風格**：位運算設計，支援組合攻擊模式。

### 2.3 能量系統

#### EnergyGainType - 能量獲得類型
```csharp
public enum EnergyGainType
{
    None = 0,
    RoundStartRecover, // 回合開始恢復
    GainEffect,        // 獲得效果
}
```

#### EnergyLoseType - 能量消耗類型
```csharp
public enum EnergyLoseType
{
    None = 0,
    PlayCardConsume, // 打牌消耗
    LoseEffect,      // 失去效果
}
```
**能量管理**：區分自然恢復與效果影響，支援複雜的能量經濟系統。

### 2.4 Buff屬性系統

#### PlayerBuffPropertyDuration - 玩家Buff持續時間
```csharp
public enum PlayerBuffPropertyDuration
{
    None = 0,
    ThisTurn,   // 本回合
    ThisBattle, // 本戰鬥
    ThisGame    // 本局遊戲
}
```

#### PlayerBuffProperty - 玩家Buff屬性
```csharp
public enum PlayerBuffProperty
{
    None = 0,
    AllCardPower,              // 全卡威力
    AllCardCost,               // 全卡費用
    NormalDamageAddition,      // 普通傷害加成
    PenetrateDamageAddition,   // 穿透傷害加成
    EffectiveDamageAddition,   // 有效傷害加成
    AdditionalDamageAddition,  // 額外傷害加成
    HealAddition,              // 治療加成
    ShieldAddition,            // 護盾加成
    NormalDamageRatio,         // 普通傷害比例
    PenetrateDamageRatio,      // 穿透傷害比例
    EffectiveDamageRatio,      // 有效傷害比例
    AdditionalDamageRatio,     // 額外傷害比例
    HealRatio,                 // 治療比例
    ShieldRatio,               // 護盾比例
    MaxHealth,                 // 最大血量
    MaxEnergy,                 // 最大能量
}
```

#### CharacterBuffProperty - 角色Buff屬性
```csharp
public enum CharacterBuffProperty
{
    None = 0,
    EffectAttribute, // 效果屬性
    MaxHealth,       // 最大血量
    MaxEnergy,       // 最大能量
}
```
**層級設計**：玩家級Buff影響範圍更廣，角色級Buff更專精。

### 2.5 目標選擇系統

#### SelectType - 選擇類型
```csharp
public enum SelectType
{
    None = 0,
    Character,      // 角色
    AllyCharacter,  // 友方角色
    EnemyCharacter, // 敵方角色
    Card,           // 卡片
    AllyCard,       // 友方卡片
    EnemyCard,      // 敵方卡片
}
```

#### TargetType - 目標類型
```csharp
public enum TargetType
{
    None = 0,
    AllyCard,       // 友方卡片
    EnemyCard,      // 敵方卡片
    AllyCharacter,  // 友方角色
    EnemyCharacter, // 敵方角色
}
```
**目標系統**：`SelectType` 用於選擇階段，`TargetType` 用於效果執行階段。

### 2.6 效果系統

#### EffectType - 效果類型
```csharp
public enum EffectType
{
    None = 0,
    Damage,                  // 傷害
    Heal,                    // 治療
    Shield,                  // 護盾
    GainEnergy,              // 獲得能量
    LoseEnergy,              // 失去能量
    AdjustDisposition,       // 調整位置
    RecycleDeck,             // 回收牌庫
    DrawCard,                // 抽牌
    DiscardCard,             // 棄牌
    ConsumeCard,             // 消耗卡片
    DisposeCard,             // 廢棄卡片
    CreateCard,              // 創造卡片
    CloneCard,               // 複製卡片
    AddPlayerBuff,           // 添加玩家Buff
    RemovePlayerBuff,        // 移除玩家Buff
    AddCardBuff,             // 添加卡片Buff
    RemoveCardBuff,          // 移除卡片Buff
    AddCharacterBuff,        // 添加角色Buff
    RemoveCharacterBuff,     // 移除角色Buff
    CardPlayEffectAttribute, // 卡片打出效果屬性
}
```
**效果分類**：
- **基礎效果**：Damage, Heal, Shield, Energy
- **卡片操作**：Draw, Discard, Create, Clone
- **Buff管理**：Player/Card/Character Buff 操作
- **特殊效果**：Position, Recycle, EffectAttribute

### 2.7 遊戲時機系統

#### GameTiming - 遊戲時機
```csharp
public enum GameTiming
{
    None = 0,
    GameStart,         // 遊戲開始
    TurnStart,         // 回合開始
    TurnEnd,           // 回合結束
    ExecuteStart,      // 執行開始
    ExecuteEnd,        // 執行結束
    CharacterSummon,   // 角色召喚
    CharacterDeath,    // 角色死亡
    DrawCard,          // 抽牌
    PlayCardStart,     // 打牌開始
    PlayCardEnd,       // 打牌結束
    EffectIntent,      // 效果意圖
    EffectTargetIntent,// 效果目標意圖
    EffectTargetResult,// 效果目標結果
    TriggerBuffStart,  // 觸發Buff開始
    TriggerBuffEnd,    // 觸發Buff結束
}
```
**時機系統**：支援精細的事件觸發控制，是反應式系統的基礎。

#### SessionLifeTime - 會話生命週期
```csharp
public enum SessionLifeTime
{
    WholeGame,  // 整場遊戲
    WholeTurn,  // 整個回合
    PlayCard    // 打牌階段
}
```
**生命週期管理**：定義不同層級的持續時間範圍。

### 2.8 效果屬性系統

#### EffectAttributeAdditionType - 效果屬性加成類型
```csharp
public enum EffectAttributeAdditionType
{
    None = 0,
    CostAddition,               // 費用加成
    PowerAddition,              // 威力加成
    NormalDamageAddition,       // 普通傷害加成
    PenetrateDamageAddition,    // 穿透傷害加成
    EffectiveDamageAddition,    // 有效傷害加成
    AdditionalDamageAddition,   // 額外傷害加成
    HealAddition,               // 治療加成
    ShieldAddition,             // 護盾加成
}
```

#### EffectAttributeRatioType - 效果屬性比例類型
```csharp
public enum EffectAttributeRatioType
{
    None = 0,
    NormalDamageRatio,    // 普通傷害比例
    PenetrateDamageRatio, // 穿透傷害比例
    EffectiveDamageRatio, // 有效傷害比例
    AdditionalDamageRatio,// 額外傷害比例
    HealRatio,            // 治療比例
    ShieldRatio,          // 護盾比例
}
```
**屬性修改**：區分加成（Addition）和比例（Ratio）兩種修改方式。

---

## 3. 枚舉設計模式分析

### 3.1 位運算標記模式
**應用場景**：`CardProperty`, `DamageStyle`
```csharp
// 支援多重屬性組合
CardProperty combined = CardProperty.Preserved | CardProperty.Recycle;
```

### 3.2 分層分類模式
**應用場景**：Buff系統（Player vs Character）、目標系統（Select vs Target）
- 同一概念在不同層級有不同的詳細程度
- 支援系統間的清晰職責劃分

### 3.3 武俠主題化設計
**應用場景**：`CardType`, `CardTheme`, `DamageStyle`
- 遊戲機制與主題世界觀緊密結合
- 提升代碼可讀性和設計一致性

### 3.4 對稱性設計
**應用場景**：Energy（Gain vs Lose）、Damage（Addition vs Ratio）
- 成對的概念使用對稱的枚舉結構
- 簡化邏輯處理和理解成本

---

## 4. 系統整合關係

### 4.1 卡片系統整合
```
CardType + CardTheme → 卡片身份定義
CardProperty + CardTriggeredTiming → 卡片行為模式
CardCollectionType + TargetLogicTag → 卡片狀態管理
```

### 4.2 戰鬥系統整合
```
Faction + TargetType → 目標選擇邏輯
DamageType + DamageStyle → 傷害計算系統
EffectType + GameTiming → 效果觸發機制
```

### 4.3 屬性系統整合
```
PlayerBuffProperty + PlayerBuffPropertyDuration → 玩家狀態管理
CharacterBuffProperty + SessionLifeTime → 角色狀態管理
EffectAttributeAdditionType + EffectAttributeRatioType → 數值修改系統
```

---

## 5. 使用建議

### 5.1 命名約定
- 所有枚舉值使用 PascalCase
- 避免使用縮寫，保持語義清晰
- 武俠術語使用拼音，保持主題一致性

### 5.2 擴展指導
- 新增枚舉值時保持向後相容性
- 位運算枚舉注意值的唯一性
- 考慮與現有系統的整合影響

### 5.3 性能考量
- 枚舉比較操作高效
- 位運算枚舉支援快速組合檢查
- 避免頻繁的字符串轉換

---

## 6. 相關系統連結

- **[Card_System.md](Card_System.md)** - 卡片系統使用的枚舉詳解
- **[Player_System.md](Player_System.md)** - 玩家系統相關枚舉
- **[Character_System.md](Character_System.md)** - 角色系統相關枚舉
- **[CardBuff_System.md](CardBuff_System.md)** - 卡片Buff系統枚舉應用
- **[PlayerBuff_System.md](PlayerBuff_System.md)** - 玩家Buff系統枚舉應用
- **[CharacterBuff_System.md](CharacterBuff_System.md)** - 角色Buff系統枚舉應用

---

**檔案資訊**：
- 建立日期：2024-12-20
- 涵蓋檔案：GameEnum.cs, CardEnum.cs
- 枚舉總數：23個主要枚舉類型
- 特色：武俠主題、位運算、分層設計