# GameView 系統 - 視覺呈現層架構總覽

## 🎯 系統定位與職責

**GameView 是 MortalGame 的視覺呈現層**，負責將 GameModel 的資料狀態轉換為玩家可見的視覺元素與互動介面。採用**View-Presenter 分離設計**，確保視覺邏輯與業務邏輯的清晰分工。

## 📊 系統架構設計

### 視覺分層設計思路
**View 元件層**：負責具體的視覺呈現與基礎互動回應
**Factory 創建層**：管理 View 元件的生命週期與資源回收
**Presenter 協調層**：處理複雜的視覺邏輯與跨 View 協作

### 核心子系統職責

#### [CardView](Assets/Scripts/GameView/CardView) - 卡牌視覺系統
處理各種卡牌狀態的視覺呈現，包含手牌、選中卡牌、詳細資訊等不同展示模式

#### [CharacterView](Assets/Scripts/GameView/CharacterView) - 角色視覺系統
管理玩家與敵人角色的視覺呈現，處理生命值、能量、狀態指示器等角色資訊顯示

#### [BuffView](Assets/Scripts/GameView/BuffView) - 增益視覺系統
負責各種 Buff 效果的視覺化呈現，包含圖示、數值、持續時間等狀態資訊

#### [EventView](Assets/Scripts/GameView/EventView) - 事件視覺系統
處理遊戲事件的動畫呈現，如傷害數值、治療效果、能量變化等即時回饋

#### [Factory](Assets/Scripts/GameView/Factory) - 工廠創建系統
實現物件池模式管理 View 元件，優化效能並統一創建流程

#### [Panel](Assets/Scripts/GameView/Panel) - 面板介面系統
管理各種 UI 面板與資訊顯示，提供玩家操作介面

### 互動選擇機制
透過 [ISelectableView](Assets/Scripts/GameView/ISelectableView.cs) 介面統一處理玩家選擇互動，支援靈活的目標指定系統

## 🔧 設計模式應用

### 工廠模式與物件池
[PrefabFactory](Assets/Scripts/GameView/Factory/PrefabFactory.cs) 結合物件池實現高效的 View 元件管理

### 介面驅動設計
透過 ISelectableView、IRecyclable 等介面確保系統擴展性

### 事件驅動渲染
基於 GameModel 事件流進行視覺更新，保持資料與視覺的同步

## 🌐 系統間協作

**與 GameModel 的連接**：接收遊戲狀態變化，轉換為視覺呈現
**與 UI 系統的整合**：提供互動回饋，處理玩家操作輸入
**與資源管理的協作**：透過 Factory 系統優化效能與記憶體使用

---

**系統複雜度**：⭐⭐⭐⭐ (複雜的視覺邏輯與互動處理)  
**維護重點**：效能最佳化、視覺一致性、互動回應性