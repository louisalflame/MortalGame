# CardInstance 卡片實例筆記

## 類別概述
`CardInstance`是卡片系統中的中間層，代表Runtime時玩家擁有的卡片實例。它採用C# Record結構設計，確保不可變性並提供高效的資料操作。每個CardInstance都有唯一的識別符，可以追蹤卡片的獲得和使用歷程。

**檔案位置**: [Assets/Scripts/GameModel/Instance/CardInstance.cs](../../../Assets/Scripts/GameModel/Instance/CardInstance.cs)

## Record結構定義

```csharp
public record CardInstance(
    // static data
    Guid InstanceGuid,
    string CardDataId,
    // dynamic data
    IReadOnlyList<ICardPropertyData> AdditionPropertyDatas)
```

## 欄位詳解

### 🆔 靜態資料 (Static Data)

#### InstanceGuid
- **類型**: `Guid`
- **用途**: 卡片實例的唯一識別符
- **特性**: 
  - 全域唯一，永不重複
  - 用於追蹤和查詢特定卡片實例
  - 支援跨系統的卡片引用

#### CardDataId  
- **類型**: `string`
- **用途**: 引用對應的CardData模板
- **關係**: 🔗*參考CardData_Class.md*
- **功能**: 決定卡片的基礎屬性和行為

### 📊 動態資料 (Dynamic Data)

#### AdditionPropertyDatas
- **類型**: `IReadOnlyList<ICardPropertyData>`
- **用途**: 額外的屬性資料，覆蓋或擴展基礎卡片屬性
- **特性**:
  - 只讀列表，確保不可變性
  - 支援卡片的個人化修改
  - 不影響原始CardData模板

## 核心工廠方法

### Create 靜態方法
```csharp
public static CardInstance Create(CardData cardData)
{
    return new CardInstance(
        InstanceGuid: Guid.NewGuid(),
        CardDataId: cardData.ID,
        AdditionPropertyDatas: Array.Empty<ICardPropertyData>()
    );
}
```

**功能說明**:
- 從CardData創建新的CardInstance
- 自動生成唯一的InstanceGuid
- 初始時沒有額外屬性資料
- 是最常用的CardInstance創建方式

## Record特性應用

### 🔒 不可變性 (Immutability)
```csharp
// Record的不可變特性
var original = CardInstance.Create(cardData);
var modified = original with { AdditionPropertyDatas = newProperties };
// original保持不變，modified是新實例
```

### 🔄 With表達式支援
```csharp
// 創建修改版本
var enhancedCard = cardInstance with 
{
    AdditionPropertyDatas = cardInstance.AdditionPropertyDatas
        .Append(newProperty)
        .ToList()
};
```

### ⚡ 自動相等比較
```csharp
// Record自動實現值相等
var card1 = CardInstance.Create(cardData);
var card2 = CardInstance.Create(cardData);
// card1 != card2 (因為InstanceGuid不同)

var clone = card1 with { };
// clone == card1 (所有值都相同)
```

## 生命週期管理

### 階段1: 創建 (Creation)
```
CardData → CardInstance.Create() → CardInstance
```
- 玩家獲得新卡片時觸發
- 生成唯一的InstanceGuid
- 記錄獲得時間和來源（透過擴展資料）

### 階段2: 修改 (Modification)
```
CardInstance → with表達式 → 新CardInstance
```
- 卡片強化、升級或獲得Buff時
- 透過AdditionPropertyDatas記錄變更
- 保持原始實例不變

### 階段3: 戰鬥轉換 (Battle Conversion)
```
CardInstance → CardEntity.CreateFromInstance() → CardEntity
```
- 進入戰鬥時轉換為CardEntity
- 🔗*需要CardEntity_Class.md*
- 載入完整的戰鬥功能

### 階段4: 存檔 (Persistence)
```
CardInstance → 序列化 → 存檔系統
```
- 持久化玩家的卡片收藏
- Record結構便於序列化

## 設計模式應用

### 🏗️ 工廠模式 (Factory Pattern)
```csharp
// 靜態工廠方法
public static CardInstance Create(CardData cardData)

// 隱式工廠 - Record構造函數
new CardInstance(guid, id, properties)
```

### 📋 值物件模式 (Value Object Pattern)
- Record自然實現值物件語義
- 基於值的相等比較
- 不可變性確保一致性

### 🎯 建造者模式 (Builder Pattern)
```csharp
// With表達式提供類似建造者的功能
var cardInstance = baseInstance
    .with { AdditionPropertyDatas = newProps }
    .with { /* 其他修改 */ };
```

## 屬性系統整合

### ICardPropertyData整合
```csharp
// 添加屬性的範例
var buffedCard = originalCard with 
{
    AdditionPropertyDatas = originalCard.AdditionPropertyDatas
        .Append(new CostReductionProperty { Value = 1 })
        .Append(new PowerBoostProperty { Value = 2 })
        .ToList()
};
```

### 屬性合併邏輯
1. **基礎屬性**: 來自CardData.PropertyDatas
2. **額外屬性**: 來自AdditionPropertyDatas
3. **最終屬性**: 在CardEntity中合併計算

## UniRx整合

### 響應式程式設計支援
```csharp
using UniRx;

// 觀察CardInstance變更
IObservable<CardInstance> cardInstanceStream = ...;

cardInstanceStream
    .Where(card => card.CardDataId == "fireball")
    .Subscribe(card => Debug.Log($"火球術實例: {card.InstanceGuid}"));
```

### 事件流整合
- 卡片獲得事件
- 卡片修改事件
- 卡片使用事件

## 序列化考量

### JSON序列化友好
```json
{
    "InstanceGuid": "12345678-1234-1234-1234-123456789012",
    "CardDataId": "attack_fireball",
    "AdditionPropertyDatas": [
        {
            "$type": "CostReductionProperty",
            "Value": 1
        }
    ]
}
```

### Unity序列化
- Record結構可能需要特殊處理
- 考慮使用序列化代理模式
- AdditionPropertyDatas的多型序列化

## 效能考量

### 📈 記憶體效率
- Record使用值語義，減少引用
- 不可變性避免防禦性複製
- 共享CardDataId字串

### ⚡ 查詢效能
```csharp
// 高效的Guid比較
var targetCard = cardCollection
    .FirstOrDefault(c => c.InstanceGuid == searchGuid);

// 基於ID的群組查詢
var cardGroups = cardCollection
    .GroupBy(c => c.CardDataId)
    .ToDictionary(g => g.Key, g => g.ToList());
```

## 資料完整性

### 🔒 不變性保證
- Record確保建立後無法修改
- 修改必須創建新實例
- 避免意外的狀態變更

### ✅ 驗證機制
```csharp
public record CardInstance(
    Guid InstanceGuid,
    string CardDataId,
    IReadOnlyList<ICardPropertyData> AdditionPropertyDatas)
{
    public CardInstance
    {
        ArgumentNullException.ThrowIfNull(CardDataId);
        ArgumentNullException.ThrowIfNull(AdditionPropertyDatas);
        
        if (InstanceGuid == Guid.Empty)
            throw new ArgumentException("InstanceGuid cannot be empty");
    }
}
```

## 使用範例

### 基本創建
```csharp
// 從CardData創建實例
var cardData = cardLibrary.GetCardData("attack_fireball");
var cardInstance = CardInstance.Create(cardData);

Debug.Log($"創建卡片實例: {cardInstance.InstanceGuid}");
Debug.Log($"卡片類型: {cardInstance.CardDataId}");
```

### 屬性修改
```csharp
// 添加強化屬性
var enhancedCard = cardInstance with 
{
    AdditionPropertyDatas = new List<ICardPropertyData>
    {
        new PowerBoostProperty { Boost = 3 },
        new CostReductionProperty { Reduction = 1 }
    }
};
```

### 戰鬥轉換
```csharp
// 轉換為戰鬥實體
var cardEntity = CardEntity.CreateFromInstance(cardInstance, cardLibrary);
```

### 集合操作
```csharp
// 玩家卡片收藏
var playerCards = new List<CardInstance>();

// 添加新卡片
var newCard = CardInstance.Create(someCardData);
playerCards.Add(newCard);

// 查詢特定卡片
var fireballCards = playerCards
    .Where(c => c.CardDataId.Contains("fireball"))
    .ToList();
```

## 依賴關係

### 依賴的組件
- **🔗 CardData**: 提供卡片模板 *參考CardData_Class.md*
- **🔗 ICardPropertyData**: 屬性資料接口 *需要CardProperty_System.md*
- **System.Guid**: .NET框架的唯一識別符
- **UniRx**: 響應式程式設計框架

### 被依賴的組件
- **🔗 CardEntity**: 從CardInstance創建戰鬥實體 *需要CardEntity_Class.md*
- **🔗 玩家收藏系統**: 管理玩家的卡片實例 *需要PlayerCollection_System.md*
- **🔗 存檔系統**: 持久化卡片實例資料 *需要SaveSystem_Class.md*

## 擴展可能性

### 🚀 未來擴展
```csharp
// 可能的擴展欄位
public record CardInstance(
    Guid InstanceGuid,
    string CardDataId,
    IReadOnlyList<ICardPropertyData> AdditionPropertyDatas,
    
    // 未來可能添加
    DateTime AcquiredTime,           // 獲得時間
    string AcquisitionSource,        // 獲得來源
    int UsageCount,                  // 使用次數
    PlayerRating PlayerRating        // 玩家評分
);
```

### 🔧 行為擴展
- 擴展方法添加查詢功能
- 自訂序列化行為
- 驗證規則擴展

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CardData.cs](../../../Assets/Scripts/GameData/Card/CardData.cs) | 依賴 | 提供卡片模板資料 |
| [CardEntity.cs](../../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs) | 被依賴 | 從實例創建戰鬥實體 |
| CardProperty_System.md | 依賴 | 屬性資料系統 |
| PlayerCollection_System.md | 被依賴 | 玩家收藏管理 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成