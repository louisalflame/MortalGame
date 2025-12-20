# HealthManager 血量管理系統筆記

## 系統概述
HealthManager是角色生存機制的核心管理系統，負責處理角色的血量(Hp)、護甲(Dp)和最大血量(MaxHp)管理。這個系統實現了完整的戰鬥生存邏輯，包括多種傷害類型處理、治療機制、護盾系統，是決定角色生死和戰鬥結果的關鍵組件。

**設計核心**：
- **雙重防護**：血量+護甲的雙重生存機制
- **傷害分層**：不同傷害類型有不同的處理邏輯
- **數值安全**：完善的邊界檢查和溢出處理
- **結果追蹤**：詳細的操作結果記錄

**檔案位置**: [Assets/Scripts/GameModel/Entity/HealthManager.cs](../../Assets/Scripts/GameModel/Entity/HealthManager.cs)

## 系統架構

### 接口定義
```csharp
public interface IHealthManager
{
    // 狀態屬性
    int Hp { get; }      // 當前血量
    int MaxHp { get; }   // 最大血量
    int Dp { get; }      // 當前護甲值
    
    // 核心操作
    TakeDamageResult TakeDamage(int amount, GameContext context, DamageType damageType);
    GetHealResult GetHeal(int amount, GameContext context);
    GetShieldResult GetShield(int amount, GameContext context);
}
```

### 核心實現
```csharp
public class HealthManager : IHealthManager
{
    private int _hp;      // 當前血量
    private int _maxHp;   // 最大血量
    private int _dp;      // 當前護甲

    public HealthManager(int currentHealth, int maxHealth)
    {
        _maxHp = maxHealth;
        _hp = currentHealth;
        _dp = 0;  // 護甲初始為0
    }
}
```

## 核心功能分析

### 🩸 血量系統設計

#### 三重狀態管理
```csharp
public int Hp => _hp;      // 當前血量 (0 <= Hp <= MaxHp)
public int MaxHp => _maxHp; // 最大血量上限
public int Dp => _dp;      // 護甲值 (0 <= Dp <= MaxHp)
```

**狀態特色**：
- **血量範圍**：0 到 MaxHp 之間，0 代表角色死亡
- **護甲上限**：最大護甲值等於最大血量
- **動態變化**：三個數值都可以透過不同操作動態變化
- **邊界安全**：使用 Mathf.Clamp 確保數值在有效範圍內

#### 初始化策略
```csharp
public HealthManager(int currentHealth, int maxHealth)
{
    _maxHp = maxHealth;     // 設定血量上限
    _hp = currentHealth;    // 可以非滿血開始
    _dp = 0;               // 護甲預設為0
}
```

**初始化特色**：
- **靈活起始**：支援非滿血狀態開始
- **護甲重置**：護甲總是從0開始，需要後續獲得
- **上限優先**：先設定最大值，確保當前值有參考標準

### ⚔️ 傷害系統設計

#### 傷害類型分類
```csharp
public enum DamageType
{
    Normal,      // 普通傷害：護甲 → 血量
    Additional,  // 額外傷害：護甲 → 血量  
    Penetrate,   // 穿透傷害：直接扣血，無視護甲
    Effective    // 有效傷害：直接扣血，無視護甲
}
```

#### 傷害處理邏輯
```csharp
public TakeDamageResult TakeDamage(int amount, GameContext context, DamageType damageType)
{
    int deltaDp = 0;   // 護甲變化量
    int deltaHp = 0;   // 血量變化量
    int damageOver = 0; // 溢出傷害

    switch (damageType)
    {
        case DamageType.Normal:
        case DamageType.Additional:
            // 先扣護甲，再扣血量
            deltaDp = _AcceptArmorDamage(amount, out var damageRemain);
            deltaHp = _AcceptHealthDamage(damageRemain, out damageOver);
            break;

        case DamageType.Penetrate:
        case DamageType.Effective:
            // 直接扣血量，無視護甲
            deltaHp = _AcceptHealthDamage(amount, out damageOver);
            deltaDp = 0;
            break;
    }

    return new TakeDamageResult(
        Type: damageType,
        DamagePoint: amount,
        DeltaHp: deltaHp,
        DeltaDp: deltaDp,
        OverHp: damageOver
    );
}
```

**傷害處理特色**：
- **分層防護**：Normal/Additional傷害先被護甲吸收
- **穿透機制**：Penetrate/Effective傷害直接作用於血量
- **溢出計算**：記錄超過目標承受能力的傷害
- **完整追蹤**：返回詳細的傷害分配結果

### 🛡️ 護甲系統實現

#### 護甲傷害吸收
```csharp
private int _AcceptArmorDamage(int amount, out int damageRemain)
{ 
    var originDp = _dp;
    _dp = Mathf.Clamp(_dp - amount, 0, originDp);  // 護甲不能為負
    var deltaDp = originDp - _dp;                  // 實際消耗的護甲
    damageRemain = Mathf.Max(amount - deltaDp, 0); // 剩餘傷害

    return deltaDp;
}
```

**護甲機制特色**：
- **優先吸收**：護甲在血量之前吸收傷害
- **完全消耗**：護甲可以完全被消耗至0
- **剩餘傳遞**：護甲無法吸收的傷害傳遞給血量
- **數值安全**：確保護甲值不會變為負數

#### 護甲增加機制
```csharp
public GetShieldResult GetShield(int amount, GameContext context)
{
    var deltaDp = _AcceptArmorGain(amount, out var dpOver);

    return new GetShieldResult(
        ShieldPoint: amount,
        DeltaDp: deltaDp,
        OverDp: dpOver
    );
}

private int _AcceptArmorGain(int amount, out int dpOver)
{
    var originDp = _dp;
    _dp = Mathf.Clamp(_dp + amount, originDp, _maxHp);  // 護甲上限為最大血量
    var deltaDp = _dp - originDp;                       // 實際增加的護甲
    dpOver = Mathf.Max(amount - deltaDp, 0);           // 溢出的護甲

    return deltaDp;
}
```

**護甲增加特色**：
- **上限控制**：護甲最大值等於角色最大血量
- **溢出處理**：超過上限的護甲會被記錄但不生效
- **漸進增加**：支援多次小量護甲增加
- **結果回報**：明確報告實際增加量和溢出量

### 💊 治療系統實現

#### 血量治療邏輯
```csharp
public GetHealResult GetHeal(int amount, GameContext context)
{
    var deltaHp = _AcceptHealthHeal(amount, out var hpOver);

    return new GetHealResult(
        HealPoint: amount,
        DeltaHp: deltaHp,
        OverHp: hpOver
    );
}

private int _AcceptHealthHeal(int amount, out int hpOver)
{
    var originHp = _hp;
    _hp = Mathf.Clamp(_hp + amount, originHp, _maxHp);  // 不能超過最大血量
    var deltaHp = _hp - originHp;                       // 實際治療量
    hpOver = Mathf.Max(amount - deltaHp, 0);           // 溢出治療

    return deltaHp;
}
```

**治療機制特色**：
- **上限恢復**：血量只能恢復到最大血量
- **溢出記錄**：過量治療會被記錄但不生效
- **漸進治療**：支援多次治療累積
- **效果追蹤**：精確記錄實際治療效果

## 結果類型系統

### TakeDamageResult 傷害結果
```csharp
public record TakeDamageResult(
    DamageType Type,     // 傷害類型
    int DamagePoint,     // 原始傷害值
    int DeltaHp,         // 血量變化（負數）
    int DeltaDp,         // 護甲變化（負數）
    int OverHp           // 溢出傷害
);
```

**傷害結果特色**：
- **類型記錄**：記住傷害的具體類型
- **分層展示**：分別顯示對血量和護甲的影響
- **溢出追蹤**：記錄無效的超額傷害
- **不可變性**：使用record確保結果不被修改

### GetHealResult 治療結果
```csharp
public record GetHealResult(
    int HealPoint,       // 原始治療值
    int DeltaHp,         // 血量變化（正數）
    int OverHp           // 溢出治療
);
```

### GetShieldResult 護盾結果
```csharp
public record GetShieldResult(
    int ShieldPoint,     // 原始護盾值
    int DeltaDp,         // 護甲變化（正數）
    int OverDp           // 溢出護甲
);
```

## 系統整合與依賴

### 與CharacterEntity的整合
```csharp
// CharacterEntity中的使用方式
public class CharacterEntity : ICharacterEntity
{
    private readonly IHealthManager _healthManager;
    
    // 角色死亡判定
    public bool IsDead => _healthManager.Hp <= 0;
    
    // 接受傷害
    public TakeDamageResult TakeDamage(int amount, DamageType type, GameContext context)
    {
        var result = _healthManager.TakeDamage(amount, context, type);
        
        // 觸發死亡事件
        if (IsDead)
        {
            OnCharacterDeath?.Invoke(this);
        }
        
        return result;
    }
}
```

### 與戰鬥系統的整合
```csharp
// 戰鬥中的傷害計算
public void ProcessCombat(ICharacterEntity attacker, ICharacterEntity defender, int baseDamage)
{
    // 計算最終傷害
    var finalDamage = CalculateDamage(attacker, defender, baseDamage);
    
    // 應用傷害
    var damageResult = defender.TakeDamage(finalDamage, DamageType.Normal, gameContext);
    
    // 檢查戰鬥結果
    if (defender.IsDead)
    {
        EndCombat(attacker, CombatResult.Victory);
    }
}
```

## 使用範例

### 標準戰鬥傷害處理
```csharp
// 創建角色的HealthManager
var healthManager = new HealthManager(
    currentHealth: 100,  // 當前血量
    maxHealth: 100       // 最大血量
);

// 給角色添加護甲
var shieldResult = healthManager.GetShield(25, gameContext);
Debug.Log($"獲得護甲：{shieldResult.DeltaDp}點"); // 輸出：獲得護甲：25點

// 角色受到普通傷害
var damageResult = healthManager.TakeDamage(40, gameContext, DamageType.Normal);
Debug.Log($"護甲損失：{damageResult.DeltaDp}，血量損失：{damageResult.DeltaHp}");
// 輸出：護甲損失：25，血量損失：15

// 角色受到穿透傷害
var penetrateResult = healthManager.TakeDamage(20, gameContext, DamageType.Penetrate);
Debug.Log($"直接血量損失：{penetrateResult.DeltaHp}"); // 輸出：直接血量損失：20

// 治療角色
var healResult = healthManager.GetHeal(30, gameContext);
Debug.Log($"血量恢復：{healResult.DeltaHp}點"); // 輸出：血量恢復：30點
```

### 護甲上限測試
```csharp
var healthManager = new HealthManager(50, 100);

// 嘗試添加超過上限的護甲
var excessShield = healthManager.GetShield(150, gameContext);
Debug.Log($"實際護甲增加：{excessShield.DeltaDp}");  // 實際護甲增加：100
Debug.Log($"溢出護甲：{excessShield.OverDp}");        // 溢出護甲：50
```

### 治療上限測試
```csharp
var healthManager = new HealthManager(80, 100);

// 嘗試過量治療
var excessHeal = healthManager.GetHeal(50, gameContext);
Debug.Log($"實際治療：{excessHeal.DeltaHp}");    // 實際治療：20
Debug.Log($"溢出治療：{excessHeal.OverHp}");      // 溢出治療：30
```

### 死亡判定邏輯
```csharp
public bool IsCharacterAlive(IHealthManager health)
{
    return health.Hp > 0;
}

public void CheckBattleEnd(List<ICharacterEntity> characters)
{
    var aliveCharacters = characters.Where(c => IsCharacterAlive(c.HealthManager)).ToList();
    
    if (aliveCharacters.Count <= 1)
    {
        EndBattle(aliveCharacters.FirstOrDefault());
    }
}
```

## 設計亮點

### 🔒 數值安全保障
- **邊界檢查**：所有數值操作都使用Mathf.Clamp確保在有效範圍
- **溢出處理**：明確處理和記錄超過限制的數值
- **非負保證**：血量和護甲永遠不會變為負數
- **一致性**：所有操作都遵循相同的數值安全原則

### 📊 完整的結果追蹤
- **操作透明**：每個操作都返回詳細的變化信息
- **溢出記錄**：保留無效操作的信息，便於平衡調整
- **類型記憶**：記錄操作類型，支援後續分析
- **不可變結果**：使用record確保結果不被意外修改

### ⚡ 靈活的傷害系統
- **分層防護**：護甲和血量的分層保護機制
- **類型多樣**：支援多種傷害類型，豐富戰鬥策略
- **穿透機制**：穿透傷害提供反制護甲的策略選項
- **擴展性**：易於添加新的傷害類型

### 🎯 戰略深度
- **資源管理**：護甲作為可消耗的防護資源
- **時機選擇**：穿透傷害vs普通傷害的選擇
- **恢復策略**：治療和護甲的不同恢復路徑
- **極限追蹤**：溢出機制避免資源浪費

## 系統擴展潛力

### 可能的增強功能
```csharp
// 護甲類型系統
public enum ArmorType
{
    Physical,   // 物理護甲
    Magical,    // 魔法護甲
    Universal   // 通用護甲
}

// 傷害減免系統
public interface IDamageReduction
{
    int CalculateReduction(int damage, DamageType type);
}

// 生命值回復系統
public interface IHealthRegeneration
{
    void ProcessRegen(IHealthManager health, GameContext context);
}
```

HealthManager系統是角色生存機制的基石，它的設計兼顧了數值安全、戰略深度和擴展性，為複雜的戰鬥系統提供了穩固的血量管理基礎。透過護甲和血量的雙重機制，以及多樣的傷害類型，這個系統支撐起了豐富而平衡的戰鬥體驗。

---

## 相關系統引用
- **CharacterEntity**: 角色實體的生存狀態管理 → [CharacterEntity.md](CharacterEntity.md)
- **Character_System**: 角色系統的核心組件 → [Character_System.md](Character_System.md)
- **GameContext**: 遊戲上下文和環境信息 → GameContext.md ⏳
- **DamageType**: 傷害類型枚舉定義 → DamageType_Enum.md ⏳