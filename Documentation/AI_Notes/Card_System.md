# Card 卡片系統 - 總覽筆記

## 系統概述
卡片系統是MortalGame的核心機制，管理從資料定義到戰鬥執行的完整卡片生命週期。系統採用三層架構設計，實現了資料與邏輯的清晰分離。

## 核心架構

### 三層卡片架構
```
ScriptableAsset層    Runtime層         Battle層
    CardData ────→ CardInstance ────→ CardEntity
     (模板)         (玩家擁有)       (戰鬥實體)
```

### 1. CardData (模板層)
- **位置**: [Assets/Scripts/GameData/Card/CardData.cs](../../Assets/Scripts/GameData/Card/CardData.cs)
- **用途**: ScriptableAsset上的卡牌資料結構，定義卡片的靜態屬性
- **特點**: 
  - 不可變的模板資料
  - 在編輯器中配置
  - 所有相同ID卡片共用同一個CardData

### 2. CardInstance (實例層)
- **位置**: [Assets/Scripts/GameModel/Instance/CardInstance.cs](../../Assets/Scripts/GameModel/Instance/CardInstance.cs)
- **用途**: Runtime時玩家擁有的卡片實例
- **特點**:
  - Record結構，不可變
  - 包含唯一的InstanceGuid
  - 可以有額外的屬性修改

### 3. CardEntity (實體層)
- **位置**: [Assets/Scripts/GameModel/Entity/Card/CardEntity.cs](../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs)
- **用途**: 進入戰鬥時在手牌/牌堆的卡片實體
- **特點**:
  - 完整的戰鬥功能
  - 可以有臨時Buff和屬性修改
  - 包含目標選擇和效果執行邏輯

## 資料流轉生命週期

### 階段1: 設計階段
```
編輯器 → CardData.asset
```
- 在Unity編輯器中配置卡片基礎資料
- 定義卡片類型、稀有度、費用、威力等

### 階段2: 收集階段
```
CardData → CardInstance
```
- 玩家獲得卡片時創建CardInstance
- 分配唯一的InstanceGuid
- 可添加收集時的屬性加成

### 階段3: 戰鬥階段
```
CardInstance → CardEntity
```
- 進入戰鬥時轉換為CardEntity
- 載入完整的戰鬥功能
- 支援動態修改和Buff系統

## 核心組件系統

### 🎯 目標系統
- **MainTargetSelectLogic**: 主要目標選擇邏輯
- **ISubSelectionGroup**: 子目標選擇組
- **相關文件**: 🔗*需要Target_System.md*

### ⚡ 效果系統
- **ICardEffect**: 卡片效果接口
- **TriggeredCardEffect**: 觸發式效果
- **相關文件**: [Assets/Scripts/GameData/Card/CardEffect.cs](../../Assets/Scripts/GameData/Card/CardEffect.cs)

### 🏷️ 屬性系統
- **CardProperty**: 卡片屬性枚舉（使用位元遮罩）
- **ICardPropertyData**: 屬性資料接口
- **ICardPropertyEntity**: 屬性實體接口

### 🎭 Buff系統
- **ICardBuffManager**: Buff管理器
- **ICardBuffEntity**: Buff實體
- **相關文件**: 🔗*需要CardBuff_System.md*

## 卡片分類系統

### 卡片類型 (CardType)
- **Attack**: 攻擊類型 - 捅人
- **Defense**: 防禦類型 - 備揍
- **Speech**: 語言類型 - 嘴攻
- **Sneak**: 暗器類型 - 暗器
- **Special**: 絕招類型 - 絕招
- **Item**: 道具類型

### 卡片稀有度 (CardRarity)
- **Common**: 普通
- **Rare**: 稀有
- **Epic**: 史詩
- **Legendary**: 傳說

### 卡片主題 (CardTheme)
- **TangSect**: 唐門
- **Emei**: 峨嵋
- **Songshan**: 嵩山
- **BeggarClan**: 丐幫
- **DianCang**: 點蒼

## 核心特性

### 🔄 動態修改支援
```csharp
// CardEntity支援動態修改
card.GetCardProperty(triggerContext, CardProperty.CostAddition)
card.HasProperty(CardProperty.Consumable)
```

### 🎲 運行時創建
```csharp
// 支援運行時創建卡片
CardEntity.RuntimeCreateFromId(cardDataId, cardLibrary)
```

### 📋 克隆機制
```csharp
// 支援卡片克隆，可選擇包含屬性和Buff
card.Clone(includeCardProperties: true, includeCardBuffs: false)
```

### 🔍 查詢系統
```csharp
// 全域卡片查詢
model.GetCard(cardGuid)
card.Owner(model)
card.Faction(gameplayWatcher)
```

## 系統依賴關係

### 依賴的系統
- **🔗 GameModel**: 核心遊戲模型 *需要GameModel_System.md*
- **🔗 Target系統**: 目標選擇邏輯 *需要Target_System.md*
- **🔗 PlayerEntity**: 玩家實體系統 *需要PlayerEntity_Class.md*
- **🔗 CardLibrary**: 卡片資料庫 *需要CardLibrary_Class.md*
- **🔗 Faction**: 陣營系統 *需要Faction_Enum.md*

### 被依賴的系統
- **GameplayManager**: 使用卡片進行遊戲流程控制
- **UI系統**: 顯示卡片資訊和交互
- **Action系統**: 執行卡片相關動作

## 設計模式應用

### 1. 工廠模式
```csharp
CardEntity.CreateFromInstance(cardInstance, cardLibrary)
CardEntity.RuntimeCreateFromId(cardDataId, cardLibrary)
```

### 2. 組合模式
- CardData組合多種效果和屬性
- CardEntity組合Buff和Properties

### 3. 策略模式
- ICardEffect接口支援多種效果策略
- 不同的目標選擇策略

### 4. 原型模式
- CardEntity的Clone方法實現原型模式

## 重要設計原則

### 📊 資料驅動設計
- 卡片行為完全由資料配置決定
- ScriptableObject提供編輯器友好的配置方式

### 🔒 不可變性原則
- CardData和CardInstance設計為不可變
- 只有CardEntity在戰鬥中可變

### 🎯 單一責任原則
- 每個層次有明確的責任分工
- 模板、實例、實體各司其職

### 🔧 開放封閉原則
- 透過接口擴展新的效果類型
- 不修改現有代碼即可添加新功能

## 效能考量

### 💾 記憶體管理
- 使用Record減少記憶體分配
- CardData共用減少重複資料

### ⚡ 查詢優化
- 使用Guid進行快速查詢
- 適當的緩存機制

### 🔄 生命週期管理
- 明確的創建和銷毀時機
- 避免記憶體洩漏

---

## 相關檔案列表
| 檔案 | 類型 | 描述 |
|------|------|------|
| [CardData.cs](../../Assets/Scripts/GameData/Card/CardData.cs) | Class | 卡片資料模板定義 |
| [CardEnum.cs](../../Assets/Scripts/GameData/Card/CardEnum.cs) | Enum | 卡片相關枚舉定義 |
| [CardEffect.cs](../../Assets/Scripts/GameData/Card/CardEffect.cs) | Interface | 卡片效果系統 |
| [CardInstance.cs](../../Assets/Scripts/GameModel/Instance/CardInstance.cs) | Record | 卡片實例結構 |
| [CardEntity.cs](../../Assets/Scripts/GameModel/Entity/Card/CardEntity.cs) | Class | 卡片戰鬥實體 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成