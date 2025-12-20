# CharacterBuff 角色Buff系統筆記

## 系統概述
CharacterBuff系統是作用於Character身上的狀態效果系統，類似於CardBuff作用於卡片的方式。Character可以擁有多種Buff，這些Buff會影響角色的血量、護甲、行為等屬性，並具有完整的生命週期管理。CharacterBuff系統採用Data-Entity雙層架構，支援複雜的屬性修正、反應會話和條件觸發機制。

**設計核心**：
- **狀態豐富**：支援複雜的角色狀態效果
- **生命週期**：完整的Buff創建、更新、過期管理
- **層數系統**：支援可疊加的Buff效果
- **反應機制**：支援Buff間的互動邏輯
- **屬性修正**：動態修改角色屬性

**檔案位置**: 
- [CharacterBuffData.cs](../../Assets/Scripts/GameData/CharacterBuff/CharacterBuffData.cs)
- [CharacterBuffEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterBuff/CharacterBuffEntity.cs)

## 與CardBuff系統對比

### 設計理念對比
| 特性 | CardBuff | CharacterBuff |
|------|----------|---------------|
| **作用目標** | 卡片實體 | 角色實體 |
| **影響屬性** | 攻擊力、消耗、效果強度 | 血量上限、護甲、傷害減免 |
| **生命週期** | 跟隨卡片生命週期 | 獨立的持續時間管理 |
| **觸發時機** | 卡片使用、在手時 | 回合開始/結束、受傷時 |
| **層數疊加** | 修改數值效果 | 狀態強度累積 |
| **施術者** | 通常無施術者概念 | 記錄施放Buff的玩家 |

### 應用場景對比
```csharp
// CardBuff 範例：強化卡片
CardBuff 強化：使此卡片攻擊力+2
CardBuff 減費：使此卡片消耗-1

// CharacterBuff 範例：狀態效果
CharacterBuff 中毒：每回合結束受到3點傷害
CharacterBuff 護甲：增加5點護甲值
CharacterBuff 虛弱：受到傷害時額外+50%
```

## CharacterBuffData 資料定義

### 核心結構
```csharp
public class CharacterBuffData
{
    [TitleGroup("BasicData")]
    public string ID;               // Buff唯一標識
    public int MaxLevel;            // 最大疊加層數
    
    [ShowInInspector]
    [BoxGroup("Effects")]
    public Dictionary<string, IReactionSessionData> Sessions;       // 反應會話系統
    
    [BoxGroup("Effects")]
    public Dictionary<GameTiming, ConditionalCharacterBuffEffect[]> BuffEffects; // 時機效果
    
    [BoxGroup("Properties")]
    public List<ICharacterBuffPropertyData> PropertyDatas;         // 屬性修正
    
    [BoxGroup("LifeTime")]
    public ICharacterBuffLifeTimeData LifeTimeData;               // 生命週期
}
```

### 基礎資料設計

#### 🆔 Buff標識系統
```csharp
[TitleGroup("BasicData")]
public string ID;           // 如 "poison", "armor_boost", "weakness"
public int MaxLevel;        // 如 5（最多疊加5層）
```

**標識特色**：
- **唯一性**：ID確保Buff類型的唯一識別
- **層數限制**：MaxLevel防止無限疊加導致的數值爆炸
- **分組展示**：Odin Inspector的TitleGroup提供清晰的編輯界面

#### 🔄 反應會話系統
```csharp
[ShowInInspector]
[BoxGroup("Effects")]
public Dictionary<string, IReactionSessionData> Sessions;
```

**反應會話特色**：
- **互動邏輯**：處理Buff之間的相互作用
- **會話管理**：每個會話有獨立的狀態和數值
- **動態響應**：可以根據遊戲狀態動態調整
- **擴展性**：支援複雜的Buff組合效果

#### ⏰ 時機效果系統
```csharp
[BoxGroup("Effects")]
public Dictionary<GameTiming, ConditionalCharacterBuffEffect[]> BuffEffects;
```

**時機效果特色**：
- **GameTiming驅動**：在特定時機觸發效果
- **條件判斷**：ConditionalCharacterBuffEffect支援條件邏輯
- **多效果**：每個時機可配置多個效果
- **靈活組合**：不同時機的效果可以組合使用

**可能的GameTiming**：
```csharp
// 推測的時機類型
GameTiming.TurnStart        // 回合開始
GameTiming.TurnEnd          // 回合結束
GameTiming.TakeDamage       // 受到傷害時
GameTiming.BeforeAttack     // 攻擊前
GameTiming.AfterAttack      // 攻擊後
GameTiming.PlayCard         // 使用卡片時
```

#### 📊 屬性修正系統
```csharp
[BoxGroup("Properties")]
public List<ICharacterBuffPropertyData> PropertyDatas;
```

**屬性修正特色**：
- **數值影響**：修改角色的各種數值屬性
- **類型多樣**：支援不同類型的屬性修正
- **計算整合**：與角色的屬性計算系統整合
- **動態更新**：屬性變化會即時反映

**可能的屬性類型**：
```csharp
// 推測的屬性修正類型
MaxHealthProperty      // 最大血量修正
ArmorProperty         // 護甲值修正
DamageReductionProperty // 傷害減免
HealingBonusProperty   // 治療加成
SpeedProperty         // 行動速度
```

#### ⏳ 生命週期管理
```csharp
[BoxGroup("LifeTime")]
public ICharacterBuffLifeTimeData LifeTimeData;
```

**生命週期特色**：
- **持續時間**：控制Buff的存在時間
- **觸發條件**：基於條件的生命週期管理
- **自動清理**：過期Buff的自動移除
- **靈活配置**：支援不同的生命週期策略

---

## CharacterBuffProperty 屬性系統詳解

### 屬性系統架構
```
ICharacterBuffPropertyData (資料層)
        ↓ CreateEntity()
ICharacterBuffPropertyEntity (實體層)
```

**檔案位置**：
- **Data層**: [CharacterBuffPropertyData.cs](../../Assets/Scripts/GameData/CharacterBuff/CharacterBuffPropertyData.cs)
- **Entity層**: [CharacterBuffPropertyEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterBuff/CharacterBuffPropertyEntity.cs)

### 屬性資料定義 (CharacterBuffPropertyData)

#### 核心接口
```csharp
public interface ICharacterBuffPropertyData
{
    ICharacterBuffPropertyEntity CreateEntity(TriggerContext triggerContext);
}
```

#### 最大血量屬性
```csharp
[Serializable]
public class MaxHealthPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(TriggerContext triggerContext)
    {
        return new MaxHealthPropertyCharacterBuffEntity();
    }
}
```

#### 最大能量屬性
```csharp
[Serializable]
public class MaxEnergyPropertyCharacterBuffData : ICharacterBuffPropertyData
{
    public ICharacterBuffPropertyEntity CreateEntity(TriggerContext triggerContext)
    {
        return new MaxEnergyPropertyCharacterBuffEntity();
    }
}
```

### 屬性實體實現 (CharacterBuffPropertyEntity)

#### 屬性實體接口
```csharp
public interface ICharacterBuffPropertyEntity
{
    CharacterBuffProperty Property { get; }     // 屬性類型標識
    
    int Eval(IGameplayModel gameWatcher, ITriggeredSource triggerSource);  // 數值計算
}
```

#### 最大血量屬性實體
```csharp
public class MaxHealthPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxHealth;

    public MaxHealthPropertyCharacterBuffEntity() { }
    
    public int Eval(IGameplayModel gameWatcher, ITriggeredSource triggerSource) => 0;
}
```

#### 最大能量屬性實體
```csharp
public class MaxEnergyPropertyCharacterBuffEntity : ICharacterBuffPropertyEntity
{
    public CharacterBuffProperty Property => CharacterBuffProperty.MaxEnergy;

    public MaxEnergyPropertyCharacterBuffEntity() { }
    
    public int Eval(IGameplayModel gameWatcher, ITriggeredSource triggerSource) => 0;
}
```

### CharacterBuffProperty 枚舉
```csharp
public enum CharacterBuffProperty
{
    MaxHealth,    // 最大血量修正
    MaxEnergy,    // 最大能量修正
    // 更多屬性類型...
}
```

### 屬性系統特色

#### 🎯 策略模式應用
- **多態實現**：不同屬性類型有不同的計算邏輯
- **開放擴展**：新增屬性類型只需實現對應接口
- **統一調用**：統一的Eval方法進行數值計算
- **類型安全**：枚舉確保屬性類型的一致性

#### 📊 動態數值計算
```csharp
public int Eval(IGameplayModel gameWatcher, ITriggeredSource triggerSource)
```
- **上下文感知**：基於遊戲狀態進行計算
- **觸發源考慮**：根據觸發源調整計算邏輯
- **實時更新**：支援動態的數值重新計算
- **彈性配置**：可以根據複雜條件調整數值

#### 🔧 工廠模式集成
- **Data→Entity轉換**：透過CreateEntity工廠方法創建實體
- **上下文傳遞**：創建時傳入必要的觸發上下文
- **類型確保**：確保創建正確的屬性實體類型
- **資源管理**：統一的創建和銷毀管理

---

## CharacterBuffLifeTime 生命週期系統詳解

### 生命週期系統架構
```
ICharacterBuffLifeTimeData (資料層)
        ↓ CreateEntity()
ICharacterBuffLifeTimeEntity (實體層)
```

**檔案位置**：
- **Data層**: [CharacterBuffLifeTimeData.cs](../../Assets/Scripts/GameData/CharacterBuff/CharacterBuffLifeTimeData.cs)
- **Entity層**: [CharacterBuffLifeTimeEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterBuff/CharacterBuffLifeTimeEntity.cs)

### 生命週期資料定義 (CharacterBuffLifeTimeData)

#### 核心接口
```csharp
public interface ICharacterBuffLifeTimeData
{
    ICharacterBuffLifeTimeEntity CreateEntity(TriggerContext triggerContext);
}
```

#### 永久生命週期
```csharp
[Serializable]
public class AlwaysLifeTimeCharacterBuffData : ICharacterBuffLifeTimeData
{
    public ICharacterBuffLifeTimeEntity CreateEntity(TriggerContext triggerContext)
    {
        return new AlwaysLifeTimeCharacterBuffEntity();
    }
}
```

#### 回合限制生命週期
```csharp
[Serializable]
public class TurnLifeTimeCharacterBuffData : ICharacterBuffLifeTimeData
{
    public int Turn;  // 持續回合數

    public ICharacterBuffLifeTimeEntity CreateEntity(TriggerContext triggerContext)
    {
        return new TurnLifeTimeCharacterBuffEntity(Turn);
    }
}
```

### 生命週期實體實現 (CharacterBuffLifeTimeEntity)

#### 生命週期實體接口
```csharp
public interface ICharacterBuffLifeTimeEntity
{
    bool IsExpired();                               // 過期檢查
    bool Update(TriggerContext triggerContext);     // 生命週期更新
}
```

#### 永久生命週期實體
```csharp
public class AlwaysLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    public bool IsExpired()
    {
        return false;  // 永不過期
    }

    public bool Update(TriggerContext triggerContext)
        => false;  // 無需更新
}
```

#### 回合限制生命週期實體
```csharp
public class TurnLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    private int _turn;

    public TurnLifeTimeCharacterBuffEntity(int turn)
    {
        _turn = turn;
    }

    public bool IsExpired()
    {
        return _turn <= 0;
    }

    public bool Update(TriggerContext triggerContext)
    {
        if (triggerContext.Action is UpdateTimingAction timingAction &&
            timingAction.Timing == GameTiming.TurnEnd)
        {
            _turn--;
            return true;  // 回合數有變化
        }
        return false;
    }
}
```

### 生命週期系統特色

#### ⏰ 多樣化生命週期策略
- **永久Buff**：永不過期，適用於被動效果
- **回合限制**：指定回合數後自動過期
- **條件觸發**：基於特定遊戲事件的生命週期
- **複雜組合**：可組合多種過期條件

#### 🔄 智能更新機制
```csharp
public bool Update(TriggerContext triggerContext)
{
    if (triggerContext.Action is UpdateTimingAction timingAction &&
        timingAction.Timing == GameTiming.TurnEnd)
    {
        _turn--;
        return true;  // 返回是否有變化
    }
    return false;
}
```

**更新特色**：
- **事件驅動**：基於具體的遊戲時機觸發更新
- **變化追蹤**：返回布林值指示是否發生變化
- **精確時機**：只在相關的時機進行更新
- **效率優化**：避免不必要的更新操作

#### 🛡️ 安全過期檢查
```csharp
public bool IsExpired()
{
    return _turn <= 0;
}
```

**過期檢查特色**：
- **即時查詢**：隨時可以查詢過期狀態
- **邊界安全**：明確的過期條件判斷
- **性能友好**：簡單快速的檢查邏輯
- **一致性**：統一的過期判斷標準

### 生命週期擴展潛力

#### 複雜條件生命週期
```csharp
// 基於傷害次數的生命週期
public class DamageCountLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    private int _remainingDamageCount;
    
    public bool Update(TriggerContext triggerContext)
    {
        if (triggerContext.Action is TakeDamageAction)
        {
            _remainingDamageCount--;
            return true;
        }
        return false;
    }
    
    public bool IsExpired() => _remainingDamageCount <= 0;
}
```

#### 複合條件生命週期
```csharp
// 多重條件組合的生命週期
public class CompositeLifeTimeCharacterBuffEntity : ICharacterBuffLifeTimeEntity
{
    private readonly List<ICharacterBuffLifeTimeEntity> _conditions;
    private readonly LifeTimeLogic _logic; // AND, OR, XOR
    
    public bool IsExpired()
    {
        return _logic switch
        {
            LifeTimeLogic.AND => _conditions.All(c => c.IsExpired()),
            LifeTimeLogic.OR => _conditions.Any(c => c.IsExpired()),
            _ => _conditions.First().IsExpired()
        };
    }
}
```

---

## CharacterBuffManager 管理器系統

### 接口定義
```csharp
public interface ICharacterBuffManager
{
    // 狀態查詢
    IReadOnlyCollection<ICharacterBuffEntity> Buffs { get; }
    
    // Buff操作
    bool AddBuff(
        TriggerContext triggerContext,
        string buffId,
        int level,
        out ICharacterBuffEntity resultBuff);
    bool RemoveBuff(
        TriggerContext triggerContext,
        string buffId,
        out ICharacterBuffEntity resultBuff);
    
    // 系統更新
    IEnumerable<ICharacterBuffEntity> Update(TriggerContext triggerContext);
}
```

### 核心實現
```csharp
public class CharacterBuffManager : ICharacterBuffManager
{
    private List<ICharacterBuffEntity> _buffs;

    public IReadOnlyCollection<ICharacterBuffEntity> Buffs => _buffs;

    public CharacterBuffManager()
    {
        _buffs = new List<ICharacterBuffEntity>();
    }
}
```

### CharacterBuffManager 獨特設計

#### 🎯 out參數設計
```csharp
public bool AddBuff(
    TriggerContext triggerContext,
    string buffId,
    int level,
    out ICharacterBuffEntity resultBuff)
```

**out參數特色**：
- **雙重返回**：bool表示是否新增，out參數返回實際Buff
- **即時操作**：調用方可以立即操作返回的Buff
- **性能優化**：避免額外的結果類型包裝
- **傳統風格**：使用經典的C#模式

#### 📊 內建BuffLibrary訪問
```csharp
var buffLibrary = triggerContext.Model.ContextManager.CharacterBuffLibrary;
```

**內建訪問特色**：
- **自動獲取**：從ContextManager自動獲取BuffLibrary
- **上下文整合**：與遊戲上下文深度整合
- **簡化調用**：不需要外部傳入Library參數
- **統一管理**：所有CharacterBuff使用同一個Library

#### 🔄 IEnumerable更新返回
```csharp
public IEnumerable<ICharacterBuffEntity> Update(TriggerContext triggerContext)
{
    foreach (var buff in _buffs.ToList())
    {
        var isUpdated = false;
        var triggeredBuff = new CharacterBuffTrigger(buff);
        var updateCharacterBuffContext = triggerContext with { Triggered = triggeredBuff };
        
        foreach (var session in buff.ReactionSessions.Values)
        {
            isUpdated |= session.Update(updateCharacterBuffContext);
        }

        isUpdated |= buff.LifeTime.Update(updateCharacterBuffContext);

        if (isUpdated)
        { 
            yield return buff;
        }
    }
}
```

**IEnumerable返回特色**：
- **延遲執行**：使用yield return提供延遲執行
- **精確追蹤**：只返回實際發生變化的Buff
- **記憶體效率**：避免創建不必要的集合
- **即時反饋**：可以即時處理變化的Buff

### Buff來源追蹤系統

#### 目標角色解析
```csharp
var owner = triggerContext.Action switch
{
    IActionTargetUnit actionTargetUnit => actionTargetUnit.Target switch
    {
        CharacterTarget characterTarget => Option.Some(characterTarget.Character),
        _ => Option.None<ICharacterEntity>()
    },
    _ => Option.None<ICharacterEntity>()
};
```

**目標解析特色**：
- **動作追蹤**：從TriggerContext的Action中解析目標
- **類型安全**：使用Pattern Matching安全解析
- **選項模式**：使用Option處理可能的空值
- **多層解析**：支援複雜的目標類型系統

#### 施術者追蹤
```csharp
var caster = triggerContext.Action.Source switch
{
    CardPlaySource cardSource => cardSource.Card.Owner(triggerContext.Model),
    PlayerBuffSource playerBuffSource => playerBuffSource.Buff.Caster,
    _ => Option.None<IPlayerEntity>()
};
```

**施術者追蹤特色**：
- **來源多樣**：支援卡片或PlayerBuff作為來源
- **鏈式追蹤**：PlayerBuff可以追蹤到原始施術者
- **責任歸屬**：記錄誰施放了這個Buff
- **效果計算**：便於計算基於施術者的效果

### CharacterBuff創建流程

#### 工廠方法創建
```csharp
var buffLibrary = triggerContext.Model.ContextManager.CharacterBuffLibrary;
resultBuff = new CharacterBuffEntity(
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
            session => session.Key,
            session => session.Value.CreateEntity(triggerContext)));
```

**創建流程特色**：
- **完整參數**：提供所有必要的初始化參數
- **唯一ID**：使用Guid.NewGuid()生成唯一實例ID
- **資料驅動**：基於BuffLibrary的配置創建各個組件
- **即時轉換**：將Data層對象轉換為Entity層對象

### 與其他BuffManager對比

| 特性 | CharacterBuffManager | CardBuffManager | PlayerBuffManager |
|------|---------------------|-----------------|-------------------|
| **初始化** | 空列表開始 | 支援初始Buff列表 | 空列表開始 |
| **Library訪問** | 從ContextManager獲取 | 需要外部BuffLibrary | 從ContextManager獲取 |
| **Add返回** | bool + out參數 | 專用Result類型 | 專用Result類型 |
| **Remove返回** | bool + out參數 | 專用Result類型 | 專用Result類型 |
| **Update返回** | IEnumerable（變化的Buff） | bool（是否有變化） | IEnumerable（變化的Buff） |
| **目標解析** | 從Action中解析Character | 卡片自身 | 從Action中解析Player |
| **過期處理** | 在Update中自動處理 | 在Update中處理 | 專用RemoveExpiredBuff |

### 使用範例

#### 給角色添加中毒效果
```csharp
var characterBuffManager = character.BuffManager;

bool isNewBuff = characterBuffManager.AddBuff(
    triggerContext,
    "poison",           // 中毒Buff ID
    2,                  // 2層中毒
    out var poisonBuff
);

if (isNewBuff)
{
    Debug.Log($"角色中毒：{poisonBuff.Level}層");
}
else
{
    Debug.Log($"中毒加深至：{poisonBuff.Level}層");
}
```

#### 回合結束時更新角色Buff
```csharp
// 回合結束時觸發所有角色的Buff更新
var turnEndContext = new TriggerContext(/* ... */);

foreach (var character in battleField.Characters)
{
    var updatedBuffs = character.BuffManager.Update(turnEndContext);
    
    foreach (var updatedBuff in updatedBuffs)
    {
        // 處理具體的Buff效果
        ProcessBuffEffect(character, updatedBuff, turnEndContext);
        
        // 檢查角色是否因為Buff效果死亡
        if (character.CurrentHealth <= 0)
        {
            HandleCharacterDeath(character);
            break;
        }
    }
}
```

#### 移除特定Buff
```csharp
// 淨化效果：移除角色身上的負面Buff
bool removed = character.BuffManager.RemoveBuff(
    triggerContext,
    "weakness",         // 虛弱效果
    out var removedBuff
);

if (removed)
{
    Debug.Log($"移除虛弱效果：{removedBuff.Level}層");
    
    // 觸發移除Buff的後續效果
    TriggerBuffRemovedEffect(character, removedBuff, triggerContext);
}
```

#### Buff效果計算
```csharp
// 計算角色的實際血量上限（包含Buff效果）
public int CalculateMaxHealth(ICharacterEntity character, TriggerContext context)
{
    int baseHealth = character.BaseMaxHealth;
    int buffBonus = 0;
    
    foreach (var buff in character.BuffManager.Buffs)
    {
        foreach (var property in buff.Properties)
        {
            if (property is MaxHealthProperty healthProperty)
            {
                buffBonus += healthProperty.GetBonus(buff.Level, context);
            }
        }
    }
    
    return Math.Max(1, baseHealth + buffBonus); // 至少保持1點血量
}
```

#### Buff間的互動處理
```csharp
// 處理Buff間的相互作用（如免疫效果）
public bool CanApplyBuff(ICharacterEntity character, string buffId)
{
    foreach (var existingBuff in character.BuffManager.Buffs)
    {
        // 檢查是否有免疫特定Buff的效果
        if (existingBuff.HasImmunity(buffId))
        {
            return false;
        }
        
        // 檢查是否有互斥的Buff
        if (existingBuff.ConflictsWith(buffId))
        {
            return false;
        }
    }
    
    return true;
}
```

---

## CharacterBuffEntity 實體狀態

### 接口定義
```csharp
public interface ICharacterBuffEntity
{
    // 基礎資訊
    string Id { get; }              // Buff類型標識
    Guid Identity { get; }          // 實體唯一標識
    int Level { get; }              // 當前層數
    Option<IPlayerEntity> Caster { get; }  // 施術者
    
    // 管理組件
    IReadOnlyCollection<ICharacterBuffPropertyEntity> Properties { get; }
    ICharacterBuffLifeTimeEntity LifeTime { get; }
    IReadOnlyDictionary<string, IReactionSessionEntity> ReactionSessions { get; }
    
    // 狀態操作
    bool IsExpired();               // 過期檢查
    void AddLevel(int level);       // 增加層數
}
```

### 實體實現
```csharp
public class CharacterBuffEntity : ICharacterBuffEntity
{
    // 核心欄位
    private readonly string _id;
    private readonly Guid _identity;
    private int _level;
    private readonly Option<IPlayerEntity> _caster;
    private readonly IReadOnlyList<ICharacterBuffPropertyEntity> _properties;
    private readonly ICharacterBuffLifeTimeEntity _lifeTime;
    private readonly IReadOnlyDictionary<string, IReactionSessionEntity> _reactionSessions;
    
    // 空值物件支援
    public bool IsDummy => this == DummyBuff;
    public static ICharacterBuffEntity DummyBuff = new DummyCharacterBuff();
}
```

### 核心功能設計

#### 🆔 身份管理
```csharp
string Id { get; }              // Buff類型：如 "poison", "armor"
Guid Identity { get; }          // 實體標識：區分同類型的不同實例
```

**身份管理特色**：
- **類型識別**：Id用於識別Buff的類型和效果
- **實例區分**：Identity確保每個Buff實體的唯一性
- **追蹤便利**：便於在複雜的Buff系統中追蹤特定實例

#### 📊 層數管理
```csharp
int Level { get; }              // 當前層數
void AddLevel(int level);       // 增加層數

// 使用範例
public void AddLevel(int level)
{
    _level += level;
    // 可能需要重新計算屬性影響
    // 可能需要觸發層數變化事件
}
```

**層數管理特色**：
- **疊加效果**：支援相同Buff的疊加
- **強度控制**：層數影響Buff的效果強度
- **上限限制**：配合MaxLevel防止無限疊加
- **動態調整**：運行時可以調整層數

#### 👤 施術者追蹤
```csharp
Option<IPlayerEntity> Caster { get; }
```

**施術者追蹤特色**：
- **來源記錄**：記錄施放Buff的玩家
- **所有權歸屬**：某些效果可能與施術者相關
- **互動邏輯**：支援基於施術者的特殊邏輯
- **安全處理**：使用Option處理無施術者的情況

#### ⏰ 生命週期管理
```csharp
ICharacterBuffLifeTimeEntity LifeTime { get; }
bool IsExpired();

// 使用範例
public bool IsExpired()
{
    return _lifeTime.IsExpired();
}
```

**生命週期特色**：
- **過期檢查**：自動判斷Buff是否應該移除
- **時間管理**：支援回合計數、時間計時等
- **條件觸發**：支援基於條件的過期邏輯
- **清理機制**：配合管理器實現自動清理

#### 🔄 反應會話管理
```csharp
IReadOnlyDictionary<string, IReactionSessionEntity> ReactionSessions { get; }
```

**反應會話特色**：
- **狀態維護**：維護與其他系統的互動狀態
- **數值追蹤**：記錄會話相關的數值變化
- **事件響應**：響應特定的遊戲事件
- **複雜邏輯**：支援複雜的Buff互動邏輯

## 空值物件系統

### DummyCharacterBuff 實現
```csharp
public class DummyCharacterBuff : CharacterBuffEntity
{
    public DummyCharacterBuff() : base(
        string.Empty,                           // ID
        Guid.Empty,                            // Identity  
        1,                                     // Level
        Option.None<IPlayerEntity>(),          // Caster
        Enumerable.Empty<ICharacterBuffPropertyEntity>(),  // Properties
        new AlwaysLifeTimeCharacterBuffEntity(),           // LifeTime
        new Dictionary<string, IReactionSessionEntity>()   // Sessions
    ) { }
}

public static ICharacterBuffEntity DummyBuff = new DummyCharacterBuff();
```

**空值物件特色**：
- **永不過期**：AlwaysLifeTimeCharacterBuffEntity確保不會過期
- **無副作用**：所有操作都是安全的空操作
- **接口統一**：實現相同的ICharacterBuffEntity接口
- **調試友好**：便於識別和處理錯誤狀態

### 使用場景
```csharp
// 避免null檢查
public ICharacterBuffEntity GetBuff(string id)
{
    return FindBuff(id) ?? CharacterBuffEntity.DummyBuff;
}

// 安全的方法鏈
var level = character.GetBuff("poison").Level; // 不會拋出異常
```

## 擴展方法系統

### CharacterBuffEntityExtensions
```csharp
public static class CharacterBuffEntityExtensions
{
    // 轉換為顯示資訊
    public static CharacterBuffInfo ToInfo(this ICharacterBuffEntity characterBuff, IGameplayModel gameWatcher)
    
    // 查找Buff的擁有者
    public static Option<IPlayerEntity> Owner(this ICharacterBuffEntity characterBuff, IGameplayModel gameplayWatcher)
}
```

### ToInfo轉換方法
```csharp
public static CharacterBuffInfo ToInfo(this ICharacterBuffEntity characterBuff, IGameplayModel gameWatcher)
{
    return new CharacterBuffInfo(
        characterBuff.Id,
        characterBuff.Identity,
        characterBuff.Level,
        characterBuff.ReactionSessions
            .Where(kvp => kvp.Value.IntegerValue.HasValue)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.IntegerValue.ValueOr(0)));
}
```

**轉換特色**：
- **UI友好**：轉換為適合UI顯示的資料結構
- **數值過濾**：只包含有數值的會話資訊
- **安全處理**：使用ValueOr處理可能的空值
- **資料簡化**：提供簡化的數據視圖

### Owner查詢方法
```csharp
public static Option<IPlayerEntity> Owner(this ICharacterBuffEntity characterBuff, IGameplayModel gameplayWatcher)
{
    // 查找友軍角色
    if (gameplayWatcher.GameStatus.Ally.Characters.Any(c => c.BuffManager.Buffs.Contains(characterBuff)))
        return (gameplayWatcher.GameStatus.Ally as IPlayerEntity).Some();
    
    // 查找敵軍角色
    if (gameplayWatcher.GameStatus.Enemy.Characters.Any(c => c.BuffManager.Buffs.Contains(characterBuff)))
        return (gameplayWatcher.GameStatus.Enemy as IPlayerEntity).Some();
    
    return Option.None<IPlayerEntity>();
}
```

**查詢特色**：
- **全域搜索**：跨所有Character查找Buff的歸屬
- **陣營識別**：確定Buff屬於友軍還是敵軍
- **安全返回**：使用Option處理未找到的情況
- **效率優化**：優先查找友軍，再查找敵軍

## 與其他系統整合

### 與CharacterEntity的整合
```csharp
public class CharacterEntity : ICharacterEntity
{
    private readonly ICharacterBuffManager _buffManager;
    
    public ICharacterBuffManager BuffManager => _buffManager;
    
    // 透過BuffManager管理所有Buff
    // Buff的屬性修正會影響角色的實際屬性
}
```

### 與屬性計算系統整合
```csharp
// 範例：計算受到傷害時的減傷效果
public int CalculateDamageReduction(ICharacterEntity character, int baseDamage)
{
    int totalReduction = 0;
    
    foreach (var buff in character.BuffManager.Buffs)
    {
        foreach (var property in buff.Properties)
        {
            if (property is DamageReductionProperty reductionProperty)
            {
                totalReduction += reductionProperty.GetReduction(buff.Level);
            }
        }
    }
    
    return Math.Max(0, baseDamage - totalReduction);
}
```

### 與生命週期管理整合
```csharp
// 範例：更新角色身上所有Buff的生命週期
public void UpdateCharacterBuffs(ICharacterEntity character, TriggerContext context)
{
    var expiredBuffs = new List<ICharacterBuffEntity>();
    
    foreach (var buff in character.BuffManager.Buffs)
    {
        // 更新Buff的生命週期
        buff.LifeTime.Update(context);
        
        // 檢查是否過期
        if (buff.IsExpired())
        {
            expiredBuffs.Add(buff);
        }
    }
    
    // 移除過期的Buff
    foreach (var expiredBuff in expiredBuffs)
    {
        character.BuffManager.RemoveBuff(expiredBuff.Id);
    }
}
```

## 使用範例

### Buff創建和添加
```csharp
// 創建中毒Buff
var poisonBuffData = GetCharacterBuffData("poison");
var poisonBuff = new CharacterBuffEntity(
    id: "poison",
    identity: Guid.NewGuid(),
    level: 3,  // 3層中毒
    caster: casterPlayer.Some(),
    properties: CreatePoisonProperties(),
    lifeTime: new TurnBasedLifeTime(5), // 持續5回合
    reactionSessions: new Dictionary<string, IReactionSessionEntity>()
);

// 添加到角色
character.BuffManager.AddBuff(poisonBuff);
```

### Buff查詢和操作
```csharp
// 查詢特定Buff
var poisonBuff = character.BuffManager.Buffs
    .FirstOrDefault(b => b.Id == "poison");

if (poisonBuff != null)
{
    Debug.Log($"中毒層數: {poisonBuff.Level}");
    
    // 增加層數
    poisonBuff.AddLevel(1);
    
    // 檢查是否過期
    if (poisonBuff.IsExpired())
    {
        character.BuffManager.RemoveBuff("poison");
    }
}
```

### Buff效果計算
```csharp
// 計算所有Buff提供的血量上限加成
public int GetMaxHealthBonus(ICharacterEntity character)
{
    int bonus = 0;
    
    foreach (var buff in character.BuffManager.Buffs)
    {
        foreach (var property in buff.Properties)
        {
            if (property is MaxHealthProperty healthProperty)
            {
                bonus += healthProperty.GetBonus(buff.Level);
            }
        }
    }
    
    return bonus;
}
```

### Buff資訊顯示
```csharp
// 轉換為UI顯示資訊
public void DisplayCharacterBuffs(ICharacterEntity character, IGameplayModel model)
{
    foreach (var buff in character.BuffManager.Buffs)
    {
        var buffInfo = buff.ToInfo(model);
        
        Debug.Log($"Buff: {buffInfo.Id}");
        Debug.Log($"層數: {buffInfo.Level}");
        Debug.Log($"會話數值: {string.Join(", ", buffInfo.SessionValues)}");
        
        // 查找Buff的來源
        var owner = buff.Owner(model);
        if (owner.HasValue)
        {
            Debug.Log($"來源: {owner.Value.Faction}");
        }
    }
}
```

## 設計模式應用

### 🏗️ 組合模式 (Composite Pattern)
```csharp
CharacterBuffEntity = Id + Level + Caster + Properties + LifeTime + Sessions
```

### 📋 策略模式 (Strategy Pattern)
```csharp
ICharacterBuffLifeTimeEntity → 不同的生命週期策略
ICharacterBuffPropertyEntity → 不同的屬性修正策略
```

### 🚫 空值物件模式 (Null Object Pattern)
```csharp
DummyCharacterBuff → 安全的預設值
AlwaysLifeTimeCharacterBuffEntity → 永不過期的生命週期
```

### 🔍 查詢模式 (Query Pattern)
```csharp
擴展方法提供靈活的查詢和轉換功能
```

### 📝 資料傳輸物件模式 (DTO Pattern)
```csharp
CharacterBuffInfo → UI顯示專用的資料結構
```

## 依賴關係

### 依賴的組件
- **🔗 ICharacterBuffPropertyEntity**: 屬性修正 *需要CharacterBuffProperty_System.md*
- **🔗 ICharacterBuffLifeTimeEntity**: 生命週期 *需要CharacterBuffLifeTime_System.md*
- **🔗 IReactionSessionEntity**: 反應會話 *需要ReactionSession_System.md*
- **🔗 GameTiming**: 遊戲時機 *需要GameTiming_Enum.md*
- **🔗 TriggerContext**: 觸發上下文 *需要TriggerContext_Class.md*
- **🔗 IPlayerEntity**: 施術者 *參考PlayerEntity_Class.md*
- **🔗 IGameplayModel**: 遊戲狀態 *需要GameplayModel_Class.md*
- **🔗 Optional**: 安全空值處理 *需要Optional_Library.md*

### 被依賴的組件
- **🔗 CharacterEntity**: 使用CharacterBuff *參考CharacterEntity_Class.md*
- **🔗 ICharacterBuffManager**: 管理CharacterBuff *需要CharacterBuffManager_Class.md*
- **🔗 CardEffect**: 卡片效果可能創建CharacterBuff *需要CardEffect_System.md*
- **🔗 UI系統**: 顯示Buff狀態 *需要UI_System.md*

## 系統擴展計劃

### 複雜Buff互動
```csharp
// Buff間的相互作用
public interface ICharacterBuffInteraction
{
    bool CanCoexist(ICharacterBuffEntity otherBuff);    // 是否可以共存
    void OnOtherBuffAdded(ICharacterBuffEntity newBuff); // 其他Buff添加時
    void OnOtherBuffRemoved(ICharacterBuffEntity removedBuff); // 其他Buff移除時
}
```

### 觸發式Buff
```csharp
// 基於事件觸發的Buff
public interface ITriggeredCharacterBuff
{
    void OnCharacterTakeDamage(DamageEvent damageEvent);
    void OnCharacterHeal(HealEvent healEvent);
    void OnTurnStart(TurnStartEvent turnStartEvent);
    void OnCardPlayed(CardPlayEvent cardPlayEvent);
}
```

### Buff模板系統
```csharp
// Buff創建模板
public class CharacterBuffTemplate
{
    public string BuffId { get; set; }
    public int DefaultLevel { get; set; }
    public int DefaultDuration { get; set; }
    public ICharacterBuffPropertyData[] PropertyTemplates { get; set; }
    
    public ICharacterBuffEntity CreateBuff(Option<IPlayerEntity> caster)
    {
        // 基於模板創建Buff實例
    }
}
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CharacterBuffData.cs](../../Assets/Scripts/GameData/CharacterBuff/CharacterBuffData.cs) | 核心 | CharacterBuff資料定義 |
| [CharacterBuffEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterBuff/CharacterBuffEntity.cs) | 核心 | CharacterBuff實體實現 |
| [CharacterEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterEntity.cs) | 關聯 | 使用CharacterBuff的Character |
| [CardBuffEntity.cs](../../Assets/Scripts/GameModel/Entity/CardBuff/CardBuffEntity.cs) | 對比 | 類似的Card Buff系統 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成