# GameHistory - 遊戲歷史記錄系統

## 概述
GameHistory 是 MortalGame 專案中的遊戲歷史記錄系統，負責追蹤和記錄玩家在遊戲過程中的所有動作和效果。目前此系統**尚未完全實作**，僅有基礎框架結構。

## ⚠️ 開發狀態
**此系統目前為未完成狀態，需要後續實作補完**

### 預期功能目標
- 讓玩家確認這回合中打出了哪些牌
- 記錄每張卡片造成的具體效果
- 提供歷史動作的回顧機制
- 支援遊戲重播功能（潛在需求）

---

## 文件位置
**路徑**: `Assets/Scripts/GameModel/GameHistory.cs`

---

## 類別結構分析

### 1. GameHistory - 主要歷史記錄類別

```csharp
public class GameHistory
{
    private List<TurnRecord> _turnRecords = new();
    private IGameEventWatcher _gameEventWatcher;

    public GameHistory(IGameEventWatcher gameEventWatcher)
    {
        _gameEventWatcher = gameEventWatcher;
    }
}
```

#### 設計特點
- **分回合記錄**: 使用 `_turnRecords` 列表組織各回合的記錄
- **事件監聽**: 透過 `IGameEventWatcher` 監聽遊戲事件
- **依賴注入**: 建構子注入 `IGameEventWatcher`，支援測試和模組化

#### 目前狀態
- ✅ 基礎資料結構已定義
- ❌ 具體記錄邏輯尚未實作
- ❌ 查詢和檢索方法待添加

### 2. TurnRecord - 回合記錄類別

```csharp
public class TurnRecord
{
    private List<ActionRecord> _actionRecords = new();
}
```

#### 設計概念
- **動作集合**: 包含單一回合內的所有動作記錄
- **模組化設計**: 將回合與動作分離，便於管理

#### 待實作功能
- 回合開始/結束時間記錄
- 回合索引或識別碼
- 回合統計資訊（總傷害、卡片數量等）

### 3. ActionRecord - 動作記錄類別

```csharp
public class ActionRecord
{
    // 目前為空實作
}
```

#### 預期內容
根據遊戲需求，ActionRecord 應該包含：

```csharp
// 未來可能的實作方向
public class ActionRecord
{
    public string ActionType { get; set; }        // 動作類型（打牌、效果觸發等）
    public CardInstance PlayedCard { get; set; }  // 打出的卡片
    public List<Effect> Effects { get; set; }     // 產生的效果
    public TargetInfo Target { get; set; }        // 目標資訊
    public DateTime Timestamp { get; set; }       // 時間戳
    public string Description { get; set; }       // 動作描述
}
```

---

## 系統集成關係

### 依賴關係
```
GameHistory
├── IGameEventWatcher (事件監聽接口)
├── TurnRecord (回合記錄)
└── ActionRecord (動作記錄)

預期整合系統：
├── CardEntity (卡片實體記錄)
├── EffectType (效果類型追蹤)
├── TargetSystem (目標資訊)
└── GameTiming (事件時機)
```

### 與其他系統的關聯
- **[GameModel](GameModel_System.md)**: 作為 GameModel 的子系統
- **[GameEvent](GameEvent.md)**: 透過事件系統接收遊戲狀態變化
- **[Card_System.md](Card_System.md)**: 記錄卡片打出和效果
- **[GameEnum_Reference.md](GameEnum_Reference.md)**: 使用 GameTiming, EffectType 等枚舉

---

## 實作建議

### 階段一：基礎記錄功能
1. **完善 ActionRecord 結構**
   ```csharp
   public class ActionRecord
   {
       public GameTiming Timing { get; set; }
       public string ActionType { get; set; }
       public object ActionData { get; set; }
       public DateTime Timestamp { get; set; }
   }
   ```

2. **實作基本記錄方法**
   ```csharp
   public void RecordAction(ActionRecord record)
   public void StartNewTurn()
   public TurnRecord GetCurrentTurn()
   ```

### 階段二：查詢和檢索
1. **回合查詢**
   ```csharp
   public TurnRecord GetTurn(int turnIndex)
   public List<TurnRecord> GetTurnRange(int start, int end)
   ```

2. **動作過濾**
   ```csharp
   public List<ActionRecord> GetActionsByType(string actionType)
   public List<ActionRecord> GetActionsInTurn(int turnIndex)
   ```

### 階段三：進階功能
1. **統計分析**
   ```csharp
   public TurnStatistics GetTurnStats(int turnIndex)
   public GameStatistics GetGameStats()
   ```

2. **序列化支援**
   ```csharp
   public string SerializeHistory()
   public void LoadFromSerialized(string data)
   ```

---

## 事件集成方案

### 建議監聽事件
根據 [GameEnum_Reference.md](GameEnum_Reference.md) 中的 GameTiming：

```csharp
// 需要監聽的關鍵時機
- PlayCardStart/PlayCardEnd: 記錄卡片打出
- EffectTargetResult: 記錄效果結果
- TurnStart/TurnEnd: 管理回合邊界
- CharacterDeath: 記錄重要事件
```

### 實作模式
```csharp
public class GameHistory : IGameEventListener
{
    public void OnGameEvent(GameTiming timing, GameEventArgs args)
    {
        var record = new ActionRecord
        {
            Timing = timing,
            Timestamp = DateTime.Now,
            ActionData = args
        };
        
        RecordAction(record);
    }
}
```

---

## UI 展示需求

### 回合歷史面板
- 回合列表顯示
- 每回合的卡片打出記錄
- 效果詳細說明
- 傷害/治療統計

### 動作詳情視窗
- 卡片資訊展示
- 目標選擇記錄
- 效果鏈展示
- 數值變化追蹤

---

## 性能考量

### 記憶體管理
- 長期遊戲中歷史記錄可能過大
- 考慮實作記錄清理機制
- 使用物件池減少 GC 壓力

### 查詢優化
- 為常用查詢建立索引
- 實作分頁載入機制
- 考慮使用快取策略

---

## 測試策略

### 單元測試重點
1. **記錄功能測試**
   - 動作記錄正確性
   - 回合邊界處理
   - 事件監聽機制

2. **查詢功能測試**
   - 歷史查詢準確性
   - 過濾條件有效性
   - 邊界條件處理

### 集成測試場景
1. **完整遊戲流程記錄**
2. **多回合連續記錄**
3. **複雜效果鏈記錄**

---

## 待解決問題

### 技術問題
- [ ] IGameEventWatcher 接口定義
- [ ] ActionRecord 具體結構設計
- [ ] 序列化格式選擇
- [ ] 性能優化方案

### 設計問題
- [ ] 歷史記錄的詳細程度控制
- [ ] 重播功能的實作方式
- [ ] UI 展示的資訊組織

### 整合問題
- [ ] 與 GameEvent 系統的具體整合
- [ ] 與 UI 系統的資料傳遞
- [ ] 存檔系統的配合

---

## 相關系統連結

- **[GameModel_System.md](GameModel_System.md)** - 上層遊戲模型系統
- **[GameEvent.md](GameEvent.md)** - 事件系統整合 ⏳ 待建立
- **[Card_System.md](Card_System.md)** - 卡片系統記錄
- **[GameEnum_Reference.md](GameEnum_Reference.md)** - 相關枚舉定義
- **[UI_System.md](UI_System.md)** - 歷史展示介面 ⏳ 待建立

---

**開發優先級**: 🔴 高優先級（影響用戶體驗的核心功能）
**估計工作量**: 📊 中等（需要與多個系統整合）
**風險評估**: ⚠️ 中風險（依賴事件系統的穩定性）

---

**檔案資訊**：
- 建立日期：2024-12-20
- 對應程式碼：GameHistory.cs
- 開發狀態：⏳ 框架已建立，核心功能待實作
- 下次更新：實作基礎記錄功能後