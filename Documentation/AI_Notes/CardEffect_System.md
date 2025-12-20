# CardEffect 卡片效果系統筆記

## 系統概述
卡片效果系統是MortalGame的核心機制之一，定義了所有卡片可以產生的效果。系統採用接口導向設計，透過`ICardEffect`接口實現高度模組化的效果配置，支援複雜的卡片行為組合。

**檔案位置**: [Assets/Scripts/GameData/Card/CardEffect.cs](../../../Assets/Scripts/GameData/Card/CardEffect.cs)

## 系統架構

### 核心接口
```csharp
public interface ICardEffect
{
    // 標記接口，所有卡片效果的基礎
}
```

**設計理念**:
- 使用標記接口(Marker Interface)模式
- 支援多型性和類型安全
- 便於序列化和編輯器整合

## 效果分類架構

### 📊 按目標類型分類

```
ICardEffect
├── Target-Character Effects    (角色目標效果)
├── Target-Player Effects       (玩家目標效果)
└── Target-Card Effects         (卡片目標效果)
```

### 🎯 Target-Character Effects (角色目標效果)

#### 傷害類效果
```csharp
[Serializable]
public class DamageEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;  // 目標角色集合
    public IIntegerValue Value;                       // 傷害數值
}
```

```csharp
[Serializable]
public class PenetrateDamageEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;  // 目標角色集合
    public IIntegerValue Value;                       // 穿透傷害數值
}
```

**穿透傷害 vs 一般傷害**:
- **DamageEffect**: 可被護盾阻擋的一般傷害
- **PenetrateDamageEffect**: 無視護盾的穿透傷害

#### 攻擊強化類效果
```csharp
[Serializable]
public class AdditionalAttackEffect : ICardEffect 
{
    public ITargetCharacterCollectionValue Targets;  // 目標角色集合
    public IIntegerValue Value;                       // 額外攻擊次數
}
```

```csharp
[Serializable]
public class EffectiveAttackEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;  // 目標角色集合
    public IIntegerValue Value;                       // 有效攻擊加成
}
```

#### 防護和回復類效果
```csharp
[Serializable]
public class ShieldEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;  // 目標角色集合
    public IIntegerValue Value;                       // 護盾數值
}
```

```csharp
[Serializable]
public class HealEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;  // 目標角色集合
    public IIntegerValue Value;                       // 治療數值
}
```

### 👤 Target-Player Effects (玩家目標效果)

#### 能量管理效果
```csharp
[Serializable]
public class GainEnergyEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;     // 目標玩家集合
    public IIntegerValue Value;                       // 獲得能量數值
}
```

```csharp
[Serializable]
public class LoseEnegyEffect : ICardEffect  // 注意：原碼中有拼寫錯誤
{
    public ITargetPlayerCollectionValue Targets;     // 目標玩家集合
    public IIntegerValue Value;                       // 失去能量數值
}
```

**注意**: `LoseEnegyEffect`在原碼中有拼寫錯誤，應為`LoseEnergyEffect`

#### 玩家Buff管理效果
```csharp
[Serializable]
public class AddPlayerBuffEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;     // 目標玩家集合
    [ValueDropdown("@DropdownHelper.PlayerBuffNames")]
    public string BuffId;                             // Buff ID (下拉選單)
    public IIntegerValue Level;                       // Buff等級
}
```

```csharp
[Serializable]
public class RemovePlayerBuffEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;     // 目標玩家集合
    [ValueDropdown("@DropdownHelper.PlayerBuffNames")]
    public string BuffId;                             // 要移除的Buff ID
}
```

#### 好感度系統效果
```csharp
[Serializable]
public class IncreaseDispositionEffect : ICardEffect
{
    // Only Ally Has Disposition
    public ITargetPlayerCollectionValue Targets;     // 目標玩家 (僅友軍有好感度)
    public IIntegerValue Value;                       // 增加好感度數值
}
```

```csharp
[Serializable]
public class DecreaseDispositionEffect : ICardEffect
{
    // Only Ally Has Disposition  
    public ITargetPlayerCollectionValue Targets;     // 目標玩家 (僅友軍有好感度)
    public IIntegerValue Value;                       // 減少好感度數值
}
```

### 🃏 Target-Card Effects (卡片目標效果)

#### 卡片流轉效果
```csharp
[Serializable]
public class DrawCardEffect : ICardEffect
{
    public ITargetPlayerCollectionValue Targets;     // 抽牌的玩家
    public IIntegerValue Value;                       // 抽牌數量
}
```

```csharp
[Serializable]
public class DiscardCardEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;   // 要丟棄的卡片
}
```

#### 卡片狀態變更效果
```csharp
[Serializable]
public class ConsumeCardEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;   // 要消耗的卡片
}
```

```csharp
[Serializable]
public class DisposeCardEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;   // 要銷毀的卡片
}
```

#### 卡片生成效果
```csharp
[Serializable]
public class CreateCardEffect : ICardEffect
{
    public ITargetPlayerValue Target;                 // 目標玩家
    [ShowInInspector]
    public List<string> CardDataIds;                  // 要創建的卡片ID列表
    [ShowInInspector]
    public List<AddCardBuffData> AddCardBuffDatas;    // 創建時附加的Buff
    public CardCollectionType CreateDestination;      // 創建到哪個卡片集合
}
```

#### 卡片克隆效果
```csharp
[Serializable]
public class CloneCardEffect : ICardEffect
{
    public ITargetPlayerValue Target;                 // 目標玩家
    public ITargetCardCollectionValue ClonedCards;    // 要克隆的卡片
    [ShowInInspector]
    public List<AddCardBuffData> AddCardBuffDatas;    // 克隆時附加的Buff
    public CardCollectionType CloneDestination;       // 克隆到哪個卡片集合
}
```

#### 卡片強化效果
```csharp
[Serializable]
public class AddCardBuffEffect : ICardEffect
{
    public ITargetCardCollectionValue TargetCards;    // 目標卡片集合
    [ShowInInspector]
    public List<AddCardBuffData> AddCardBuffDatas;    // 要添加的Buff資料
}
```

## 目標系統整合

### 目標值接口
卡片效果系統與目標系統緊密整合：

- **`ITargetCharacterCollectionValue`**: 角色目標集合
- **`ITargetPlayerCollectionValue`**: 玩家目標集合  
- **`ITargetCardCollectionValue`**: 卡片目標集合
- **`ITargetPlayerValue`**: 單一玩家目標
- **`IIntegerValue`**: 整數數值

🔗*需要Target_System.md和Value_System.md*

### 數值系統整合
- **`IIntegerValue`**: 支援動態數值計算
- 可以是固定值或計算值
- 支援上下文相關的數值解析

## Odin Inspector 整合

### 編輯器友好特性
```csharp
[ShowInInspector]
public List<string> CardDataIds = new ();    // 顯示通常不序列化的欄位

[ValueDropdown("@DropdownHelper.PlayerBuffNames")]
public string BuffId;                         // 下拉選單選擇Buff

[ShowInInspector]
public List<AddCardBuffData> AddCardBuffDatas; // 複雜物件編輯
```

### 視覺化配置
- 使用Odin Inspector提供更好的編輯器體驗
- 支援複雜資料結構的視覺化編輯
- 動態下拉選單提升配置效率

## 設計模式應用

### 🏗️ 策略模式 (Strategy Pattern)
```csharp
public interface ICardEffect  // 策略接口
{
    // 所有效果實現此接口
}

// 具體策略
public class DamageEffect : ICardEffect { ... }
public class HealEffect : ICardEffect { ... }
```

### 🎯 命令模式 (Command Pattern)
每個效果類別都封裝了一個特定的行為：
- 效果類別 = 命令
- 目標和數值 = 命令參數
- 效果執行器 = 命令調用者

### 🔧 組合模式 (Composite Pattern)
```csharp
// 卡片可以組合多個效果
public List<ICardEffect> Effects;
public List<TriggeredCardEffect> TriggeredEffects;
```

## 系統特性分析

### 🎲 效果類型統計
| 類型 | 數量 | 主要用途 |
|------|------|----------|
| **角色目標** | 6 | 戰鬥核心機制 |
| **玩家目標** | 6 | 資源和狀態管理 |
| **卡片目標** | 6 | 牌組操作和卡片流轉 |
| **總計** | 18 | 完整的遊戲機制覆蓋 |

### ⚡ 效果複雜度分析
- **簡單效果**: DamageEffect, HealEffect等 (單一目標+數值)
- **中等複雜度**: AddPlayerBuffEffect (目標+ID+等級)
- **高複雜度**: CreateCardEffect, CloneCardEffect (多參數配置)

### 🔄 生命週期管理
- **立即效果**: 卡片使用時立即執行
- **觸發效果**: 特定條件滿足時執行
- **持續效果**: 透過Buff系統實現

## 擴展性設計

### 📈 新增效果的步驟
1. 創建實現`ICardEffect`的新類別
2. 定義所需的目標和參數
3. 添加Odin Inspector特性(如需要)
4. 在🔗*需要EffectExecutor_Class.md*中實現執行邏輯

### 🔧 參數化設計
```csharp
// 範例：可參數化的效果設計
public class DamageEffect : ICardEffect
{
    public ITargetCharacterCollectionValue Targets;  // 支援多種目標選擇
    public IIntegerValue Value;                       // 支援動態數值計算
}
```

## 依賴關係

### 依賴的系統
- **🔗 Target系統**: 提供目標選擇功能 *需要Target_System.md*
- **🔗 Value系統**: 提供數值計算功能 *需要Value_System.md*
- **🔗 CardEnum**: 使用CardCollectionType *參考CardEnum_Reference.md*
- **🔗 Buff系統**: 提供AddCardBuffData *需要CardBuff_System.md*

### 被依賴的系統
- **🔗 CardData**: 配置效果列表 *參考CardData_Class.md*
- **🔗 EffectExecutor**: 執行效果邏輯 *需要EffectExecutor_Class.md*
- **🔗 GameplayManager**: 觸發效果執行 *需要GameplayManager_Class.md*

## 使用範例

### 創建基本傷害效果
```csharp
var damageEffect = new DamageEffect
{
    Targets = new SingleEnemyTarget(),  // 🔗需要Target_System.md
    Value = new FixedIntegerValue(5)    // 🔗需要Value_System.md
};
```

### 配置複雜創建效果
```csharp
var createEffect = new CreateCardEffect
{
    Target = new SelfPlayer(),
    CardDataIds = new List<string> { "fireball", "heal_potion" },
    AddCardBuffDatas = new List<AddCardBuffData>
    {
        new AddCardBuffData { /* Buff配置 */ }
    },
    CreateDestination = CardCollectionType.HandCard
};
```

### 組合多重效果
```csharp
var cardData = new CardData
{
    Effects = new List<ICardEffect>
    {
        new DamageEffect { /* 配置 */ },
        new DrawCardEffect { /* 配置 */ }
    }
};
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CardData.cs](../../../Assets/Scripts/GameData/Card/CardData.cs) | 被依賴 | 使用效果接口配置卡片行為 |
| [CardEnum.cs](../../../Assets/Scripts/GameData/Card/CardEnum.cs) | 依賴 | 使用CardCollectionType |
| Target_System.md | 依賴 | 提供目標選擇功能 |
| Value_System.md | 依賴 | 提供數值計算功能 |
| EffectExecutor_Class.md | 被依賴 | 執行具體效果邏輯 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成