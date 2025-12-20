# CardEntity 卡片戰鬥實體筆記

## 類別概述
`CardEntity`是卡片系統的最高層實現，代表進入戰鬥時在手牌/牌堆中的卡片實體。它整合了靜態的卡片資料、動態的Buff系統和完整的戰鬥功能，是卡片在戰鬥中的完整表現形式。

**檔案位置**: [Assets/Scripts/GameModel/Entity/Card/CardEntity.cs](../../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs)

## 接口定義

### ICardEntity 核心接口
```csharp
public interface ICardEntity
{
    // 身份識別
    Guid Identity { get; }
    Option<Guid> OriginCardInstanceGuid { get; }
    string CardDataId { get; }

    // 基本屬性
    CardType Type { get; }
    CardRarity Rarity { get; }
    IEnumerable<CardTheme> Themes { get; }

    // 戰鬥邏輯
    MainTargetSelectLogic MainSelect { get; }
    IEnumerable<ISubSelectionGroup> SubSelects { get; }
    IEnumerable<ICardEffect> Effects { get; }
    IReadOnlyDictionary<CardTriggeredTiming, IEnumerable<ICardEffect>> TriggeredEffects { get; }

    // 動態系統
    IEnumerable<ICardPropertyEntity> Properties { get; }
    ICardBuffManager BuffManager { get; }

    // 數值屬性
    int OriginCost { get; }
    int OriginPower { get; }

    // 實用功能
    ICardEntity Clone(bool includeCardProperties, bool includeCardBuffs);
}
```

## CardEntity實現類別

### 🔧 內部欄位結構
```csharp
public class CardEntity : ICardEntity
{
    // 卡片靜態資料
    private readonly Guid _indentity;
    private readonly Option<Guid> _originCardInstanceGuid;
    private readonly string _mainCardDataId;

    // 卡片運行時資料
    private readonly List<string> _mutationCardDataIds = new();
    private readonly IReadOnlyList<ICardPropertyEntity> _properties;

    // 卡片組件
    private readonly ICardBuffManager _buffManager;
    private readonly CardLibrary _cardLibrary;
}
```

### 🎭 動態卡片資料系統
```csharp
// 動態卡片資料選擇邏輯
private string _actingCardDataId => _mutationCardDataIds.FirstOrDefault() ?? _mainCardDataId;
private CardData _actingCardData => _cardLibrary.GetCardData(_actingCardDataId);
```

**特色功能**：
- **_mutationCardDataIds**: 支援卡片變異，可臨時改變卡片的基礎資料
- **_actingCardDataId**: 優先使用變異資料，回退到原始資料
- 實現運行時的卡片變形效果

### 🏗️ 三種構造方式

#### 1. 從CardInstance創建
```csharp
public static ICardEntity CreateFromInstance(CardInstance cardInstance, CardLibrary cardLibrary)
{
    return new CardEntity(
        indentity: Guid.NewGuid(),
        originCardInstanceGuid: cardInstance.InstanceGuid.Some(),
        cardDataId: cardInstance.CardDataId,
        properties: cardLibrary.GetCardData(cardInstance.CardDataId).PropertyDatas
            .Select(p => p.CreateEntity())
            .Concat(cardInstance.AdditionPropertyDatas.Select(p => p.CreateEntity())),
        buffs: Array.Empty<ICardBuffEntity>(),
        cardLibrary: cardLibrary
    );
}
```

**使用場景**: 玩家牌組進入戰鬥時的標準轉換

#### 2. 運行時創建
```csharp
public static ICardEntity RuntimeCreateFromId(string cardDataId, CardLibrary cardLibrary)
{
    return new CardEntity(
        indentity: Guid.NewGuid(),
        originCardInstanceGuid: Option.None<Guid>(),
        cardDataId: cardDataId,
        properties: cardLibrary.GetCardData(cardDataId).PropertyDatas.Select(p => p.CreateEntity()),
        buffs: Array.Empty<ICardBuffEntity>(),
        cardLibrary: cardLibrary
    );
}
```

**使用場景**: 戰鬥中動態生成的卡片（如法術創造的卡片）

#### 3. 虛擬卡片 (DummyCard)
```csharp
public static ICardEntity DummyCard = new CardEntity(
    indentity: Guid.Empty,
    originCardInstanceGuid: Option.None<Guid>(),
    cardDataId: string.Empty,
    properties: new List<ICardPropertyEntity>(),
    buffs: new List<ICardBuffEntity>(),
    cardLibrary: null
);
```

**使用場景**: 空值物件模式，避免null reference異常

## 核心功能系統

### 🆔 身份識別系統
```csharp
public Guid Identity => _indentity;              // 戰鬥中的唯一識別
public Option<Guid> OriginCardInstanceGuid;      // 原始CardInstance的引用
public bool IsDummy => this == DummyCard;        // 檢查是否為虛擬卡片
```

### 🎯 屬性代理系統
所有CardData的屬性都通過代理模式獲取：
```csharp
public string CardDataId => _actingCardDataId;
public CardType Type => _actingCardData.Type;
public CardRarity Rarity => _actingCardData.Rarity;
public int OriginCost => _actingCardData.Cost;
public int OriginPower => _actingCardData.Power;
public IEnumerable<CardTheme> Themes => _actingCardData.Themes;
```

### ⚡ 效果系統代理
```csharp
public IEnumerable<ICardEffect> Effects => _actingCardData.Effects;
public IReadOnlyDictionary<CardTriggeredTiming, IEnumerable<ICardEffect>> TriggeredEffects
    => _actingCardData.TriggeredEffects.ToDictionary(
        pair => pair.Timing,
        pair => (IEnumerable<ICardEffect>)pair.Effects);
```

### 🎯 目標系統代理
```csharp
public MainTargetSelectLogic MainSelect => _actingCardData.MainSelect;
public IEnumerable<ISubSelectionGroup> SubSelects => _actingCardData.SubSelects;
```

## 擴展功能系統

### 🔄 克隆功能
```csharp
public ICardEntity Clone(bool includeCardProperties, bool includeCardBuffs)
{
    var cloneCard = new CardEntity(
        indentity: Guid.NewGuid(),
        originCardInstanceGuid: Option.None<Guid>(),
        cardDataId: _mainCardDataId,
        properties: includeCardProperties
            ? _properties.Select(p => p.Clone())
            : Array.Empty<ICardPropertyEntity>(),
        buffs: includeCardBuffs
            ? _buffManager.Buffs.Select(b => b.Clone())
            : Array.Empty<ICardBuffEntity>(),
        cardLibrary: _cardLibrary
    );
    return cloneCard;
}
```

**靈活性**：
- 可選擇性複製屬性和Buff
- 支援不同層次的克隆需求
- 新克隆的卡片有獨立的Identity

## 擴展方法系統

### 🔍 全域查詢
```csharp
public static Option<ICardEntity> GetCard(this IGameplayModel model, Guid identity)
{
    var allyCardOpt = model.GameStatus.Ally.CardManager.GetCard(card => card.Identity == identity);
    if (allyCardOpt.HasValue) return allyCardOpt;
    
    var enemyCardOpt = model.GameStatus.Enemy.CardManager.GetCard(card => card.Identity == identity);
    if (enemyCardOpt.HasValue) return enemyCardOpt;
    
    return Option.None<ICardEntity>();
}
```

### 👥 所有權查詢
```csharp
public static Option<IPlayerEntity> Owner(this ICardEntity card, IGameplayModel model)
{
    var gameStatus = model.GameStatus;
    var allyCardOpt = gameStatus.Ally.CardManager.GetCard(c => c.Identity == card.Identity);
    if (allyCardOpt.HasValue)
        return (gameStatus.Ally as IPlayerEntity).Some();
        
    var enemyCardOpt = gameStatus.Enemy.CardManager.GetCard(c => c.Identity == card.Identity);
    if (enemyCardOpt.HasValue)
        return (gameStatus.Enemy as IPlayerEntity).Some();
        
    return Option.None<IPlayerEntity>();
}
```

### 🏛️ 陣營查詢
```csharp
public static Faction Faction(this ICardEntity card, IGameplayModel gameplayWatcher)
{
    return card.Owner(gameplayWatcher).ValueOr(PlayerEntity.DummyPlayer).Faction;
}
```

## 屬性查詢系統

### 🏷️ 基礎屬性檢查
```csharp
public static bool IsConsumable(this ICardEntity card)
{
    return card.HasProperty(CardProperty.Consumable);
}

public static bool IsDisposable(this ICardEntity card)
{
    return card.HasProperty(CardProperty.Dispose) || card.HasProperty(CardProperty.AutoDispose);
}
```

### 🔍 複合屬性檢查
```csharp
public static bool HasProperty(this ICardEntity card, CardProperty property)
{
    return
        card.Properties.Any(p => p.Property == property) ||
        card.BuffManager.Buffs.Any(b => b.Properties.Any(p => p.Property == property));
}
```

**來源**：
- **卡片本身屬性**: `card.Properties`
- **Buff附加屬性**: `card.BuffManager.Buffs`

### 📊 屬性數值計算
```csharp
public static int GetCardProperty(
    this ICardEntity card, TriggerContext triggerContext, CardProperty targetProperty)
{
    var value = 0;

    // 計算卡片本身屬性
    var cardTrigger = new CardTrigger(card);
    var propertyContext = triggerContext with { Triggered = cardTrigger };
    foreach (var property in card.Properties.Where(p => p.Property == targetProperty))
    {
        value += property.Eval(propertyContext);
    }

    // 計算Buff附加屬性
    foreach (var buff in card.BuffManager.Buffs)
    {
        var cardBuffTrigger = new CardBuffTrigger(buff);
        var cardBuffContext = triggerContext with { Triggered = cardBuffTrigger };
        foreach (var property in buff.Properties.Where(p => p.Property == targetProperty))
        {
            value += property.Eval(cardBuffContext);
        }
    }

    return value;
}
```

**計算邏輯**：
1. 創建適當的TriggerContext
2. 累加卡片本身的屬性值
3. 累加所有Buff的屬性值
4. 返回總和

## Optional模式應用

### Option<T>的廣泛使用
```csharp
public Option<Guid> OriginCardInstanceGuid { get; }  // 可能沒有原始實例
```

**優勢**：
- 明確表示可能為空的值
- 避免null reference異常
- 強制進行空值處理

### 安全的值獲取
```csharp
card.Owner(model).ValueOr(PlayerEntity.DummyPlayer)  // 提供預設值
```

## 設計模式應用

### 🎭 代理模式 (Proxy Pattern)
CardEntity代理CardData的所有屬性訪問：
```csharp
public CardType Type => _actingCardData.Type;
public int OriginCost => _actingCardData.Cost;
```

### 🏗️ 工廠模式 (Factory Pattern)
多種靜態建立方法：
- `CreateFromInstance()`: 從實例創建
- `RuntimeCreateFromId()`: 運行時創建
- `DummyCard`: 空值物件

### 🔧 策略模式 (Strategy Pattern)
- 動態效果系統透過ICardEffect
- 屬性系統透過ICardPropertyEntity
- Buff系統透過ICardBuffEntity

### 📋 組合模式 (Composite Pattern)
CardEntity組合多個子系統：
- 屬性系統 (`Properties`)
- Buff系統 (`BuffManager`)
- 效果系統 (`Effects`)

## 生命週期管理

### 創建階段
```
CardInstance → CardEntity.CreateFromInstance() → 完整戰鬥實體
```

### 戰鬥階段
- 接收動態Buff
- 觸發各種效果
- 實時屬性計算

### 銷毀階段
- 檢查屬性標記 (Consumable/Dispose)
- 決定卡片的最終歸宿
- 清理Buff和臨時效果

## 依賴關係分析

### 核心依賴
- **🔗 CardData**: 提供基礎卡片資料 *參考CardData_Class.md*
- **🔗 CardInstance**: 創建來源 *參考CardInstance_Class.md*
- **🔗 CardLibrary**: 資料查詢服務 *需要CardLibrary_Class.md*
- **🔗 Optional**: 安全的空值處理 *需要Optional_Library.md*

### 系統整合
- **🔗 PlayerEntity**: 所有權系統 *需要PlayerEntity_Class.md*
- **🔗 IGameplayModel**: 遊戲狀態查詢 *需要GameplayModel_Interface.md*
- **🔗 BuffManager**: Buff系統管理 *需要CardBuffManager_Class.md*
- **🔗 TriggerContext**: 觸發上下文 *需要TriggerContext_Class.md*

### 被依賴的系統
- **戰鬥系統**: 使用卡片實體進行戰鬥
- **UI系統**: 顯示卡片實體狀態
- **AI系統**: 評估卡片實體價值

## 使用範例

### 從實例創建戰鬥卡片
```csharp
// 玩家牌組進入戰鬥
var cardInstance = playerDeck.GetCard(cardId);
var cardEntity = CardEntity.CreateFromInstance(cardInstance, cardLibrary);

Debug.Log($"卡片 {cardEntity.CardDataId} 進入戰鬥");
Debug.Log($"原始費用: {cardEntity.OriginCost}, 威力: {cardEntity.OriginPower}");
```

### 動態創建卡片
```csharp
// 魔法效果創造火球術
var fireball = CardEntity.RuntimeCreateFromId("spell_fireball", cardLibrary);
playerHand.AddCard(fireball);

Debug.Log($"創造了 {fireball.Type} 類型卡片");
```

### 屬性查詢和計算
```csharp
// 檢查卡片屬性
if (card.HasProperty(CardProperty.Recycle))
{
    Debug.Log("這張卡片可以回收");
}

// 計算實際費用（包含Buff修正）
var actualCost = card.OriginCost + 
                card.GetCardProperty(context, CardProperty.CostAddition);
```

### 卡片克隆
```csharp
// 克隆卡片但不包含Buff
var clonedCard = originalCard.Clone(
    includeCardProperties: true, 
    includeCardBuffs: false
);

// 添加到不同的卡片集合
enemyHand.AddCard(clonedCard);
```

### 全域查詢
```csharp
// 根據Identity查找卡片
var cardOpt = gameplayModel.GetCard(targetCardId);
if (cardOpt.HasValue)
{
    var card = cardOpt.Value;
    var ownerOpt = card.Owner(gameplayModel);
    if (ownerOpt.HasValue)
    {
        Debug.Log($"卡片擁有者: {ownerOpt.Value.Faction}");
    }
}
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CardData.cs](../../../Assets/Scripts/GameData/Card/CardData.cs) | 依賴 | 提供卡片基礎資料 |
| [CardInstance.cs](../../../Assets/Scripts/GameModel/Instance/CardInstance.cs) | 依賴 | 作為創建來源 |
| [CardEnum.cs](../../../Assets/Scripts/GameData/Card/CardEnum.cs) | 依賴 | 使用枚舉定義 |
| CardLibrary_Class.md | 依賴 | 卡片資料查詢服務 |
| PlayerEntity_Class.md | 依賴 | 玩家實體系統 |
| CardBuffManager_Class.md | 依賴 | Buff管理系統 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成