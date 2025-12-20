# CardData 類別筆記

## 類別概述
`CardData` 是卡片系統的核心資料結構，定義在ScriptableAsset上，作為所有卡片的資料模板。它包含了卡片的所有靜態屬性，包括基本資訊、目標邏輯、效果配置和屬性資料。

**檔案位置**: [Assets/Scripts/GameData/Card/CardData.cs](../../../Assets/Scripts/GameData/Card/CardData.cs)

## 類別結構

### 主要類別定義
```csharp
public class CardData
{
    // 卡片識別
    [BoxGroup("Identification")]
    public string ID;

    // 基本資料
    [TitleGroup("BasicData")]
    public CardRarity Rarity;           // 稀有度
    public CardType Type;               // 類型
    public CardTheme[] Themes;          // 主題標籤
    public int Cost;                    // 費用 (0-10)
    public int Power;                   // 威力 (0-20)

    // 目標邏輯
    [BoxGroup("Target")]
    public MainTargetSelectLogic MainSelect;     // 主要目標選擇
    public List<ISubSelectionGroup> SubSelects;  // 子目標選擇

    // 效果配置
    [BoxGroup("Effects")]
    public List<ICardEffect> Effects;                    // 立即效果
    public List<TriggeredCardEffect> TriggeredEffects;   // 觸發效果

    // 屬性資料
    [BoxGroup("Properties")]
    public List<ICardPropertyData> PropertyDatas;        // 卡片屬性
}
```

## 資料分組詳解

### 🆔 識別資料 (Identification)
- **ID**: 卡片的唯一識別符，用於引用特定卡片模板

### 📊 基本資料 (BasicData)
| 欄位 | 類型 | 範圍 | 描述 |
|------|------|------|------|
| **Rarity** | CardRarity | Enum | 卡片稀有度（普通/稀有/史詩/傳說）|
| **Type** | CardType | Enum | 卡片類型（攻擊/防禦/語言/暗器/絕招/道具）|
| **Themes** | CardTheme[] | Array | 卡片主題標籤（唐門/峨嵋/嵩山/丐幫/點蒼）|
| **Cost** | int | 0-10 | 使用卡片所需的費用 |
| **Power** | int | 0-20 | 卡片的基礎威力值 |

### 🎯 目標系統 (Target)
- **MainSelect**: 主要目標選擇邏輯
  - 定義卡片可以選擇的主要目標類型
  - 包含邏輯標籤（敵人/友軍/隨機等）
- **SubSelects**: 子目標選擇組列表
  - 支援複合目標選擇
  - 可配置多個子選擇條件

### ⚡ 效果系統 (Effects)
- **Effects**: 立即效果列表
  - 卡片使用時立即觸發的效果
  - 實現`ICardEffect`接口
- **TriggeredEffects**: 觸發效果列表
  - 在特定時機觸發的效果
  - 每個效果都有對應的觸發時機

### 🏷️ 屬性系統 (Properties)
- **PropertyDatas**: 卡片屬性資料列表
  - 定義卡片的特殊屬性（如可回收、可保留等）
  - 實現`ICardPropertyData`接口

## 嵌套類別

### TriggeredCardEffect
```csharp
[Serializable]
public class TriggeredCardEffect
{
    [TableColumnWidth(150, false)]
    public CardTriggeredTiming Timing;    // 觸發時機
    
    [ShowInInspector]
    public ICardEffect[] Effects;         // 觸發的效果陣列
}
```

**用途**: 定義在特定時機觸發的卡片效果
- **Timing**: 何時觸發（抽牌時/使用時/丟棄時等）
- **Effects**: 觸發時執行的效果列表

### MainTargetSelectLogic
```csharp
[Serializable]
public class MainTargetSelectLogic
{
    public IMainTargetSelectable MainSelectable;  // 主要可選目標
    public TargetLogicTag LogicTag;              // 邏輯標籤
}
```

**用途**: 定義卡片的主要目標選擇邏輯
- **MainSelectable**: 實現主要目標選擇接口的物件
- **LogicTag**: 目標邏輯標籤（無/敵人/友軍/隨機）

## Odin Inspector 特性

### 編輯器視覺化
CardData大量使用Odin Inspector特性來改善編輯器體驗：

- **[BoxGroup]**: 將相關欄位分組到視覺框中
- **[TitleGroup]**: 為欄位群組添加標題
- **[ShowInInspector]**: 顯示通常不可序列化的欄位
- **[TableList]**: 以表格形式顯示列表
- **[TableColumnWidth]**: 控制表格列寬
- **[Range]**: 為數值欄位添加滑桿和範圍限制

### 編輯器友好設計
```csharp
[TitleGroup("BasicData")]
[Range(0, 10)]
public int Cost;        // 費用滑桿，範圍0-10

[ShowInInspector]
public CardTheme[] Themes = new CardTheme[0];  // 顯示陣列編輯器

[TableList]
public List<TriggeredCardEffect> TriggeredEffects;  // 表格形式的觸發效果
```

## 設計模式應用

### 🏗️ 模板模式 (Template Pattern)
- CardData作為所有卡片實例的模板
- 定義卡片的標準結構和預設值

### 🔧 組合模式 (Composite Pattern)
- 組合多種效果、屬性和目標選擇邏輯
- 透過組合實現複雜的卡片行為

### 📋 策略模式 (Strategy Pattern)
- `ICardEffect`、`IMainTargetSelectable`等接口支援策略模式
- 可動態配置不同的效果和目標選擇策略

## 資料流程

### 1. 設計階段
```
設計師 → Unity編輯器 → CardData.asset
```
- 在Unity編輯器中配置CardData
- 設定基本屬性、效果和目標邏輯
- 保存為ScriptableObject資產

### 2. 載入階段
```
CardData.asset → 記憶體 → CardLibrary
```
- 遊戲啟動時載入所有CardData
- 🔗*需要CardLibrary_Class.md* 

### 3. 實例化階段
```
CardData → CardInstance → CardEntity
```
- 根據CardData創建CardInstance
- 戰鬥時轉換為CardEntity
- 🔗*需要CardInstance_Class.md*
- 🔗*需要CardEntity_Class.md*

## 依賴關係

### 依賴的組件
- **🔗 CardEnum**: 卡片相關枚舉 *需要CardEnum_Reference.md*
- **🔗 CardEffect**: 效果系統接口 *需要CardEffect_System.md*
- **🔗 Target系統**: 目標選擇邏輯 *需要Target_System.md*
- **🔗 CardProperty**: 屬性系統 *需要CardProperty_System.md*

### 被依賴的組件
- **CardInstance**: 使用CardData.ID創建實例
- **CardEntity**: 使用CardData配置戰鬥行為
- **CardLibrary**: 管理和查詢CardData
- **UI系統**: 顯示CardData資訊

## 設計考量

### 📊 資料驅動設計
- 所有卡片行為都由資料配置決定
- 無需程式碼修改即可新增卡片
- 設計師友好的編輯器界面

### 🔒 不可變性
- CardData在運行時不應被修改
- 所有動態修改都在CardEntity層進行
- 確保模板資料的完整性

### 🎯 可擴展性
- 使用接口設計支援新的效果類型
- 模組化的屬性系統便於擴展
- 靈活的目標選擇機制

### 📈 效能考量
- ScriptableObject提供高效的資料序列化
- 靜態資料共用減少記憶體使用
- 編輯時驗證減少運行時錯誤

## 使用範例

### 創建攻擊卡片
```csharp
CardData attackCard = new CardData
{
    ID = "attack_fireball",
    Type = CardType.Attack,
    Rarity = CardRarity.Common,
    Themes = new[] { CardTheme.TangSect },
    Cost = 3,
    Power = 5,
    MainSelect = new MainTargetSelectLogic
    {
        MainSelectable = new SingleEnemySelectable(),
        LogicTag = TargetLogicTag.ToEnemy
    },
    Effects = new List<ICardEffect>
    {
        new DamageEffect { /* 配置傷害效果 */ }
    }
};
```

### 配置觸發效果
```csharp
TriggeredCardEffect preserveEffect = new TriggeredCardEffect
{
    Timing = CardTriggeredTiming.Discarded,
    Effects = new[]
    {
        new DrawCardEffect { /* 棄牌時抽牌 */ }
    }
};
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CardEnum.cs](../../../Assets/Scripts/GameData/Card/CardEnum.cs) | 依賴 | 提供枚舉定義 |
| [CardEffect.cs](../../../Assets/Scripts/GameData/Card/CardEffect.cs) | 依賴 | 提供效果接口 |
| [CardInstance.cs](../../../Assets/Scripts/GameModel/Instance/CardInstance.cs) | 被依賴 | 使用CardData創建實例 |
| [CardEntity.cs](../../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs) | 被依賴 | 基於CardData建立實體 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成