# PlayerData 玩家資料類別筆記

## 類別概述
PlayerData是MortalGame中定義玩家基礎戰鬥數據的ScriptableAsset類別。它包含了玩家的核心屬性、卡牌配置和本地化資訊，是構建戰鬥實體的數據基礎。透過AllyData和EnemyData的擴展，形成了完整的友軍/敵軍資料體系。

**設計理念**：
- **資料驅動**：所有玩家屬性都可在編輯器中配置
- **可復用性**：基礎資料可被友軍和敵軍共同使用
- **擴展性**：支援透過繼承添加特化功能
- **編輯器友好**：使用Odin Inspector提供優秀的編輯體驗

**檔案位置**：
- [PlayerData.cs](../../Assets/Scripts/GameData/PlayerData.cs)
- [AllyData.cs](../../Assets/Scripts/GameData/AllyData.cs) 
- [EnemyData.cs](../../Assets/Scripts/GameData/EnemyData.cs)

## PlayerData 基礎類別

### 類別結構
```csharp
public class PlayerData
{
    [BoxGroup("Identification")]
    public string ID;                           // 唯一識別符

    [TitleGroup("BasicData")]
    public int MaxHealth;                       // 最大生命值
    public int MaxEnergy;                       // 最大能量值
    [PropertyRange(0, "MaxHealth")]
    public int InitialHealth;                   // 初始生命值
    [PropertyRange(0, "MaxEnergy")]
    public int InitialEnergy;                   // 初始能量值

    [BoxGroup("Cards")]
    public DeckScriptable Deck;                 // 牌組配置
    public int HandCardMaxCount;                // 手牌上限

    [TitleGroup("Localization")]
    public string NameKey;                      // 名稱本地化鍵值
}
```

### 資料分組詳解

#### 🆔 識別資訊 (Identification)
- **ID**: 玩家的唯一識別符
- 用於引用特定的玩家配置
- 支援資料庫查詢和關聯

#### 📊 基礎數據 (BasicData)
| 欄位 | 類型 | 驗證規則 | 描述 |
|------|------|----------|------|
| **MaxHealth** | int | > 0 | 玩家的最大生命值上限 |
| **MaxEnergy** | int | > 0 | 玩家的最大能量值上限 |
| **InitialHealth** | int | 0 ~ MaxHealth | 戰鬥開始時的初始生命值 |
| **InitialEnergy** | int | 0 ~ MaxEnergy | 戰鬥開始時的初始能量值 |

**設計特色**：
- **動態驗證**：`[PropertyRange(0, "MaxHealth")]`確保數值合理性
- **靈活配置**：初始值可以低於最大值，支援受傷開局等設計
- **平衡考量**：分離最大值和初始值，便於難度調整

#### 🃏 卡牌系統 (Cards)
```csharp
[BoxGroup("Cards")]
public DeckScriptable Deck;          // 牌組配置
public int HandCardMaxCount;         // 手牌容量上限
```

**功能說明**：
- **Deck**: 引用具體的牌組ScriptableObject
- **HandCardMaxCount**: 限制手牌數量，影響戰術選擇
- **擴展性**: 為未來的多牌組系統預留空間

#### 🌍 本地化 (Localization)
```csharp
[TitleGroup("Localization")]
public string NameKey;               // 本地化名稱鍵值
```

**本地化策略**：
- 使用鍵值而非直接文字，支援多語言
- 配合本地化系統提供動態語言切換
- 🔗*需要LocalizeLibrary_Class.md*

## AllyData 友軍資料

### 類別結構
```csharp
public class AllyData
{
    [BoxGroup("AllyOnly")]
    public string GameMode;             // 遊戲模式標識
    [BoxGroup("AllyOnly")]
    [Range(0, 10)]
    public int InitialDisposition;      // 初始好感度（0-10）

    public PlayerData PlayerData;       // 基礎玩家資料
}
```

### 友軍專屬功能

#### 🎮 遊戲模式系統
```csharp
public string GameMode;
```
- 標識當前的遊戲模式（如"Story"、"Arena"、"Tutorial"等）
- 用於切換不同的遊戲邏輯和UI呈現
- 支援多種遊戲模式的配置

#### 💖 好感度系統
```csharp
[Range(0, 10)]
public int InitialDisposition;
```

**好感度機制**：
- **範圍**: 0-10的整數值
- **用途**: 影響特定卡片效果和劇情發展
- **初始化**: 在戰鬥開始時設定基礎好感度
- **動態變化**: 戰鬥中的行為會影響好感度數值

### 組合設計模式
```csharp
public PlayerData PlayerData;        // 包含所有基礎功能
```

**設計優勢**：
- **代碼復用**: 避免重複定義PlayerData中的欄位
- **維護性**: PlayerData的修改會自動影響AllyData
- **清晰分離**: 友軍專屬功能與基礎功能明確分離

## EnemyData 敵軍資料

### 類別結構
```csharp
public class EnemyData
{
    [BoxGroup("EnemyOnly")]
    public string EnemyID;              // 敵人唯一標識
    public int Level;                   // 敵人等級
    public int SelectedCardMaxCount;    // AI選卡上限
    public int TurnStartDrawCardCount;  // 回合開始抽牌數
    public int EnergyRecoverPoint;      // 能量回復點數

    public PlayerData PlayerData;       // 基礎玩家資料
}
```

### 敵軍專屬功能

#### 🤖 AI系統配置
| 欄位 | 類型 | 功能 | 描述 |
|------|------|------|------|
| **EnemyID** | string | 識別 | 敵人的唯一標識符 |
| **Level** | int | 難度 | 敵人等級，影響AI決策強度 |
| **SelectedCardMaxCount** | int | AI限制 | AI每回合最多選擇的卡片數量 |
| **TurnStartDrawCardCount** | int | 抽牌 | 每回合開始時的抽牌數量 |
| **EnergyRecoverPoint** | int | 資源 | 每回合的能量回復點數 |

#### 🎯 AI行為調整
```csharp
// AI選卡限制
public int SelectedCardMaxCount;     // 限制AI的選卡數量，避免過度優化

// 資源管理
public int TurnStartDrawCardCount;   // 控制AI的手牌補充速度
public int EnergyRecoverPoint;       // 調整AI的能量經濟
```

**平衡設計**：
- **SelectedCardMaxCount**: 防止AI進行完美優化，保持遊戲趣味性
- **動態難度**: 透過調整數值實現不同難度的敵人
- **策略多樣性**: 不同的配置創造不同的AI行為模式

## Odin Inspector 整合

### 編輯器視覺化
PlayerData系列大量使用Odin Inspector特性：

#### 分組管理
```csharp
[BoxGroup("Identification")]    // 識別資訊框組
[TitleGroup("BasicData")]       // 基礎數據標題組
[BoxGroup("Cards")]             // 卡牌系統框組
[BoxGroup("AllyOnly")]          // 友軍專屬框組
[BoxGroup("EnemyOnly")]         // 敵軍專屬框組
```

#### 數值驗證
```csharp
[PropertyRange(0, "MaxHealth")]  // 動態範圍驗證
[Range(0, 10)]                  // 靜態範圍限制
```

**編輯器優勢**：
- **視覺化分組**: 相關欄位組織清晰
- **動態驗證**: 防止配置錯誤
- **即時反饋**: 修改時立即顯示驗證結果

## 資料流程分析

### 設計階段流程
```
遊戲設計師 → Unity編輯器 → PlayerData.asset
                        → AllyData.asset  
                        → EnemyData.asset
```

### 運行時轉換流程
```
PlayerData系列 → PlayerEntity創建 → 戰鬥實體初始化
```

### 配置繼承關係
```
PlayerData (基礎配置)
├── AllyData (+ 好感度 + 遊戲模式)
└── EnemyData (+ AI配置 + 等級系統)
```

## 設計模式應用

### 🔧 組合模式 (Composition Pattern)
```csharp
public class AllyData
{
    public PlayerData PlayerData;   // 組合而非繼承
}
```

**優勢**：
- 避免深度繼承層次
- 保持PlayerData的獨立性
- 支援運行時動態配置

### 📋 模板方法模式 (Template Method Pattern)
- PlayerData定義通用模板
- AllyData和EnemyData提供特化實現
- 統一的配置流程和驗證邏輯

### 🎭 策略模式 (Strategy Pattern)
- 不同的EnemyData配置代表不同的AI策略
- 通過數據配置而非代碼實現策略變化

## 擴展性設計

### 🚀 未來擴展可能性

#### 多角色支援
```csharp
// 未來可能的擴展
public class PlayerData
{
    public CharacterData[] Characters;  // 支援多角色配置
    public FormationData Formation;     // 角色陣型配置
}
```

#### 進階AI配置
```csharp
// 敵軍AI策略擴展
public class EnemyData
{
    public AIPersonalityData Personality;  // AI性格配置
    public DifficultyScalingData Scaling;  // 動態難度調整
}
```

#### 動態屬性系統
```csharp
// 屬性修正系統
public class PlayerData
{
    public List<IAttributeModifier> Modifiers;  // 屬性修正器
}
```

## 平衡性考量

### 數值設計原則
- **初始值 ≤ 最大值**: 確保數值邏輯一致性
- **合理範圍**: 好感度0-10的直觀範圍
- **可調節性**: 所有數值都可在編輯器中調整

### AI平衡機制
- **選卡限制**: 防止AI過度優化
- **資源控制**: 平衡AI的經濟能力
- **等級系統**: 提供漸進式難度曲線

## 依賴關係

### 依賴的組件
- **🔗 DeckScriptable**: 牌組資料 *需要DeckScriptable_Class.md*
- **🔗 Odin Inspector**: 編輯器增強功能
- **🔗 Unity ScriptableObject**: 資產系統基礎

### 被依賴的組件
- **🔗 PlayerEntity**: 使用PlayerData創建實體 *需要PlayerEntity_Class.md*
- **🔗 遊戲初始化**: 載入資料進行遊戲配置
- **🔗 關卡系統**: 選擇對應的敵人配置 *需要LevelSystem_Class.md*

## 使用範例

### 基礎PlayerData配置
```csharp
// 在Unity編輯器中配置PlayerData
var playerData = CreateInstance<PlayerData>();
playerData.ID = "hero_001";
playerData.MaxHealth = 100;
playerData.MaxEnergy = 3;
playerData.InitialHealth = 100;
playerData.InitialEnergy = 1;
playerData.HandCardMaxCount = 7;
playerData.NameKey = "hero.protagonist.name";
```

### AllyData配置
```csharp
var allyData = CreateInstance<AllyData>();
allyData.GameMode = "Story";
allyData.InitialDisposition = 5;
allyData.PlayerData = playerData;  // 引用基礎配置
```

### EnemyData配置
```csharp
var enemyData = CreateInstance<EnemyData>();
enemyData.EnemyID = "bandit_001";
enemyData.Level = 1;
enemyData.SelectedCardMaxCount = 2;
enemyData.TurnStartDrawCardCount = 1;
enemyData.EnergyRecoverPoint = 1;
enemyData.PlayerData = playerData;  // 引用基礎配置
```

### 運行時數據獲取
```csharp
// 獲取玩家基礎數據
var maxHealth = allyData.PlayerData.MaxHealth;
var initialEnergy = allyData.PlayerData.InitialEnergy;
var handCardMax = allyData.PlayerData.HandCardMaxCount;

// 獲取友軍專屬數據
var gameMode = allyData.GameMode;
var disposition = allyData.InitialDisposition;

// 獲取敵軍AI配置
var enemyLevel = enemyData.Level;
var aiSelectLimit = enemyData.SelectedCardMaxCount;
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [PlayerData.cs](../../Assets/Scripts/GameData/PlayerData.cs) | 核心 | 基礎玩家資料定義 |
| [AllyData.cs](../../Assets/Scripts/GameData/AllyData.cs) | 核心 | 友軍資料擴展 |
| [EnemyData.cs](../../Assets/Scripts/GameData/EnemyData.cs) | 核心 | 敵軍資料擴展 |
| [PlayerEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerEntity.cs) | 被依賴 | 使用這些資料創建實體 |
| DeckScriptable_Class.md | 依賴 | 牌組配置系統 |
| LocalizeLibrary_Class.md | 依賴 | 本地化系統 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成