# PlayerEntity 玩家實體類別筆記

## 類別概述
PlayerEntity是MortalGame戰鬥系統的核心實體類別，將ScriptableAsset的PlayerData轉換為具備完整戰鬥功能的運行時物件。透過IPlayerEntity接口和抽象基類設計，實現了友軍(AllyEntity)和敵軍(EnemyEntity)的統一管理，並整合了能量、卡牌、Buff、角色等全部戰鬥子系統。

**設計亮點**：
- **統一接口**：友軍和敵軍使用相同的IPlayerEntity接口
- **多角色準備**：為未來多角色系統預留完整架構
- **完整管理**：整合所有戰鬥相關的子系統
- **AI差異化**：敵軍具備專屬的AI決策系統
- **可克隆性**：支援遊戲狀態的備份和回滾

**檔案位置**: [PlayerEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerEntity.cs)

## IPlayerEntity 核心接口

### 接口結構
```csharp
public interface IPlayerEntity
{
    // 身份識別系統
    Guid Identity { get; }                                      // 實體唯一標識
    Faction Faction { get; }                                    // 陣營標識
    
    // 角色系統
    IReadOnlyCollection<ICharacterEntity> Characters { get; }   // 角色集合
    ICharacterEntity MainCharacter { get; }                     // 主要角色
    
    // 戰鬥資源系統
    int CurrentEnergy { get; }                                  // 當前能量
    int MaxEnergy { get; }                                      // 最大能量
    IEnergyManager EnergyManager { get; }                       // 能量管理器
    
    // 卡牌系統
    IPlayerCardManager CardManager { get; }                     // 卡牌管理器
    
    // Buff系統
    IPlayerBuffManager BuffManager { get; }                     // Buff管理器
    
    // 狀態查詢
    bool IsDead { get; }                                        // 死亡狀態
    
    // 系統更新
    IGameEvent Update(TriggerContext triggerContext);           // 統一更新機制
}
```

### 接口設計理念

#### 🆔 身份識別系統
- **Identity**: 戰鬥中的唯一標識，用於事件追蹤和引用
- **Faction**: 區分友軍/敵軍，影響目標選擇和效果判定

#### 👥 角色管理系統
```csharp
IReadOnlyCollection<ICharacterEntity> Characters { get; }  // 多角色集合
ICharacterEntity MainCharacter { get; }                    // 主角色快速訪問
```

**多角色設計**：
- **當前狀態**: 每個玩家只有一個角色
- **未來擴展**: 支援主角色+助理角色的組合戰鬥
- **死亡判定**: 所有角色死亡才算玩家死亡

#### ⚡ 戰鬥系統整合
- **能量管理**: 控制卡牌使用的核心資源
- **卡牌管理**: 處理抽牌、出牌、棄牌等邏輯
- **Buff管理**: 處理各種臨時效果和狀態

## PlayerEntity 抽象基類

### 核心實現
```csharp
public abstract class PlayerEntity : IPlayerEntity
{
    // 核心欄位
    private readonly Guid _identity;                           // 唯一標識
    private readonly Faction _faction;                         // 陣營歸屬
    private readonly IEnergyManager _energyManager;            // 能量管理
    private readonly IPlayerBuffManager _buffManager;          // Buff管理
    
    // 子類配置欄位
    protected Option<Guid> _originPlayerInstanceGuid;         // 原始實例引用
    protected IPlayerCardManager _cardManager;                // 卡牌管理
    protected IReadOnlyCollection<CharacterEntity> _characters; // 角色集合
    
    // 屬性代理
    public Guid Identity => _identity;
    public Faction Faction => _faction;
    public int CurrentEnergy => EnergyManager.Energy;
    public int MaxEnergy => EnergyManager.MaxEnergy;
    public bool IsDead => Characters.All(character => character.IsDead);
    
    // 角色系統
    public ICharacterEntity MainCharacter => Characters.First(); // 當前實現
}
```

### 建構函數設計
```csharp
public PlayerEntity(
    Faction faction,
    int currentEnergy,
    int maxEnergy
)
{
    _identity = Guid.NewGuid();                               // 自動生成ID
    _faction = faction;
    _energyManager = new EnergyManager(currentEnergy, maxEnergy);
    _buffManager = new PlayerBuffManager();                  // 空Buff管理器
}
```

**設計特點**：
- **自動ID生成**: 確保每個實體都有唯一標識
- **基礎初始化**: 只初始化通用組件
- **子類責任**: 具體的角色和卡牌由子類初始化

### 死亡判定邏輯
```csharp
public bool IsDead => Characters.All(character => character.IsDead);
```

**多角色死亡機制**：
- **全員死亡**: 所有角色都死亡才算玩家死亡
- **戰術意義**: 支援保護重要角色的策略
- **擴展準備**: 為多角色戰鬥預留邏輯

### 主角色系統
```csharp
// TODO: Implement main character with skills/assistant character
public ICharacterEntity MainCharacter => Characters.First();
```

**當前實現與未來規劃**：
- **當前**: 簡單返回第一個角色
- **未來**: 區分主角色和助理角色，支援技能系統
- **設計預留**: 接口已支援複雜的角色關係

## 統一更新機制

### Update方法實現
```csharp
public IGameEvent Update(TriggerContext triggerContext)
{
    // 更新玩家Buff
    var updatedPlayerBuffInfos = _buffManager
        .Update(triggerContext)
        .Select(buff => buff.ToInfo(triggerContext.Model));

    // 更新角色Buff
    var updatedCharacterBuffInfos = _characters
        .Select(character => character.BuffManager)
        .SelectMany(buffManager => buffManager.Update(triggerContext))
        .Select(buff => buff.ToInfo(triggerContext.Model));

    // 更新卡片狀態
    var updatedCardInfos = _cardManager
        .Update(triggerContext)
        .Select(card => card.ToInfo(triggerContext.Model));

    return new GeneralUpdateEvent(
        updatedPlayerBuffInfos.ToList(),
        updatedCharacterBuffInfos.ToList(),
        updatedCardInfos.ToList());
}
```

**更新流程**：
1. **Buff更新**: 處理玩家和角色身上的所有Buff
2. **卡牌更新**: 更新所有卡片的狀態和生命週期
3. **事件生成**: 將所有變更封裝為GeneralUpdateEvent
4. **信息傳遞**: 透過事件系統通知其他組件

## AllyEntity 友軍實體

### 類別定義
```csharp
public class AllyEntity : PlayerEntity
{
    // 友軍專屬系統
    public IDispositionManager DispositionManager;         // 好感度管理
    
    // 建構函數
    public AllyEntity(
        Guid originPlayerInstanceGuid,                     // 原始實例引用
        CharacterParameter[] characterParams,              // 角色參數
        int currentEnergy,                                 // 當前能量
        int maxEnergy,                                     // 最大能量
        int handCardMaxCount,                             // 手牌上限
        int currentDisposition,                           // 當前好感度
        int maxDisposition,                               // 最大好感度
        IGameContextManager gameContext                   // 遊戲上下文
    ) : base(Faction.Ally, currentEnergy, maxEnergy)
    {
        _originPlayerInstanceGuid = originPlayerInstanceGuid.Some();
        _characters = characterParams
            .Select(param => CharacterEntity.Create(param))
            .ToList();
        _cardManager = new PlayerCardManager(handCardMaxCount);
        DispositionManager = new DispositionManager(currentDisposition, maxDisposition);
    }
}
```

### 友軍特色功能

#### 💖 好感度系統
```csharp
public IDispositionManager DispositionManager;
```

**好感度管理**：
- **影響效果**: 某些卡片效果會根據好感度調整
- **劇情整合**: 好感度影響劇情發展和對話選項
- **動態變化**: 戰鬥中的表現會影響好感度數值

#### 🔄 克隆功能
```csharp
public AllyEntity Clone(IGameContextManager gameContext)
{
    var cloneAlly = new AllyEntity(
        originPlayerInstanceGuid: _originPlayerInstanceGuid.ValueOr(Guid.Empty),
        characterParams: _characters
            .Select(c => new CharacterParameter
            {
                NameKey         = c.NameKey,
                CurrentHealth   = c.CurrentHealth,
                MaxHealth       = c.MaxHealth
            })
            .ToArray(),
        currentEnergy: CurrentEnergy,
        maxEnergy: MaxEnergy,
        handCardMaxCount: _cardManager.HandCard.MaxCount,
        currentDisposition: DispositionManager.CurrentDisposition,
        maxDisposition: DispositionManager.MaxDisposition,
        gameContext: gameContext
    );

    //cloneAlly._cardManager = new PlayerCardManager(); TODO
    return cloneAlly;
}
```

**克隆特色**：
- **狀態保留**: 克隆時保留當前的健康狀態和好感度
- **新ID生成**: 克隆體有獨立的Identity
- **卡牌管理**: TODO標記表示卡牌克隆仍待完善

## EnemyEntity 敵軍實體

### 類別定義
```csharp
public class EnemyEntity : PlayerEntity
{
    // AI專屬系統
    public ISelectedCardEntity SelectedCards;          // AI選卡管理
    public int EnergyRecoverPoint;                     // 能量回復點數
    public int TurnStartDrawCardCount;                 // 回合開始抽牌數
    
    // 建構函數
    public EnemyEntity(
        CharacterParameter[] characterParams,          // 角色參數
        int currentEnergy,                             // 當前能量
        int maxEnergy,                                 // 最大能量
        int handCardMaxCount,                         // 手牌上限
        int selectedCardMaxCount,                     // AI選卡上限
        int turnStartDrawCardCount,                   // 回合抽牌數
        int energyRecoverPoint,                       // 能量回復點
        IGameContextManager gameContext               // 遊戲上下文
    ) : base(Faction.Enemy, currentEnergy, maxEnergy)
    {
        _originPlayerInstanceGuid = Option.None<Guid>();  // 敵軍無原始實例
        _characters = characterParams
            .Select(param => CharacterEntity.Create(param))
            .ToList();
        _cardManager = new PlayerCardManager(handCardMaxCount);
        SelectedCards = new SelectedCardEntity(selectedCardMaxCount, new List<ICardEntity>());
        TurnStartDrawCardCount = turnStartDrawCardCount;
        EnergyRecoverPoint = energyRecoverPoint;
    }
}
```

### AI決策系統

#### 🤖 AI選卡邏輯
```csharp
public bool TryGetRecommandSelectCard(IGameplayModel gameplayWatcher, out ICardEntity cardEntity)
{
    if (UseCardLogic.TryGetRecommandSelectCard(gameplayWatcher, this, out cardEntity))
    {
        return SelectedCards.TryAddCard(cardEntity);
    }
    
    cardEntity = null;
    return false;
}
```

**選卡特色**：
- **策略分析**: 基於戰場狀況進行卡片選擇
- **數量限制**: 透過SelectedCardMaxCount限制選卡數量
- **失敗處理**: 選卡失敗時安全返回

#### 🎯 AI出牌邏輯
```csharp
public bool TryGetNextUseCardAction(IGameplayModel gameplayWatcher, out UseCardAction useCardAction)
{
    if (UseCardLogic.TryGetNextUseCardAction(gameplayWatcher, this, out useCardAction))
    {
        var cardIdentity = useCardAction.CardIndentity;
        return CardManager.GetCard(card => card.Identity == cardIdentity)
            .Map(card => SelectedCards.RemoveCard(card))
            .ValueOr(false);
    }

    useCardAction = null;
    return false;
}
```

**出牌特色**：
- **動態決策**: 根據當前狀況選擇最優行動
- **卡片管理**: 出牌後自動從選卡集合中移除
- **安全處理**: 使用Option模式避免空引用

#### 📊 AI資源管理
```csharp
public int EnergyRecoverPoint;          // 能量回復速度
public int TurnStartDrawCardCount;      // 手牌補充速度
```

**平衡機制**：
- **能量控制**: 調整AI的行動頻率
- **手牌管理**: 控制AI的選擇多樣性
- **難度調整**: 透過資源配置實現不同難度

### 🔄 敵軍克隆功能
```csharp
public EnemyEntity Clone(IGameContextManager gameContext)
{
    var cloneEnemy = new EnemyEntity(
        characterParams: _characters
            .Select(c => new CharacterParameter
            {
                NameKey         = c.NameKey,
                CurrentHealth   = c.CurrentHealth,
                MaxHealth       = c.MaxHealth
            })
            .ToArray(),
        currentEnergy: CurrentEnergy,
        maxEnergy: MaxEnergy,
        handCardMaxCount: _cardManager.HandCard.MaxCount,
        selectedCardMaxCount: 0,                           // 重置選卡
        turnStartDrawCardCount: TurnStartDrawCardCount,
        energyRecoverPoint: EnergyRecoverPoint,
        gameContext: gameContext
    );

    cloneEnemy.SelectedCards = new SelectedCardEntity(SelectedCards.MaxCount, new List<ICardEntity>());
    // cloneEnemy._cardManager = new PlayerCardManager(); // TODO

    return cloneEnemy;
}
```

**克隆特點**：
- **AI狀態重置**: 選卡狀態歸零，避免AI狀態污染
- **配置保留**: 保留能量回復和抽牌配置
- **待完善**: 卡牌管理克隆仍需完善

## DummyPlayer 空值物件

### 實現
```csharp
public class DummyPlayer : PlayerEntity
{
    public DummyPlayer() : base(Faction.None, 0, 0)
    { }
}

public static IPlayerEntity DummyPlayer = new DummyPlayer();
```

**空值物件模式**：
- **避免null**: 提供安全的預設值
- **統一接口**: 實現相同的IPlayerEntity接口
- **無副作用**: 所有操作都是安全的空操作

## 擴展方法系統

### Buff屬性計算
```csharp
public static int GetPlayerBuffAdditionProperty(
    this IPlayerEntity player, TriggerContext triggerContext, PlayerBuffProperty targetProperty)
{
    var value = 0;
    foreach (var playerBuff in player.BuffManager.Buffs)
    {
        var triggerBuff = new PlayerBuffTrigger(playerBuff);
        var playerBuffTriggerContext = triggerContext with { Triggered = triggerBuff };
        foreach (var property in playerBuff.Properties)
        {
            if (property is IPlayerBuffIntegerPropertyEntity integerEntity &&
                property.Property == targetProperty)
            {
                value += integerEntity.Eval(playerBuffTriggerContext);
            }
        }
    }
    return value;
}
```

**功能特色**：
- **Buff整合**: 計算所有Buff提供的屬性加成
- **上下文傳遞**: 使用TriggerContext提供計算環境
- **類型安全**: 透過類型檢查確保計算正確

### 玩家查詢功能
```csharp
public static Option<IPlayerEntity> GetPlayer(this GameStatus status, Guid playerIdentity)
{
    return status.Ally.Identity == playerIdentity ?
        (status.Ally as IPlayerEntity).Some() :
        status.Enemy.Identity == playerIdentity ?
            (status.Enemy as IPlayerEntity).Some() :
            Option.None<IPlayerEntity>();
}
```

**查詢特色**：
- **全域查詢**: 根據ID查找任意玩家
- **安全返回**: 使用Option避免null返回
- **高效查詢**: 直接比較避免遍歷

## 設計模式應用

### 🎭 策略模式 (Strategy Pattern)
```csharp
// 不同的玩家類型有不同的行為策略
AllyEntity    // 好感度策略
EnemyEntity   // AI決策策略
```

### 🏗️ 工廠模式 (Factory Pattern)
- 透過建構函數參數配置不同類型的玩家
- CharacterEntity.Create()工廠方法創建角色

### 📋 組合模式 (Composite Pattern)
```csharp
// PlayerEntity組合多個管理器
_energyManager + _buffManager + _cardManager + _characters
```

### 🔄 原型模式 (Prototype Pattern)
```csharp
// 支援實體克隆
public AllyEntity Clone(IGameContextManager gameContext)
public EnemyEntity Clone(IGameContextManager gameContext)
```

### 🚫 空值物件模式 (Null Object Pattern)
```csharp
public static IPlayerEntity DummyPlayer = new DummyPlayer();
```

## 依賴關係

### 依賴的組件
- **🔗 Faction**: 陣營枚舉 *需要Faction_Enum.md*
- **🔗 CharacterEntity**: 角色實體 *需要CharacterEntity_Class.md*
- **🔗 EnergyManager**: 能量管理 *需要EnergyManager_Class.md*
- **🔗 PlayerBuffManager**: 玩家Buff管理 *需要PlayerBuffManager_Class.md*
- **🔗 PlayerCardManager**: 卡牌管理 *需要PlayerCardManager_Class.md*
- **🔗 DispositionManager**: 好感度管理 *需要DispositionManager_Class.md*
- **🔗 SelectedCardEntity**: AI選卡管理 *需要SelectedCardEntity_Class.md*
- **🔗 UseCardLogic**: AI決策邏輯 *需要UseCardLogic_Class.md*
- **🔗 TriggerContext**: 觸發上下文 *需要TriggerContext_Class.md*
- **🔗 GameEvent**: 遊戲事件 *需要GameEvent_System.md*
- **🔗 Optional**: 安全空值處理 *需要Optional_Library.md*

### 被依賴的組件
- **🔗 CardEntity**: 卡片實體查詢玩家信息 *參考CardEntity_Class.md*
- **🔗 GameplayModel**: 遊戲狀態管理 *需要GameplayModel_Class.md*
- **🔗 戰鬥系統**: 使用玩家實體進行戰鬥
- **🔗 UI系統**: 顯示玩家狀態信息

## 使用範例

### 友軍創建
```csharp
var allyEntity = new AllyEntity(
    originPlayerInstanceGuid: playerInstance.Guid,
    characterParams: new CharacterParameter[]
    {
        new CharacterParameter
        {
            NameKey = "hero.protagonist",
            CurrentHealth = 100,
            MaxHealth = 100
        }
    },
    currentEnergy: 1,
    maxEnergy: 3,
    handCardMaxCount: 7,
    currentDisposition: 5,
    maxDisposition: 10,
    gameContext: gameContext
);
```

### 敵軍創建
```csharp
var enemyEntity = new EnemyEntity(
    characterParams: new CharacterParameter[]
    {
        new CharacterParameter
        {
            NameKey = "enemy.bandit",
            CurrentHealth = 80,
            MaxHealth = 80
        }
    },
    currentEnergy: 1,
    maxEnergy: 2,
    handCardMaxCount: 5,
    selectedCardMaxCount: 2,
    turnStartDrawCardCount: 1,
    energyRecoverPoint: 1,
    gameContext: gameContext
);
```

### AI行為使用
```csharp
// AI選卡
if (enemy.TryGetRecommandSelectCard(gameplayModel, out var selectedCard))
{
    Debug.Log($"AI選擇了: {selectedCard.CardDataId}");
}

// AI出牌
if (enemy.TryGetNextUseCardAction(gameplayModel, out var useAction))
{
    Debug.Log($"AI使用: {useAction.CardIndentity}");
    // 執行出牌邏輯
}
```

### 系統更新
```csharp
// 更新玩家狀態
var updateEvent = player.Update(triggerContext);

if (updateEvent is GeneralUpdateEvent generalUpdate)
{
    // 處理更新事件
    ProcessPlayerBuffUpdates(generalUpdate.UpdatedPlayerBuffInfos);
    ProcessCharacterBuffUpdates(generalUpdate.UpdatedCharacterBuffInfos);
    ProcessCardUpdates(generalUpdate.UpdatedCardInfos);
}
```

### Buff計算
```csharp
// 計算Buff提供的能量加成
var energyBonus = player.GetPlayerBuffAdditionProperty(
    triggerContext, 
    PlayerBuffProperty.EnergyAddition
);

Debug.Log($"能量加成: +{energyBonus}");
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [PlayerEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerEntity.cs) | 核心 | 玩家實體完整實現 |
| [PlayerData.cs](../../Assets/Scripts/GameData/PlayerData.cs) | 依賴 | 提供初始化資料 |
| [AllyData.cs](../../Assets/Scripts/GameData/AllyData.cs) | 依賴 | 友軍配置資料 |
| [EnemyData.cs](../../Assets/Scripts/GameData/EnemyData.cs) | 依賴 | 敵軍配置資料 |
| [CardEntity.cs](../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs) | 被依賴 | 使用玩家信息 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成