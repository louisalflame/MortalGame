# PlayerBuff 玩家Buff系統筆記

## 系統概述
PlayerBuff系統是作用於Player身上的狀態效果系統，與CardBuff和CharacterBuff形成完整的Buff體系。PlayerBuff直接影響玩家層級的屬性和行為，如能量獲得、抽牌數量、全域效果增強等。該系統採用Data-Entity-Manager三層架構，支援複雜的屬性修正、反應會話、生命週期管理和層數疊加機制。

**核心設計理念**：
- **玩家層級影響**：直接作用於Player而非具體的Card或Character
- **全域效果**：影響玩家的整體戰略和資源管理
- **架構統一**：與CardBuff/CharacterBuff保持一致的設計模式
- **層數疊加**：支援相同Buff的層數累積
- **來源追蹤**：記錄Buff的施放來源

**檔案位置**: 
- [PlayerBuffData.cs](../../Assets/Scripts/GameData/PlayerBuff/PlayerBuffData.cs)
- [PlayerBuffEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerBuff/PlayerBuffEntity.cs)
- [PlayerBuffManager.cs](../../Assets/Scripts/GameModel/Entity/PlayerBuff/PlayerBuffManager.cs)

## 三種Buff系統對比

### 設計理念對比
| 特性 | CardBuff | CharacterBuff | PlayerBuff |
|------|----------|---------------|------------|
| **作用目標** | 單張卡片 | 單個角色 | 整個玩家 |
| **影響範圍** | 卡片屬性、效果 | 角色血量、護甲 | 全域資源、策略 |
| **典型效果** | 攻擊力+2、消耗-1 | 中毒、護甲、虛弱 | 能量回復+1、抽牌+1 |
| **生命週期** | 跟隨卡片 | 跟隨角色 | 跟隨玩家 |
| **施術者** | 通常無 | 記錄施術者 | 記錄施術者 |
| **管理器** | CardBuffManager | CharacterBuffManager | PlayerBuffManager |

### 應用場景對比
```csharp
// CardBuff 範例：增強特定卡片
"此卡片攻擊力+3"
"此卡片消耗-1"

// CharacterBuff 範例：角色狀態
"中毒：每回合結束受到3點傷害"
"護甲：增加5點護甲值"

// PlayerBuff 範例：玩家全域效果
"能量大師：每回合額外獲得1點能量"
"抽牌專家：每回合開始多抽1張牌"
"全域強化：所有卡片攻擊力+1"
```

## PlayerBuffData 資料定義

### 核心結構
```csharp
public class PlayerBuffData
{
    [TitleGroup("BasicData")]
    public string ID;               // Buff唯一標識
    public int MaxLevel;            // 最大疊加層數
    
    [ShowInInspector]
    [BoxGroup("Effects")]
    public Dictionary<string, IReactionSessionData> Sessions;       // 反應會話系統
    
    [BoxGroup("Effects")]
    public Dictionary<GameTiming, ConditionalPlayerBuffEffect[]> BuffEffects; // 時機效果
    
    [BoxGroup("Properties")]
    public List<IPlayerBuffPropertyData> PropertyDatas;            // 屬性修正
    
    [BoxGroup("LifeTime")]
    public IPlayerBuffLifeTimeData LifeTimeData;                  // 生命週期
}
```

### 設計特色分析

#### 🆔 基礎資料系統
```csharp
[TitleGroup("BasicData")]
public string ID;           // 如 "energy_master", "card_draw_bonus"
public int MaxLevel;        // 如 3（最多疊加3層）
```

**標識特色**：
- **語義命名**：ID使用描述性名稱，便於理解Buff功能
- **層數控制**：MaxLevel防止某些強力Buff無限疊加
- **編輯友好**：Odin Inspector的TitleGroup提供清晰分類

#### 🔄 反應會話系統
```csharp
[ShowInInspector]
[BoxGroup("Effects")]
public Dictionary<string, IReactionSessionData> Sessions;
```

**會話特色**：
- **狀態維護**：維護Buff相關的動態狀態
- **數值追蹤**：記錄會話相關的累積數值
- **互動支援**：支援與其他系統的複雜互動
- **動態響應**：根據遊戲事件動態調整會話狀態

#### ⏰ 時機效果系統
```csharp
[BoxGroup("Effects")]
public Dictionary<GameTiming, ConditionalPlayerBuffEffect[]> BuffEffects;
```

**時機效果特色**：
- **精確觸發**：基於GameTiming的精確時機控制
- **條件邏輯**：ConditionalPlayerBuffEffect支援條件判斷
- **多效果支援**：每個時機可配置多個效果
- **靈活組合**：不同時機的效果可以協同作用

**可能的觸發時機**：
```csharp
// 推測的PlayerBuff觸發時機
GameTiming.TurnStart        // 回合開始：能量回復、抽牌
GameTiming.TurnEnd          // 回合結束：效果結算、持續傷害
GameTiming.CardPlayed       // 卡片使用：觸發額外效果
GameTiming.EnergyGained     // 獲得能量：增強能量獲得
GameTiming.DamageTaken      // 受到傷害：傷害減免、反擊
GameTiming.BuffAdded        // 添加Buff：Buff增強效果
```

#### 📊 屬性修正系統
```csharp
[BoxGroup("Properties")]
public List<IPlayerBuffPropertyData> PropertyDatas;
```

**屬性修正特色**：
- **全域影響**：修改玩家的全域屬性
- **累積計算**：多個Buff的屬性修正可以累積
- **即時生效**：屬性變化立即影響遊戲狀態
- **類型豐富**：支援多種不同的屬性類型

**可能的屬性類型**：
```csharp
// 推測的PlayerBuff屬性類型
EnergyGainProperty         // 能量獲得加成
CardDrawProperty           // 抽牌數量加成
DamageReductionProperty    // 傷害減免
HealingBonusProperty       // 治療效果加成
CardCostReductionProperty  // 卡片消耗減少
MaxEnergyProperty          // 最大能量增加
HandSizeProperty           // 手牌上限增加
```

#### ⏳ 生命週期管理
```csharp
[BoxGroup("LifeTime")]
public IPlayerBuffLifeTimeData LifeTimeData;
```

**生命週期特色**：
- **持續時間**：控制Buff的存在時間
- **條件過期**：基於條件的過期邏輯
- **自動清理**：過期Buff的自動移除
- **策略配置**：支援不同的生命週期策略

---

## PlayerBuffManager 管理器系統

### 接口定義
```csharp
public interface IPlayerBuffManager
{
    // 狀態查詢
    IReadOnlyCollection<IPlayerBuffEntity> Buffs { get; }
    
    // Buff操作
    AddPlayerBuffResult AddBuff(
        TriggerContext triggerContext,
        string buffId, 
        int level);
    RemovePlayerBuffResult RemoveBuff(
        TriggerContext triggerContext,
        string buffId);
    
    // 系統更新
    IEnumerable<IPlayerBuffEntity> Update(TriggerContext triggerContext);
}
```

### 核心實現
```csharp
public class PlayerBuffManager : IPlayerBuffManager
{
    private List<IPlayerBuffEntity> _buffs;

    public IReadOnlyCollection<IPlayerBuffEntity> Buffs => _buffs;

    public PlayerBuffManager()
    {
        _buffs = new List<IPlayerBuffEntity>();
    }
}
```

### PlayerBuffManager 獨特設計

#### 📊 專用Result類型
```csharp
public AddPlayerBuffResult AddBuff(
    TriggerContext triggerContext,
    string buffId, 
    int level)
{
    // 返回專用的AddPlayerBuffResult
    return new AddPlayerBuffResult(
        IsNewBuff: true,
        Buff: resultBuff,
        DeltaLevel: level
    );
}

public RemovePlayerBuffResult RemoveBuff(
    TriggerContext triggerContext,
    string buffId)
{
    // 返回專用的RemovePlayerBuffResult
    return new RemovePlayerBuffResult(
        Buffs: new List<IPlayerBuffEntity> { existBuff }
    );
}
```

**Result類型特色**：
- **結構化返回**：提供結構化的操作結果信息
- **詳細資訊**：包含操作狀態、影響的Buff和變化量
- **類型安全**：避免使用out參數，提高代碼可讀性
- **一致性**：與CardBuffManager使用相同的Result模式

#### 🔧 內建過期清理功能
```csharp
// TODO: 待實現的專用過期清理方法
public RemovePlayerBuffResult RemoveExpiredBuff(
    TriggerContext triggerContext)
{
    var expiredBuffs = new List<IPlayerBuffEntity>();
    foreach (var existBuff in _buffs)
    {
        if (existBuff.IsExpired())
        {
            expiredBuffs.Add(existBuff);
            _buffs.Remove(existBuff);
        }
    }

    return new RemovePlayerBuffResult(Buffs: expiredBuffs);
}
```

**過期清理特色**：
- **專門方法**：獨立的過期Buff清理邏輯
- **批量處理**：一次性處理所有過期的Buff
- **結果追蹤**：返回被移除的Buff列表
- **性能優化**：避免在Update中進行頻繁的過期檢查

#### 🔄 IEnumerable更新返回
```csharp
public IEnumerable<IPlayerBuffEntity> Update(TriggerContext triggerContext)
{
    foreach (var buff in _buffs.ToList())
    {
        var isUpdated = false;
        var triggerBuff = new PlayerBuffTrigger(buff);
        var updateBuffTriggerContext = triggerContext with { Triggered = triggerBuff };

        foreach (var session in buff.ReactionSessions.Values)
        {
            isUpdated |= session.Update(updateBuffTriggerContext);
        }

        isUpdated |= buff.LifeTime.Update(updateBuffTriggerContext);

        if (isUpdated)
        {
            yield return buff;
        }
    }
}
```

**Update特色**：
- **精確追蹤**：只返回實際發生變化的PlayerBuff
- **延遲執行**：使用yield return提供記憶體效率
- **完整更新**：同時更新ReactionSession和LifeTime
- **觸發上下文**：為每個Buff創建專門的觸發上下文

### PlayerBuff來源追蹤系統

#### 複雜來源解析
```csharp
var caster = triggerContext.Action.Source switch
{
    PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
    CardPlaySource cardPlaySource => cardPlaySource.Card.Owner(triggerContext.Model),
    _ => Option.None<IPlayerEntity>()
};
```

**來源追蹤特色**：
- **多層追蹤**：PlayerBuff可以由其他PlayerBuff觸發
- **卡片觸發**：卡片效果可以給玩家添加Buff
- **原始施術者**：始終追蹤到最初的施術者
- **鏈式責任**：支援Buff效果的責任鏈追蹤

#### BuffLibrary整合訪問
```csharp
var buffLibrary = triggerContext.Model.ContextManager.PlayerBuffLibrary;
var resultBuff = new PlayerBuffEntity(
    buffId, 
    Guid.NewGuid(), 
    level,
    caster,
    buffLibrary.GetBuffProperties(buffId)
        .Select(p => p.CreateEntity(triggerContext)),
    buffLibrary.GetBuffLifeTime(buffId)
        .CreateEntity(triggerContext),
    buffLibrary.GetBuffSessions(buffId)
        .ToDictionary(
            kvp => kvp.Key, 
            kvp => kvp.Value.CreateEntity(triggerContext)));
```

**BuffLibrary特色**：
- **統一訪問**：從ContextManager統一獲取PlayerBuffLibrary
- **即時創建**：將Data層對象即時轉換為Entity層對象
- **完整初始化**：確保所有組件都正確初始化
- **資源整合**：與遊戲資源管理系統深度整合

### 三大BuffManager終極對比

| 特性維度 | CardBuffManager | CharacterBuffManager | PlayerBuffManager |
|---------|-----------------|---------------------|-------------------|
| **🏗️ 初始化策略** | 支援初始Buff集合 | 空列表起始 | 空列表起始 |
| **📚 Library獲取** | 外部傳入參數 | ContextManager內建 | ContextManager內建 |
| **📤 Add方法返回** | AddCardBuffResult | bool + out參數 | AddPlayerBuffResult |
| **📤 Remove方法返回** | RemoveCardBuffResult | bool + out參數 | RemovePlayerBuffResult |
| **🔄 Update返回** | bool（有無變化） | IEnumerable<變化Buff> | IEnumerable<變化Buff> |
| **🎯 目標解析** | 卡片自身 | Action→CharacterTarget | Action→PlayerTarget |
| **⏰ 過期處理** | Update中自動 | Update中自動 | 專用RemoveExpiredBuff |
| **📋 主要用途** | 卡片屬性增強 | 角色狀態效果 | 玩家全域策略 |
| **🔗 依賴關係** | 相對獨立 | 與角色生命週期綁定 | 與玩家回合週期綁定 |

### PlayerBuffManager 使用範例

#### 給玩家添加能量掌控Buff
```csharp
var playerBuffManager = player.BuffManager;

var addResult = playerBuffManager.AddBuff(
    triggerContext,
    "energy_mastery",   // 能量掌控
    2                   // 2層效果
);

if (addResult.IsNewBuff)
{
    Debug.Log($"玩家獲得能量掌控：{addResult.Buff.Level}層");
    
    // 觸發獲得Buff的額外效果
    TriggerBuffGainedEffect(player, addResult.Buff, triggerContext);
}
else
{
    Debug.Log($"能量掌控強化：+{addResult.DeltaLevel}層，總計{addResult.Buff.Level}層");
}
```

#### 回合開始時的PlayerBuff處理
```csharp
// 每回合開始時更新玩家的所有Buff
var turnStartContext = CreateTurnStartContext(currentPlayer);

var updatedBuffs = currentPlayer.BuffManager.Update(turnStartContext);

foreach (var updatedBuff in updatedBuffs)
{
    Debug.Log($"PlayerBuff更新：{updatedBuff.PlayerBuffDataId} - {updatedBuff.Level}層");
    
    // 根據Buff類型處理特定效果
    switch (updatedBuff.PlayerBuffDataId)
    {
        case "extra_card_draw":
            // 額外抽牌效果
            HandleExtraCardDraw(currentPlayer, updatedBuff.Level);
            break;
            
        case "energy_bonus":
            // 能量加成效果
            HandleEnergyBonus(currentPlayer, updatedBuff.Level);
            break;
            
        case "damage_amplifier":
            // 傷害放大效果
            HandleDamageAmplifier(currentPlayer, updatedBuff.Level);
            break;
    }
}
```

#### 戰鬥結束時的Buff清理
```csharp
// 戰鬥結束時移除臨時性PlayerBuff
var battleEndContext = CreateBattleEndContext();

var expiredResult = player.BuffManager.RemoveExpiredBuff(battleEndContext);

if (expiredResult.Buffs.Any())
{
    Debug.Log($"戰鬥結束，移除{expiredResult.Buffs.Count()}個過期Buff");
    
    foreach (var expiredBuff in expiredResult.Buffs)
    {
        Debug.Log($"移除Buff：{expiredBuff.PlayerBuffDataId}");
        
        // 觸發移除Buff的收尾效果
        TriggerBuffExpiredEffect(player, expiredBuff, battleEndContext);
    }
}
```

#### 條件移除特定Buff
```csharp
// 使用淨化卡牌移除負面PlayerBuff
public void UsePurificationCard(IPlayerEntity player, TriggerContext context)
{
    var removeResult = player.BuffManager.RemoveBuff(
        context,
        "curse_of_weakness"  // 虛弱詛咒
    );
    
    if (removeResult.Buffs.Any())
    {
        var removedBuff = removeResult.Buffs.First();
        Debug.Log($"淨化成功：移除{removedBuff.PlayerBuffDataId}");
        
        // 淨化成功後的獎勵效果
        player.EnergyManager.GainEnergy(2); // 獲得2點能量
        player.DrawCard(1);                 // 抽1張牌
    }
    else
    {
        Debug.Log("沒有找到可淨化的負面效果");
    }
}
```

#### PlayerBuff的全域效果計算
```csharp
// 計算PlayerBuff對卡牌費用的影響
public int CalculateCardCost(ICardEntity card, IPlayerEntity player, TriggerContext context)
{
    int baseCost = card.BaseCost;
    int playerBuffModifier = 0;
    
    foreach (var playerBuff in player.BuffManager.Buffs)
    {
        foreach (var property in playerBuff.Properties)
        {
            if (property is CardCostReductionProperty costProperty)
            {
                // 檢查是否適用於此卡片
                if (costProperty.AppliesTo(card, context))
                {
                    playerBuffModifier -= costProperty.GetReduction(playerBuff.Level, context);
                }
            }
        }
    }
    
    return Math.Max(0, baseCost + playerBuffModifier); // 費用不能低於0
}
```

#### PlayerBuff觸發其他系統的連鎖效果
```csharp
// PlayerBuff觸發CardBuff的連鎖效果
public void ProcessPlayerBuffCardEffect(IPlayerEntity player, ICardEntity targetCard, TriggerContext context)
{
    foreach (var playerBuff in player.BuffManager.Buffs)
    {
        // 檢查PlayerBuff是否有對卡片施加Buff的效果
        if (playerBuff.PlayerBuffDataId == "card_enhancer")
        {
            var enhanceLevel = playerBuff.Level;
            
            // 給目標卡片添加增強Buff
            targetCard.BuffManager.AddBuff(
                context.Model.ContextManager.CardBuffLibrary,
                context,
                "power_boost",
                enhanceLevel
            );
            
            Debug.Log($"玩家Buff觸發：卡片獲得{enhanceLevel}層強化");
        }
    }
}
```

---

## PlayerBuffEntity 實體狀態

### 接口定義
```csharp
public interface IPlayerBuffEntity
{
    // 基礎資訊
    string PlayerBuffDataId { get; }        // Buff類型標識
    Guid Identity { get; }                  // 實體唯一標識
    int Level { get; }                      // 當前層數
    Option<IPlayerEntity> Caster { get; }   // 施術者
    
    // 管理組件
    IReadOnlyCollection<IPlayerBuffPropertyEntity> Properties { get; }
    IPlayerBuffLifeTimeEntity LifeTime { get; }
    IReadOnlyDictionary<string, IReactionSessionEntity> ReactionSessions { get; }
    
    // 狀態操作
    bool IsExpired();                       // 過期檢查
    void AddLevel(int level);               // 增加層數
}
```

### 實體實現
```csharp
public class PlayerBuffEntity : IPlayerBuffEntity
{
    // 核心欄位
    private readonly string _playerBuffDataId;
    private readonly Guid _identity;
    private int _level;
    private readonly Option<IPlayerEntity> _caster;
    private readonly IReadOnlyList<IPlayerBuffPropertyEntity> _properties;
    private readonly IPlayerBuffLifeTimeEntity _lifeTime;
    private readonly IReadOnlyDictionary<string, IReactionSessionEntity> _reactionSessions;
    
    // 空值物件支援
    public bool IsDummy => this == DummyBuff;
    public static IPlayerBuffEntity DummyBuff = new DummyPlayerBuff();
}
```

### 核心功能特色

#### 🆔 身份管理
```csharp
string PlayerBuffDataId { get; }    // Buff類型：如 "energy_master"
Guid Identity { get; }              // 實體標識：區分同類型的不同實例
```

**身份管理優勢**：
- **類型識別**：PlayerBuffDataId用於識別Buff的具體類型
- **實例區分**：Identity確保每個Buff實體的唯一性
- **查詢便利**：便於在管理器中查找和操作特定Buff

#### 📊 層數疊加
```csharp
int Level { get; }                  // 當前層數
void AddLevel(int level);           // 增加層數

// 使用範例
public void AddLevel(int level)
{
    _level += level;
    // 層數變化可能觸發屬性重新計算
    // 可能需要通知UI更新顯示
}
```

**層數管理特色**：
- **疊加效果**：相同Buff可以疊加增強效果
- **上限保護**：配合MaxLevel防止過度疊加
- **線性增長**：大多數效果隨層數線性增長
- **策略深度**：層數機制增加策略選擇

#### 👤 施術者系統
```csharp
Option<IPlayerEntity> Caster { get; }
```

**施術者特色**：
- **來源追蹤**：記錄施放Buff的玩家
- **互動邏輯**：某些效果可能與施術者相關
- **歸屬識別**：便於確定Buff的歸屬關係
- **安全處理**：使用Option處理無施術者的情況

### 空值物件系統

#### DummyPlayerBuff 實現
```csharp
public class DummyPlayerBuff : PlayerBuffEntity
{
    public DummyPlayerBuff() : base(
        string.Empty,                           // PlayerBuffDataId
        Guid.Empty,                            // Identity
        1,                                     // Level
        Option.None<IPlayerEntity>(),          // Caster
        Enumerable.Empty<IPlayerBuffPropertyEntity>(),  // Properties
        new AlwaysLifeTimePlayerBuffEntity(),          // LifeTime
        new Dictionary<string, IReactionSessionEntity>() // Sessions
    ) { }
}

public static IPlayerBuffEntity DummyBuff = new DummyPlayerBuff();
```

**空值物件特色**：
- **永不過期**：AlwaysLifeTimePlayerBuffEntity確保不會意外過期
- **無副作用**：所有操作都是安全的空操作
- **接口統一**：實現相同的IPlayerBuffEntity接口
- **調試友好**：便於識別和處理錯誤狀態

## PlayerBuffEntity 擴展方法

### 資訊轉換系統
```csharp
public static PlayerBuffInfo ToInfo(this IPlayerBuffEntity playerBuff, IGameplayModel gameWatcher)
{
    return new PlayerBuffInfo(
        playerBuff.PlayerBuffDataId,
        playerBuff.Identity,
        playerBuff.Level,
        playerBuff.ReactionSessions
            .Where(kvp => kvp.Value.IntegerValue.HasValue)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.IntegerValue.ValueOr(0))
    );
}
```

**轉換特色**：
- **UI友好**：轉換為適合UI顯示的資料結構
- **會話過濾**：只包含有數值的會話資訊
- **安全處理**：使用ValueOr處理可能的空值
- **資料簡化**：提供簡化的數據視圖

### 所有權查詢系統
```csharp
public static Option<IPlayerEntity> Owner(this IPlayerBuffEntity playerBuffEntity, IGameplayModel watcher)
{
    // 檢查友軍
    if (watcher.GameStatus.Ally.BuffManager.Buffs.Contains(playerBuffEntity))
        return (watcher.GameStatus.Ally as IPlayerEntity).Some();
    
    // 檢查敵軍
    if (watcher.GameStatus.Enemy.BuffManager.Buffs.Contains(playerBuffEntity))
        return (watcher.GameStatus.Enemy as IPlayerEntity).Some();
    
    return Option.None<IPlayerEntity>();
}
```

**查詢特色**：
- **全域搜索**：跨友軍和敵軍查找Buff的歸屬
- **陣營識別**：確定Buff屬於哪個陣營
- **安全返回**：使用Option處理未找到的情況
- **效率優化**：優先查找友軍，再查找敵軍

### 會話數據訪問
```csharp
// 布林值會話
public static Option<bool> GetSessionBoolean(
    this IPlayerBuffEntity playerBuffEntity,
    string key)
{
    if (playerBuffEntity.ReactionSessions.TryGetValue(key, out var session))
    {
        return session.BooleanValue;
    }
    return Option.None<bool>();
}

// 整數值會話
public static Option<int> GetSessionInteger(
    this IPlayerBuffEntity playerBuffEntity,
    string key)
{
    if (playerBuffEntity.ReactionSessions.TryGetValue(key, out var session))
    {
        return session.IntegerValue;
    }
    return Option.None<int>();
}
```

**會話訪問特色**：
- **類型安全**：分別處理不同類型的會話值
- **便捷訪問**：提供簡化的會話數據訪問方法
- **安全處理**：使用Option處理不存在的會話
- **擴展性**：可以輕鬆添加其他類型的會話訪問

## PlayerBuffManager 管理器

### 接口定義
```csharp
public interface IPlayerBuffManager
{
    // 狀態查詢
    IReadOnlyCollection<IPlayerBuffEntity> Buffs { get; }
    
    // Buff操作
    AddPlayerBuffResult AddBuff(TriggerContext triggerContext, string buffId, int level);
    RemovePlayerBuffResult RemoveBuff(TriggerContext triggerContext, string buffId);
    
    // 系統更新
    IEnumerable<IPlayerBuffEntity> Update(TriggerContext triggerContext);
}
```

### 核心實現
```csharp
public class PlayerBuffManager : IPlayerBuffManager
{
    private List<IPlayerBuffEntity> _buffs;

    public IReadOnlyCollection<IPlayerBuffEntity> Buffs => _buffs;

    public PlayerBuffManager()
    {
        _buffs = new List<IPlayerBuffEntity>();
    }
}
```

### Buff添加機制

#### AddBuff實現
```csharp
public AddPlayerBuffResult AddBuff(
    TriggerContext triggerContext,
    string buffId, 
    int level)
{
    // 檢查是否已存在相同Buff
    foreach (var existBuff in _buffs)
    {
        if (existBuff.PlayerBuffDataId == buffId)
        {
            existBuff.AddLevel(level);
            return new AddPlayerBuffResult(
                IsNewBuff: false,
                Buff: existBuff,
                DeltaLevel: level
            );
        }
    }

    // 創建新Buff
    var caster = triggerContext.Action.Source switch
    {
        PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
        CardPlaySource cardPlaySource => cardPlaySource.Card.Owner(triggerContext.Model),
        _ => Option.None<IPlayerEntity>()
    };

    var buffLibrary = triggerContext.Model.ContextManager.PlayerBuffLibrary;
    var resultBuff = new PlayerBuffEntity(
        buffId, 
        Guid.NewGuid(), 
        level,
        caster,
        buffLibrary.GetBuffProperties(buffId)
            .Select(p => p.CreateEntity(triggerContext)),
        buffLibrary.GetBuffLifeTime(buffId)
            .CreateEntity(triggerContext),
        buffLibrary.GetBuffSessions(buffId)
            .ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value.CreateEntity(triggerContext)));
    
    _buffs.Add(resultBuff);
    return new AddPlayerBuffResult(
        IsNewBuff: true,
        Buff: resultBuff,
        DeltaLevel: level
    );
}
```

**添加機制特色**：
- **疊加優先**：相同ID的Buff會疊加而不是創建新實例
- **來源追蹤**：根據TriggerContext追蹤Buff的來源
- **工廠創建**：通過BuffLibrary創建完整的Buff實體
- **結果詳細**：返回詳細的添加結果資訊

#### 來源追蹤邏輯
```csharp
var caster = triggerContext.Action.Source switch
{
    PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
    CardPlaySource cardPlaySource => cardPlaySource.Card.Owner(triggerContext.Model),
    _ => Option.None<IPlayerEntity>()
};
```

**來源追蹤特色**：
- **多來源支援**：支援PlayerBuff、卡片等多種來源
- **鏈式追蹤**：PlayerBuff來源會追蹤到原始施術者
- **卡片來源**：卡片使用時追蹤到卡片擁有者
- **預設處理**：無法確定來源時安全處理

### Buff移除機制

#### RemoveBuff實現
```csharp
public RemovePlayerBuffResult RemoveBuff(
    TriggerContext triggerContext,
    string buffId)
{
    foreach (var existBuff in _buffs)
    {
        if (existBuff.PlayerBuffDataId == buffId)
        {
            _buffs.Remove(existBuff);
            return new RemovePlayerBuffResult(
                Buffs: new List<IPlayerBuffEntity> { existBuff }
            );
        }
    }

    return new RemovePlayerBuffResult(
        Buffs: Array.Empty<IPlayerBuffEntity>()
    );   
}
```

**移除特色**：
- **精確匹配**：根據BuffId精確移除對應Buff
- **結果記錄**：返回被移除的Buff列表
- **安全處理**：未找到時返回空列表而不拋出異常
- **單一移除**：每次只移除一個匹配的Buff

#### 過期Buff清理
```csharp
// TODO
public RemovePlayerBuffResult RemoveExpiredBuff(
    TriggerContext triggerContext)
{
    var expiredBuffs = new List<IPlayerBuffEntity>();
    foreach (var existBuff in _buffs)
    {
        if (existBuff.IsExpired())
        {
            expiredBuffs.Add(existBuff);
            _buffs.Remove(existBuff);
        }
    }

    return new RemovePlayerBuffResult(
        Buffs: expiredBuffs
    );
}
```

**清理特色**：
- **批量處理**：一次性清理所有過期Buff
- **過期檢查**：使用IsExpired()判斷Buff是否過期
- **結果統計**：返回所有被清理的Buff
- **TODO標記**：表示功能可能還需要完善

### Buff更新機制

#### Update實現
```csharp
public IEnumerable<IPlayerBuffEntity> Update(TriggerContext triggerContext)
{
    foreach (var buff in _buffs.ToList())
    {
        var isUpdated = false;
        var triggerBuff = new PlayerBuffTrigger(buff);
        var updateBuffTriggerContext = triggerContext with { Triggered = triggerBuff };

        // 更新會話
        foreach (var session in buff.ReactionSessions.Values)
        {
            isUpdated |= session.Update(updateBuffTriggerContext);
        }

        // 更新生命週期
        isUpdated |= buff.LifeTime.Update(updateBuffTriggerContext);

        // 返回有更新的Buff
        if (isUpdated)
        {
            yield return buff;
        }
    }
}
```

**更新機制特色**：
- **上下文傳遞**：創建專門的PlayerBuffTrigger上下文
- **會話更新**：更新所有反應會話的狀態
- **生命週期更新**：更新Buff的生命週期狀態
- **變化追蹤**：只返回有變化的Buff，提高效率

## 結果記錄系統

### AddPlayerBuffResult 添加結果
```csharp
// 推測的結果結構
public record AddPlayerBuffResult(
    bool IsNewBuff,                 // 是否為新Buff
    IPlayerBuffEntity Buff,         // 相關的Buff實體
    int DeltaLevel                  // 層數變化
);
```

### RemovePlayerBuffResult 移除結果
```csharp
// 推測的結果結構
public record RemovePlayerBuffResult(
    IReadOnlyList<IPlayerBuffEntity> Buffs    // 被移除的Buff列表
);
```

**結果記錄優勢**：
- **操作追蹤**：詳細記錄每次操作的結果
- **UI支援**：UI可以根據結果顯示適當的反饋
- **日誌友好**：便於生成詳細的操作日誌
- **調試支援**：幫助開發者理解Buff操作的結果

## 使用範例

### Buff添加操作
```csharp
// 添加能量大師Buff
var addResult = player.BuffManager.AddBuff(
    triggerContext, 
    "energy_master", 
    2  // 添加2層
);

if (addResult.IsNewBuff)
{
    Debug.Log($"新增Buff: {addResult.Buff.PlayerBuffDataId}");
}
else
{
    Debug.Log($"Buff疊加: +{addResult.DeltaLevel} 層");
}

// 顯示當前層數
Debug.Log($"當前層數: {addResult.Buff.Level}");
```

### Buff查詢和管理
```csharp
// 查詢特定Buff
var energyBuff = player.BuffManager.Buffs
    .FirstOrDefault(b => b.PlayerBuffDataId == "energy_master");

if (energyBuff != null)
{
    Debug.Log($"能量大師等級: {energyBuff.Level}");
    
    // 檢查會話數據
    var bonusEnergy = energyBuff.GetSessionInteger("bonus_energy");
    if (bonusEnergy.HasValue)
    {
        Debug.Log($"額外能量獲得: {bonusEnergy.Value}");
    }
}

// 查看所有Buff
foreach (var buff in player.BuffManager.Buffs)
{
    var buffInfo = buff.ToInfo(gameplayModel);
    Debug.Log($"Buff: {buffInfo.PlayerBuffDataId}, 層數: {buffInfo.Level}");
}
```

### 能量增益效果應用
```csharp
// 計算PlayerBuff提供的能量加成
public int GetEnergyGainBonus(IPlayerEntity player, TriggerContext context)
{
    int bonus = 0;
    
    foreach (var buff in player.BuffManager.Buffs)
    {
        foreach (var property in buff.Properties)
        {
            if (property is EnergyGainProperty energyProperty)
            {
                bonus += energyProperty.GetBonus(buff.Level, context);
            }
        }
    }
    
    return bonus;
}

// 回合開始時應用能量加成
public void StartTurnWithBuffs(IPlayerEntity player, TriggerContext context)
{
    // 基礎能量回復
    int baseRecover = 1;
    
    // 計算Buff加成
    int buffBonus = GetEnergyGainBonus(player, context);
    
    // 應用總能量回復
    var result = player.EnergyManager.RecoverEnergy(baseRecover + buffBonus);
    
    Debug.Log($"回合開始：基礎回復 {baseRecover}，Buff加成 {buffBonus}，實際獲得 {result.DeltaEp}");
}
```

### Buff更新和清理
```csharp
// 回合結束更新所有Buff
public void EndTurnUpdateBuffs(IPlayerEntity player, TriggerContext context)
{
    // 更新所有PlayerBuff
    var updatedBuffs = player.BuffManager.Update(context).ToList();
    
    if (updatedBuffs.Any())
    {
        Debug.Log($"更新了 {updatedBuffs.Count} 個PlayerBuff");
        
        // 通知UI更新
        foreach (var buff in updatedBuffs)
        {
            var buffInfo = buff.ToInfo(gameplayModel);
            UI.UpdateBuffDisplay(buffInfo);
        }
    }
    
    // 清理過期Buff
    var removeResult = player.BuffManager.RemoveExpiredBuff(context);
    if (removeResult.Buffs.Any())
    {
        Debug.Log($"清理了 {removeResult.Buffs.Count} 個過期Buff");
    }
}
```

### 屬性計算整合
```csharp
// 計算玩家的總體屬性加成
public PlayerModifiers CalculatePlayerModifiers(IPlayerEntity player, TriggerContext context)
{
    var modifiers = new PlayerModifiers();
    
    foreach (var buff in player.BuffManager.Buffs)
    {
        foreach (var property in buff.Properties)
        {
            switch (property)
            {
                case EnergyGainProperty energyProp:
                    modifiers.EnergyGainBonus += energyProp.GetBonus(buff.Level, context);
                    break;
                    
                case CardDrawProperty drawProp:
                    modifiers.CardDrawBonus += drawProp.GetBonus(buff.Level, context);
                    break;
                    
                case DamageReductionProperty defenseProp:
                    modifiers.DamageReduction += defenseProp.GetReduction(buff.Level, context);
                    break;
                    
                case CardCostReductionProperty costProp:
                    modifiers.CardCostReduction += costProp.GetReduction(buff.Level, context);
                    break;
            }
        }
    }
    
    return modifiers;
}
```

## 高級應用場景

### 玩家Buff互動系統
```csharp
public class PlayerBuffInteractionHandler
{
    // 處理Buff間的相互作用
    public static void HandleBuffInteraction(
        IPlayerBuffEntity newBuff, 
        IPlayerEntity player, 
        TriggerContext context)
    {
        foreach (var existingBuff in player.BuffManager.Buffs)
        {
            // 能量大師 + 抽牌專家 = 額外效果
            if (newBuff.PlayerBuffDataId == "energy_master" && 
                existingBuff.PlayerBuffDataId == "card_draw_expert")
            {
                // 觸發組合效果：每獲得能量時額外抽牌
                TriggerComboEffect("energy_draw_combo", player, context);
            }
            
            // 互斥Buff檢查
            if (IsConflictingBuff(newBuff, existingBuff))
            {
                player.BuffManager.RemoveBuff(context, existingBuff.PlayerBuffDataId);
                UI.ShowMessage($"{newBuff.PlayerBuffDataId} 替代了 {existingBuff.PlayerBuffDataId}");
            }
        }
    }
    
    private static bool IsConflictingBuff(IPlayerBuffEntity buff1, IPlayerBuffEntity buff2)
    {
        // 定義互斥的Buff組合
        var conflicts = new Dictionary<string, string[]>
        {
            ["energy_master"] = new[] { "energy_weakness" },
            ["card_draw_expert"] = new[] { "card_draw_penalty" }
        };
        
        return conflicts.TryGetValue(buff1.PlayerBuffDataId, out var conflictList) && 
               conflictList.Contains(buff2.PlayerBuffDataId);
    }
}
```

### 玩家Buff統計系統
```csharp
public class PlayerBuffStatistics
{
    public static BuffStatistics GetStatistics(IPlayerEntity player)
    {
        var buffs = player.BuffManager.Buffs;
        
        return new BuffStatistics
        {
            TotalBuffs = buffs.Count,
            TotalLevels = buffs.Sum(b => b.Level),
            PositiveBuffs = buffs.Count(b => IsPositiveBuff(b.PlayerBuffDataId)),
            NegativeBuffs = buffs.Count(b => IsNegativeBuff(b.PlayerBuffDataId)),
            LongestDuration = buffs.Max(b => b.LifeTime.RemainingDuration()),
            MostStackedBuff = buffs.OrderByDescending(b => b.Level).First()
        };
    }
    
    private static bool IsPositiveBuff(string buffId) =>
        buffId.Contains("master") || buffId.Contains("expert") || buffId.Contains("bonus");
        
    private static bool IsNegativeBuff(string buffId) =>
        buffId.Contains("weakness") || buffId.Contains("penalty") || buffId.Contains("curse");
}
```

## 設計模式應用

### 🏗️ 組合模式 (Composite Pattern)
```csharp
PlayerBuffEntity = Id + Level + Caster + Properties + LifeTime + Sessions
```

### 📋 策略模式 (Strategy Pattern)
```csharp
IPlayerBuffPropertyEntity → 不同的屬性修正策略
IPlayerBuffLifeTimeEntity → 不同的生命週期策略
```

### 🏭 工廠模式 (Factory Pattern)
```csharp
PlayerBuffLibrary.CreateBuff() → 通過BuffLibrary創建完整的Buff實體
```

### 🚫 空值物件模式 (Null Object Pattern)
```csharp
DummyPlayerBuff → 安全的預設值
AlwaysLifeTimePlayerBuffEntity → 永不過期的生命週期
```

### 🔍 觀察者模式 (Observer Pattern)
```csharp
// Buff變化可以通知相關系統
Update() → 返回變化的Buff供系統響應
```

### 📝 命令模式 (Command Pattern)
```csharp
AddPlayerBuffResult/RemovePlayerBuffResult → 操作結果記錄
TriggerContext → 包含操作上下文的命令物件
```

## 依賴關係

### 依賴的組件
- **🔗 IPlayerBuffPropertyEntity**: 屬性修正 *需要PlayerBuffProperty_System.md*
- **🔗 IPlayerBuffLifeTimeEntity**: 生命週期 *需要PlayerBuffLifeTime_System.md*
- **🔗 IReactionSessionEntity**: 反應會話 *需要ReactionSession_System.md*
- **🔗 GameTiming**: 遊戲時機 *需要GameTiming_Enum.md*
- **🔗 TriggerContext**: 觸發上下文 *需要TriggerContext_Class.md*
- **🔗 PlayerBuffLibrary**: Buff資料庫 *需要PlayerBuffLibrary_Class.md*
- **🔗 IGameplayModel**: 遊戲狀態 *需要GameplayModel_Class.md*
- **🔗 Optional**: 安全空值處理 *需要Optional_Library.md*

### 被依賴的組件
- **🔗 PlayerEntity**: 每個Player都有PlayerBuffManager *參考PlayerEntity_Class.md*
- **🔗 CardEffect**: 某些卡片效果會創建PlayerBuff *需要CardEffect_System.md*
- **🔗 EnergyManager**: PlayerBuff影響能量獲得 *參考EnergyManager_Class.md*
- **🔗 回合系統**: 回合切換時更新PlayerBuff *需要TurnSystem_Class.md*
- **🔗 UI系統**: 顯示PlayerBuff狀態 *需要UI_System.md*

### 整合組件
- **🔗 CardBuff系統**: 類似的Buff設計模式 *參考CardBuff_System.md*
- **🔗 CharacterBuff系統**: 類似的Buff設計模式 *參考CharacterBuff_System.md*

## 擴展計劃

### Buff分類系統
```csharp
public enum PlayerBuffCategory
{
    Resource,       // 資源相關：能量、抽牌
    Combat,         // 戰鬥相關：傷害、防護
    Utility,        // 功能相關：手牌上限、消耗減少
    Curse,          // 詛咒：負面效果
    Blessing        // 祝福：正面效果
}

public interface IPlayerBuffEntity
{
    PlayerBuffCategory Category { get; }
    bool IsPositive { get; }
    bool IsStackable { get; }
}
```

### Buff觸發器系統
```csharp
public interface IPlayerBuffTrigger
{
    bool ShouldTrigger(GameEvent gameEvent, IPlayerEntity player);
    void OnTrigger(TriggerContext context, IPlayerBuffEntity buff);
}

// 範例：能量獲得觸發器
public class EnergyGainTrigger : IPlayerBuffTrigger
{
    public bool ShouldTrigger(GameEvent gameEvent, IPlayerEntity player)
    {
        return gameEvent is EnergyGainEvent energyEvent && 
               energyEvent.Target == player;
    }
    
    public void OnTrigger(TriggerContext context, IPlayerBuffEntity buff)
    {
        // 觸發額外效果：每獲得能量時抽一張牌
        var owner = buff.Owner(context.Model);
        if (owner.HasValue)
        {
            owner.Value.CardManager.DrawCards(1);
        }
    }
}
```

### Buff升級系統
```csharp
public interface IPlayerBuffUpgrade
{
    bool CanUpgrade(IPlayerBuffEntity buff);
    string GetUpgradedBuffId(string currentBuffId);
    int GetUpgradeCost(IPlayerBuffEntity buff);
}

// 範例：等級升級
// energy_master_1 → energy_master_2 → energy_master_3
public class TieredBuffUpgrade : IPlayerBuffUpgrade
{
    public bool CanUpgrade(IPlayerBuffEntity buff)
    {
        return buff.Level >= GetRequiredLevel(buff.PlayerBuffDataId) &&
               HasUpgradedVersion(buff.PlayerBuffDataId);
    }
}
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [PlayerBuffData.cs](../../Assets/Scripts/GameData/PlayerBuff/PlayerBuffData.cs) | 核心 | PlayerBuff資料定義 |
| [PlayerBuffEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerBuff/PlayerBuffEntity.cs) | 核心 | PlayerBuff實體實現 |
| [PlayerBuffManager.cs](../../Assets/Scripts/GameModel/Entity/PlayerBuff/PlayerBuffManager.cs) | 核心 | PlayerBuff管理器 |
| [PlayerEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerEntity.cs) | 被依賴 | 使用PlayerBuffManager |
| [CardBuff_System.md](CardBuff_System.md) | 對比 | 類似的Card Buff系統 |
| [CharacterBuff_System.md](CharacterBuff_System.md) | 對比 | 類似的Character Buff系統 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成