# 无限背包系统修改说明

## 修改概述

本次修改实现了无限容量背包系统，具有以下特点：

1. **无限容量**：背包可以容纳无限数量的物品
2. **动态格子扩充**：每种类型的物品最多显示20个格子，满时自动扩充5个
3. **滑动面板支持**：使用ScrollRect实现滑动显示，不超出原有容器范围
4. **分类显示**：按物品类型（武器、装备、道具）分别显示

## 主要修改内容

### 1. 修改InventoryUI.cs

**新增功能**：
- 支持滑动面板（ScrollRect）
- 动态格子扩充系统
- 按类型分类显示物品

**新增配置参数**：
```csharp
[Header("背包配置")]
[SerializeField] private int initialSlotsPerType = 20; // 每种类型初始格子数
[SerializeField] private int expandSlotsAmount = 5; // 每次扩充的格子数
[SerializeField] private int maxSlotsPerType = 100; // 每种类型最大格子数
```

**新增数据结构**：
```csharp
private Dictionary<ItemType, List<InventorySlotUI>> slotUIsByType;
private Dictionary<ItemType, int> currentSlotsPerType;
```

**核心方法**：

1. **CreateSlotsForType**：为指定类型创建格子
```csharp
private void CreateSlotsForType(ItemType itemType, int slotCount)
{
    var slotUIs = slotUIsByType[itemType];
    
    // 计算需要新增的格子数
    int existingSlots = slotUIs.Count;
    int newSlotsNeeded = slotCount - existingSlots;

    if (newSlotsNeeded <= 0) return;

    // 创建新格子
    for (int i = 0; i < newSlotsNeeded; i++)
    {
        GameObject slotObj = Instantiate(slotPrefab, inventoryContent);
        InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
        
        if (slotUI != null)
        {
            int slotIndex = existingSlots + i;
            slotUI.Initialize(slotIndex, this);
            slotUIs.Add(slotUI);
        }
    }
}
```

2. **CheckAndExpandSlots**：检查并扩充格子
```csharp
private void CheckAndExpandSlots(ItemType itemType)
{
    var slots = slotUIsByType[itemType];
    int currentSlots = currentSlotsPerType[itemType];
    
    // 检查是否需要扩充
    if (slots.Count >= currentSlots && currentSlots < maxSlotsPerType)
    {
        int newSlotCount = Mathf.Min(currentSlots + expandSlotsAmount, maxSlotsPerType);
        currentSlotsPerType[itemType] = newSlotCount;
        
        CreateSlotsForType(itemType, newSlotCount);
        
        Debug.Log($"扩充 {itemType} 类型格子，从 {currentSlots} 增加到 {newSlotCount}");
    }
}
```

3. **RefreshInventorySlots**：刷新背包格子显示
```csharp
private void RefreshInventorySlots()
{
    if (inventoryManager == null) return;

    var slots = slotUIsByType[currentTab];
    var itemsByType = inventoryManager.GetItemsByType(currentTab);

    // 检查是否需要扩充格子
    CheckAndExpandSlots(currentTab);

    // 更新格子显示
    for (int i = 0; i < slots.Count; i++)
    {
        if (i < itemsByType.Count)
        {
            var slot = itemsByType[i];
            slots[i].SetSlot(slot);
            slots[i].gameObject.SetActive(true);
        }
        else
        {
            slots[i].ClearSlot();
            slots[i].gameObject.SetActive(true);
        }
    }

    // 隐藏多余的格子
    for (int i = itemsByType.Count; i < slots.Count; i++)
    {
        slots[i].gameObject.SetActive(false);
    }
}
```

### 2. 修改Inventory.cs

**移除限制**：
- 移除 `maxSlots` 限制
- 移除 `IsFull` 限制（总是返回false）
- 移除 `CanAddItem` 限制（总是返回true）

**新增功能**：
- 动态创建格子
- 自动移除空格子
- 无限容量支持

**核心修改**：

1. **AddItem方法**：支持动态创建格子
```csharp
// 如果还有剩余数量，创建新格子
while (remainingQuantity > 0)
{
    var newSlot = new InventorySlot(slots.Count);
    newSlot.OnSlotChanged += HandleSlotChanged;
    slots.Add(newSlot);

    int added = newSlot.AddItem(item, remainingQuantity);
    remainingQuantity -= added;
    totalAdded += added;
}
```

2. **RemoveItem方法**：自动移除空格子
```csharp
// 如果格子空了，移除该格子
if (slot.IsEmpty)
{
    slots.RemoveAt(i);
}
```

3. **ClearInventory方法**：清空所有格子
```csharp
public void ClearInventory()
{
    foreach (var slot in slots)
    {
        slot.ClearSlot();
    }

    // 移除所有空格子
    slots.Clear();

    OnInventoryChanged?.Invoke();
}
```

## 系统特点

### 1. 无限容量
- **无格子限制**：背包可以容纳无限数量的物品
- **动态创建**：需要时自动创建新格子
- **自动清理**：物品移除后自动删除空格子

### 2. 分类显示
- **按类型分组**：武器、装备、道具分别显示
- **标签页切换**：可以切换不同物品类型
- **独立格子管理**：每种类型有独立的格子列表

### 3. 动态扩充
- **初始20格**：每种类型初始显示20个格子
- **自动扩充**：满时自动增加5个格子
- **最大限制**：每种类型最多100个格子

### 4. 滑动支持
- **ScrollRect集成**：使用Unity的ScrollRect组件
- **不超出范围**：新格子通过滑动显示
- **流畅体验**：支持鼠标滚轮和拖拽

## 使用方法

### 1. 设置UI组件
在InventoryUI的Inspector中设置：
- `inventoryScrollRect`：滑动面板组件
- `inventoryGridLayout`：网格布局组件
- `initialSlotsPerType`：初始格子数（默认20）
- `expandSlotsAmount`：扩充数量（默认5）
- `maxSlotsPerType`：最大格子数（默认100）

### 2. 添加物品
```csharp
// 添加物品到背包（自动创建格子）
inventoryManager.AddItem(itemData, quantity);
```

### 3. 切换标签页
```csharp
// 切换到武器标签页
inventoryUI.SetActiveTab(ItemType.Weapon);
```

### 4. 查看统计信息
```csharp
// 获取背包统计
string stats = inventoryManager.GetInventoryStats();
Debug.Log(stats);
```

## 配置参数

### InventoryUI配置
- **initialSlotsPerType**: 每种类型的初始格子数（默认20）
- **expandSlotsAmount**: 每次扩充的格子数（默认5）
- **maxSlotsPerType**: 每种类型的最大格子数（默认100）

### 滑动面板配置
- **ScrollRect**: 滑动面板组件
- **GridLayoutGroup**: 网格布局组件
- **Content**: 内容容器

## 注意事项

1. **性能考虑**：
   - 每种类型最多100个格子，避免性能问题
   - 空格子会自动隐藏，减少渲染负担
   - 使用对象池可以进一步优化性能

2. **UI设置**：
   - 确保ScrollRect正确配置
   - GridLayoutGroup设置合适的间距
   - Content容器大小要适应内容

3. **内存管理**：
   - 空格子会自动清理
   - 物品移除时会删除对应格子
   - 避免内存泄漏

## 文件结构

```
Assets/Scripts/背包系统/
├── UI/
│   ├── InventoryUI.cs              # 修改：无限容量UI
│   └── InventorySlotUI.cs          # 修改：格子UI支持
└── Core/
    └── Inventory.cs                # 修改：无限容量核心逻辑
```

## 版本信息

- **修改日期**: 2025年1月
- **Unity版本**: 2021.3+
- **修改类型**: 功能增强
- **兼容性**: 向后兼容，不影响现有功能

## 测试建议

1. **容量测试**：
   - 添加大量物品，验证无限容量
   - 检查格子自动创建和清理

2. **UI测试**：
   - 测试滑动面板功能
   - 验证标签页切换
   - 检查格子扩充动画

3. **性能测试**：
   - 测试大量物品时的性能
   - 验证内存使用情况
   - 检查UI响应性

4. **功能测试**：
   - 测试物品添加/移除
   - 验证分类显示
   - 检查事件触发 