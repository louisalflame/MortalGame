# CardBuff 卡片Buff系統筆記

## 系統概述
CardBuff系統實現了卡片的動態效果機制，為卡片提供臨時性的能力增強、屬性修改和行為變更。與靜態的CardProperty不同，CardBuff具有生命週期管理、等級系統和複雜的效果觸發機制，是遊戲中實現動態平衡和豐富策略的核心系統。

**設計理念**：
- **臨時性效果**：具有明確的生命週期，會自動過期
- **可堆疊性**：支援等級系統，同類Buff可疊加
- **複雜觸發**：支援多種觸發時機和條件判斷
- **完整功能**：包含屬性、效果、反應會話等完整功能

**使用場景**：
- 費用減免：這回合費用-1
- 臨時屬性：3回合內具有preserved效果
- 觸發效果：抽牌時觸發額外效果
- 反應機制：對特定事件進行反應

## 系統架構

### 雙層架構設計
```
CardBuffData (模板層)
        ↓ CreateFromData()
CardBuffEntity (實體層)
```

**檔案位置**：
- **Data層**: [Assets/Scripts/GameData/CardBuff/CardBuffData.cs](../../../Assets/Scripts/GameData/CardBuff/CardBuffData.cs)
- **Entity層**: [Assets/Scripts/GameModel/Entity/CardBuff/CardBuffEntity.cs](../../../Assets/Scripts/GameModel/Entity/CardBuff/CardBuffEntity.cs)

## Data層分析 (CardBuffData)

### 核心資料結構
```csharp
public class CardBuffData
{
    [BoxGroup("Identification")]
    public string ID;                    // Buff唯一識別符

    [BoxGroup("Effects")]
    // 反應會話系統 - 對特定事件的反應
    public Dictionary<string, IReactionSessionData> Sessions = new();
    
    // 觸發效果系統 - 在特定時機觸發的效果
    public Dictionary<CardTriggeredTiming, ConditionalCardBuffEffect[]> Effects = new();

    [TitleGroup("Properties")]
    // 屬性系統 - Buff提供的屬性修正
    public List<ICardBuffPropertyData> PropertyDatas = new();

    [TitleGroup("LifeTime")]
    // 生命週期管理 - 決定Buff何時過期
    public ICardBuffLifeTimeData LifeTimeData;
}
```

### 系統組件詳解

#### 🆔 識別系統
- **ID**: Buff的唯一標識符，用於引用和管理

#### ⚡ 反應會話系統 (Sessions)
```csharp
public Dictionary<string, IReactionSessionData> Sessions = new();
```
- **用途**: 對特定遊戲事件進行反應
- **結構**: 事件名稱 → 反應處理邏輯
- **範例**: 
  - `"OnCardDrawn"` → 抽牌時的反應
  - `"OnDamageDealt"` → 造成傷害時的反應

#### 🎯 觸發效果系統 (Effects)
```csharp
public Dictionary<CardTriggeredTiming, ConditionalCardBuffEffect[]> Effects = new();
```
- **用途**: 在特定時機觸發預定義的效果
- **結構**: 觸發時機 → 條件效果陣列
- **特色**: `ConditionalCardBuffEffect`支援條件判斷
- **範例**:
  - `CardTriggeredTiming.Drawed` → 被抽到時觸發
  - `CardTriggeredTiming.Played` → 被使用時觸發

#### 🏷️ 屬性系統 (PropertyDatas)
```csharp
public List<ICardBuffPropertyData> PropertyDatas = new();
```
- **用途**: 提供屬性修正（如費用減免、威力加成）
- **接口**: `ICardBuffPropertyData`
- **區別**: 與CardProperty不同，這些屬性有生命週期

#### ⏱️ 生命週期系統 (LifeTimeData)
```csharp
public ICardBuffLifeTimeData LifeTimeData;
```
- **用途**: 管理Buff的存續時間
- **類型**:
  - 回合數限制
  - 條件觸發過期
  - 永久持續（特殊情況）

---

## CardBuffManager 管理器系統

### 接口定義
```csharp
public interface ICardBuffManager
{
    // 狀態查詢
    IReadOnlyCollection<ICardBuffEntity> Buffs { get; }
    
    // Buff操作
    AddCardBuffResult AddBuff(
        CardBuffLibrary buffLibrary,
        TriggerContext triggerContext,
        string buffId,
        int level);
    RemoveCardBuffResult RemoveBuff(
        CardBuffLibrary buffLibrary,
        TriggerContext triggerContext,
        string buffId);
    
    // 系統更新
    bool Update(TriggerContext triggerContext);
}
```

### 核心實現
```csharp
public class CardBuffManager : ICardBuffManager
{
    private readonly List<ICardBuffEntity> _buffs;
    
    public IReadOnlyCollection<ICardBuffEntity> Buffs => _buffs;
    
    public CardBuffManager(IEnumerable<ICardBuffEntity> buffs)
    {
        _buffs = new List<ICardBuffEntity>(buffs);
    }
}
```

### CardBuffManager 獨特設計

#### 🎯 BuffLibrary依賴
```csharp
public AddCardBuffResult AddBuff(
    CardBuffLibrary cardBuffLibrary,     // 專用的卡片Buff資料庫
    TriggerContext triggerContext,
    string buffId,
    int level)
```

**BuffLibrary特色**：
- **外部資料庫**：CardBuffManager需要外部提供CardBuffLibrary
- **資料分離**：管理器專注於管理邏輯，資料存取交由Library
- **可擴展性**：不同的CardBuffLibrary可以提供不同的Buff配置
- **測試友好**：可以輕鬆模擬BuffLibrary進行單元測試

#### 📦 初始化Buff支援
```csharp
public CardBuffManager(IEnumerable<ICardBuffEntity> buffs)
{
    _buffs = new List<ICardBuffEntity>(buffs);
}
```

**初始化特色**：
- **預設Buff**：卡片可以在創建時就攜帶初始Buff
- **批量設置**：支援一次性設置多個初始Buff
- **狀態恢復**：便於從存檔或序列化狀態恢復Buff
- **靈活配置**：不同卡片可以有不同的初始Buff組合

#### 🔄 Update返回設計
```csharp
public bool Update(TriggerContext triggerContext)
{
    var isUpdated = false;
    foreach (var buff in _buffs.ToList())
    {
        var triggerBuff = new CardBuffTrigger(buff);
        var updateBuffContext = triggerContext with { Triggered = triggerBuff };
        
        foreach (var session in buff.ReactionSessions.Values)
        {
            isUpdated |= session.Update(updateBuffContext);
        }
        
        isUpdated |= buff.LifeTime.Update(updateBuffContext);
    }
    return isUpdated;
}
```

**Update特色**：
- **布林返回**：返回是否有任何Buff發生變化
- **效率優化**：調用方可以根據返回值決定是否需要後續處理
- **批量檢測**：一次性檢測所有Buff的變化狀態
- **簡潔接口**：不需要返回具體變化的Buff列表

### Buff添加機制

#### 來源追蹤系統
```csharp
var caster = triggerContext.Action switch
{
    CardPlaySource cardSource => cardSource.Card.Owner(triggerContext.Model),
    PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
    _ => Option.None<IPlayerEntity>()
};
```

**來源追蹤特色**：
- **卡片來源**：卡片使用時的Buff來自卡片擁有者
- **玩家Buff來源**：PlayerBuff觸發的CardBuff追蹤到原始施術者
- **鏈式追蹤**：支援Buff的連鎖追蹤關係
- **安全處理**：無法確定來源時使用None

#### Buff創建流程
```csharp
var resultBuff = CardBuffEntity.CreateFromData(
    buffId,
    level,
    caster,
    triggerContext,
    cardBuffLibrary);
```

**創建特色**：
- **工廠模式**：使用CardBuffEntity.CreateFromData工廠方法
- **完整參數**：提供創建Buff所需的所有上下文資訊
- **資料驅動**：基於CardBuffLibrary的資料配置創建Buff
- **類型安全**：確保創建的Buff符合預期類型

### 與其他BuffManager對比

| 特性 | CardBuffManager | CharacterBuffManager | PlayerBuffManager |
|------|-----------------|---------------------|-------------------|
| **初始化** | 支援初始Buff列表 | 空列表開始 | 空列表開始 |
| **Library依賴** | 需要外部BuffLibrary | 從ContextManager獲取 | 從ContextManager獲取 |
| **Update返回** | bool（是否有變化） | IEnumerable（變化的Buff） | IEnumerable（變化的Buff） |
| **結果類型** | 專用Result類型 | out參數 | 專用Result類型 |
| **過期清理** | 在Update中處理 | 在Update中處理 | 專用RemoveExpiredBuff |
| **主要用途** | 卡片增強效果 | 角色狀態效果 | 玩家全域效果 |

### 使用範例

#### 卡片創建時的Buff初始化
```csharp
// 創建帶有初始Buff的卡片
var initialBuffs = new List<ICardBuffEntity>
{
    CreateBuff("power_boost", 2),    // 攻擊力提升
    CreateBuff("cost_reduction", 1)  // 消耗減少
};

var cardBuffManager = new CardBuffManager(initialBuffs);
var cardEntity = new CardEntity(cardData, cardBuffManager);
```

#### Buff動態添加
```csharp
// 透過卡片效果添加Buff
var addResult = card.BuffManager.AddBuff(
    cardBuffLibrary,
    triggerContext,
    "temporary_boost",
    3  // 3層增強
);

if (addResult.IsNewBuff)
{
    Debug.Log($"卡片獲得新Buff: {addResult.Buff.CardBuffDataID}");
}
else
{
    Debug.Log($"Buff層數增加: +{addResult.DeltaLevel}");
}
```

#### 回合更新處理
```csharp
// 回合結束時更新所有卡片的Buff
foreach (var card in player.HandCards)
{
    bool hasChanges = card.BuffManager.Update(triggerContext);
    
    if (hasChanges)
    {
        // 重新計算卡片屬性
        card.RecalculateProperties();
        
        // 更新UI顯示
        UI.UpdateCardDisplay(card);
    }
}
```

#### Buff效果計算整合
```csharp
// 計算所有CardBuff對攻擊力的影響
public int CalculateAttackPower(ICardEntity card, TriggerContext context)
{
    int baseAttack = card.BaseAttackPower;
    int buffBonus = 0;
    
    foreach (var buff in card.BuffManager.Buffs)
    {
        foreach (var property in buff.Properties)
        {
            if (property is AttackPowerProperty attackProperty)
            {
                buffBonus += attackProperty.GetBonus(buff.Level, context);
            }
        }
    }
    
    return baseAttack + buffBonus;
}
```

### AddCardBuffData 輔助結構
```csharp
[Serializable]
public class AddCardBuffData
{
    [ValueDropdown("@DropdownHelper.CardBuffNames")]
    public string CardBuffId;        // 要添加的Buff ID（下拉選單）
    
    public IIntegerValue Level;      // Buff等級
}
```

**用途**：
- 在效果中指定要添加的Buff
- 編輯器友好的下拉選單選擇
- 支援動態等級計算

## Entity層分析 (CardBuffEntity)

### 核心接口 (ICardBuffEntity)
```csharp
public interface ICardBuffEntity
{
    // 基本資訊
    string CardBuffDataID { get; }              // 對應的資料ID
    Guid Identity { get; }                      // 實體唯一識別符
    int Level { get; }                          // 當前等級
    Option<IPlayerEntity> Caster { get; }       // 施放者（可能為空）

    // 功能系統
    IReadOnlyCollection<ICardBuffPropertyEntity> Properties { get; }    // 屬性集合
    ICardBuffLifeTimeEntity LifeTime { get; }                          // 生命週期管理
    IReadOnlyDictionary<string, IReactionSessionEntity> ReactionSessions { get; }  // 反應會話
    IReadOnlyDictionary<CardTriggeredTiming, IEnumerable<ConditionalCardBuffEffect>> Effects { get; }  // 觸發效果

    // 輔助功能
    IEnumerable<string> Keywords { get; }       // 關鍵字集合（UI顯示）
    
    // 操作方法
    bool IsExpired();                           // 檢查是否過期
    void AddLevel(int level);                   // 增加等級
    ICardBuffEntity Clone();                    // 克隆Buff
}
```

### CardBuffEntity實現類別

#### 🏗️ 內部欄位結構
```csharp
public class CardBuffEntity : ICardBuffEntity
{
    // 基本資料
    private readonly string _cardBuffDataId;
    private readonly Guid _identity;
    private int _level;                          // 可變的等級
    private readonly Option<IPlayerEntity> _caster;

    // 功能組件
    private readonly IReadOnlyList<ICardBuffPropertyEntity> _properties;
    private readonly ICardBuffLifeTimeEntity _lifeTime;
    private readonly IReadOnlyDictionary<string, IReactionSessionEntity> _reactionSessions;
    
    // 外部依賴
    private readonly CardBuffLibrary _cardBuffLibrary;
}
```

#### ⚡ 動態效果代理
```csharp
public IReadOnlyDictionary<CardTriggeredTiming, IEnumerable<ConditionalCardBuffEffect>> Effects =>
    _cardBuffLibrary.GetCardBuffData(_cardBuffDataId).Effects.ToDictionary(
        kvp => kvp.Key,
        kvp => (IEnumerable<ConditionalCardBuffEffect>)kvp.Value
    );
```

**設計特點**：
- 效果資料從CardBuffLibrary動態獲取
- 避免重複存儲，節省記憶體
- 支援效果的熱更新

#### 🔤 關鍵字系統
```csharp
public IEnumerable<string> Keywords
    => Effects.Keys.Where(timing => timing != CardTriggeredTiming.None)
        .Select(t => t.ToString())
        .Concat(_properties.SelectMany(p => p.Keywords))
        .Distinct();
```

**組合邏輯**：
- 觸發時機 → 關鍵字
- 屬性關鍵字 → 關鍵字
- 去重處理，避免重複顯示

### 🏭 工廠方法 (CreateFromData)
```csharp
public static CardBuffEntity CreateFromData(
    string cardBuffDataID,
    int level,
    Option<IPlayerEntity> caster,
    TriggerContext triggerContext,
    CardBuffLibrary cardBuffLibrary)
{
    var buffData = cardBuffLibrary.GetCardBuffData(cardBuffDataID);
    
    // 創建屬性實體
    var properties = buffData.PropertyDatas
        .Select(p => p.CreateEntity(triggerContext));
    
    // 創建生命週期實體
    var lifeTime = buffData.LifeTimeData.CreateEntity(triggerContext);
    
    // 創建反應會話實體
    var reactionSessions = buffData.Sessions.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value.CreateEntity(triggerContext)
    );

    return new CardBuffEntity(/* 參數列表 */);
}
```

**創建流程**：
1. 從CardBuffLibrary獲取資料模板
2. 基於TriggerContext創建各組件實體
3. 組裝完整的CardBuffEntity
4. 分配新的Identity

### 🔄 生命週期管理

#### 過期檢查
```csharp
public bool IsExpired()
{
    return LifeTime.IsExpired();
}
```

#### 等級管理
```csharp
public void AddLevel(int level)
{
    _level += level;
}
```

**等級系統特點**：
- 支援正負值調整
- 同類Buff可疊加等級
- 等級影響效果強度

### 📋 克隆功能
```csharp
public ICardBuffEntity Clone()
{
    return new CardBuffEntity(
        cardBuffDataID: _cardBuffDataId,
        identity: Guid.NewGuid(),        // 新的Identity
        level: _level,
        caster: _caster,
        properties: _properties.Select(p => p.Clone()),        // 深度克隆屬性
        lifeTime: _lifeTime.Clone(),                           // 深度克隆生命週期
        reactionSessions: _reactionSessions.ToDictionary(      // 深度克隆反應會話
            kvp => kvp.Key,
            kvp => kvp.Value.Clone()
        ),
        cardBuffLibrary: _cardBuffLibrary
    );
}
```

## 擴展方法系統

### 所有權查詢 (CardBuffEntityExtensions)
```csharp
public static Option<IPlayerEntity> Owner(this ICardBuffEntity cardBuff, IGameplayModel gameplayWatcher)
{
    // 在友軍卡片中查找
    if (gameplayWatcher.GameStatus.Ally.CardManager.GetCard(card => 
        card.BuffManager.Buffs.Contains(cardBuff)).HasValue)
        return (gameplayWatcher.GameStatus.Ally as IPlayerEntity).Some();
    
    // 在敵軍卡片中查找
    if (gameplayWatcher.GameStatus.Enemy.CardManager.GetCard(card => 
        card.BuffManager.Buffs.Contains(cardBuff)).HasValue)
        return (gameplayWatcher.GameStatus.Enemy as IPlayerEntity).Some();
    
    return Option.None<IPlayerEntity>();
}
```

**查詢邏輯**：
1. 遍歷所有玩家的卡片
2. 檢查卡片的BuffManager.Buffs集合
3. 使用Contains進行直接比較
4. 返回Optional結果避免null

## 系統整合分析

### 與CardEntity的整合
```csharp
// CardEntity中的Buff使用
public ICardBuffManager BuffManager => _buffManager;
public IReadOnlyCollection<ICardBuffPropertyEntity> Properties => _properties;
```

**整合要點**：
- CardEntity包含BuffManager管理所有Buff
- 屬性計算時會合併Buff提供的屬性
- 🔗*詳見CardEntity_Class.md*

### 與效果系統的整合
```csharp
// AddCardBuffEffect效果
public class AddCardBuffEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;
    public List<AddCardBuffData> AddCardBuffDatas;
}
```

- 效果系統可以動態添加Buff
- 🔗*參考CardEffect_System.md*

### 生命週期與遊戲流程整合
- 回合開始/結束時檢查Buff過期
- 特定事件觸發時更新Buff狀態
- 與遊戲狀態同步

## 設計模式應用

### 🏗️ 工廠模式 (Factory Pattern)
```csharp
// 從Data創建Entity
public static CardBuffEntity CreateFromData(/* 參數 */)
```

### 🎭 代理模式 (Proxy Pattern)
```csharp
// 效果代理到CardBuffLibrary
public IReadOnlyDictionary<CardTriggeredTiming, IEnumerable<ConditionalCardBuffEffect>> Effects =>
    _cardBuffLibrary.GetCardBuffData(_cardBuffDataId).Effects./* ... */;
```

### 📋 組合模式 (Composite Pattern)
- Buff組合多個子系統：屬性、效果、生命週期、反應會話
- 每個子系統都有獨立的接口和實現

### 🔄 觀察者模式 (Observer Pattern)
- 反應會話系統實現事件響應
- 觸發效果系統監聽遊戲事件

## 使用場景範例

### 💰 費用減免Buff
```csharp
// 創建費用-1的Buff，持續1回合
var costReductionData = new AddCardBuffData
{
    CardBuffId = "cost_reduction_1",
    Level = new FixedIntegerValue(1)
};

// 應用到目標卡片
targetCard.BuffManager.AddBuff(
    CardBuffEntity.CreateFromData(
        cardBuffDataID: costReductionData.CardBuffId,
        level: costReductionData.Level.Eval(context),
        caster: currentPlayer.Some(),
        triggerContext: context,
        cardBuffLibrary: cardBuffLibrary
    )
);
```

### 🛡️ 臨時保留Buff
```csharp
// 創建preserved效果，持續3回合
var preservedBuff = CardBuffEntity.CreateFromData(
    cardBuffDataID: "temporary_preserved",
    level: 1,
    caster: Option.None<IPlayerEntity>(),
    triggerContext: context,
    cardBuffLibrary: cardBuffLibrary
);

// 檢查是否具有保留效果（包括Buff）
if (card.HasProperty(CardProperty.Preserved))
{
    // 卡片會被保留（可能來自Buff或原生屬性）
    handCards.Preserve(card);
}
```

### ⚡ 觸發效果Buff
```csharp
// Buff資料配置
var triggerBuffData = new CardBuffData
{
    ID = "draw_trigger",
    Effects = new Dictionary<CardTriggeredTiming, ConditionalCardBuffEffect[]>
    {
        [CardTriggeredTiming.Drawed] = new[]
        {
            new ConditionalCardBuffEffect
            {
                Condition = /* 條件判斷 */,
                Effects = new ICardEffect[]
                {
                    new DrawCardEffect { /* 抽牌效果 */ }
                }
            }
        }
    },
    LifeTimeData = new TurnCountLifeTimeData { TurnCount = 2 } // 持續2回合
};
```

## 效能考量

### 📊 記憶體管理
- Buff實體相對重量級，包含多個子系統
- 使用只讀集合避免意外修改
- 及時清理過期的Buff

### ⚡ 查詢優化
```csharp
// 批量處理Buff過期
var expiredBuffs = buffManager.Buffs
    .Where(b => b.IsExpired())
    .ToList();

foreach (var buff in expiredBuffs)
{
    buffManager.RemoveBuff(buff);
}
```

### 🔄 事件處理效率
- 避免過度頻繁的觸發檢查
- 使用適當的緩存機制
- 批量處理相同類型的事件

## 依賴關係

### 依賴的組件
- **🔗 CardProperty**: 屬性枚舉定義 *參考CardProperty_System.md*
- **🔗 CardEnum**: 觸發時機枚舉 *參考CardEnum_Reference.md*
- **🔗 TriggerContext**: 上下文系統 *需要TriggerContext_Class.md*
- **🔗 Optional**: 安全的空值處理 *需要Optional_Library.md*
- **🔗 CardBuffLibrary**: 資料查詢服務 *需要CardBuffLibrary_Class.md*

### 被依賴的組件
- **🔗 CardEntity**: 使用Buff系統 *參考CardEntity_Class.md*
- **🔗 CardEffect**: 添加Buff效果 *參考CardEffect_System.md*
- **🔗 BuffManager**: Buff集合管理 *需要CardBuffManager_Class.md*
- **🔗 遊戲流程**: 生命週期管理和觸發處理

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CardBuffData.cs](../../../Assets/Scripts/GameData/CardBuff/CardBuffData.cs) | 核心 | Buff資料模板定義 |
| [CardBuffEntity.cs](../../../Assets/Scripts/GameModel/Entity/CardBuff/CardBuffEntity.cs) | 核心 | Buff實體運行時實現 |
| [CardPropertyData.cs](../../../Assets/Scripts/GameData/Card/CardPropertyData.cs) | 相關 | 對比：靜態屬性系統 |
| [CardEntity.cs](../../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs) | 被依賴 | 使用Buff系統 |
| [CardEffect.cs](../../../Assets/Scripts/GameData/Card/CardEffect.cs) | 被依賴 | 添加Buff的效果 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成