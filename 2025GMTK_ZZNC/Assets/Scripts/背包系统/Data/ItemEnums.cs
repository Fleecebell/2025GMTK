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
    public enum Quality
    {
        Rare,       // 神秘
        Epic,       // 史诗
        Legendary,   // 传说
        Artifact//神器
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
    /// 属性类型枚�?
    /// </summary>
    public enum AttributeType
    {
        power,       //力量
        armor,            // 护甲
        intelligence,         // 智力
        attackSpeed,        // 攻击速度
        moveSpeed,      // 移动速度
        critical,   // 暴击�?
    }
}