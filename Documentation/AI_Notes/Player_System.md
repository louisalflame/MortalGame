# Player 玩家系統總覽筆記

## 系統概述
Player系統是MortalGame戰鬥系統的核心參與者管理機制，負責處理友軍(Ally)和敵軍(Enemy)的完整戰鬥邏輯。系統採用Data-Entity雙層架構，實現了資料定義與運行時邏輯的清晰分離，並且預留了多角色擴展的設計空間。

**核心特色**：
- **對稱設計**：友軍和敵軍使用統一的基礎架構
- **可擴展角色**：支援未來的多角色戰鬥系統
- **完整管理**：涵蓋能量、卡牌、Buff、角色等全部戰鬥要素
- **AI整合**：敵軍具備完整的AI決策系統

## 系統架構

### 雙層架構設計
```
ScriptableAsset層 (資料定義)
├── PlayerData (基礎玩家數據)
├── AllyData (友軍專屬數據) 
└── EnemyData (敵軍專屬數據)
        ↓ 戰鬥轉換
Runtime層 (戰鬥實體)
├── IPlayerEntity (玩家實體接口)
├── AllyEntity (友軍戰鬥實體)
├── EnemyEntity (敵軍戰鬥實體)
└── DummyPlayer (空值物件)
```

**檔案位置**：
- **Data層**: 
  - [PlayerData.cs](../../Assets/Scripts/GameData/PlayerData.cs)
  - [AllyData.cs](../../Assets/Scripts/GameData/AllyData.cs) 
  - [EnemyData.cs](../../Assets/Scripts/GameData/EnemyData.cs)
- **Entity層**: [PlayerEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerEntity.cs)

## Data層架構分析

### PlayerData (基礎玩家數據)
```csharp
public class PlayerData
{
    // 識別資訊
    public string ID;
    
    // 基礎數值
    public int MaxHealth;           // 最大生命值
    public int MaxEnergy;           // 最大能量
    public int InitialHealth;       // 初始生命值
    public int InitialEnergy;       // 初始能量
    
    // 卡牌系統
    public DeckScriptable Deck;     // 牌組配置
    public int HandCardMaxCount;    // 手牌上限
    
    // 本地化
    public string NameKey;          // 名稱本地化鍵值
}
```

### AllyData (友軍數據)
```csharp
public class AllyData
{
    // 友軍專屬
    public string GameMode;         // 遊戲模式
    public int InitialDisposition;  // 初始好感度
    
    // 繼承基礎數據
    public PlayerData PlayerData;   // 包含所有PlayerData
}
```

### EnemyData (敵軍數據)
```csharp
public class EnemyData  
{
    // 敵軍專屬
    public string EnemyID;              // 敵人ID
    public int Level;                   // 等級
    public int SelectedCardMaxCount;    // AI選卡上限
    public int TurnStartDrawCardCount;  // 回合開始抽牌數
    public int EnergyRecoverPoint;      // 能量回復點數
    
    // 繼承基礎數據
    public PlayerData PlayerData;       // 包含所有PlayerData
}
```

## Entity層架構分析

### IPlayerEntity (核心接口)
```csharp
public interface IPlayerEntity
{
    // 身份識別
    Guid Identity { get; }                                      // 實體唯一ID
    Faction Faction { get; }                                    // 陣營(Ally/Enemy)
    
    // 角色系統
    IReadOnlyCollection<ICharacterEntity> Characters { get; }   // 角色集合
    ICharacterEntity MainCharacter { get; }                     // 主角色
    
    // 戰鬥系統
    IPlayerCardManager CardManager { get; }                     // 卡牌管理
    int CurrentEnergy { get; }                                  // 當前能量
    int MaxEnergy { get; }                                      // 最大能量
    IEnergyManager EnergyManager { get; }                       // 能量管理
    IPlayerBuffManager BuffManager { get; }                     // Buff管理
    
    // 狀態
    bool IsDead { get; }                                        // 是否死亡
    
    // 更新
    IGameEvent Update(TriggerContext triggerContext);           // 系統更新
}
```

### PlayerEntity (抽象基類)
- 實現IPlayerEntity的通用邏輯
- 管理身份、陣營、能量、Buff等基礎功能
- 提供Update機制統一處理各子系統更新

### AllyEntity (友軍實體)
```csharp
public class AllyEntity : PlayerEntity
{
    // 友軍專屬
    public IDispositionManager DispositionManager;  // 好感度管理
    
    // 特殊功能
    public AllyEntity Clone(IGameContextManager gameContext);  // 克隆功能
}
```

**友軍特色**：
- **好感度系統**：影響特殊卡片效果和劇情發展
- **完整克隆**：支援遊戲狀態備份和回滾

### EnemyEntity (敵軍實體) 
```csharp
public class EnemyEntity : PlayerEntity
{
    // AI系統
    public ISelectedCardEntity SelectedCards;      // AI選定的卡片
    public int EnergyRecoverPoint;                 // 能量回復點數
    public int TurnStartDrawCardCount;             // 回合開始抽牌數
    
    // AI行為
    public bool TryGetRecommandSelectCard(...);   // AI選卡邏輯
    public bool TryGetNextUseCardAction(...);     // AI出牌邏輯
    public EnemyEntity Clone(...);                // 克隆功能
}
```

**敵軍特色**：
- **AI選卡系統**：智能選擇最優卡片組合
- **動態決策**：根據戰場狀況調整策略
- **可配置難度**：通過數據調整AI行為

## 角色系統設計

### 多角色架構預留
```csharp
// 當前實現：單角色
public ICharacterEntity MainCharacter => Characters.First();

// 未來擴展：多角色系統
// TODO: Implement main character with skills/assistant character
IReadOnlyCollection<ICharacterEntity> Characters { get; }
```

**設計考量**：
- **可擴展性**：為多角色戰鬥預留接口
- **向後兼容**：當前單角色邏輯不受影響
- **靈活配置**：支援不同的角色組合策略

### 死亡判定機制
```csharp
public bool IsDead => Characters.All(character => character.IsDead);
```
- 所有角色死亡才算玩家死亡
- 為多角色生存戰術預留空間

## 管理器系統整合

### 🔋 能量管理 (EnergyManager)
- 管理當前能量和最大能量
- 處理能量消耗和回復
- 🔗*需要EnergyManager_Class.md*

### 🃏 卡牌管理 (PlayerCardManager)
- 管理手牌、牌組、墓地等卡片集合
- 處理抽牌、打牌、棄牌邏輯
- 🔗*需要PlayerCardManager_Class.md*

### 🎭 Buff管理 (PlayerBuffManager)
- 管理玩家身上的所有Buff效果
- 處理Buff的添加、更新、移除
- 🔗*需要PlayerBuffManager_Class.md*

### 💖 好感度管理 (DispositionManager)
- **僅友軍擁有**：管理與玩家的好感度關係
- 影響特定卡片效果和劇情發展
- 🔗*需要DispositionManager_Class.md*

## AI系統整合

### 選卡邏輯 (UseCardLogic)
```csharp
// AI選卡推薦
bool TryGetRecommandSelectCard(IGameplayModel gameplayWatcher, out ICardEntity cardEntity)

// AI出牌決策  
bool TryGetNextUseCardAction(IGameplayModel gameplayWatcher, out UseCardAction useCardAction)
```

**AI特色**：
- **策略分析**：基於當前戰場狀況進行決策
- **動態調整**：根據玩家行為調整策略
- **可配置性**：透過EnemyData調整AI強度

### 選卡管理 (SelectedCardEntity)
- 管理AI選定但尚未使用的卡片
- 限制AI每回合的選卡數量
- 實現AI的戰術規劃功能

## 更新機制設計

### 統一更新流程
```csharp
public IGameEvent Update(TriggerContext triggerContext)
{
    // 更新玩家Buff
    var updatedPlayerBuffInfos = _buffManager.Update(triggerContext);
    
    // 更新角色Buff
    var updatedCharacterBuffInfos = _characters
        .SelectMany(character => character.BuffManager.Update(triggerContext));
    
    // 更新卡片狀態
    var updatedCardInfos = _cardManager.Update(triggerContext);
    
    return new GeneralUpdateEvent(/* 所有更新資訊 */);
}
```

**更新特色**：
- **統一介面**：所有子系統使用相同的更新機制
- **事件驅動**：更新結果以事件形式通知外部系統
- **上下文傳遞**：TriggerContext提供更新所需的完整環境

## 擴展功能分析

### Buff屬性計算
```csharp
// 整數型屬性加成
public static int GetPlayerBuffAdditionProperty(
    this IPlayerEntity player, TriggerContext triggerContext, PlayerBuffProperty targetProperty)

// 比例型屬性加成
public static float GetPlayerBuffRatioProperty(
    this IPlayerEntity player, TriggerContext triggerContext, PlayerBuffProperty targetProperty)
```

### 玩家查詢功能
```csharp
public static Option<IPlayerEntity> GetPlayer(this GameStatus status, Guid playerIdentity)
```

## 設計模式應用

### 🏭 工廠模式 (Factory Pattern)
```csharp
// 從資料創建實體
AllyEntity.Create(AllyData allyData)
EnemyEntity.Create(EnemyData enemyData)
```

### 🎭 策略模式 (Strategy Pattern)
- 友軍和敵軍有不同的行為策略
- AI邏輯可插拔式替換

### 📋 組合模式 (Composite Pattern)  
- PlayerEntity組合多個管理器系統
- 角色集合支援多角色組合

### 🔄 原型模式 (Prototype Pattern)
```csharp
// 支援玩家實體克隆
public AllyEntity Clone(IGameContextManager gameContext)
public EnemyEntity Clone(IGameContextManager gameContext)
```

## 遊戲流程整合

### 戰鬥初始化流程
```
1. 載入AllyData/EnemyData
2. 創建AllyEntity/EnemyEntity
3. 初始化角色、卡牌、能量等系統
4. 進入戰鬥循環
```

### 簡化選關機制
- **當前實現**：直接使用第一個Enemy進入戰鬥
- **未來擴展**：完整的選關和關卡配置系統
- **設計彈性**：數據結構已支援複雜的關卡配置

## 依賴關係

### 依賴的組件
- **🔗 Faction**: 陣營枚舉 *需要Faction_Enum.md*
- **🔗 CharacterEntity**: 角色實體系統 *需要CharacterEntity_Class.md*
- **🔗 TriggerContext**: 觸發上下文 *需要TriggerContext_Class.md*
- **🔗 GameEvent**: 遊戲事件系統 *需要GameEvent_Class.md*
- **🔗 Optional**: 安全空值處理 *需要Optional_Library.md*

### 被依賴的組件
- **🔗 CardEntity**: 卡片系統使用PlayerEntity *參考CardEntity_Class.md*
- **🔗 GameplayModel**: 遊戲狀態模型 *需要GameplayModel_Class.md*
- **🔗 戰鬥系統**: 使用Player作為戰鬥參與者
- **🔗 UI系統**: 顯示玩家狀態和數據

## 使用場景範例

### 戰鬥初始化
```csharp
// 創建友軍
var allyEntity = new AllyEntity(
    originPlayerInstanceGuid: playerInstance.Guid,
    characterParams: CreateCharacterParams(allyData.PlayerData),
    currentEnergy: allyData.PlayerData.InitialEnergy,
    maxEnergy: allyData.PlayerData.MaxEnergy,
    handCardMaxCount: allyData.PlayerData.HandCardMaxCount,
    currentDisposition: allyData.InitialDisposition,
    maxDisposition: maxDisposition,
    gameContext: gameContext
);

// 創建敵軍
var enemyEntity = new EnemyEntity(
    characterParams: CreateCharacterParams(enemyData.PlayerData),
    currentEnergy: enemyData.PlayerData.InitialEnergy,
    maxEnergy: enemyData.PlayerData.MaxEnergy,
    handCardMaxCount: enemyData.PlayerData.HandCardMaxCount,
    selectedCardMaxCount: enemyData.SelectedCardMaxCount,
    turnStartDrawCardCount: enemyData.TurnStartDrawCardCount,
    energyRecoverPoint: enemyData.EnergyRecoverPoint,
    gameContext: gameContext
);
```

### AI決策流程
```csharp
// AI選卡
if (enemy.TryGetRecommandSelectCard(gameplayModel, out ICardEntity selectedCard))
{
    Debug.Log($"AI選擇了卡片: {selectedCard.CardDataId}");
}

// AI出牌
if (enemy.TryGetNextUseCardAction(gameplayModel, out UseCardAction action))
{
    Debug.Log($"AI決定使用卡片: {action.CardIndentity}");
    // 執行出牌動作
}
```

### 狀態更新
```csharp
// 系統更新
var updateEvent = player.Update(triggerContext);

// 處理更新事件
if (updateEvent is GeneralUpdateEvent generalUpdate)
{
    // 處理Buff更新
    foreach (var buffInfo in generalUpdate.UpdatedPlayerBuffInfos)
    {
        UI.UpdateBuffDisplay(buffInfo);
    }
    
    // 處理卡片更新
    foreach (var cardInfo in generalUpdate.UpdatedCardInfos)
    {
        UI.UpdateCardDisplay(cardInfo);
    }
}
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [PlayerData.cs](../../Assets/Scripts/GameData/PlayerData.cs) | 核心 | 基礎玩家資料定義 |
| [AllyData.cs](../../Assets/Scripts/GameData/AllyData.cs) | 核心 | 友軍專屬資料定義 |
| [EnemyData.cs](../../Assets/Scripts/GameData/EnemyData.cs) | 核心 | 敵軍專屬資料定義 |
| [PlayerEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerEntity.cs) | 核心 | 玩家戰鬥實體實現 |
| [CardEntity.cs](../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs) | 被依賴 | 卡片實體使用Player信息 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成