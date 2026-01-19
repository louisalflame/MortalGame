# TriggerContext - 觸發上下文系統

## 概述
TriggerContext 是 MortalGame 專案中的事件響應機制核心，負責連接事件發生 (`IActionUnit`) 和事件監聽者 (`ITriggeredSource`)。此系統提供了統一的觸發上下文，讓各種遊戲元件（特別是 Buff 系統）可以監聽特定事件並作出相應的反應。

## 文件位置
**TriggerSource.cs**: `Assets/Scripts/GameModel/Action/TriggerSource.cs`

---

## 核心設計架構

### TriggerContext - 觸發上下文記錄

```csharp
public record TriggerContext(
    IGameplayModel Model,
    ITriggeredSource Triggered,
    IActionUnit Action);
```

#### 設計特點
- **不可變性**: 使用 Record 類型確保上下文資料不可變
- **完整上下文**: 包含遊戲模型、觸發源、觸發動作三個核心要素
- **事件連接**: 連接事件發生者和事件響應者

#### 屬性說明
- **Model**: `IGameplayModel` - 當前遊戲模型狀態，提供完整的遊戲上下文
- **Triggered**: `ITriggeredSource` - 被觸發的來源，標識哪個組件在監聽此事件
- **Action**: `IActionUnit` - 觸發的動作事件，來自 [Action_System.md](Action_System.md)

---

## 觸發源系統

### ITriggeredSource - 觸發源介面

```csharp
public interface ITriggeredSource
{
}
```

**設計原理**:
- 標記介面（Marker Interface）設計
- 提供類型安全的觸發源分類
- 支援統一的觸發源處理機制

### 具體觸發源實作

#### 1. 卡片相關觸發源

##### CardPlayTrigger - 卡片打出觸發
```csharp
public class CardPlayTrigger : ITriggeredSource
{
    public CardPlaySource CardPlay { get; private set; }

    public CardPlayTrigger(CardPlaySource cardPlay)
    {
        CardPlay = cardPlay;
    }
}
```

**特點**:
- 監聽卡片打出事件
- 保存完整的 `CardPlaySource` 上下文
- 適用於需要回應卡片打出的 Buff 或效果

##### CardTrigger - 通用卡片觸發
```csharp
public class CardTrigger : ITriggeredSource
{
    public ICardEntity Card { get; private set; }

    public CardTrigger(ICardEntity card)
    {
        Card = card;
    }
}
```

**用途**:
- 監聽特定卡片相關的各種事件
- 比 `CardPlayTrigger` 更通用的卡片事件監聽
- 適用於卡片被抽到、被棄置等場景

#### 2. Buff 相關觸發源

##### CardBuffTrigger - 卡片 Buff 觸發
```csharp
public class CardBuffTrigger : ITriggeredSource
{
    public ICardBuffEntity Buff { get; private set; }

    public CardBuffTrigger(ICardBuffEntity buff)
    {
        Buff = buff;
    }
}
```

##### PlayerBuffTrigger - 玩家 Buff 觸發
```csharp
public class PlayerBuffTrigger : ITriggeredSource
{
    public IPlayerBuffEntity Buff { get; private set; }

    public PlayerBuffTrigger(IPlayerBuffEntity buff)
    {
        Buff = buff;
    }
}
```

##### CharacterBuffTrigger - 角色 Buff 觸發
```csharp
public class CharacterBuffTrigger : ITriggeredSource
{
    public ICharacterBuffEntity Buff { get; private set; }

    public CharacterBuffTrigger(ICharacterBuffEntity buff)
    {
        Buff = buff;
    }
}
```

**Buff 觸發源設計模式**:
- 統一的屬性命名 (`Buff`)
- 一致的建構子簽名
- 對應三個不同層級的 Buff 系統

#### 3. 玩家觸發源

##### PlayerTrigger - 玩家觸發
```csharp
public class PlayerTrigger : ITriggeredSource
{
    public IPlayerEntity Player { get; private set; }

    public PlayerTrigger(IPlayerEntity player)
    {
        Player = player;
    }
}
```

**應用場景**:
- 監聽玩家相關的系統事件
- 回合開始/結束的玩家狀態變化
- 玩家能量變化等事件

---

## 系統運作機制

### 當前架構分析
基於現有代碼，TriggerContext 系統提供了事件響應的基礎框架：

### 核心組件
1. **TriggerContext Record**: 封裝觸發上下文的不可變數據結構
2. **ITriggeredSource Interface**: 觸發源的標記介面
3. **具體觸發源類別**: 六種預定義的觸發源類型

### 預期流程
```
1. 遊戲事件發生（來自其他系統）
   ↓
2. 創建對應的 IActionUnit（來自 Action_System）
   ↓
3. 確定監聽此事件的組件
   ↓
4. 創建對應的 ITriggeredSource
   ↓
5. 組合成 TriggerContext
   ↓
6. 傳遞給具體的響應邏輯（待實作）
```

**注意**: 具體的事件監聽和響應邏輯需要在各個使用此系統的模組中實作。

---

## 與其他系統的整合

### 系統依賴關係

```
TriggerContext_System
├── IGameplayModel (遊戲模型)
├── Action_System (事件動作)
│   └── IActionUnit
├── Card_System (卡片相關觸發)
│   ├── ICardEntity
│   └── CardPlaySource
├── Buff Systems (Buff 觸發)
│   ├── ICardBuffEntity
│   ├── IPlayerBuffEntity
│   └── ICharacterBuffEntity
└── Player_System (玩家觸發)
    └── IPlayerEntity
```

### 核心整合概念
TriggerContext 系統的設計目標是連接事件發生和事件響應，預期的整合方式包括：

1. **與 Action_System 的關係**: Action_System 提供 IActionUnit 事件，TriggerContext 提供響應機制框架
2. **與 Buff 系統的整合**: 各種 Buff 系統預期使用對應的 Trigger 類別來監聽事件
3. **與 ReactionSession 系統的整合**: ReactionSession 可能使用 TriggerContext 來處理更新邏輯

**注意**: 具體的整合實作方式尚未在當前代碼中體現，需要後續開發確認。

---

## 使用場景分析

### 設計意圖分析
根據現有代碼結構，TriggerContext 系統的預期使用場景包括：

### 1. Buff 效果觸發機制
不同層級的 Buff 觸發器對應不同的 Buff 系統：
- `CardBuffTrigger`: 對應卡片 Buff 系統
- `PlayerBuffTrigger`: 對應玩家 Buff 系統  
- `CharacterBuffTrigger`: 對應角色 Buff 系統

### 2. 卡片事件監聽
- `CardTrigger`: 通用卡片事件監聽
- `CardPlayTrigger`: 專門監聽卡片打出事件，包含完整的 CardPlaySource 上下文

### 3. 玩家事件響應
- `PlayerTrigger`: 監聽玩家相關的系統事件

### 基本使用模式
```csharp
// 基本的觸發上下文創建
var triggerContext = new TriggerContext(
    Model: gameplayModel,
    Triggered: new PlayerBuffTrigger(somePlayerBuff),
    Action: someActionUnit
);
```

**注意**: 具體的觸發邏輯和效果執行方式需要在各個 Buff 系統中實作。

---

## 設計模式分析

### 1. 標記介面模式 (Marker Interface)
- `ITriggeredSource` 使用空介面作為類型標記
- 提供類型安全的觸發源分類

### 2. 記錄模式 (Record Pattern)
- `TriggerContext` 使用 Record 類型確保不可變性
- 提供結構化的數據封裝

### 3. 組合模式概念
- 不同的觸發源類別可以與任意的 `IActionUnit` 組合
- 支援靈活的事件響應配置

**注意**: 其他設計模式的應用需要在具體的整合實作中體現。

---

## 擴展性設計

### 當前架構支援的擴展方向

### 1. 新觸發源類型
基於現有的設計模式，可以輕鬆添加新的觸發源類型：
```csharp
// 範例：環境觸發源
public class EnvironmentTrigger : ITriggeredSource
{
    public IEnvironmentEntity Environment { get; private set; }
    
    public EnvironmentTrigger(IEnvironmentEntity environment)
    {
        Environment = environment;
    }
}
```

### 2. 複合觸發源
```csharp
// 範例：複合觸發條件
public class CompositeTrigger : ITriggeredSource
{
    public IReadOnlyList<ITriggeredSource> Sources { get; private set; }
    
    public CompositeTrigger(params ITriggeredSource[] sources)
    {
        Sources = sources.ToList().AsReadOnly();
    }
}
```

### 3. 條件觸發源
```csharp
// 範例：條件性觸發
public class ConditionalTrigger : ITriggeredSource
{
    public ITriggeredSource BaseTrigger { get; private set; }
    public Func<TriggerContext, bool> Condition { get; private set; }
    
    public ConditionalTrigger(ITriggeredSource baseTrigger, Func<TriggerContext, bool> condition)
    {
        BaseTrigger = baseTrigger;
        Condition = condition;
    }
}
```

**注意**: 這些是基於當前架構的擴展可能性，實際實作時需要考慮具體需求。

---

## 性能考量

### 當前架構的性能特點
基於 Record 類型和簡單類別的設計，當前架構具有以下特點：

### 優勢
- **輕量級結構**: Record 類型和簡單類別的內存佔用較小
- **不可變性**: TriggerContext 的不可變特性避免了意外修改
- **類型安全**: 強類型設計減少運行時錯誤

### 潛在考量
- **對象創建**: 頻繁的 TriggerContext 創建可能產生 GC 壓力
- **類型檢查**: ITriggeredSource 的具體類型判斷需要運行時檢查

### 未來優化方向
- 考慮對象池模式減少頻繁創建
- 使用泛型版本提供編譯時類型安全

**注意**: 具體的性能表現需要在實際整合和壓力測試中驗證。

---

## 已知問題與限制

### 當前實作狀態
基於現有代碼，此系統目前處於基礎框架階段：

### 架構限制
- [ ] 觸發源類型需要預定義
- [ ] 缺少觸發邏輯的具體實作
- [ ] 沒有觸發管理機制

### 待實作功能
- [ ] 具體的事件監聽機制
- [ ] 觸發條件的判斷邏輯
- [ ] 與各 Buff 系統的整合實作
- [ ] 觸發優先級管理
- [ ] 防止無限觸發鏈的機制

### 整合需求
- [ ] 與 GameplayModel 的具體整合
- [ ] 與各 Buff 系統的介面定義
- [ ] 觸發事件的生命週期管理

---

## 測試策略

### 基礎結構測試
1. **觸發上下文創建測試**
   - TriggerContext Record 的正確創建
   - 不同觸發源類別的正確實例化
   - 屬性訪問的正確性

2. **類型安全測試**
   - ITriggeredSource 介面的正確實作
   - 類型轉換的安全性

3. **數據完整性測試**
   - Record 不可變性驗證
   - 屬性值的正確保存

**注意**: 具體的觸發邏輯測試需要等待整合實作完成。

---

## 相關系統連結

- **[Action_System.md](Action_System.md)** - 動作事件系統（提供 IActionUnit）
- **[CardBuff_System.md](CardBuff_System.md)** - 卡片Buff系統（使用 CardBuffTrigger）
- **[PlayerBuff_System.md](PlayerBuff_System.md)** - 玩家Buff系統（使用 PlayerBuffTrigger）
- **[CharacterBuff_System.md](CharacterBuff_System.md)** - 角色Buff系統（使用 CharacterBuffTrigger）
- **[ReactionSession_System.md](ReactionSession_System.md)** - 反應會話系統（使用 TriggerContext 更新）
- **[Card_System.md](Card_System.md)** - 卡片系統（提供 CardTrigger 相關）
- **[Player_System.md](Player_System.md)** - 玩家系統（提供 PlayerTrigger）
- **GameplayModel.md** - 遊戲模型系統 ⏳ 待建立

---

**設計特點**: 🏗️ 事件響應機制的基礎框架，提供清晰的觸發源分類
**複雜度**: 📊 低-中等（簡潔的介面設計，基礎的數據結構）
**重要性**: ⭐⭐⭐ 核心（為 Buff 系統和事件驅動架構提供基礎）
**實作狀態**: 🔧 基礎框架完成，具體整合邏輯待實作

---

**檔案資訊**：
- 建立日期：2024-12-25
- 對應程式碼：TriggerSource.cs
- 開發狀態：🏗️ 基礎結構完整，整合機制待開發
- 下次更新：具體觸發邏輯實作完成後