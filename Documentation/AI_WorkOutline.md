# AI 工作大綱 - MortalGame 卡牌遊戲專案

## 專案概述
這是一個使用Unity開發的卡牌遊戲專案，採用MVC架構模式設計。

## 工作原則
1. **優先閱讀**：每次開始任何工作前，必須先閱讀此大綱檔案
2. **系統性分析**：按照系統模組逐一分析，不遺漏任何重要組件
3. **文檔完整性**：確保每個系統都有對應的詳細筆記
4. **交叉引用**：當筆記中提到其他系統時，必須建立適當的引用連結

## 專案架構概覽

### 核心目錄結構
```
Assets/Scripts/
├── GameModel/           # 遊戲核心邏輯層
├── GameView/           # 視圖展示層
├── UI/                 # 用戶介面
├── GameData/           # 遊戲資料定義
├── Scene/              # 場景管理
├── LevelMap/           # 關卡地圖
└── Editor/             # 編輯器擴展
```

### 主要系統模組
1. **遊戲模型系統 (GameModel)**
   - GameModel.cs - 核心遊戲狀態管理
   - GameplayManager.cs - 遊戲流程控制
   - GameContextManager.cs - 遊戲上下文管理
   - GameHistory.cs - 遊戲歷史記錄
   - GameEvent.cs - 事件系統
   - Entity/ - 遊戲實體（卡牌、角色等）
   - Action/ - 遊戲動作系統
   - Condition/ - 條件判斷系統
   - Instance/ - 實例管理
   - Target/ - 目標系統
   - EnemyLogic/ - 敵人AI邏輯

2. **遊戲視圖系統 (GameView)**
   - 負責所有視覺呈現和動畫效果

3. **用戶介面系統 (UI)**
   - 處理用戶交互和介面邏輯

4. **遊戲資料系統 (GameData)**
   - 定義所有遊戲資料結構

5. **場景管理系統 (Scene)**
   - 場景切換和管理

## AI 工作流程

### 階段一：系統分析
1. 分析 GameModel 核心系統
2. 分析 GameView 視圖系統
3. 分析 UI 交互系統
4. 分析 GameData 資料系統
5. 分析其他輔助系統

### 階段二：文檔創建
1. 為每個主要系統創建詳細筆記
2. 記錄系統間的依賴關係
3. 標記需要後續補完的交叉引用
4. 更新總筆記索引

### 階段三：文檔完善
1. 補完所有交叉引用
2. 確保文檔完整性
3. 建立系統關係圖

## 筆記命名規範

### 基本原則
- **單一腳本筆記**：直接使用腳本檔案名稱，如 `CardData.md`、`PlayerEntity.md`
- **系統整合筆記**：涵蓋多個相關腳本的筆記使用 `[SystemName]_System.md`
- **枚舉參考筆記**：枚舉集合使用 `[EnumName]_Reference.md`

### 命名範例
```
✅ 推薦命名：
- CardData.md              # 對應 CardData.cs
- PlayerEntity.md          # 對應 PlayerEntity.cs  
- EnergyManager.md         # 對應 EnergyManager.cs
- Card_System.md           # 涵蓋整個卡片系統的多個檔案
- CardBuff_System.md       # 涵蓋 CardBuffData.cs + CardBuffEntity.cs + CardBuffManager.cs
- CardEnum_Reference.md    # 涵蓋所有卡片相關枚舉

❌ 避免使用：
- CardData_Class.md        # 冗余的_Class後綴
- PlayerEntity_Class.md    # 冗余的_Class後綴
- SomeFeature_Feature.md   # 冗余的_Feature後綴
```

### 命名決策原則
1. **單一職責**：一個腳本對應一個筆記時，直接使用腳本名
2. **系統整合**：多個密切相關的腳本整合為一個系統筆記時，使用_System後綴
3. **簡潔明確**：避免冗余後綴，讓檔案名直接反映內容
4. **一致性**：同類型筆記使用統一的命名模式

## 待辦事項追蹤
- [ ] 完成 GameModel 系統分析
- [ ] 完成 GameView 系統分析
- [ ] 完成 UI 系統分析
- [ ] 完成 GameData 系統分析
- [ ] 建立系統關係圖
- [ ] 補完所有交叉引用

## 注意事項
- 每次修改或新增筆記後，必須更新 `AI_Notes_Index.md`
- 發現系統間新的依賴關係時，要及時記錄
- 保持筆記的可讀性和實用性
- 定期檢查文檔的完整性和正確性

---
最後更新：2024-12-20
版本：v1.0