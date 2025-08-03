# 武器系统使用说明

## 概述

武器系统包含以下主要组件：
- `Weapon.cs` - 武器脚本，挂载在武器预制体上
- `WeaponInstanceManager.cs` - 武器实例管理器，负责武器的创建和管理
- `Bullet.cs` - 子弹脚本，用于枪械类武器

## 在编辑器中的使用步骤

### 1. 设置武器数据 (WeaponData)

1. 在Project窗口中右键 → Create → Inventory → Weapon Data
2. 设置武器属性：
   - **Item Name**: 武器名称
   - **Icon**: 武器图标
   - **Attack Type**: 攻击类型（Slash/Thrust/Firearm）
   - **Damage**: 伤害值
   - **Attack Speed**: 攻击速度
   - **Attack Instance**: 武器预制体（可选）
   - **Attack Effect**: 攻击特效预制体（可选）
   - **Attack Sound**: 攻击音效（可选）

### 2. 创建武器预制体

#### 方法一：使用现有预制体
1. 创建一个GameObject作为武器预制体
2. 添加SpriteRenderer组件，设置武器精灵
3. 添加Weapon脚本组件
4. 设置Weapon脚本的参数：
   - **Fire Point**: 攻击发射点（子物体）
   - **Weapon Sprite**: 武器精灵渲染器
   - **Audio Source**: 音频源组件
   - **Target Layer Mask**: 目标图层遮罩

#### 方法二：让系统自动创建
1. 在WeaponData中不设置Attack Instance
2. 系统会自动创建基础武器对象

### 3. 设置玩家角色

1. 在玩家GameObject上添加以下组件：
   - `InventoryManager` - 背包管理器
   - `EquipmentManager` - 装备管理器
   - `WeaponInstanceManager` - 武器实例管理器

2. 设置WeaponInstanceManager：
   - **Weapon Holder**: 武器挂载点（Transform）
   - **Player Transform**: 玩家Transform引用

### 4. 武器挂载点设置

1. 在玩家角色下创建一个空的GameObject命名为"WeaponHolder"
2. 调整位置到合适的武器持握位置
3. 将这个Transform赋值给WeaponInstanceManager的Weapon Holder字段

### 5. 装备武器的流程

当玩家从背包装备武器时：
1. `EquipmentManager.EquipWeapon()` 被调用
2. `WeaponInstanceManager.CreateWeaponInstance()` 创建武器实例
3. 武器实例上的`Weapon`脚本被初始化
4. 武器获得来自WeaponData的属性（伤害、攻速等）

## 武器类型说明

### 1. 挥砍武器 (Slash)
- 扇形范围攻击
- 攻击范围：2单位半径，60度角
- 适合近战武器如剑、斧头

### 2. 刺击武器 (Thrust)
- 直线攻击
- 攻击范围：3单位直线
- 适合长矛、匕首等

### 3. 枪械武器 (Firearm)
- 发射子弹
- 需要设置子弹预制体
- 子弹会自动寻找目标并造成伤害

## 子弹系统设置

### 创建子弹预制体
1. 创建GameObject命名为"Bullet"
2. 添加SpriteRenderer设置子弹外观
3. 添加Bullet脚本
4. 设置Bullet参数：
   - **Life Time**: 子弹生存时间
   - **Target Layer Mask**: 目标图层
   - **Destroy On Hit**: 击中后是否销毁

### 在武器中使用子弹
1. 将子弹预制体赋值给WeaponData的Attack Instance字段
2. 枪械类武器会自动发射这个子弹

## 调试和测试

### 武器脚本调试
- 在Scene视图中选中武器对象，可以看到攻击范围的Gizmos
- 使用Context Menu中的调试功能

### WeaponInstanceManager调试
- **显示武器状态**: 在Inspector中右键选择
- **测试攻击**: 手动触发攻击测试

### 控制台日志
系统会输出详细的日志信息：
- 武器初始化信息
- 攻击执行信息
- 伤害造成信息

## 扩展功能

### 自定义攻击逻辑
可以继承Weapon类并重写攻击方法：
```csharp
public class CustomWeapon : Weapon
{
    protected override void ExecuteAttackLogic(Vector2 direction)
    {
        // 自定义攻击逻辑
    }
}
```

### 添加新的武器类型
1. 在AttackType枚举中添加新类型
2. 在Weapon.ExecuteAttackLogic()中添加对应的处理逻辑

### 武器升级系统
使用Weapon.SetWeaponStats()方法可以在运行时修改武器属性

## 注意事项

1. **图层设置**: 确保目标对象在正确的图层上
2. **碰撞检测**: 目标对象需要有Collider2D组件
3. **生命值系统**: 目标对象需要实现IHealth接口
4. **性能优化**: 大量子弹时注意对象池的使用
5. **音效管理**: 确保AudioSource组件正确设置

## 常见问题

### Q: 武器不攻击？
A: 检查Input输入、攻击冷却时间、目标图层设置

### Q: 子弹不造成伤害？
A: 检查目标是否实现IHealth接口、图层遮罩设置

### Q: 武器朝向不正确？
A: 检查Camera.main是否正确设置、鼠标输入是否正常

### Q: 特效不显示？
A: 检查特效预制体路径、FirePoint位置设置