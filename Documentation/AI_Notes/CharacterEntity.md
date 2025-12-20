# CharacterEntity 角色實體類別筆記

## 類別概述
CharacterEntity是MortalGame戰鬥系統中具有血量的核心戰鬥單位。每個Character代表一個獨立的戰鬥實體，擁有血量、護甲、Buff狀態等屬性，並且是決定戰鬥勝負的關鍵要素。**玩家的所有Character血量歸零時，該玩家戰敗**，這是整個戰鬥系統的核心規則。

**設計亮點**：
- **生存核心**：Character的存活狀態直接決定戰鬥勝負
- **管理器組合**：透過專業管理器處理血量和Buff
- **空值安全**：提供DummyCharacter避免null引用
- **查詢友好**：豐富的擴展方法支援靈活查詢
- **工廠創建**：統一的建構參數和創建流程

**檔案位置**: [CharacterEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterEntity.cs)

## ICharacterEntity 核心接口

### 接口設計
```csharp
public interface ICharacterEntity
{
    // 身份識別系統
    Guid Identity { get; }              // 實體唯一標識
    string NameKey { get; }             // 本地化名稱鍵值
    
    // 管理器系統
    IHealthManager HealthManager { get; }           // 血量管理器
    ICharacterBuffManager BuffManager { get; }     // Buff管理器
    
    // 快速屬性訪問
    int CurrentHealth { get; }          // 當前血量
    int MaxHealth { get; }              // 最大血量
    int CurrentArmor { get; }           // 當前護甲
    bool IsDead { get; }               // 死亡狀態
}
```

### 接口設計理念

#### 🆔 身份識別系統
```csharp
Guid Identity { get; }      // 戰鬥中的唯一標識，用於技能目標選擇
string NameKey { get; }     // 本地化鍵值，支援多語言顯示
```

**身份管理特色**：
- **全域唯一**：Identity確保在整個戰鬥中的唯一性
- **本地化支援**：NameKey配合本地化系統顯示正確名稱
- **引用安全**：基於GUID的引用比直接引用更安全

#### 🏥 管理器委託設計
```csharp
IHealthManager HealthManager { get; }       // 專業血量管理
ICharacterBuffManager BuffManager { get; }  // 專業Buff管理
```

**委託優勢**：
- **職責分離**：複雜邏輯交由專業管理器處理
- **擴展彈性**：可替換不同實現的管理器
- **測試友好**：可以輕鬆模擬管理器行為
- **維護簡潔**：Character本身保持簡潔的職責

#### ⚡ 快速訪問屬性
```csharp
int CurrentHealth { get; }  // 避免 character.HealthManager.Hp 的冗長調用
int MaxHealth { get; }      // 常用屬性的便捷訪問
int CurrentArmor { get; }   // 護甲值的快速查詢
bool IsDead { get; }       // 關鍵狀態的直接判定
```

**快速訪問特色**：
- **使用便利**：減少鏈式調用的複雜度
- **性能友好**：避免重複的管理器訪問
- **表達清晰**：直觀地表達Character的狀態

## CharacterEntity 實現類別

### 核心實現
```csharp
public class CharacterEntity : ICharacterEntity
{
    // 核心欄位
    private readonly Guid _identity;
    private readonly string _nameKey;
    private readonly IHealthManager _healthManager;
    private readonly ICharacterBuffManager _buffManager;

    // 屬性代理
    public Guid Identity => _identity;
    public string NameKey => _nameKey;
    public IHealthManager HealthManager => _healthManager;
    public ICharacterBuffManager BuffManager => _buffManager;
    
    // 計算屬性
    public int CurrentHealth => HealthManager.Hp;
    public int MaxHealth => HealthManager.MaxHp;
    public int CurrentArmor => HealthManager.Dp;
    public bool IsDead => CurrentHealth <= 0;
    
    // 空值物件支援
    public bool IsDummy => this == DummyCharacter;
    public static ICharacterEntity DummyCharacter = new DummyCharacter();
}
```

### 建構函數設計
```csharp
public CharacterEntity(
    string nameKey,
    int currentHealth,
    int maxHealth)
{
    _identity = Guid.NewGuid();
    _nameKey = nameKey;
    _healthManager = new HealthManager(currentHealth, maxHealth);
    _buffManager = new CharacterBuffManager();
}
```

**建構特點**：
- **自動ID生成**：每個Character都有唯一的Identity
- **管理器初始化**：創建對應的血量和Buff管理器
- **參數簡潔**：只需要必要的初始化參數
- **狀態清潔**：新Character沒有任何Buff狀態

### 死亡判定邏輯
```csharp
public bool IsDead => CurrentHealth <= 0;
```

**判定特色**：
- **簡潔明確**：血量≤0即為死亡
- **即時計算**：每次訪問都獲得最新狀態
- **邏輯單一**：只依據血量，不受其他因素影響
- **擴展預留**：未來可加入復活、不死等特殊機制

### 空值物件系統
```csharp
public bool IsDummy => this == DummyCharacter;
public static ICharacterEntity DummyCharacter = new DummyCharacter();
```

**空值物件優勢**：
- **null安全**：避免空引用異常
- **統一接口**：實現相同的ICharacterEntity
- **調試友好**：可以識別是否為Dummy物件
- **預設行為**：提供安全的預設操作

## CharacterParameter 建構參數

### 參數結構
```csharp
public record CharacterParameter
{
    public string NameKey;          // 角色名稱鍵值
    public int CurrentHealth;       // 當前血量
    public int MaxHealth;           // 最大血量
}
```

**Record特色**：
- **不可變性**：建立後無法修改，確保建構參數穩定
- **值語義**：基於值的相等比較
- **簡潔語法**：自動產生ToString、Equals等方法
- **資料傳輸**：適合作為資料傳輸物件

### Create工廠方法
```csharp
public static CharacterEntity Create(CharacterParameter characterParameter)
{
    return new CharacterEntity(
        characterParameter.NameKey,
        characterParameter.CurrentHealth,
        characterParameter.MaxHealth
    );
}
```

**工廠優勢**：
- **參數封裝**：將多個參數封裝為單一物件
- **擴展友好**：未來可加入更複雜的建構邏輯
- **統一介面**：提供統一的創建方式
- **驗證集中**：可在工廠中加入參數驗證

## DummyCharacter 空值物件

### 實現
```csharp
public class DummyCharacter : CharacterEntity
{
    public DummyCharacter() : base(string.Empty, 0, 0) { }
}

public static ICharacterEntity DummyCharacter = new DummyCharacter();
```

**空值物件特色**：
- **繼承實現**：繼承自CharacterEntity，保持接口一致
- **安全預設值**：空名稱、0血量的安全狀態
- **單例模式**：靜態實例避免重複創建
- **調試標識**：通過特殊值便於識別

### 使用場景
```csharp
// 避免null檢查
public ICharacterEntity GetTargetOrDefault(Guid targetId)
{
    return GetCharacter(targetId) ?? CharacterEntity.DummyCharacter;
}

// 安全的方法鏈調用
var health = character.GetCharacter(id).CurrentHealth; // 不會拋出NullReferenceException
```

## 查詢擴展系統

### CharacterEntityExtensions 擴展方法
```csharp
public static class CharacterEntityExtensions
{
    // 全域查詢：根據ID查找Character
    public static Option<ICharacterEntity> GetCharacter(this IGameplayModel model, Guid identity)
    
    // 所有權查詢：查找Character的擁有者
    public static Option<IPlayerEntity> Owner(this ICharacterEntity character, IGameplayModel model)
    
    // 陣營識別：確定Character的陣營
    public static Faction Faction(this ICharacterEntity character, IGameplayModel model)
}
```

### 全域查詢實現
```csharp
public static Option<ICharacterEntity> GetCharacter(this IGameplayModel model, Guid identity)
{
    // 先查詢友軍的Character
    var allyCharacterOpt = LinqEnumerableExtensions.FirstOrNone(
        model.GameStatus.Ally.Characters
            .Where(c => c.Identity == identity));
    if (allyCharacterOpt.HasValue)
        return allyCharacterOpt;
    
    // 再查詢敵軍的Character
    var enemyCharacterOpt = LinqEnumerableExtensions.FirstOrNone(
        model.GameStatus.Enemy.Characters
            .Where(c => c.Identity == identity));
    if (enemyCharacterOpt.HasValue)
        return enemyCharacterOpt;
    
    // 未找到時返回空值
    return Option.None<ICharacterEntity>();
}
```

**查詢特色**：
- **跨陣營搜索**：可以查找任意陣營的Character
- **安全返回**：使用Option避免null返回
- **高效查詢**：使用LINQ提高查詢效率
- **優先順序**：友軍優先，敵軍次之

### 所有權查詢實現
```csharp
public static Option<IPlayerEntity> Owner(this ICharacterEntity character, IGameplayModel model)
{
    // 檢查是否屬於友軍
    if (model.GameStatus.Ally.Characters.Any(c => c.Identity == character.Identity))
        return (model.GameStatus.Ally as IPlayerEntity).Some();
    
    // 檢查是否屬於敵軍
    if (model.GameStatus.Enemy.Characters.Any(c => c.Identity == character.Identity))
        return (model.GameStatus.Enemy as IPlayerEntity).Some();
    
    // 無法確定歸屬
    return Option.None<IPlayerEntity>();
}
```

**所有權特色**：
- **歸屬確定**：快速確定Character的擁有者
- **雙向查詢**：支援友軍和敵軍的查詢
- **類型安全**：返回IPlayerEntity接口類型
- **失敗處理**：無法確定時安全返回None

### 陣營識別實現
```csharp
public static Faction Faction(this ICharacterEntity character, IGameplayModel model)
{
    return character.Owner(model).ValueOr(PlayerEntity.DummyPlayer).Faction;
}
```

**陣營識別特色**：
- **組合查詢**：基於Owner查詢的結果
- **預設處理**：無法確定時使用DummyPlayer的陣營
- **簡潔實現**：一行程式碼完成複雜邏輯
- **鏈式調用**：支援流暢的方法鏈

## 管理器整合

### IHealthManager 血量管理
```csharp
public interface IHealthManager
{
    int Hp { get; }      // 當前血量
    int MaxHp { get; }   // 最大血量
    int Dp { get; }      // 當前護甲 (Defense Point)
    
    // 血量操作方法（推測）
    void TakeDamage(int damage);    // 受到傷害
    void Heal(int amount);          // 恢復血量
    void SetMaxHp(int maxHp);       // 設定血量上限
    void AddArmor(int armor);       // 增加護甲
}
```

**血量管理特色**：
- **血量系統**：支援當前血量和血量上限
- **護甲機制**：Dp(Defense Point)提供額外防護
- **傷害處理**：複雜的傷害計算邏輯
- **恢復機制**：支援血量恢復效果

### ICharacterBuffManager Buff管理
```csharp
public interface ICharacterBuffManager
{
    IReadOnlyCollection<ICharacterBuffEntity> Buffs { get; }
    
    // Buff操作方法（推測）
    void AddBuff(ICharacterBuffEntity buff);        // 添加Buff
    bool RemoveBuff(string buffId);                 // 移除指定Buff
    void UpdateBuffs(TriggerContext context);      // 更新所有Buff
    void ClearExpiredBuffs();                       // 清理過期Buff
    int GetBuffLevel(string buffId);                // 獲取Buff層數
}
```

**Buff管理特色**：
- **集合管理**：維護所有作用於Character的Buff
- **生命週期**：處理Buff的添加、更新、過期
- **層數管理**：支援可疊加的Buff效果
- **條件觸發**：基於TriggerContext的觸發機制

## 戰鬥系統整合

### 與Player系統的關係
```csharp
// PlayerEntity中的Character管理
public class PlayerEntity : IPlayerEntity
{
    protected IReadOnlyCollection<CharacterEntity> _characters;
    
    // 死亡判定：所有Character都死亡時Player戰敗
    public bool IsDead => Characters.All(character => character.IsDead);
    
    // 主Character（當前實現）
    public ICharacterEntity MainCharacter => Characters.First();
}
```

**整合特色**：
- **集合管理**：Player管理多個Character
- **勝負邏輯**：Character的存活決定Player的勝負
- **主次關係**：未來支援主Character + 助理Character
- **擴展預留**：為多Character戰術預留架構

### 與Card系統的關係
```csharp
// 卡片效果對Character的影響（範例）
public interface ICharacterTargetEffect
{
    void ApplyToCharacter(ICharacterEntity target, TriggerContext context);
}

// 可能的效果類型
class DamageEffect : ICharacterTargetEffect
{
    public void ApplyToCharacter(ICharacterEntity target, TriggerContext context)
    {
        target.HealthManager.TakeDamage(damageAmount);
    }
}

class HealEffect : ICharacterTargetEffect
{
    public void ApplyToCharacter(ICharacterEntity target, TriggerContext context)
    {
        target.HealthManager.Heal(healAmount);
    }
}

class BuffEffect : ICharacterTargetEffect
{
    public void ApplyToCharacter(ICharacterEntity target, TriggerContext context)
    {
        var buff = CreateBuff();
        target.BuffManager.AddBuff(buff);
    }
}
```

## 使用範例

### Character創建
```csharp
// 使用建構參數
var heroParam = new CharacterParameter
{
    NameKey = "hero.protagonist",
    CurrentHealth = 100,
    MaxHealth = 100
};

// 工廠創建
var hero = CharacterEntity.Create(heroParam);

// 直接創建
var enemy = new CharacterEntity("enemy.bandit", 80, 80);
```

### 狀態查詢
```csharp
// 基本狀態
Debug.Log($"角色: {character.NameKey}");
Debug.Log($"血量: {character.CurrentHealth}/{character.MaxHealth}");
Debug.Log($"護甲: {character.CurrentArmor}");
Debug.Log($"存活狀態: {(character.IsDead ? "死亡" : "存活")}");

// 管理器狀態
Debug.Log($"當前血量: {character.HealthManager.Hp}");
Debug.Log($"Buff數量: {character.BuffManager.Buffs.Count}");
```

### 查詢操作
```csharp
// 全域查詢
var targetCharacter = gameplayModel.GetCharacter(targetId);
if (targetCharacter.HasValue)
{
    Debug.Log($"找到目標: {targetCharacter.Value.NameKey}");
}

// 所有權查詢
var owner = character.Owner(gameplayModel);
if (owner.HasValue)
{
    Debug.Log($"擁有者: {owner.Value.Faction}");
}

// 陣營確定
var faction = character.Faction(gameplayModel);
Debug.Log($"陣營: {faction}");
```

### 戰鬥操作
```csharp
// 傷害處理
if (!character.IsDead)
{
    character.HealthManager.TakeDamage(30);
    
    if (character.IsDead)
    {
        Debug.Log($"{character.NameKey} 已死亡！");
    }
}

// Buff管理
var poisonBuff = CreatePoisonBuff();
character.BuffManager.AddBuff(poisonBuff);

// 狀態更新
character.BuffManager.UpdateBuffs(triggerContext);
```

### 安全操作
```csharp
// 使用空值物件
ICharacterEntity safeCharacter = character ?? CharacterEntity.DummyCharacter;

// 檢查是否為Dummy
if (!character.IsDummy)
{
    // 執行實際操作
    character.HealthManager.TakeDamage(damage);
}
```

## 設計模式應用

### 🏗️ 組合模式 (Composite Pattern)
```csharp
CharacterEntity = Identity + Name + HealthManager + BuffManager
```

### 🏭 工廠模式 (Factory Pattern)
```csharp
CharacterEntity.Create(CharacterParameter) → CharacterEntity
```

### 🚫 空值物件模式 (Null Object Pattern)
```csharp
DummyCharacter → 安全的預設值
```

### 📋 委託模式 (Delegation Pattern)
```csharp
Character → HealthManager (血量管理)
Character → BuffManager (Buff管理)
```

### 🔍 擴展方法模式 (Extension Methods Pattern)
```csharp
CharacterEntityExtensions → 豐富的查詢功能
```

### 📝 記錄模式 (Record Pattern)
```csharp
CharacterParameter → 不可變的建構參數
```

## 依賴關係

### 依賴的組件
- **🔗 IHealthManager**: 血量管理器 *需要HealthManager_Class.md*
- **🔗 ICharacterBuffManager**: Buff管理器 *需要CharacterBuffManager_Class.md*
- **🔗 IGameplayModel**: 遊戲狀態查詢 *需要GameplayModel_Class.md*
- **🔗 IPlayerEntity**: 玩家實體查詢 *參考PlayerEntity_Class.md*
- **🔗 Faction**: 陣營枚舉 *需要Faction_Enum.md*
- **🔗 Optional**: 安全空值處理 *需要Optional_Library.md*
- **🔗 Guid**: 唯一標識符 *內建類型*

### 被依賴的組件
- **🔗 PlayerEntity**: 管理Characters集合 *參考PlayerEntity_Class.md*
- **🔗 CardEffect**: 卡片效果影響Character *需要CardEffect_System.md*
- **🔗 Target系統**: Character作為效果目標 *需要Target_System.md*
- **🔗 UI系統**: 顯示Character資訊 *需要UI_System.md*
- **🔗 戰鬥系統**: 使用Character進行戰鬥計算 *需要Battle_System.md*

## 擴展計劃

### 多角色戰術系統
```csharp
// 未來可能的擴展
public interface ICharacterEntity
{
    CharacterRole Role { get; }         // 角色定位（主力/輔助/坦克等）
    ISkillManager SkillManager { get; } // 技能管理
    
    // 角色關係
    Option<ICharacterEntity> ProtectedBy { get; }  // 被保護關係
    IReadOnlyCollection<ICharacterEntity> Protecting { get; }  // 保護關係
}
```

### 複活機制
```csharp
public interface ICharacterEntity
{
    bool CanRevive { get; }                     // 是否可以復活
    void Revive(int healthAmount);              // 復活方法
    event Action<ICharacterEntity> OnDeath;     // 死亡事件
    event Action<ICharacterEntity> OnRevive;    // 復活事件
}
```

### 特殊狀態
```csharp
public interface ICharacterEntity
{
    bool IsImmortal { get; }        // 不死狀態
    bool IsStunned { get; }         // 暈眩狀態
    bool IsSilenced { get; }        // 沉默狀態
    bool IsInvisible { get; }       // 隱身狀態
}
```

---

## 相關檔案
| 檔案 | 關係 | 描述 |
|------|------|------|
| [CharacterEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterEntity.cs) | 核心 | Character實體完整實現 |
| [PlayerEntity.cs](../../Assets/Scripts/GameModel/Entity/PlayerEntity.cs) | 被依賴 | 管理Character集合 |
| [CharacterBuffEntity.cs](../../Assets/Scripts/GameModel/Entity/CharacterBuff/CharacterBuffEntity.cs) | 關聯 | Character的Buff系統 |

---

**最後更新**: 2024-12-20  
**版本**: v1.0  
**狀態**: ✅ 已完成