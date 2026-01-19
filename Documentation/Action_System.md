# Action_System - 遊戲動作事件系統

## 概述
Action_System 是 MortalGame 專案中的事件驅動核心系統，負責定義和管理遊戲中所有事件的來源、時機和類型。此系統透過 `IActionUnit` 介面族群提供統一的事件處理架構，支援複雜的事件追蹤和響應機制。

## ⚠️ 設計狀態
**此系統目前可能存在過度設計的問題，未來需要優化和重構**

### 設計目標
- 統一管理所有遊戲事件
- 追蹤事件的來源和觸發時機
- 支援複雜的事件鏈和響應機制
- 為事件驅動架構提供基礎

## 文件位置
- **IAction.cs**: `Assets/Scripts/GameModel/Action/IAction.cs`
- **ActionSource.cs**: `Assets/Scripts/GameModel/Action/ActionSource.cs`

---

## 核心介面設計

### IActionUnit - 基礎動作單元

```csharp
public interface IActionUnit
{
    GameTiming Timing { get; }
    IActionSource Source { get; }
}
```

**設計原則**:
- 所有遊戲事件都必須實作此介面
- `Timing` 定義事件發生的遊戲時機
- `Source` 追蹤事件的發起源頭

### IActionTargetUnit - 目標動作單元

```csharp
public interface IActionTargetUnit : IActionUnit
{
    IActionTarget Target { get; }
}
```

**擴展特性**:
- 繼承基礎動作單元
- 新增 `Target` 屬性，支援有目標的動作

---

## 動作類型分類

### 1. 時機更新動作

#### UpdateTimingAction - 時機更新事件
```csharp
public record UpdateTimingAction(GameTiming Timing, IActionSource Source) : IActionUnit;
```

**用途**:
- 表示遊戲時機的變化事件
- 觸發基於時機的響應邏輯
- 使用 Record 類型確保不可變性

### 2. 效果動作系列

#### IEffectAction - 效果動作介面
```csharp
public interface IEffectAction : IActionUnit
{
    EffectType EffectType { get; }
}
```

**特點**:
- 繼承基礎動作單元
- 新增 `EffectType` 屬性標識效果類型
- 與 [GameEnum_Reference.md](GameEnum_Reference.md) 中的 EffectType 枚舉整合

#### IEffectTargetAction - 有目標的效果動作
```csharp
public interface IEffectTargetAction : IEffectAction, IActionTargetUnit
{
}
```

#### IEffectResultAction - 效果結果動作
```csharp
public interface IEffectResultAction : IEffectAction, IActionTargetUnit
{
}
```

**設計模式**:
- 使用多重繼承組合不同特性
- `IEffectTargetAction` 表示效果意圖階段
- `IEffectResultAction` 表示效果執行結果階段

### 3. 查詢動作系列

#### 卡片查詢動作
```csharp
public record CardLookIntentAction(ICardEntity Card) : IActionUnit
{
    public GameTiming Timing => GameTiming.None;
    public IActionSource Source => SystemSource.Instance;
}
```

#### 屬性查詢動作
```csharp
public record CardBuffPropertyLookAction(ICardBuffPropertyEntity Property) : IActionUnit;
public record PlayerBuffPropertyLookAction(IPlayerBuffPropertyEntity Property) : IActionUnit;
```

**設計特點**:
- 所有查詢動作都使用 `GameTiming.None`
- 來源都設定為 `SystemSource.Instance`
- 使用 Record 類型的簡潔語法

### 4. 系統動作系列

#### 卡片系統動作
```csharp
public record CardCreateSystemAction(
    IPlayerEntity Player,
    CardCollectionType Destination) : IActionUnit
{
    public GameTiming Timing => GameTiming.GameStart;
    public IActionSource Source => SystemSource.Instance;
}
```

#### 卡片打出動作鏈
```csharp
public record CardPlayIntentAction(CardPlaySource CardPlaySource) : IActionUnit
{
    public GameTiming Timing => GameTiming.PlayCardStart;
    public IActionSource Source => CardPlaySource;
}

public record CardPlayResultAction(CardPlayResultSource CardPlayResultSource) : IActionUnit
{
    public GameTiming Timing => GameTiming.PlayCardEnd;
    public IActionSource Source => CardPlayResultSource;
}
```

**動作鏈設計**:
- Intent → Result 的兩階段處理
- 不同階段使用不同的來源類型
- 時機從 `PlayCardStart` 到 `PlayCardEnd`

---

## 動作來源系統

### IActionSource - 來源介面

```csharp
public interface IActionSource
{
}
```

**設計哲學**:
- 標記介面（Marker Interface）
- 提供類型安全的來源分類
- 支援多種來源類型的統一處理

### 系統來源類型

#### SystemSource - 系統來源
```csharp
public class SystemSource : IActionSource
{
    public static readonly SystemSource Instance = new();
}
```

**單例模式**:
- 表示由遊戲系統本身發起的動作
- 使用靜態實例避免重複創建

#### 系統執行來源
```csharp
public record SystemExectueStartSource(IPlayerEntity Player) : IActionSource;
public record SystemExectueEndSource(IPlayerEntity Player) : IActionSource;
```

**特點**:
- 記錄系統執行階段的起始和結束
- 關聯到特定玩家實體

### 遊戲實體來源

#### CardPlaySource - 卡片打出來源
```csharp
public record CardPlaySource(
    ICardEntity Card,
    int HandCardIndex,
    int HandCardsCount,
    LoseEnergyResult CostEnergy,
    IEffectAttribute Attribute) : IActionSource
{
    public CardPlayResultSource CreateResultSource(IReadOnlyList<IEffectResultAction> effectResults)
    {
        return new CardPlayResultSource(this, effectResults);
    }
}
```

**豐富的上下文資訊**:
- `Card` - 被打出的卡片實體
- `HandCardIndex` - 手牌中的位置
- `HandCardsCount` - 手牌總數
- `CostEnergy` - 能量消耗結果
- `Attribute` - 效果屬性

**結果創建方法**:
- `CreateResultSource` 建立對應的結果來源
- 連接 Intent 和 Result 兩個階段

#### CardPlayResultSource - 卡片結果來源
```csharp
public record CardPlayResultSource(
    CardPlaySource CardPlaySource,
    IReadOnlyList<IEffectResultAction> EffectResults) : IActionSource;
```

**結果追蹤**:
- 保持對原始 `CardPlaySource` 的引用
- 記錄所有效果執行的結果

#### Buff 來源系列
```csharp
public record PlayerBuffSource(IPlayerBuffEntity Buff) : IActionSource;
public record CardBuffSource(ICardBuffEntity Buff) : IActionSource;
```

**Buff 事件追蹤**:
- 記錄由特定 Buff 實體觸發的動作
- 支援 Buff 系統的事件響應機制

---

## 系統整合關係

### 與其他系統的依賴
```
Action_System
├── GameTiming (遊戲時機枚舉)
├── EffectType (效果類型枚舉)
├── CardCollectionType (卡片區域枚舉)
├── ICardEntity (卡片實體)
├── IPlayerEntity (玩家實體)
├── IPlayerBuffEntity (玩家Buff實體)
├── ICardBuffEntity (卡片Buff實體)
├── IEffectAttribute (效果屬性)
└── IActionTarget (動作目標)
```

### 整合系統
- **[GameEnum_Reference.md](GameEnum_Reference.md)**: 使用 GameTiming、EffectType 枚舉
- **[Card_System.md](Card_System.md)**: 卡片動作的來源和目標
- **[Player_System.md](Player_System.md)**: 玩家相關動作
- **[CardBuff_System.md](CardBuff_System.md)**: 卡片 Buff 動作來源
- **[PlayerBuff_System.md](PlayerBuff_System.md)**: 玩家 Buff 動作來源
- **TriggerContext.md**: 可能使用 Action 作為觸發上下文 ⏳ 待建立

---

## 使用場景與範例

### 基本事件創建
```csharp
// 創建時機更新事件
var timingAction = new UpdateTimingAction(
    Timing: GameTiming.TurnStart,
    Source: SystemSource.Instance
);

// 創建卡片查詢事件
var cardLookAction = new CardLookIntentAction(someCard);
```

### 卡片打出事件鏈
```csharp
// 1. 創建打牌來源
var playSource = new CardPlaySource(
    Card: cardEntity,
    HandCardIndex: 2,
    HandCardsCount: 5,
    CostEnergy: energyCost,
    Attribute: effectAttribute
);

// 2. 創建打牌意圖動作
var intentAction = new CardPlayIntentAction(playSource);

// 3. 執行效果並獲得結果
var effectResults = ExecuteCardEffects(cardEntity);

// 4. 創建結果來源
var resultSource = playSource.CreateResultSource(effectResults);

// 5. 創建結果動作
var resultAction = new CardPlayResultAction(resultSource);
```

### Buff 觸發事件
```csharp
// 玩家 Buff 觸發的動作
var buffSource = new PlayerBuffSource(playerBuff);
var buffAction = new SomeBuffTriggeredAction(buffSource);

// 卡片 Buff 觸發的動作
var cardBuffSource = new CardBuffSource(cardBuff);
var cardBuffAction = new SomeCardBuffAction(cardBuffSource);
```

---

## 設計模式分析

### 1. 策略模式 (Strategy Pattern)
- 不同的 `IActionSource` 實作代表不同的事件來源策略
- 統一的 `IActionUnit` 介面支援多態處理

### 2. 組合模式 (Composite Pattern)
- 透過介面繼承組合不同的行為特性
- `IEffectTargetAction` 結合效果和目標特性

### 3. 建造者模式變體
- `CardPlaySource.CreateResultSource` 方法建立相關聯的結果來源
- 確保 Intent 和 Result 階段的連貫性

### 4. 標記介面模式 (Marker Interface)
- `IActionSource` 作為標記介面提供類型安全
- 支援來源類型的分類和識別

---

## 設計評估

### 優點

#### 1. 統一事件模型
- 所有遊戲事件都遵循相同的介面契約
- 便於事件的統一處理和響應

#### 2. 豐富的上下文資訊
- 詳細記錄事件的來源、時機、目標
- 支援複雜的事件分析和調試

#### 3. 類型安全
- 強型別的來源和動作分類
- 編譯時檢查避免類型錯誤

#### 4. 擴展性
- 易於添加新的動作類型和來源
- 介面設計支援未來功能擴展

### 缺點與風險

#### 1. 過度設計風險
- 介面層次複雜，可能存在不必要的抽象
- 小型事件也需要完整的介面實作

#### 2. 性能考量
- 大量的介面和 Record 物件創建
- 可能影響高頻事件的處理效率

#### 3. 維護複雜度
- 多層介面繼承增加理解成本
- 新增功能可能需要修改多個介面

#### 4. 使用一致性
- 開發者需要正確選擇合適的動作類型
- 缺少使用指導可能導致誤用

---

## 架構設計思考

### 當前設計的複雜度觀察
Action 系統展現了高度的介面分離設計，包含：
- 12+ 個不同的 IActionUnit 變體
- 複雜的多重繼承關係
- 精細的功能區分

### 設計權衡分析

#### 優點
- **高度類型安全**: 編譯時期可以區分不同動作類型
- **介面職責清晰**: 每個介面有明確的責任範圍
- **擴展性強**: 支援細粒度的功能擴展

#### 潛在考量
- **認知負擔**: 開發者需要理解大量介面關係
- **選擇困難**: 多個相似介面可能造成選擇困難
- **過度設計風險**: 部分介面可能實際使用頻率較低

### 架構進化方向
基於實際使用情況，未來可能的發展方向：

1. **實用性驗證**: 透過實際開發驗證各介面的必要性
2. **使用模式統計**: 分析最常使用的介面組合
3. **簡化可能性**: 考慮合併低使用率的相似介面
4. **開發者友善性**: 提供使用指南和最佳實踐

**重要**: 當前設計體現了對類型安全和功能完整性的重視，任何簡化都應該在保持核心優勢的前提下進行。

---

## 測試策略

### 單元測試重點
1. **介面實作測試**
   - 各種動作類型的正確實作
   - 屬性值的正確設定

2. **來源追蹤測試**
   - 不同來源類型的正確識別
   - 來源上下文資訊的完整性

3. **事件鏈測試**
   - Intent → Result 階段的正確連接
   - 複雜事件流的處理

### 集成測試場景
1. **完整遊戲流程事件測試**
2. **高頻事件處理性能測試**
3. **複雜事件響應鏈測試**

---

## 相關系統連結

- **[GameEnum_Reference.md](GameEnum_Reference.md)** - 遊戲枚舉參考（GameTiming, EffectType）
- **[Card_System.md](Card_System.md)** - 卡片系統（動作來源和目標）
- **[Player_System.md](Player_System.md)** - 玩家系統（玩家相關動作）
- **[CardBuff_System.md](CardBuff_System.md)** - 卡片Buff系統（Buff動作來源）
- **[PlayerBuff_System.md](PlayerBuff_System.md)** - 玩家Buff系統（Buff動作來源）
- **TriggerContext.md** - 觸發上下文系統 ⏳ 待建立
- **IActionTarget.md** - 動作目標系統 ⏳ 待建立

---

**設計狀態**: 🔄 實作完整但可能過度設計，需要重構優化
**複雜度**: 📊 高（多層介面繼承和複雜的類型系統）
**重要性**: ⭐⭐⭐ 核心（事件驅動架構的基礎）

---

**檔案資訊**：
- 建立日期：2024-12-25
- 對應程式碼：IAction.cs, ActionSource.cs
- 開發狀態：🔄 功能完整，設計需要優化
- 下次更新：重構簡化介面層次後