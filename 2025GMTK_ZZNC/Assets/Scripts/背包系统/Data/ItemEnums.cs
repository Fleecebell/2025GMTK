using UnityEngine;

namespace InventorySystem.Data
{
    /// <summary>
    /// 物品类型枚举
    /// </summary>
    public enum ItemType
    {
        Weapon,     // 武器
        Equipment,  // 装备
        Consumable  // 道具
    }

    /// <summary>
    /// 攻击方式枚举
    /// </summary>
    public enum AttackType
    {
        Slash,      // 斩击
        Thrust,     // 突刺
        Firearm     // 枪械
    }

    /// <summary>
    /// 属性类型枚举
    /// </summary>
    public enum AttributeType
    {
        Health,         // 生命值
        San,            // San值
        Attack,         // 攻击力
        Defense,        // 防御力
        MoveSpeed,      // 移动速度
        CriticalRate,   // 暴击率
        CriticalDamage  // 暴击伤害
    }
}