# 子背包系统使用指南

## 概述

本系统实现了三个独立的子背包：武器背包、装备背包、道具背包。每个子背包都有无限容量，最多显示20个格子，当需要时会自动扩充5个格子，通过滑动面板查看更多格子。

## 系统架构

### 核心组件

1. **Inventory.cs** - 背包核心逻辑，管理三个子背包
2. **SubInventoryUI.cs** - 子背包UI管理器，处理单个子背包的显示和滑动
3. **InventoryUI.cs** - 主背包UI管理器，协调三个子背包
4. **InventorySlotUI.cs** - 格子UI组件，处理单个格子的显示和交互

### 物品类型

- **Weapon** - 武器类型
- **Equipment** - 装备类型  
- **Consumable** - 道具类型

## 功能特性

### 1. 无限容量
- 每个子背包都有无限容量
- 当添加物品时，系统会自动创建新的格子

### 2. 动态扩充
- 初始每个子背包显示10个格子
- 当格子不够时，自动扩充5个格子
- 最多显示20个格子，超出部分通过滑动查看

### 3. 滑动面板
- 每个子背包都有独立的滑动面板
- 支持垂直滑动查看所有格子
- 提供滚动到顶部、底部、指定格子的功能

### 4. 标签页切换
- 通过标签页在不同子背包间切换
- 每个子背包独立显示和更新

## 使用方法

### 1. 设置UI组件

在Unity编辑器中设置以下组件：

#### InventoryUI组件
```
- inventoryPanel: 背包主面板
- weaponSubInventory: 武器子背包UI
- equipmentSubInventory: 装备子背包UI  
- consumableSubInventory: 道具子背包UI
- weaponTabButton: 武器标签页按钮
- equipmentTabButton: 装备标签页按钮
- consumableTabButton: 道具标签页按钮
```

#### SubInventoryUI组件
```
- scrollRect: 滑动面板
- contentContainer: 内容容器
- gridLayout: 网格布局
- contentSizeFitter: 内容大小适配器
- slotPrefab: 格子预制体
- itemType: 物品类型（Weapon/Equipment/Consumable）
- maxDisplaySlots: 最大显示格子数（默认20）
- expandSlotsAmount: 扩充格子数（默认5）
- initialSlots: 初始格子数（默认10）
```

### 2. 添加物品

```csharp
// 获取背包管理器
var inventoryManager = InventoryManager.Instance;

// 添加武器
WeaponData weapon = // 获取武器数据
inventoryManager.AddItem(weapon, 1);

// 添加装备
EquipmentData equipment = // 获取装备数据
inventoryManager.AddItem(equipment, 1);

// 添加消耗品
ConsumableData consumable = // 获取消耗品数据
inventoryManager.AddItem(consumable, 5);
```

### 3. 使用物品

```csharp
// 使用指定类型的物品
inventoryManager.UseItem(ItemType.Weapon, slotIndex);
inventoryManager.UseItem(ItemType.Equipment, slotIndex);
inventoryManager.UseItem(ItemType.Consumable, slotIndex);
```

### 4. 获取背包信息

```csharp
// 获取指定类型的所有格子
var weaponSlots = inventoryManager.GetAllSlotsByType(ItemType.Weapon);
var equipmentSlots = inventoryManager.GetAllSlotsByType(ItemType.Equipment);
var consumableSlots = inventoryManager.GetAllSlotsByType(ItemType.Consumable);

// 获取统计信息
string weaponStats = inventoryManager.GetSubInventoryStats(ItemType.Weapon);
string totalStats = inventoryManager.GetInventoryStats();
```

### 5. 滑动功能

```csharp
// 获取子背包UI
var subInventory = FindObjectOfType<SubInventoryUI>();

// 滚动到顶部
subInventory.ScrollToTop();

// 滚动到底部
subInventory.ScrollToBottom();

// 滚动到指定格子
subInventory.ScrollToSlot(5);
```

## 测试功能

### 自动测试
使用 `SubInventoryTester` 组件进行测试：

1. 将 `SubInventoryTester` 脚本添加到场景中的GameObject
2. 启用 `enableAutoTest` 进行自动测试
3. 使用快捷键进行手动测试：
   - F1: 测试添加武器
   - F2: 测试添加装备
   - F3: 测试添加消耗品
   - F4: 测试子背包扩充
   - F5: 测试滑动功能

### 手动测试
在Inspector中右键点击 `SubInventoryTester` 组件，选择相应的测试功能。

## 配置参数

### SubInventoryUI配置
- **maxDisplaySlots**: 最大显示格子数（默认20）
- **expandSlotsAmount**: 每次扩充的格子数（默认5）
- **initialSlots**: 初始格子数（默认10）

### GridLayout配置
- **cellSize**: 格子大小（默认60x60）
- **spacing**: 格子间距（默认5x5）
- **constraintCount**: 每行格子数（默认5）

## 事件系统

系统提供以下事件：

```csharp
// 物品添加事件
inventoryManager.OnItemAdded += (item, quantity) => {
    Debug.Log($"添加物品: {item.ItemName} x{quantity}");
};

// 物品移除事件
inventoryManager.OnItemRemoved += (item, quantity) => {
    Debug.Log($"移除物品: {item.ItemName} x{quantity}");
};

// 背包变化事件
inventoryManager.OnInventoryChanged += () => {
    Debug.Log("背包内容已变化");
};
```

## 性能优化

1. **对象池**: 格子UI使用对象池管理，避免频繁创建销毁
2. **事件优化**: 使用事件系统减少不必要的UI更新
3. **滑动优化**: 滑动面板使用虚拟化技术，只渲染可见的格子

## 扩展功能

### 添加新的物品类型
1. 在 `ItemEnums.cs` 中添加新的物品类型
2. 在 `Inventory.cs` 中添加对应的格子列表
3. 在 `SubInventoryUI.cs` 中添加对应的显示逻辑

### 自定义滑动行为
继承 `SubInventoryUI` 类并重写相关方法：

```csharp
public class CustomSubInventoryUI : SubInventoryUI
{
    protected override void SetupScrollRect()
    {
        base.SetupScrollRect();
        // 自定义滑动设置
    }
}
```

## 故障排除

### 常见问题

1. **格子不显示**: 检查 `slotPrefab` 是否正确设置
2. **滑动不工作**: 检查 `scrollRect` 和 `contentContainer` 是否正确配置
3. **物品不添加**: 检查物品数据是否正确，背包管理器是否初始化

### 调试工具

使用 `InventoryDebugger` 和 `SubInventoryTester` 进行调试：

```csharp
// 运行详细诊断
inventoryDebugger.RunDetailedDiagnosis();

// 检查物品数据完整性
inventoryDebugger.CheckItemDataIntegrity();
```

## 更新日志

### v1.0.0
- 实现三个独立子背包系统
- 添加无限容量和动态扩充功能
- 实现滑动面板查看隐藏格子
- 添加完整的测试和调试工具 