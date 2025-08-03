# WeaponInstanceManager 使用教程

## 概述

WeaponInstanceManager是武器实例管理器，负责在玩家装备武器时创建武器实例，并初始化武器脚本。它是连接背包系统和武器系统的桥梁。

## 完整设置流程

### 第一步：准备玩家GameObject

```
Player (玩家GameObject)
├── PlayerSprite (角色精灵)
├── Collider2D (碰撞器)
├── InventoryManager (背包管理器)
├── EquipmentManager (装备管理器)
├── WeaponInstanceManager (武器实例管理器) ← 我们要设置的
├── WeaponEquipmentIntegration (集成脚本) ← 可选
└── WeaponHolder (武器挂载点)
    └── [武器实例会在这里创建]
```

### 第二步：创建武器挂载点

1. **创建WeaponHolder**：
   - 在玩家GameObject下创建空子对象
   - 命名为"WeaponHolder"
   - 调整位置到角色手部位置
   - 例如：`transform.localPosition = new Vector3(0.5f, 0f, 0f)`

2. **设置层级关系**：
   ```
   Player
   └── WeaponHolder (Position: 0.5, 0, 0)
       └── [武器实例] (运行时创建)
   ```

### 第三步：配置WeaponInstanceManager

1. **添加组件**：
   - 选择玩家GameObject
   - Add Component → WeaponInstanceManager

2. **设置参数**：
   - **Weapon Holder**: 拖拽WeaponHolder到这个字段
   - **Player Transform**: 拖拽玩家GameObject的Transform到这个字段

### 第四步：集成到装备系统

#### 方法一：使用集成脚本（推荐）

1. **添加集成脚本**：
   - Add Component → WeaponEquipmentIntegration

2. **配置引用**：
   - **Equipment Manager**: 拖拽EquipmentManager组件
   - **Weapon Instance Manager**: 拖拽WeaponInstanceManager组件

#### 方法二：手动集成

在你的装备管理代码中调用WeaponInstanceManager：

```csharp
public class MyEquipmentManager : MonoBehaviour
{
    private WeaponInstanceManager weaponInstanceManager;
    
    private void Start()
    {
        weaponInstanceManager = GetComponent<WeaponInstanceManager>();
    }
    
    public void EquipWeapon(WeaponData weaponData)
    {
        // 装备武器逻辑...
        
        // 创建武器实例
        if (weaponInstanceManager != null)
        {
            weaponInstanceManager.CreateWeaponInstance(weaponData);
        }
    }
    
    public void UnequipWeapon()
    {
        // 卸下武器逻辑...
        
        // 销毁武器实例
        if (weaponInstanceManager != null)
        {
            weaponInstanceManager.DestroyCurrentWeaponInstance();
        }
    }
}
```

## 运行时工作流程

### 1. 装备武器时：

```
玩家点击装备武器
    ↓
EquipmentManager.EquipWeapon()
    ↓
触发OnWeaponEquipped事件
    ↓
WeaponEquipmentIntegration接收事件
    ↓
调用WeaponInstanceManager.CreateWeaponInstance()
    ↓
创建武器GameObject实例
    ↓
添加/获取Weapon脚本组件
    ↓
调用Weapon.Initialize()初始化武器
    ↓
武器可以使用（自动朝向鼠标，响应攻击输入）
```

### 2. 卸下武器时：

```
玩家卸下武器
    ↓
EquipmentManager.UnequipWeapon()
    ↓
触发OnWeaponUnequipped事件
    ↓
WeaponEquipmentIntegration接收事件
    ↓
调用WeaponInstanceManager.DestroyCurrentWeaponInstance()
    ↓
销毁武器GameObject实例
```

## 主要API说明

### WeaponInstanceManager 公共方法：

```csharp
// 创建武器实例
bool CreateWeaponInstance(WeaponData weaponData)

// 销毁当前武器实例
void DestroyCurrentWeaponInstance()

// 更新武器属性
void UpdateWeaponStats(float newDamage, float newAttackSpeed)

// 设置武器挂载点
void SetWeaponHolder(Transform newHolder)

// 手动触发攻击
void TriggerAttack()

// 检查是否可以攻击
bool CanAttack()

// 获取武器状态信息
string GetWeaponInstanceStatus()
```

### 属性访问器：

```csharp
GameObject CurrentWeaponInstance    // 当前武器实例
Weapon CurrentWeaponScript         // 当前武器脚本
WeaponData CurrentWeaponData       // 当前武器数据
bool HasWeaponEquipped             // 是否装备了武器
```

## 调试功能

### Inspector调试：

1. **显示武器状态**：
   - 在WeaponInstanceManager组件上右键
   - 选择"显示武器状态"
   - 控制台会输出详细的武器信息

2. **测试攻击**：
   - 在WeaponInstanceManager组件上右键
   - 选择"测试攻击"
   - 手动触发一次攻击

### 代码调试：

```csharp
// 检查武器是否正确装备
if (weaponInstanceManager.HasWeaponEquipped)
{
    Debug.Log("当前武器: " + weaponInstanceManager.CurrentWeaponData.ItemName);
    Debug.Log("武器状态: " + weaponInstanceManager.GetWeaponInstanceStatus());
}

// 检查是否可以攻击
if (weaponInstanceManager.CanAttack())
{
    Debug.Log("武器可以攻击");
}
```

## 常见问题解决

### Q: 武器实例没有创建？
A: 检查以下几点：
- WeaponData是否正确设置
- WeaponHolder是否正确配置
- 控制台是否有错误信息

### Q: 武器不朝向鼠标？
A: 检查：
- Camera.main是否正确设置
- 武器实例是否有Weapon脚本组件
- 武器脚本是否正确初始化

### Q: 武器不攻击？
A: 检查：
- 输入系统是否正常（鼠标左键或空格键）
- 武器是否在攻击冷却中
- 目标图层设置是否正确

### Q: 武器位置不对？
A: 调整：
- WeaponHolder的localPosition
- 武器预制体的pivot点
- 武器实例的localPosition

## 扩展使用

### 1. 多武器支持：

```csharp
// 可以扩展支持多个武器挂载点
public void SetSecondaryWeaponHolder(Transform secondaryHolder)
{
    // 实现副武器挂载点
}
```

### 2. 武器切换：

```csharp
// 快速切换武器
public void SwitchWeapon(WeaponData newWeaponData)
{
    DestroyCurrentWeaponInstance();
    CreateWeaponInstance(newWeaponData);
}
```

### 3. 武器升级：

```csharp
// 升级当前武器属性
public void UpgradeCurrentWeapon(float damageBonus, float speedBonus)
{
    if (CurrentWeaponScript != null)
    {
        float newDamage = CurrentWeaponData.Damage + damageBonus;
        float newSpeed = CurrentWeaponData.AttackSpeed + speedBonus;
        UpdateWeaponStats(newDamage, newSpeed);
    }
}
```

## 总结

WeaponInstanceManager是武器系统的核心管理器，它：

1. **自动化武器实例管理** - 无需手动创建/销毁武器对象
2. **无缝集成背包系统** - 通过事件系统与装备管理器协作
3. **提供丰富的调试功能** - 方便开发和测试
4. **支持扩展** - 可以轻松添加新功能

按照这个教程设置后，玩家就可以从背包装备武器，武器会自动实例化并可以立即使用。