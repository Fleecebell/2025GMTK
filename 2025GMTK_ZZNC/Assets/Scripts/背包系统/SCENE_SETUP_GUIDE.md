# 子背包系统场景设置指南

## 概述

本指南将帮助你在Unity中正确设置子背包系统的UI组件，实现三个独立的子背包功能。

## 场景层级结构

### 推荐的UI层级结构

```
Canvas
├── InventoryPanel (InventoryUI组件)
│   ├── Background
│   ├── Header
│   │   ├── Title
│   │   ├── CloseButton
│   │   └── MinimizeButton
│   ├── TabButtons
│   │   ├── WeaponTabButton
│   │   ├── EquipmentTabButton
│   │   └── ConsumableTabButton
│   ├── CharacterPanel
│   │   ├── CharacterPortrait
│   │   └── WeaponSlot
│   │       ├── WeaponIcon
│   │       └── WeaponSlotButton
│   ├── SubInventoryPanels
│   │   ├── WeaponSubInventory (SubInventoryUI组件)
│   │   │   ├── ScrollView
│   │   │   │   ├── Viewport
│   │   │   │   │   └── Content (GridLayoutGroup + ContentSizeFitter)
│   │   │   │   └── Scrollbar
│   │   │   └── StatsPanel
│   │   │       ├── StatsText
│   │   │       └── CapacityText
│   │   ├── EquipmentSubInventory (SubInventoryUI组件)
│   │   │   ├── ScrollView
│   │   │   │   ├── Viewport
│   │   │   │   │   └── Content (GridLayoutGroup + ContentSizeFitter)
│   │   │   │   └── Scrollbar
│   │   │   └── StatsPanel
│   │   │       ├── StatsText
│   │   │       └── CapacityText
│   │   └── ConsumableSubInventory (SubInventoryUI组件)
│   │       ├── ScrollView
│   │       │   ├── Viewport
│   │       │   │   └── Content (GridLayoutGroup + ContentSizeFitter)
│   │       │   └── Scrollbar
│   │       └── StatsPanel
│   │           ├── StatsText
│   │           └── CapacityText
│   └── ItemInfoPanel
│       ├── ItemIcon
│       ├── ItemNameText
│       └── ItemDescriptionText
└── InventoryButton
```

## 详细设置步骤

### 1. 创建主背包面板

1. 在Canvas下创建空GameObject，命名为"InventoryPanel"
2. 添加 `InventoryUI` 脚本组件
3. 设置RectTransform：
   - Anchor: 居中
   - Size: 800x600
   - Position: (0, 0, 0)

### 2. 创建标签页按钮

1. 在InventoryPanel下创建"TabButtons"文件夹
2. 创建三个Button：
   - WeaponTabButton (武器)
   - EquipmentTabButton (装备)
   - ConsumableTabButton (道具)
3. 设置按钮样式和位置

### 3. 创建角色面板

1. 在InventoryPanel下创建"CharacterPanel"
2. 添加角色头像Image组件
3. 创建武器槽位：
   - 创建"WeaponSlot"GameObject
   - 添加武器图标Image
   - 添加Button组件用于卸下武器

### 4. 创建子背包面板

#### 武器子背包设置

1. 创建"WeaponSubInventory"GameObject
2. 添加 `SubInventoryUI` 脚本组件
3. 设置组件参数：
   - Item Type: Weapon
   - Max Display Slots: 20
   - Expand Slots Amount: 5
   - Initial Slots: 10

4. 创建ScrollView结构：
   ```
   WeaponSubInventory
   ├── ScrollView (ScrollRect组件)
   │   ├── Viewport (Mask组件)
   │   │   └── Content (GridLayoutGroup + ContentSizeFitter)
   │   └── Scrollbar
   └── StatsPanel
       ├── StatsText (TextMeshProUGUI)
       └── CapacityText (TextMeshProUGUI)
   ```

5. 配置ScrollRect：
   - Horizontal: false
   - Vertical: true
   - Movement Type: Clamped
   - Inertia: true
   - Deceleration Rate: 0.135
   - Scroll Sensitivity: 1

6. 配置GridLayoutGroup：
   - Cell Size: 60x60
   - Spacing: 5x5
   - Constraint: Fixed Column Count
   - Constraint Count: 5

7. 配置ContentSizeFitter：
   - Vertical Fit: Preferred Size
   - Horizontal Fit: Preferred Size

#### 装备子背包设置

重复武器子背包的设置，但将Item Type设置为Equipment。

#### 道具子背包设置

重复武器子背包的设置，但将Item Type设置为Consumable。

### 5. 创建格子预制体

1. 创建"InventorySlot"预制体：
   ```
   InventorySlot (Button)
   ├── Background (Image)
   ├── ItemIcon (Image)
   └── QuantityText (TextMeshProUGUI)
   ```

2. 添加 `InventorySlotUI` 脚本组件
3. 设置UI组件引用

### 6. 创建物品信息面板

1. 创建"ItemInfoPanel"GameObject
2. 添加CanvasGroup组件
3. 创建子对象：
   - ItemIcon (Image)
   - ItemNameText (TextMeshProUGUI)
   - ItemDescriptionText (TextMeshProUGUI)

### 7. 配置组件引用

#### InventoryUI组件设置

在Inspector中设置以下引用：

```
InventoryUI组件:
├── Inventory Panel: InventoryPanel
├── Inventory Button: 背包按钮
├── Minimize Button: 最小化按钮
├── Character Panel: CharacterPanel
├── Character Portrait: 角色头像
├── Weapon Slot: 武器槽位
├── Weapon Icon: 武器图标
├── Weapon Slot Button: 武器槽位按钮
├── Weapon Sub Inventory: 武器子背包
├── Equipment Sub Inventory: 装备子背包
├── Consumable Sub Inventory: 道具子背包
├── Weapon Tab Button: 武器标签页按钮
├── Equipment Tab Button: 装备标签页按钮
├── Consumable Tab Button: 道具标签页按钮
├── Item Info Panel: 物品信息面板
├── Item Name Text: 物品名称文本
├── Item Description Text: 物品描述文本
└── Item Icon Image: 物品图标
```

#### SubInventoryUI组件设置

对每个子背包设置以下引用：

```
SubInventoryUI组件:
├── Scroll Rect: ScrollView的ScrollRect组件
├── Content Container: Content Transform
├── Grid Layout: Content的GridLayoutGroup组件
├── Content Size Fitter: Content的ContentSizeFitter组件
├── Slot Prefab: InventorySlot预制体
├── Item Type: 对应的物品类型
├── Max Display Slots: 20
├── Expand Slots Amount: 5
├── Initial Slots: 10
├── Stats Text: 统计信息文本
└── Capacity Text: 容量信息文本
```

## 样式设置

### 颜色方案

```csharp
// 推荐的颜色设置
Color weaponColor = new Color(0.8f, 0.2f, 0.2f, 1f);    // 红色
Color equipmentColor = new Color(0.2f, 0.2f, 0.8f, 1f);  // 蓝色
Color consumableColor = new Color(0.2f, 0.8f, 0.2f, 1f); // 绿色
```

### 字体设置

- 使用TextMeshPro字体
- 推荐字体大小：14-16
- 行高：1.2

### 动画设置

```csharp
// 推荐动画时长
float fadeInDuration = 0.2f;
float fadeOutDuration = 0.15f;
float scaleAnimationDuration = 0.2f;
```

## 测试设置

### 1. 添加测试组件

1. 创建"TestManager"GameObject
2. 添加 `SubInventoryTester` 脚本组件
3. 设置测试参数：
   - Enable Auto Test: false (手动测试)
   - Test Interval: 2.0f

### 2. 添加调试组件

1. 创建"DebugManager"GameObject
2. 添加 `InventoryDebugger` 脚本组件
3. 设置调试参数：
   - Enable Detailed Logging: true
   - Log Every Add Attempt: true

## 性能优化建议

### 1. UI优化

- 使用Canvas Group控制可见性
- 合理设置Mask组件
- 避免过多的UI更新

### 2. 内存优化

- 使用对象池管理格子
- 及时清理不需要的引用
- 合理设置预制体

### 3. 渲染优化

- 使用适当的Canvas设置
- 合理设置UI元素的层级
- 避免过度绘制

## 常见问题解决

### 1. 滑动不工作

检查项：
- ScrollRect组件是否正确配置
- Content是否有ContentSizeFitter
- Viewport是否有Mask组件

### 2. 格子不显示

检查项：
- SlotPrefab是否正确设置
- GridLayoutGroup是否正确配置
- ContentSizeFitter是否正确设置

### 3. 标签页切换不工作

检查项：
- 按钮事件是否正确绑定
- SubInventoryUI的SetVisible方法是否正确实现
- 标签页按钮的OnClick事件是否正确设置

## 扩展建议

### 1. 添加动画效果

```csharp
// 在SubInventoryUI中添加动画
public void PlayExpandAnimation()
{
    // 实现扩充动画
}
```

### 2. 添加拖拽功能

```csharp
// 在InventorySlotUI中添加拖拽
public void OnBeginDrag(PointerEventData eventData)
{
    // 实现拖拽开始逻辑
}
```

### 3. 添加搜索功能

```csharp
// 在InventoryUI中添加搜索
public void SearchItems(string keyword)
{
    // 实现搜索逻辑
}
```

## 完成检查清单

- [ ] 所有UI组件正确创建
- [ ] 组件引用正确设置
- [ ] 预制体正确配置
- [ ] 测试组件正常工作
- [ ] 滑动功能正常
- [ ] 标签页切换正常
- [ ] 物品添加功能正常
- [ ] 统计信息显示正常
- [ ] 性能表现良好 