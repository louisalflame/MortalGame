# CardProperty 卡片屬性系統筆記

## 系統概述
CardProperty系統實現了卡片的靜態屬性標籤功能，提供永久性的卡片行為標識。與動態的CardBuff不同，CardProperty是卡片的固有特性，通常在卡片設計時就確定，用於標識卡片的特殊行為模式。

**主要特色**：
- **靜態屬性**：一旦設定就不會改變的卡片特性
- **行為標識**：讓系統能夠識別和處理特殊卡片行為
- **雙層設計**：Data層定義模板，Entity層提供運行時功能

## 系統架構

### 雙層架構設計
```
CardPropertyData (模板層)
        ↓ CreateEntity()
CardPropertyEntity (實體層)
```

**檔案位置**：
- **Data層**: [Assets/Scripts/GameData/Card/CardPropertyData.cs](../../../Assets/Scripts/GameData/Card/CardPropertyData.cs)
- **Entity層**: [Assets/Scripts/GameModel/Entity/Card/CardPropertyEntity.cs](../../../Assets/Scripts/GameModel/Entity/Card/CardPropertyEntity.cs)

## Data層分析 (CardPropertyData)

### 核心接口
```csharp
public interface ICardPropertyData
{
    ICardPropertyEntity CreateEntity();
}
```

**設計理念**：
- 工廠模式接口，負責創建對應的Entity
- 統一的創建方式，便於多型處理
- 分離資料定義與運行時邏輯

### 實現的屬性類型

| 屬性類型 | 類別名稱 | 對應枚舉 | 功能描述 |
|---------|----------|----------|----------|
| **保留** | `PreservedPropertyData` | `CardProperty.Preserved` | 卡片不會被回合結束時丟棄 |
| **初始優先** | `InitialPriorityPropertyData` | `CardProperty.Initialize` | 戰鬥開始時觸發特殊效果 |
| **消耗品** | `ConsumablePropertyData` | `CardProperty.Consumable` | 戰鬥結束後移除，下次戰鬥重新加入 |
| **銷毀** | `DisposePropertyData` | `CardProperty.Dispose` | 使用後永久銷毀 |
| **自動銷毀** | `AutoDisposePropertyData` | `CardProperty.AutoDispose` | 滿足條件時自動銷毀 |
| **封印** | `SealedPropertyData` | `CardProperty.Sealed` | 無法使用的狀態 |
| **回收** | `RecyclePropertyData` | `CardProperty.Recycle` | 可以回收重用 |

### 標準實現模式
```csharp
[Serializable]
public class PreservedPropertyData : ICardPropertyData
{
    public ICardPropertyEntity CreateEntity()
    {
        return new PreservedPropertyEntity();
    }
}
```

**特點**：
- 每個屬性都有對應的Data類別
- `[Serializable]`支援Unity序列化
- 簡潔的工廠方法實現

## Entity層分析 (CardPropertyEntity)

### 核心接口
```csharp
public interface ICardPropertyEntity
{
    CardProperty Property { get; }           // 對應的枚舉值
    IEnumerable<string> Keywords { get; }    // 關鍵字列表
    int Eval(TriggerContext triggerContext); // 數值計算
    ICardPropertyEntity Clone();             // 克隆方法
}
```

### 接口功能詳解

#### Property 枚舉對應
- 每個Entity都對應一個`CardProperty`枚舉值
- 用於系統識別和位元遮罩操作
- 🔗*參考CardEnum_Reference.md中的CardProperty定義*

#### Keywords 關鍵字系統
```csharp
public IEnumerable<string> Keywords => Property.ToString().WrapAsEnumerable();
```
- 將枚舉值轉換為字串關鍵字
- 用於UI顯示和搜尋功能
- 可擴展為更複雜的關鍵字系統

#### Eval 數值計算
```csharp
public int Eval(TriggerContext triggerContext) => 0;
```
- 目前所有屬性都返回0（純標識性屬性）
- 為未來數值型屬性預留接口
- 接受`TriggerContext`進行上下文相關計算

#### Clone 克隆功能
- 支援屬性的深度複製
- 用於卡片克隆和Buff系統

### 實現範例分析

#### PreservedPropertyEntity
```csharp
public class PreservedPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Preserved;
    public IEnumerable<string> Keywords => Property.ToString().WrapAsEnumerable();

    public PreservedPropertyEntity() { }
    public int Eval(TriggerContext triggerContext) => 0;
    public ICardPropertyEntity Clone() => new PreservedPropertyEntity();
}
```

**功能**：
- 標識卡片具有"保留"特性
- 卡片不會在回合結束時被丟棄
- 常用於特殊策略卡片

#### SealedPropertyEntity
```csharp
public class SealedPropertyEntity : ICardPropertyEntity
{
    public CardProperty Property => CardProperty.Sealed;
    public IEnumerable<string> Keywords => Property.ToString().WrapAsEnumerable();

    public SealedPropertyEntity() { }
    public int Eval(TriggerContext triggerContext) => 0;
    public ICardPropertyEntity Clone() => new SealedPropertyEntity();
}
```

**功能**：
- 標識卡片被"封印"無法使用
- 用於負面效果或特殊機制
- 影響卡片的可使用性判斷

## 系統整合分析

### 與CardEntity的整合
在🔗*CardEntity_Class.md*中，屬性系統的使用：

```csharp
// CardEntity中的屬性檢查
public static bool HasProperty(this ICardEntity card, CardProperty property)
{
    return
        card.Properties.Any(p => p.Property == property) ||
        card.BuffManager.Buffs.Any(b => b.Properties.Any(p => p.Property == property));
}
```

**整合方式**：
- CardEntity包含`Properties`集合
- 同時檢查卡片本身屬性和Buff附加屬性
- 支援複合屬性查詢

### 與CardData的整合
```csharp
// CardData中的屬性配置
[BoxGroup("Properties")]
public List<ICardPropertyData> PropertyDatas = new();
```

- CardData定義基礎屬性
- 通過PropertyDatas配置卡片的固有特性
- 編輯器中可視化配置

### 與CardBuff的關係
```csharp
// CardBuff也可以提供屬性
ICardBuffPropertyEntity // CardBuff中的屬性接口
```

**區別對比**：
- **CardProperty**: 靜態、永久、卡片固有
- **CardBuffProperty**: 動態、臨時、有生命週期

## 設計模式應用

### 🏗️ 工廠模式 (Factory Pattern)
```csharp
// Data創建Entity
public ICardPropertyEntity CreateEntity() => new PreservedPropertyEntity();
```

### 📋 策略模式 (Strategy Pattern)
```csharp
// 不同屬性有不同的行為策略
public interface ICardPropertyEntity
{
    int Eval(TriggerContext triggerContext);  // 策略方法
}
```

### 🎭 原型模式 (Prototype Pattern)
```csharp
// 支援屬性克隆
public ICardPropertyEntity Clone() => new PreservedPropertyEntity();
```

## 使用場景分析

### 🛡️ 永久性行為標識
```csharp
// 範例：檢查卡片是否可保留
if (card.HasProperty(CardProperty.Preserved))
{
    // 卡片不會被回合結束時丟棄
    handCards.Preserve(card);
}
```

### 🔒 狀態控制
```csharp
// 範例：檢查卡片是否被封印
if (card.HasProperty(CardProperty.Sealed))
{
    // 卡片無法使用
    return false; // 無法打出
}
```

### 🔄 生命週期管理
```csharp
// 範例：處理消耗品卡片
if (card.HasProperty(CardProperty.Consumable))
{
    // 戰鬥結束後移除，下次戰鬥重新加入
    battleEndProcessor.MarkAsConsumable(card);
}
```

## 擴展性設計

### 📈 新增屬性類型
1. 在`CardProperty`枚舉中添加新值
2. 創建對應的PropertyData類別
3. 創建對應的PropertyEntity類別
4. 更新相關的處理邏輯

### 🔢 數值型屬性支援
```csharp
// 未來可能的數值型屬性
public class PowerBoostPropertyEntity : ICardPropertyEntity
{
    private readonly int _boost;
    
    public CardProperty Property => CardProperty.PowerAddition;
    public int Eval(TriggerContext triggerContext) => _boost; // 返回實際數值
}
```

### 🎯 上下文相關計算
```csharp
// 根據觸發上下文計算屬性值
public int Eval(TriggerContext triggerContext)
{
    // 可根據triggerContext.GameState等進行複雜計算
    return CalculateBasedOnContext(triggerContext);
}
```

## 性能考量

### 💾 記憶體效率
- 屬性實體通常是輕量級物件
- 使用枚舉進行快速比較
- 避免不必要的字串操作

### ⚡ 查詢優化
- 利用位元遮罩進行快速屬性檢查
- 緩存常用的屬性查詢結果
- 避免頻繁的LINQ操作

## 依賴關係

### 依賴的組件
- **🔗 CardEnum**: 使用CardProperty枚舉 *參考CardEnum_Reference.md*
- **🔗 TriggerContext**: 上下文計算 *需要TriggerContext_Class.md*
- **Unity序列化**: `[Serializable]`特性支援

### 被依賴的組件
- **🔗 CardData**: 配置基礎屬性 *參考CardData_Class.md*
- **🔗 CardEntity**: 使用屬性進行行為判斷 *參考CardEntity_Class.md*
- **🔗 CardBuff**: 提供動態屬性 *需要CardBuff_System.md*
- **🔗 遊戲邏輯**: 各種系統根據屬性決定行為

## 使用範例

### 基本屬性創建
```csharp
// 創建保留屬性
var preservedData = new PreservedPropertyData();
var preservedEntity = preservedData.CreateEntity();

Debug.Log($"屬性類型: {preservedEntity.Property}");
Debug.Log($"關鍵字: {string.Join(", ", preservedEntity.Keywords)}");
```

### 卡片屬性配置
```csharp
// 在CardData中配置屬性
var cardData = new CardData
{
    PropertyDatas = new List<ICardPropertyData>
    {
        new PreservedPropertyData(),      // 保留在手牌
        new RecyclePropertyData()         // 可回收
    }
};
```

### 運行時屬性檢查
```csharp
// 檢查卡片屬性
foreach (var property in card.Properties)
{
    switch (property.Property)
    {
        case CardProperty.Preserved:
            Debug.Log("這張卡片會保留在手牌");
            break;
        case CardProperty.Sealed:
            Debug.Log("這張卡片被封印，無法使用");
            break;
        case CardProperty.Consumable:
            Debug.Log("這張卡片是消耗品");
            break;
    }
}
```

### 屬性克隆
```csharp
// 克隆卡片時複製屬性
var originalCard = GetSomeCard();
var clonedCard = originalCard.Clone(includeCardProperties: true, includeCardBuffs: false);

// 驗證屬性被正確複製
Assert.AreEqual(
    originalCard.Properties.Count(), 
    clonedCard.Properties.Count()
);
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CardPropertyData.cs](../../../Assets/Scripts/GameData/Card/CardPropertyData.cs) | 核心 | 屬性資料模板定義 |
| [CardPropertyEntity.cs](../../../Assets/Scripts/GameModel/Entity/Card/CardPropertyEntity.cs) | 核心 | 屬性實體運行時實現 |
| [CardEnum.cs](../../../Assets/Scripts/GameData/Card/CardEnum.cs) | 依賴 | CardProperty枚舉定義 |
| [CardData.cs](../../../Assets/Scripts/GameData/Card/CardData.cs) | 被依賴 | 配置卡片基礎屬性 |
| [CardEntity.cs](../../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs) | 被依賴 | 使用屬性系統 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成