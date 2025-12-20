# GameContextManager - 遊戲上下文管理系統

## 概述
GameContextManager 是 MortalGame 專案中的遊戲上下文管理系統，負責管理遊戲中的各種資料庫資源和玩家選擇狀態。目前此系統的**職責定義尚未完全明確**，有待後續設計調整和功能擴展。

## ⚠️ 開發狀態
**此系統目前為演進中狀態，責任邊界和功能範圍可能會持續調整**

### 設計特點
- **資源聚合器**: 統一管理多種遊戲資料庫
- **狀態追蹤**: 記錄玩家當前選擇的目標
- **上下文堆疊**: 支援嵌套的選擇狀態管理

---

## 文件位置
**路徑**: `Assets/Scripts/GameModel/GameContextManager.cs`

---

## 介面設計

### IGameContextManager - 上下文管理介面

```csharp
public interface IGameContextManager : IDisposable
{
    // 資料庫屬性
    CardLibrary CardLibrary { get; }
    CardBuffLibrary CardBuffLibrary { get; }
    PlayerBuffLibrary PlayerBuffLibrary { get; }
    CharacterBuffLibrary CharacterBuffLibrary { get; }
    DispositionLibrary DispositionLibrary { get; }
    LocalizeLibrary LocalizeLibrary { get; }

    // 上下文屬性
    GameContext Context { get; }

    // 上下文操作方法
    IGameContextManager SetClone();
    IGameContextManager SetSelectedPlayer(Option<IPlayerEntity> selectedPlayer);
    IGameContextManager SetSelectedCharacter(Option<ICharacterEntity> selectedCharacter);
    IGameContextManager SetSelectedCard(Option<ICardEntity> selectedCard);
}
```

#### 設計特點
- **資源訪問**: 提供統一的資料庫訪問點
- **狀態管理**: 管理當前遊戲選擇狀態
- **流暢介面**: 方法鏈式呼叫設計
- **資源管理**: 實作 IDisposable 支援自動清理

---

## 核心實作類別

### GameContextManager - 主要實作類別

```csharp
public class GameContextManager : IGameContextManager
{
    // 私有資料庫實例
    private readonly CardLibrary _cardLibrary;
    private readonly CardBuffLibrary _cardBuffLibrary;
    private readonly PlayerBuffLibrary _playerBuffLibrary;
    private readonly CharacterBuffLibrary _characterBuffLibrary;
    private readonly DispositionLibrary _dispositionLibrary;
    private readonly LocalizeLibrary _localizeLibrary;

    // 上下文堆疊管理
    private Stack<GameContext> _contextStack = new Stack<GameContext>();
    public GameContext Context => _contextStack.Peek();
}
```

### 1. 資料庫管理功能

#### 支援的資料庫類型
- **CardLibrary**: 卡片資料庫 - 管理所有卡片模板
- **CardBuffLibrary**: 卡片Buff資料庫 - 管理卡片狀態效果
- **PlayerBuffLibrary**: 玩家Buff資料庫 - 管理玩家狀態效果
- **CharacterBuffLibrary**: 角色Buff資料庫 - 管理角色狀態效果
- **DispositionLibrary**: 配置資料庫 - 管理遊戲配置（功能待明確）
- **LocalizeLibrary**: 本地化資料庫 - 管理多語言內容

#### 依賴注入設計
```csharp
public GameContextManager(
    CardLibrary cardLibrary,
    CardBuffLibrary cardBuffLibrary,
    PlayerBuffLibrary playerBuffLibrary,
    CharacterBuffLibrary characterBuffLibrary,
    DispositionLibrary dispositionLibrary,
    LocalizeLibrary localizeLibrary)
```
**特點**: 透過建構子注入所有依賴，支援測試和模組化設計。

### 2. 上下文堆疊管理

#### 堆疊結構設計
```csharp
private Stack<GameContext> _contextStack = new Stack<GameContext>();
```

**用途**:
- 支援嵌套的選擇狀態
- 可以暫時切換上下文，後續恢復
- 類似瀏覽器的歷史記錄機制

#### 上下文操作方法

##### SetClone() - 複製當前上下文
```csharp
public IGameContextManager SetClone()
{
    _contextStack.Push(Context with { });
    return this;
}
```
**功能**: 創建當前上下文的副本並推入堆疊。

##### SetSelectedPlayer() - 設定選中玩家
```csharp
public IGameContextManager SetSelectedPlayer(Option<IPlayerEntity> selectedPlayer)
{
    return selectedPlayer.Match(
        some: player => {
            _contextStack.Push(Context with { SelectedPlayer = player.Identity });
            return this;
        },
        none: () => SetClone()
    );
}
```
**特點**:
- 使用 Optional 模式處理空值
- 函數式風格的 Match 處理
- 流暢介面設計

##### SetSelectedCharacter() - 設定選中角色
```csharp
public IGameContextManager SetSelectedCharacter(Option<ICharacterEntity> selectedCharacter)
```
**功能**: 同 SetSelectedPlayer，但針對角色實體。

##### SetSelectedCard() - 設定選中卡片
```csharp
public IGameContextManager SetSelectedCard(Option<ICardEntity> selectedCard)
```
**功能**: 同 SetSelectedPlayer，但針對卡片實體。

### 3. 資源清理機制

#### Dispose 實作
```csharp
public void Dispose()
{
    if (_contextStack.Count > 1)
    {
        _contextStack.Pop();
    }
}
```

**功能**: 
- 實作 IDisposable 介面
- 彈出當前上下文，回到上一層
- 保護機制：防止清空整個堆疊

**使用模式**:
```csharp
using (contextManager.SetSelectedPlayer(player))
{
    // 在此範圍內，player 為選中狀態
} // 自動恢復到之前的上下文
```

---

## GameContext - 上下文資料結構

### Record 定義
```csharp
public record GameContext(
    Guid SelectedPlayer,
    Guid SelectedCharacter,
    Guid SelectedCard)
{ 
    public static GameContext EMPTY => new(Guid.Empty, Guid.Empty, Guid.Empty);
}
```

### 設計特點
- **不可變性**: 使用 Record 確保資料不可變
- **識別符**: 使用 Guid 標識選中的實體
- **空狀態**: 提供 EMPTY 常數表示無選擇狀態

### 用途說明
- **目標追蹤**: 記錄玩家當前指定的目標
- **效果系統**: 配合 TriggerContext 使用，支援效果觸發
- **UI 反饋**: 為 UI 系統提供選擇狀態資訊

---

## 系統集成關係

### 依賴關係
```
GameContextManager
├── CardLibrary (卡片資料庫)
├── CardBuffLibrary (卡片Buff資料庫)
├── PlayerBuffLibrary (玩家Buff資料庫)  
├── CharacterBuffLibrary (角色Buff資料庫)
├── DispositionLibrary (配置資料庫)
├── LocalizeLibrary (本地化資料庫)
└── GameContext (選擇狀態記錄)

預期整合系統：
├── TriggerContext (觸發上下文)
├── GameModel (遊戲模型)
├── UI系統 (選擇狀態顯示)
└── 效果系統 (目標解析)
```

### 與其他系統的關聯
- **[GameModel_System.md](GameModel_System.md)**: 作為 GameModel 的組件 ⏳ 待建立
- **[Card_System.md](Card_System.md)**: 透過 CardLibrary 訪問卡片資料
- **[CardBuff_System.md](CardBuff_System.md)**: 透過 CardBuffLibrary 訪問卡片Buff
- **[PlayerBuff_System.md](PlayerBuff_System.md)**: 透過 PlayerBuffLibrary 訪問玩家Buff
- **[CharacterBuff_System.md](CharacterBuff_System.md)**: 透過 CharacterBuffLibrary 訪問角色Buff
- **TriggerContext**: 配合使用，支援效果觸發機制 ⏳ 待建立

---

## 設計模式分析

### 1. 聚合器模式 (Aggregator Pattern)
- 統一管理多個相關的資料庫
- 提供單一訪問點簡化依賴關係

### 2. 狀態堆疊模式 (State Stack Pattern)
- 使用堆疊管理嵌套狀態
- 支援狀態的推入和彈出操作

### 3. 流暢介面模式 (Fluent Interface)
- 方法返回自身支援鏈式呼叫
- 提升代碼可讀性和使用體驗

### 4. 選項模式 (Option Pattern)
- 使用 Optional 庫處理可能為空的值
- 函數式風格的安全處理

---

## 使用場景與範例

### 基本使用
```csharp
// 創建上下文管理器
var contextManager = new GameContextManager(
    cardLibrary, cardBuffLibrary, playerBuffLibrary,
    characterBuffLibrary, dispositionLibrary, localizeLibrary);

// 設定選中的玩家和角色
using (contextManager.SetSelectedPlayer(player)
                   .SetSelectedCharacter(character))
{
    // 在此範圍內處理與該玩家和角色相關的邏輯
    var currentContext = contextManager.Context;
    
    // 可以訪問選中的實體ID
    var playerId = currentContext.SelectedPlayer;
    var characterId = currentContext.SelectedCharacter;
}
// 自動恢復到之前的上下文狀態
```

### 與效果系統整合
```csharp
// 假設的使用場景：處理卡片效果
public void ProcessCardEffect(CardEntity card, PlayerEntity player)
{
    using (contextManager.SetSelectedPlayer(Option.Some<IPlayerEntity>(player))
                         .SetSelectedCard(Option.Some<ICardEntity>(card)))
    {
        // 在此上下文中，TriggerContext 可以知道當前選中的目標
        // 效果系統可以使用這些資訊來解析目標
        effectSystem.TriggerCardEffect(card, contextManager.Context);
    }
}
```

---

## 待擴展功能

### 可能的功能擴展
1. **更多選擇狀態**: 選中的技能、道具、位置等
2. **歷史記錄**: 記錄選擇變更的歷史
3. **狀態驗證**: 檢查選擇狀態的有效性
4. **事件通知**: 選擇狀態變更時發送事件
5. **序列化支援**: 保存和載入上下文狀態

### 架構優化建議
1. **職責分離**: 將資料庫管理和狀態管理分離
2. **介面細化**: 為不同用途提供專門的介面
3. **性能優化**: 對頻繁訪問的資料進行快取
4. **測試支援**: 增加測試專用的模擬實作

---

## 已知問題與限制

### 設計問題
- [ ] 職責邊界不清晰（資源管理 vs 狀態管理）
- [ ] 資料庫訪問可能造成性能問題
- [ ] 上下文堆疊可能過度成長

### 實作限制
- [ ] 沒有狀態有效性檢查機制
- [ ] 缺少並行訪問的安全保護
- [ ] 堆疊深度沒有限制

### 整合問題
- [ ] 與 TriggerContext 的具體整合方式待定
- [ ] UI 系統如何監聽狀態變更
- [ ] 效果系統的目標解析機制

---

## 測試策略

### 單元測試重點
1. **上下文操作測試**
   - 堆疊推入和彈出正確性
   - 選擇狀態設定和恢復
   - Dispose 機制驗證

2. **資料庫訪問測試**
   - 各個資料庫屬性的正確返回
   - 依賴注入的正確性

### 集成測試場景
1. **嵌套上下文測試**
2. **與效果系統的整合測試**
3. **記憶體洩漏測試**

---

## 相關系統連結

- **[GameModel_System.md](GameModel_System.md)** - 上層遊戲模型系統 ⏳ 待建立
- **[Card_System.md](Card_System.md)** - 卡片系統整合
- **[CardBuff_System.md](CardBuff_System.md)** - 卡片Buff系統
- **[PlayerBuff_System.md](PlayerBuff_System.md)** - 玩家Buff系統  
- **[CharacterBuff_System.md](CharacterBuff_System.md)** - 角色Buff系統
- **TriggerContext.md** - 觸發上下文系統 ⏳ 待建立
- **UI_System.md** - 用戶介面系統 ⏳ 待建立

---

**開發狀態**: 🔄 持續演進中（職責和功能範圍可能調整）
**複雜度**: 📊 中等（涉及多系統整合）
**維護風險**: ⚠️ 中等（職責不明確可能導致過度膨脹）

---

**檔案資訊**：
- 建立日期：2024-12-20
- 對應程式碼：GameContextManager.cs
- 開發狀態：🔄 功能基本完整，設計持續調整中
- 下次更新：職責邊界明確化後