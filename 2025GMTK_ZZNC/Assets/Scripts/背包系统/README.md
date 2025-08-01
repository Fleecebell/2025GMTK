# 背包系统使用指南

## 系统概述

这是一个完整的Unity背包系统实现，遵循OOP原则、数据与方法分离原则和易读性原则。系统包含以下核心功能：

- 背包物品管理（武器、装备、消耗品）
- 装备系统（武器装备/卸下）
- 消耗品冷却管理
- 完整的UI界面
- 事件驱动架构

## 文件结构

```
背包系统/
├── Data/                    # 数据定义
│   ├── ItemEnums.cs        # 枚举定义
│   └── AttributeModifier.cs # 属性修改器
├── Items/                   # 物品数据类
│   ├── BaseItemData.cs     # 物品基类
│   ├── WeaponData.cs       # 武器数据
│   ├── EquipmentData.cs    # 装备数据
│   └── ConsumableData.cs   # 消耗品数据
├── Core/                    # 核心系统
│   ├── InventorySlot.cs    # 背包槽位
│   └── Inventory.cs        # 背包核心
├── Managers/                # 管理器
│   ├── InventoryManager.cs # 背包管理器（主控制器）
│   ├── EquipmentManager.cs # 装备管理器
│   └── ConsumableManager.cs # 消耗品管理器
├── UI/                      # 用户界面
│   ├── InventoryUI.cs      # 背包UI控制器
│   └── InventorySlotUI.cs  # 槽位UI组件
├── Testing/                 # 测试工具
│   └── InventorySystemTester.cs # 系统测试器
└── README.md               # 使用说明
```

## 快速开始

### 1. 设置场景

1. 在场景中创建一个空GameObject，命名为"InventoryManager"
2. 添加`InventoryManager`组件
3. 系统会自动添加`EquipmentManager`和`ConsumableManager`组件

### 2. 创建物品数据

在Project窗口中右键创建物品数据：

- **武器**: Create -> Inventory -> Weapon Data
- **装备**: Create -> Inventory -> Equipment Data  
- **消耗品**: Create -> Inventory -> Consumable Data

### 3. 设置UI

1. 创建Canvas和背包UI界面
2. 添加`InventoryUI`组件
3. 设置UI引用（面板、按钮、槽位预制体等）
4. 槽位预制体需要添加`InventorySlotUI`组件

### 4. 测试系统

1. 添加`InventorySystemTester`组件到场景中的任意GameObject
2. 在Inspector中设置测试物品数据
3. 运行游戏后：
   - 按T键运行基础测试
   - 按C键清空背包
   - 按S键整理背包
   - 按I键打印背包信息

## 核心API使用

### 添加物品到背包

```csharp
// 获取背包管理器实例
var inventoryManager = InventoryManager.Instance;

// 添加物品
int addedAmount = inventoryManager.AddItem(itemData, quantity);
```

### 使用物品

```csharp
// 使用指定槽位的物品
bool success = inventoryManager.UseItem(slotIndex);
```

### 装备武器

```csharp
// 获取装备管理器
var equipmentManager = inventoryManager.EquipmentManager;

// 装备武器
bool equipped = equipmentManager.EquipWeapon(weaponData);

// 卸下武器
WeaponData unequipped = equipmentManager.UnequipWeapon();
```

### 检查物品数量

```csharp
// 检查是否拥有指定物品
bool hasItem = inventoryManager.HasItem(itemData, quantity);

// 获取物品数量
int count = inventoryManager.GetItemCount(itemData);
```

### 按类型获取物品

```csharp
// 获取所有武器
var weapons = inventoryManager.GetItemsByType(ItemType.Weapon);

// 获取所有装备
var equipments = inventoryManager.GetItemsByType(ItemType.Equipment);

// 获取所有消耗品
var consumables = inventoryManager.GetItemsByType(ItemType.Consumable);
```

## 事件系统

系统提供了完整的事件支持：

```csharp
// 订阅背包变化事件
inventoryManager.OnInventoryChanged += OnInventoryChanged;

// 订阅物品添加事件
inventoryManager.OnItemAdded += OnItemAdded;

// 订阅物品移除事件
inventoryManager.OnItemRemoved += OnItemRemoved;

// 订阅武器装备事件
inventoryManager.EquipmentManager.OnWeaponEquipped += OnWeaponEquipped;
```

## UI交互

### 背包界面操作

- **左键点击背包按钮**: 打开/关闭背包
- **左键点击物品**: 显示物品信息
- **右键点击物品**: 使用物品
- **点击标签页**: 切换物品类型显示
- **右键点击武器槽**: 卸下当前武器

### 标签页功能

- **武器页**: 显示所有武器，右键装备
- **装备页**: 显示所有装备，右键装备
- **道具页**: 显示所有消耗品，右键使用

## 扩展指南

### 添加新的物品类型

1. 在`ItemEnums.cs`中添加新的`ItemType`
2. 创建继承自`BaseItemData`的新物品类
3. 实现`Use`方法定义使用逻辑
4. 在UI中添加对应的标签页

### 添加新的装备槽位

1. 在`ItemEnums.cs`中添加新的`EquipmentSlotType`
2. 在`EquipmentManager`中添加对应的管理逻辑
3. 在UI中添加对应的装备槽显示

### 自定义物品效果

继承`BaseItemData`并重写`Use`方法：

```csharp
public override bool Use(GameObject user)
{
    // 实现自定义使用逻辑
    return true;
}
```

## 性能优化建议

1. **对象池**: 为UI槽位实现对象池，减少频繁创建销毁
2. **事件优化**: 及时取消不需要的事件订阅
3. **UI更新**: 只在必要时刷新UI，避免每帧更新
4. **数据缓存**: 缓存常用的物品查询结果

## 调试工具

### 控制台命令

在`InventorySystemTester`组件的Inspector中可以使用以下调试功能：

- **运行基础测试**: 测试所有核心功能
- **清空背包**: 清除所有物品
- **整理背包**: 整理和压缩背包
- **打印背包信息**: 输出详细的背包状态
- **验证系统完整性**: 检查系统组件是否正确设置

### 快捷键

- `T`: 运行基础测试
- `C`: 清空背包
- `S`: 整理背包
- `I`: 打印背包信息

## 常见问题

### Q: 物品无法添加到背包
A: 检查背包是否已满，或者物品数据是否正确设置

### Q: 武器无法装备
A: 确保`EquipmentManager`组件存在且正确初始化

### Q: UI不显示物品
A: 检查UI组件引用是否正确设置，槽位预制体是否包含`InventorySlotUI`组件

### Q: 消耗品冷却不工作
A: 确保`ConsumableManager`组件存在，且消耗品数据设置了正确的冷却时间

## 技术特点

- **OOP原则**: 使用继承、封装、多态等面向对象特性
- **SOLID原则**: 遵循单一职责、开闭原则等设计原则
- **数据分离**: 数据类与逻辑类分离，便于维护
- **事件驱动**: 使用事件系统实现松耦合
- **可扩展性**: 易于添加新的物品类型和功能
- **易读性**: 清晰的命名和完整的注释

## 版本信息

- **版本**: 1.0.0
- **Unity版本**: 2021.3+
- **创建日期**: 2025年1月
- **作者**: AI Assistant

## 支持

如果在使用过程中遇到问题，请检查：

1. 所有必需的组件是否正确添加
2. 物品数据是否正确创建和设置
3. UI引用是否正确配置
4. 控制台是否有错误信息

使用测试器可以快速验证系统是否正常工作。