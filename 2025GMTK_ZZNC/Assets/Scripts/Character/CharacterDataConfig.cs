using UnityEngine;

namespace InventorySystem.Character
{
    /// <summary>
    /// 角色数据配置类
    /// 用于在编辑器中配置玩家的基础属性
    /// 实现数据与方法分离
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterDataConfig", menuName = "Character/Character Data Config")]
    public class CharacterDataConfig : ScriptableObject
    {
        [Header("基础属性")]
        [SerializeField] private float maxHealth = 100f;           // 最大生命值
        [SerializeField] private float maxSan = 100f;             // 最大san值
        [SerializeField] private float attack = 10f;              // 攻击力
        [SerializeField] private float defense = 5f;              // 防御力
        [SerializeField] private float moveSpeed = 5f;            // 移动速度

        [Header("高级属性")]
        [SerializeField] private int skillPoints = 0;             // 技能点数
        [SerializeField] private float criticalRate = 0.05f;      // 暴击率
        [SerializeField] private float criticalDamage = 1.5f;     // 暴击伤害倍数

        // 属性访问器
        public float MaxHealth => maxHealth;
        public float MaxSan => maxSan;
        public float Attack => attack;
        public float Defense => defense;
        public float MoveSpeed => moveSpeed;
        public int SkillPoints => skillPoints;
        public float CriticalRate => criticalRate;
        public float CriticalDamage => criticalDamage;

        /// <summary>
        /// 创建CharacterData实例
        /// </summary>
        /// <returns>基于此配置的CharacterData实例</returns>
        public CharacterData CreateCharacterData()
        {
            return new CharacterData(maxHealth, maxSan, attack, defense, moveSpeed)
            {
                SkillPoints = skillPoints,
                CriticalRate = criticalRate,
                CriticalDamage = criticalDamage
            };
        }

        /// <summary>
        /// 验证配置数据
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            return maxHealth > 0 && maxSan > 0 && attack >= 0 && defense >= 0 && moveSpeed >= 0;
        }

        /// <summary>
        /// 获取配置描述
        /// </summary>
        /// <returns>配置描述字符串</returns>
        public string GetConfigDescription()
        {
            return $"生命值: {maxHealth:F1} " +
                   $"San值: {maxSan:F1} " +
                   $"攻击力: {attack:F1} " +
                   $"防御力: {defense:F1} " +
                   $"移动速度: {moveSpeed:F1} " +
                   $"暴击率: {(criticalRate * 100):F1}% " +
                   $"暴击伤害: {(criticalDamage * 100):F0}%";
        }

        /// <summary>
        /// 重置为默认值
        /// </summary>
        [ContextMenu("重置为默认值")]
        public void ResetToDefaults()
        {
            maxHealth = 100f;
            maxSan = 100f;
            attack = 10f;
            defense = 5f;
            moveSpeed = 5f;
            skillPoints = 0;
            criticalRate = 0.05f;
            criticalDamage = 1.5f;
        }
    }
} 