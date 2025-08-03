using UnityEngine;

namespace InventorySystem.Character
{
    /// <summary>
    /// 装备属性数据类
    /// 定义装备可以提供的属性加成
    /// </summary>
    [System.Serializable]
    public class EquipmentAttributes
    {
        [Header("基础属性加成")]
        [SerializeField] private float powerBonus = 0f;          // 力量加成
        [SerializeField] private float armorBonus = 0f;          // 护甲加成
        [SerializeField] private float intelligenceBonus = 0f;   // 智力加成
        [SerializeField] private float attackSpeedBonus = 0f;    // 攻击速度加成
        [SerializeField] private float moveSpeedBonus = 0f;      // 移速加成
        [SerializeField] private float criticalRateBonus = 0f;   // 暴击率加成

        [Header("技能属性加成")]
        [SerializeField] private float criticalDamageBonus = 0f; // 暴击伤害加成

        // 属性访问器
        public float PowerBonus 
        { 
            get => powerBonus; 
            set => powerBonus = value; 
        }
        
        public float ArmorBonus 
        { 
            get => armorBonus; 
            set => armorBonus = value; 
        }
        
        public float IntelligenceBonus 
        { 
            get => intelligenceBonus; 
            set => intelligenceBonus = value; 
        }
        
        public float AttackSpeedBonus 
        { 
            get => attackSpeedBonus; 
            set => attackSpeedBonus = value; 
        }
        
        public float MoveSpeedBonus 
        { 
            get => moveSpeedBonus; 
            set => moveSpeedBonus = value; 
        }
        
        public float CriticalRateBonus 
        { 
            get => criticalRateBonus; 
            set => criticalRateBonus = value; 
        }
        
        public float CriticalDamageBonus 
        { 
            get => criticalDamageBonus; 
            set => criticalDamageBonus = value; 
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public EquipmentAttributes()
        {
            // 使用默认值（全为0）
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public EquipmentAttributes(float power, float armor, float intelligence, float attackSpeed, float moveSpeed, float critical)
        {
            powerBonus = power;
            armorBonus = armor;
            intelligenceBonus = intelligence;
            attackSpeedBonus = attackSpeed;
            moveSpeedBonus = moveSpeed;
            criticalRateBonus = critical;
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        public EquipmentAttributes(EquipmentAttributes other)
        {
            if (other != null)
            {
                powerBonus = other.powerBonus;
                armorBonus = other.armorBonus;
                intelligenceBonus = other.intelligenceBonus;
                attackSpeedBonus = other.attackSpeedBonus;
                moveSpeedBonus = other.moveSpeedBonus;
                criticalRateBonus = other.criticalRateBonus;
                criticalDamageBonus = other.criticalDamageBonus;
            }
        }

        /// <summary>
        /// 将装备属性应用到角色数据
        /// </summary>
        /// <param name="characterData">角色数据</param>
        /// <param name="isEquipping">是否为装备操作（true为装备，false为卸下）</param>
        public void ApplyToCharacter(CharacterData characterData, bool isEquipping = true)
        {
            if (characterData == null) return;

            float multiplier = isEquipping ? 1f : -1f;

            // 应用属性加成
            characterData.Power += powerBonus * multiplier;
            characterData.Armor += armorBonus * multiplier;
            characterData.Intelligence += intelligenceBonus * multiplier;
            characterData.AttackSpeed += attackSpeedBonus * multiplier;
            characterData.MoveSpeed += moveSpeedBonus * multiplier;
            characterData.CriticalRate += criticalRateBonus * multiplier;
            characterData.CriticalDamage += criticalDamageBonus * multiplier;
        }

        /// <summary>
        /// 检查是否有任何属性加成
        /// </summary>
        public bool HasAnyBonus()
        {
            return powerBonus != 0 || armorBonus != 0 || intelligenceBonus != 0 || 
                   attackSpeedBonus != 0 || moveSpeedBonus != 0 || 
                   criticalRateBonus != 0 || criticalDamageBonus != 0;
        }

        /// <summary>
        /// 获取属性加成描述
        /// </summary>
        public string GetBonusDescription()
        {
            var description = new System.Text.StringBuilder();

            if (powerBonus != 0)
                description.AppendLine($"力量: {(powerBonus > 0 ? "+" : "")}{powerBonus:F1}");
            
            if (armorBonus != 0)
                description.AppendLine($"护甲: {(armorBonus > 0 ? "+" : "")}{armorBonus:F1}");
            
            if (intelligenceBonus != 0)
                description.AppendLine($"智力: {(intelligenceBonus > 0 ? "+" : "")}{intelligenceBonus:F1}");
            
            if (attackSpeedBonus != 0)
                description.AppendLine($"攻击速度: {(attackSpeedBonus > 0 ? "+" : "")}{attackSpeedBonus:F1}");
            
            if (moveSpeedBonus != 0)
                description.AppendLine($"移速: {(moveSpeedBonus > 0 ? "+" : "")}{moveSpeedBonus:F1}");
            
            if (criticalRateBonus != 0)
                description.AppendLine($"暴击率: {(criticalRateBonus > 0 ? "+" : "")}{(criticalRateBonus * 100):F1}%");
            
            if (criticalDamageBonus != 0)
                description.AppendLine($"暴击伤害: {(criticalDamageBonus > 0 ? "+" : "")}{(criticalDamageBonus * 100):F1}%");

            return description.ToString().TrimEnd();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        public override string ToString()
        {
            return GetBonusDescription();
        }

        /// <summary>
        /// 重置所有属性为0
        /// </summary>
        public void Reset()
        {
            powerBonus = 0f;
            armorBonus = 0f;
            intelligenceBonus = 0f;
            attackSpeedBonus = 0f;
            moveSpeedBonus = 0f;
            criticalRateBonus = 0f;
            criticalDamageBonus = 0f;
        }

        /// <summary>
        /// 添加另一个装备属性的加成
        /// </summary>
        public void Add(EquipmentAttributes other)
        {
            if (other == null) return;

            powerBonus += other.powerBonus;
            armorBonus += other.armorBonus;
            intelligenceBonus += other.intelligenceBonus;
            attackSpeedBonus += other.attackSpeedBonus;
            moveSpeedBonus += other.moveSpeedBonus;
            criticalRateBonus += other.criticalRateBonus;
            criticalDamageBonus += other.criticalDamageBonus;
        }

        /// <summary>
        /// 减去另一个装备属性的加成
        /// </summary>
        public void Subtract(EquipmentAttributes other)
        {
            if (other == null) return;

            powerBonus -= other.powerBonus;
            armorBonus -= other.armorBonus;
            intelligenceBonus -= other.intelligenceBonus;
            attackSpeedBonus -= other.attackSpeedBonus;
            moveSpeedBonus -= other.moveSpeedBonus;
            criticalRateBonus -= other.criticalRateBonus;
            criticalDamageBonus -= other.criticalDamageBonus;
        }
    }
}