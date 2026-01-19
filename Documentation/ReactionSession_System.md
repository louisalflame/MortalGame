# ReactionSession - 反應會話系統

## 概述
ReactionSession 是 MortalGame 專案中的動態數值管理系統，用於處理戰鬥中各種 Buff 效果自訂的臨時數值。與血量、護甲等固定屬性不同，ReactionSession 提供了靈活的、可自訂的數值容器，讓不同的 Buff 可以創建和管理專屬於自己的數值狀態。

## 文件位置
- **ReactionSessionData.cs**: `Assets/Scripts/GameData/Session/ReactionSessionData.cs`
- **ReactionSessionEntity.cs**: `Assets/Scripts/GameModel/Entity/Session/ReactionSessionEntity.cs`

---

## 系統架構

### 設計特點
- **動態數值容器**: 支援 Buff 自訂專屬數值
- **類型安全**: Boolean 和 Integer 兩種數據類型
- **生命週期管理**: 支援整局遊戲、整回合、單次打牌的生命週期
- **條件更新**: 基於 GameTiming 和條件規則的自動更新
- **Optional 模式**: 使用 Optional 庫安全處理空值

---

## 資料層設計

### IReactionSessionData - 數據介面

```csharp
public interface IReactionSessionData
{
    IReactionSessionEntity CreateEntity(TriggerContext triggerContext);
}
```

**職責**: 定義 Session 資料的統一創建介面，支援多態性。

### SessionBoolean - 布林型態 Session

```csharp
[Serializable]
public class SessionBoolean : IReactionSessionData
{
    [Serializable]
    public class TimingRule
    {
        [ValueDropdown("@DropdownHelper.UpdateTimings")]
        public GameTiming Timing;
        public ConditionBooleanUpdateRule[] Rules;
    }

    public bool InitialValue;
    public SessionLifeTime LifeTime;
    
    [ShowInInspector]
    [TableList]
    public List<TimingRule> UpdateRules = new ();
}
```

#### 關鍵特性
- **初始值設定**: `InitialValue` 定義預設布林狀態
- **生命週期**: `LifeTime` 控制 Session 的有效範圍
- **時機規則**: `TimingRule` 定義在特定 `GameTiming` 觸發的更新規則
- **編輯器友好**: 使用 `Odin Inspector` 的 `TableList` 提供視覺化編輯

#### 更新規則結構
```csharp
public class TimingRule
{
    public GameTiming Timing;                    // 觸發時機
    public ConditionBooleanUpdateRule[] Rules;   // 布林更新規則
}
```

### SessionInteger - 整數型態 Session

```csharp
[Serializable]
public class SessionInteger : IReactionSessionData
{
    [Serializable]
    public class TimingRule
    {
        [ValueDropdown("@DropdownHelper.UpdateTimings")]
        public GameTiming Timing;
        public ConditionIntegerUpdateRule[] Rules;
    }

    public int InitialValue;
    public SessionLifeTime LifeTime;
    
    [ShowInInspector]
    [TableList]
    public List<TimingRule> UpdateRules = new ();
}
```

#### 設計對稱性
- 與 `SessionBoolean` 採用相同的架構模式
- 使用 `ConditionIntegerUpdateRule` 處理數值型更新
- 支援相同的生命週期和時機管理機制

---

## 實體層設計

### IReactionSessionEntity - 實體介面

```csharp
public interface IReactionSessionEntity
{
    bool IsSessionValueUpdated { get; }
    Option<bool> BooleanValue { get; }
    Option<int> IntegerValue { get; }

    bool Update(TriggerContext triggerContext);
    IReactionSessionEntity Clone();
}
```

#### 介面特點
- **狀態查詢**: `IsSessionValueUpdated` 檢查是否有活躍的 Session 值
- **類型安全訪問**: `BooleanValue` 和 `IntegerValue` 使用 Optional 模式
- **更新機制**: `Update` 方法處理觸發上下文
- **深拷貝支援**: `Clone` 方法確保實體獨立性

### ReactionSessionEntity - 主要實體實作

```csharp
public class ReactionSessionEntity : IReactionSessionEntity
{
    private readonly SessionLifeTime _lifeTime;
    private readonly ISessionValueEntity _baseEntity;
    private Option<ISessionValueEntity> _currentValue;
}
```

#### 核心設計模式

##### 1. 基礎值與當前值分離
```csharp
private readonly ISessionValueEntity _baseEntity;      // 基礎模板
private Option<ISessionValueEntity> _currentValue;     // 當前活躍值
```

**優勢**:
- `_baseEntity` 作為不變的模板，可重複使用
- `_currentValue` 管理運行時狀態，支援重置和清除
- 實現了資料和狀態的清晰分離

##### 2. Optional 模式的值訪問
```csharp
public Option<bool> BooleanValue => _currentValue.Match(
    value =>
        value is SessionBooleanEntity booleanEntity ?
            booleanEntity.Value.Some() :
            Option.None<bool>(),
    () => Option.None<bool>());
```

**特點**:
- 使用 `Match` 方法安全處理 Optional 值
- 運行時類型檢查確保類型安全
- 避免了 null 引用異常

##### 3. 生命週期管理機制

```csharp
public bool Update(TriggerContext triggerContext)
{
    bool isUpdated = false;
    if (triggerContext.Action is UpdateTimingAction timingAction)
    {
        switch (_lifeTime)
        {
            case SessionLifeTime.WholeTurn:
                if (timingAction.Timing == GameTiming.TurnStart)
                {
                    isUpdated = true;
                    _Reset();       // 重置為初始狀態
                }
                else if (timingAction.Timing == GameTiming.TurnEnd)
                {
                    isUpdated = true;
                    _Clear();       // 清除當前值
                }
                break;
            case SessionLifeTime.PlayCard:
                // 類似的打牌生命週期管理
                break;
        }
    }
}
```

**生命週期類型**:
- **SessionLifeTime.WholeGame**: 整局遊戲有效（建構時初始化）
- **SessionLifeTime.WholeTurn**: 整回合有效（回合開始重置，回合結束清除）
- **SessionLifeTime.PlayCard**: 單次打牌有效（打牌開始重置，打牌結束清除）

##### 4. 狀態操作方法
```csharp
private void _Reset()
{
    _currentValue = _baseEntity.Clone().Some();  // 從基礎值創建新實例
}

private void _Clear()
{
    _currentValue = Option.None<ISessionValueEntity>();  // 清除當前值
}
```

---

## 使用場景與範例

### 典型使用場景

#### 1. Buff 計數器
```csharp
// 某個 Buff 需要追蹤觸發次數
var counterSession = new SessionInteger
{
    InitialValue = 0,
    LifeTime = SessionLifeTime.WholeTurn,
    UpdateRules = new List<SessionInteger.TimingRule>
    {
        new SessionInteger.TimingRule
        {
            Timing = GameTiming.PlayCardEnd,
            Rules = new[] { /* 增加計數的規則 */ }
        }
    }
};
```

#### 2. 條件標記
```csharp
// 追蹤某個條件是否已觸發
var flagSession = new SessionBoolean
{
    InitialValue = false,
    LifeTime = SessionLifeTime.PlayCard,
    UpdateRules = new List<SessionBoolean.TimingRule>
    {
        new SessionBoolean.TimingRule
        {
            Timing = GameTiming.EffectTargetResult,
            Rules = new[] { /* 設定標記為 true 的規則 */ }
        }
    }
};
```

### 當前架構設計理念
```csharp
// ReactionSession 的基本使用模式
var sessionData = new SessionBoolean {
    InitialValue = false,
    LifeTime = SessionLifeTime.WholeTurn
};

// 會話實體的創建和管理
var sessionEntity = new ReactionSessionEntity(
    Data: sessionData,
    Entity: Optional<IBooleanValueEntity>.None()
);
```

### 預期整合方式
基於現有代碼架構，ReactionSession 系統設計用於：

1. **Buff 系統數值管理**
   - 為各層級 Buff 提供動態數值存儲
   - 支援生命週期自動管理

2. **事件響應機制**
   - 透過 TriggerContext 驅動狀態更新
   - 支援條件性更新規則

3. **狀態持久化**
   - Optional 模式確保狀態安全
   - 支援 Null 狀態和有效狀態的明確區分

**注意**: 具體的整合實作方式需要等待各 Buff 系統的完整實現。

---

## 與其他系統的關係

### 系統依賴圖
```
ReactionSession_System
├── TriggerContext (觸發上下文)
├── GameTiming (遊戲時機枚舉)
├── SessionLifeTime (生命週期枚舉)
├── ConditionUpdateRule (條件更新規則)
└── ISessionValueEntity (數值實體介面)
```

### 整合系統
- **[CharacterBuff_System.md](CharacterBuff_System.md)**: Buff 系統使用 ReactionSession 管理自訂數值
- **[GameEnum_Reference.md](GameEnum_Reference.md)**: 使用 GameTiming、SessionLifeTime 枚舉
- **TriggerContext.md**: 依賴觸發上下文進行更新 ⏳ 待建立
- **ConditionUpdateRule.md**: 使用條件更新規則 ⏳ 待建立

---

## 設計模式分析

### 1. 策略模式 (Strategy Pattern)
- `SessionBoolean` 和 `SessionInteger` 實作不同的數據策略
- 統一的 `IReactionSessionData` 介面支援多態使用

### 2. 模板方法模式 (Template Method)
- `ReactionSessionEntity` 定義生命週期管理的標準流程
- 具體的更新邏輯由底層的 `ISessionValueEntity` 實作

### 3. 建造者模式變體
- 通過 `CreateEntity` 方法從資料配置創建運行時實體
- 分離配置定義和實體創建的責任

### 4. 選項模式 (Option Pattern)
- 大量使用 `Option<T>` 處理可能為空的值
- 函數式風格的安全編程

---

## 技術特點

### 1. 類型安全設計
```csharp
// 編譯時類型檢查，避免類型錯誤
public Option<bool> BooleanValue => _currentValue.Match(
    value => value is SessionBooleanEntity booleanEntity ?
        booleanEntity.Value.Some() : Option.None<bool>(),
    () => Option.None<bool>());
```

### 2. 編輯器整合
```csharp
[ShowInInspector]
[TableList]
public List<TimingRule> UpdateRules = new ();
```
- 使用 `Odin Inspector` 提供視覺化編輯體驗
- `ValueDropdown` 提供枚舉值選擇器

### 3. 記憶體效率
- 使用 `_baseEntity` 作為模板避免重複創建
- `Clone` 方法確保深拷貝的正確性
- `Option` 模式減少 null 檢查的開銷

---

## 擴展性設計

### 1. 新數據類型支援
可輕鬆添加新的 Session 類型：
```csharp
[Serializable]
public class SessionFloat : IReactionSessionData
{
    public float InitialValue;
    public SessionLifeTime LifeTime;
    // 類似的 TimingRule 結構
}
```

### 2. 自訂更新規則
```csharp
// 可以創建特殊的更新規則
public class CustomConditionRule : IConditionUpdateRule
{
    // 自訂的條件邏輯
}
```

### 3. 複雜生命週期
未來可支援更複雜的生命週期：
```csharp
public enum SessionLifeTime
{
    WholeGame,
    WholeTurn,
    PlayCard,
    UntilCondition,  // 直到特定條件滿足
    FixedDuration,   // 固定持續時間
}
```

---

## 性能考量

### 1. 更新頻率優化
- 使用 `IsSessionValueUpdated` 避免不必要的值訪問
- 只在相關的 `GameTiming` 觸發時進行更新檢查

### 2. 記憶體管理
- `Clone` 方法確保實體間的獨立性
- 及時的 `_Clear` 操作釋放不再需要的資源

### 3. 查找優化
- 使用 `Match` 方法的函數式處理避免多次類型檢查
- Optional 模式減少 null 檢查的性能開銷

---

## 已知問題與限制

### 設計限制
- [ ] 只支援 Boolean 和 Integer 兩種基本類型
- [ ] 更新規則的複雜度可能影響性能
- [ ] 缺少 Session 間的相互依賴機制

### 實作問題
- [ ] `ISessionValueEntity` 的具體實作未提供
- [ ] 條件更新規則的詳細邏輯待明確
- [ ] 錯誤處理機制需要加強

### 整合挑戰
- [ ] 與 Buff 系統的具體整合方式
- [ ] 序列化和反序列化的支援
- [ ] 除錯和診斷工具的缺乏

---

## 測試策略

### 單元測試重點
1. **生命週期測試**
   - 各種 `SessionLifeTime` 的正確行為
   - 重置和清除操作的正確性

2. **類型安全測試**
   - Boolean 和 Integer 值的正確訪問
   - 類型不匹配時的安全處理

3. **更新邏輯測試**
   - 不同 `GameTiming` 的觸發響應
   - 條件規則的正確執行

### 集成測試場景
1. **Buff 系統整合**
2. **複雜更新規則驗證**
3. **記憶體洩漏測試**

---

## 相關系統連結

- **[CharacterBuff_System.md](CharacterBuff_System.md)** - 角色Buff系統（主要使用方）
- **[GameEnum_Reference.md](GameEnum_Reference.md)** - 遊戲枚舉參考
- **TriggerContext.md** - 觸發上下文系統 ⏳ 待建立
- **ConditionUpdateRule.md** - 條件更新規則系統 ⏳ 待建立
- **ISessionValueEntity.md** - Session數值實體介面 ⏳ 待建立

---

**開發特點**: 🔧 高度靈活的自訂數值系統
**複雜度**: 📊 中高等（涉及多種設計模式和生命週期管理）
**重要性**: ⭐⭐⭐ 核心（Buff 系統的重要基礎）

---

**檔案資訊**：
- 建立日期：2024-12-25
- 對應程式碼：ReactionSessionData.cs, ReactionSessionEntity.cs
- 開發狀態：🔄 實作完整，待整合和擴展
- 下次更新：具體的 Buff 系統整合完成後