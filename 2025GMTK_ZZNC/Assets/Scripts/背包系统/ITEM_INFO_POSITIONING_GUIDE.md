# 物品信息面板定位功能使用指南

## 功能概述

物品信息面板定位功能允许物品详情面板在鼠标悬停时显示在对应物品槽的位置，而不是固定位置。这提供了更直观的用户体验。

## 核心功能

### 1. 智能定位
- **自动定位**: 物品详情面板会自动显示在鼠标悬停的槽位旁边
- **边界检测**: 自动检测屏幕边界，确保面板不会超出可视区域
- **动态调整**: 根据槽位位置自动调整面板显示方向（左侧/右侧）

### 2. 位置计算逻辑
```
默认显示位置: 槽位右侧 + 10像素偏移
边界处理:
  - 右侧超出 → 显示在槽位左侧
  - 左侧超出 → 居中显示
  - 上下超出 → 自动调整垂直位置
```

### 3. 坐标转换
- **世界坐标**: 获取槽位的世界坐标位置
- **屏幕坐标**: 转换为屏幕坐标系
- **Canvas坐标**: 最终转换为Canvas本地坐标

## 使用方法

### 基本使用
1. **鼠标悬停**: 将鼠标悬停在包含物品的槽位上
2. **自动显示**: 物品详情面板会自动显示在槽位旁边
3. **鼠标离开**: 移开鼠标后面板自动隐藏

### 测试功能
1. **添加测试组件**: 在场景中添加`ItemInfoPositionTester`组件
2. **快捷键测试**:
   - `F4`: 测试所有包含物品的槽位定位
   - `F5`: 切换调试可视化显示
3. **手动测试**: 右键点击组件选择测试选项

## 技术实现

### 修改的文件
1. **InventoryUI.cs**: 
   - 添加了`ShowItemInfo`方法的位置参数
   - 实现了`PositionInfoPanel`定位逻辑
   - 添加了`CalculatePanelOffset`边界计算

2. **InventorySlotUI.cs**:
   - 修改了`OnPointerEnter`传递槽位位置
   - 添加了`GetSlotWorldPosition`获取位置方法
   - 修改了`OnPointerExit`隐藏面板

### 核心方法

#### InventoryUI.ShowItemInfo
```csharp
public void ShowItemInfo(Items.BaseItemData item, Vector3? slotPosition = null)
```
- 显示物品信息，可选择指定显示位置
- 如果提供位置参数，会调用定位逻辑

#### InventoryUI.PositionInfoPanel
```csharp
private void PositionInfoPanel(Vector3 slotWorldPosition)
```
- 根据槽位世界坐标定位面板
- 处理坐标转换和边界检测

#### InventorySlotUI.GetSlotWorldPosition
```csharp
private Vector3 GetSlotWorldPosition()
```
- 获取槽位的准确世界坐标
- 使用RectTransform的GetWorldCorners方法

## 配置选项

### UI设置
- **面板偏移**: 默认右侧10像素，左侧-10像素
- **边界缓冲**: 屏幕边缘10像素缓冲区
- **垂直对齐**: 默认与槽位中心对齐

### 调试选项
- **可视化调试**: 在Scene视图中显示槽位位置标记
- **调试线条**: 显示定位计算的参考线
- **边界测试**: 测试屏幕四个角落的定位表现

## 故障排除

### 常见问题

#### 1. 面板位置不正确
**可能原因**:
- Canvas设置不正确
- RectTransform锚点设置问题
- 坐标转换错误

**解决方案**:
- 检查Canvas的Render Mode设置
- 确保物品信息面板的锚点设置为中心
- 使用测试器验证坐标计算

#### 2. 面板超出屏幕边界
**可能原因**:
- 边界检测逻辑错误
- Canvas尺寸获取不正确

**解决方案**:
- 检查`CalculatePanelOffset`方法
- 验证Canvas尺寸计算
- 调整边界缓冲区大小

#### 3. 面板不显示
**可能原因**:
- 物品信息面板GameObject被禁用
- UI层级问题
- 事件系统问题

**解决方案**:
- 检查itemInfoPanel的SetActive状态
- 确保面板在正确的UI层级
- 验证EventSystem存在

### 调试步骤

1. **基础检查**:
   ```csharp
   // 检查组件引用
   Debug.Log($"ItemInfoPanel: {itemInfoPanel != null}");
   Debug.Log($"Canvas: {GetComponentInParent<Canvas>() != null}");
   ```

2. **位置验证**:
   ```csharp
   // 使用ItemInfoPositionTester进行测试
   // 按F4键测试所有槽位
   // 按F5键切换可视化调试
   ```

3. **边界测试**:
   ```csharp
   // 右键点击ItemInfoPositionTester组件
   // 选择"测试边界情况"
   ```

## 扩展功能

### 自定义定位策略
可以通过修改`CalculatePanelOffset`方法来实现自定义的定位策略：

```csharp
private Vector2 CalculatePanelOffset(Vector2 targetPosition, Vector2 panelSize, Vector2 canvasSize)
{
    // 实现自定义定位逻辑
    // 例如：始终显示在上方、下方等
}
```

### 动画效果
可以添加面板显示/隐藏的动画效果：

```csharp
// 在ShowItemInfo中添加动画
DOTween.To(() => panelRect.anchoredPosition, 
           x => panelRect.anchoredPosition = x, 
           targetPosition, 0.2f);
```

### 多面板支持
可以扩展支持多个信息面板同时显示：

```csharp
// 管理多个面板实例
private List<GameObject> infoPanels = new List<GameObject>();
```

## 性能考虑

### 优化建议
1. **缓存计算**: 缓存Canvas尺寸等不变的值
2. **对象池**: 为信息面板使用对象池
3. **延迟更新**: 使用协程延迟非关键更新
4. **条件检查**: 只在必要时进行位置计算

### 内存管理
- 及时清理事件订阅
- 避免在Update中进行重复计算
- 合理使用临时变量

## 总结

物品信息面板定位功能提供了更好的用户体验，通过智能定位和边界检测确保信息面板始终在合适的位置显示。配合测试工具可以方便地验证和调试功能。

该功能完全兼容现有的背包系统，不会影响其他功能的正常使用。