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
        [SerializeField] private float healthBonus = 0f;          // 生命值加成
        [SerializeField] private float sanBonus = 0f;             // San值加成
        [SerializeField] private float attackBonus = 0f;          // 攻击力加成
        [SerializeField] private float defenseBonus = 0f;         // 防御力加成
        [SerializeField] private float moveSpeedBonus = 0f;       // 移速加成

        [Header("技能属性加成")]
        [SerializeField] private float criticalRateBonus = 0f;    // 暴击率加成
        [SerializeField] private float criticalDamageBonus = 0f;  // 暴击伤害加成

        // 属性访问器
        public float HealthBonus 
        { 
            get => healthBonus; 
            set => healthBonus = value; 
        }
        
        public float SanBonus 
        { 
            get => sanBonus; 
            set => sanBonus = value; 
        }
        
        public float AttackBonus 
        { 
            get => attackBonus; 
            set => attackBonus = value; 
        }
        
        public float DefenseBonus 
        { 
            get => defenseBonus; 
            set => defenseBonus = value; 
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
        public EquipmentAttributes(float health, float san, float attack, float defense, float moveSpeed)
        {
            healthBonus = health;
            sanBonus = san;
            attackBonus = attack;
            defenseBonus = defense;
            moveSpeedBonus = moveSpeed;
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        public EquipmentAttributes(EquipmentAttributes other)
        {
            if (other != null)
            {
                healthBonus = other.healthBonus;
                sanBonus = other.sanBonus;
                attackBonus = other.attackBonus;
                defenseBonus = other.defenseBonus;
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
            characterData.MaxHealth += healthBonus * multiplier;
            characterData.MaxSan += sanBonus * multiplier;
            characterData.Attack += attackBonus * multiplier;
            characterData.Defense += defenseBonus * multiplier;
            characterData.MoveSpeed += moveSpeedBonus * multiplier;
            characterData.CriticalRate += criticalRateBonus * multiplier;
            characterData.CriticalDamage += criticalDamageBonus * multiplier;

            // 如果是装备操作，需要调整当前生命值和San值
            if (isEquipping)
            {
                // 装备时，如果最大值增加，当前值也按比例增加
                if (healthBonus > 0)
                {
                    float healthRatio = characterData.HealthPercentage;
                    characterData.CurrentHealth = characterData.MaxHealth * healthRatio;
                }
                
                if (sanBonus > 0)
                {
                    float sanRatio = characterData.SanPercentage;
                    characterData.CurrentSan = characterData.MaxSan * sanRatio;
                }
            }
            else
            {
                // 卸下装备时，确保当前值不超过新的最大值
                characterData.CurrentHealth = Mathf.Min(characterData.CurrentHealth, characterData.MaxHealth);
                characterData.CurrentSan = Mathf.Min(characterData.CurrentSan, characterData.MaxSan);
            }
        }

        /// <summary>
        /// 检查是否有任何属性加成
        /// </summary>
        public bool HasAnyBonus()
        {
            return healthBonus != 0 || sanBonus != 0 || attackBonus != 0 || 
                   defenseBonus != 0 || moveSpeedBonus != 0 || 
                   criticalRateBonus != 0 || criticalDamageBonus != 0;
        }

        /// <summary>
        /// 获取属性加成描述
        /// </summary>
        public string GetBonusDescription()
        {
            var description = new System.Text.StringBuilder();

            if (healthBonus != 0)
                description.AppendLine($"生命值: {(healthBonus > 0 ? "+" : "")}{healthBonus:F1}");
            
            if (sanBonus != 0)
                description.AppendLine($"San值: {(sanBonus > 0 ? "+" : "")}{sanBonus:F1}");
            
            if (attackBonus != 0)
                description.AppendLine($"攻击力: {(attackBonus > 0 ? "+" : "")}{attackBonus:F1}");
            
            if (defenseBonus != 0)
                description.AppendLine($"防御力: {(defenseBonus > 0 ? "+" : "")}{defenseBonus:F1}");
            
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
            healthBonus = 0f;
            sanBonus = 0f;
            attackBonus = 0f;
            defenseBonus = 0f;
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

            healthBonus += other.healthBonus;
            sanBonus += other.sanBonus;
            attackBonus += other.attackBonus;
            defenseBonus += other.defenseBonus;
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

            healthBonus -= other.healthBonus;
            sanBonus -= other.sanBonus;
            attackBonus -= other.attackBonus;
            defenseBonus -= other.defenseBonus;
            moveSpeedBonus -= other.moveSpeedBonus;
            criticalRateBonus -= other.criticalRateBonus;
            criticalDamageBonus -= other.criticalDamageBonus;
        }
    }
}