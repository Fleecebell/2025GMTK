using UnityEngine;
using System;

namespace InventorySystem.Character
{
    /// <summary>
    /// 角色属性数据类
    /// 包含角色的所有基础属性
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        [Header("基础属性")]
        [SerializeField] private float maxHealth = 100f;           // 最大生命值
        [SerializeField] private float currentHealth = 100f;      // 当前生命值
        [SerializeField] private float maxSan = 100f;             // 最大san值
        [SerializeField] private float currentSan = 100f;         // 当前san值
        [SerializeField] private float attack = 10f;              // 攻击力
        [SerializeField] private float defense = 5f;              // 防御力
        [SerializeField] private float moveSpeed = 5f;            // 移动速度

        [Header("技能属性")]
        [SerializeField] private int skillPoints = 0;             // 技能点数
        [SerializeField] private float criticalRate = 0.05f;      // 暴击率
        [SerializeField] private float criticalDamage = 1.5f;     // 暴击伤害倍率

        // 属性访问器
        public float MaxHealth 
        { 
            get => maxHealth; 
            set => maxHealth = Mathf.Max(0, value); 
        }
        
        public float CurrentHealth 
        { 
            get => currentHealth; 
            set => currentHealth = Mathf.Clamp(value, 0, maxHealth); 
        }
        
        public float MaxSan 
        { 
            get => maxSan; 
            set => maxSan = Mathf.Max(0, value); 
        }
        
        public float CurrentSan 
        { 
            get => currentSan; 
            set => currentSan = Mathf.Clamp(value, 0, maxSan); 
        }
        
        public float Attack 
        { 
            get => attack; 
            set => attack = Mathf.Max(0, value); 
        }
        
        public float Defense 
        { 
            get => defense; 
            set => defense = Mathf.Max(0, value); 
        }
        
        public float MoveSpeed 
        { 
            get => moveSpeed; 
            set => moveSpeed = Mathf.Max(0, value); 
        }
        
        public int SkillPoints 
        { 
            get => skillPoints; 
            set => skillPoints = Mathf.Max(0, value); 
        }
        
        public float CriticalRate 
        { 
            get => criticalRate; 
            set => criticalRate = Mathf.Clamp01(value); 
        }
        
        public float CriticalDamage 
        { 
            get => criticalDamage; 
            set => criticalDamage = Mathf.Max(1f, value); 
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CharacterData()
        {
            // 使用默认值
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public CharacterData(float maxHealth, float maxSan, float attack, float defense, float moveSpeed)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
            this.maxSan = maxSan;
            this.currentSan = maxSan;
            this.attack = attack;
            this.defense = defense;
            this.moveSpeed = moveSpeed;
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        public CharacterData(CharacterData other)
        {
            if (other != null)
            {
                maxHealth = other.maxHealth;
                currentHealth = other.currentHealth;
                maxSan = other.maxSan;
                currentSan = other.currentSan;
                attack = other.attack;
                defense = other.defense;
                moveSpeed = other.moveSpeed;
                skillPoints = other.skillPoints;
                criticalRate = other.criticalRate;
                criticalDamage = other.criticalDamage;
            }
        }

        /// <summary>
        /// 获取生命值百分比
        /// </summary>
        public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;

        /// <summary>
        /// 获取San值百分比
        /// </summary>
        public float SanPercentage => maxSan > 0 ? currentSan / maxSan : 0f;

        /// <summary>
        /// 恢复生命值
        /// </summary>
        public void RestoreHealth(float amount)
        {
            CurrentHealth += amount;
        }

        /// <summary>
        /// 扣除生命值
        /// </summary>
        public void TakeDamage(float damage)
        {
            float actualDamage = Mathf.Max(0, damage - defense);
            CurrentHealth -= actualDamage;
        }

        /// <summary>
        /// 恢复San值
        /// </summary>
        public void RestoreSan(float amount)
        {
            CurrentSan += amount;
        }

        /// <summary>
        /// 扣除San值
        /// </summary>
        public void ConsumeSan(float amount)
        {
            CurrentSan -= amount;
        }

        /// <summary>
        /// 重置到满血满San状态
        /// </summary>
        public void FullRestore()
        {
            currentHealth = maxHealth;
            currentSan = maxSan;
        }

        /// <summary>
        /// 检查角色是否存活
        /// </summary>
        public bool IsAlive => currentHealth > 0;

        /// <summary>
        /// 检查San值是否正常
        /// </summary>
        public bool IsSane => currentSan > 0;

        /// <summary>
        /// 获取角色状态描述
        /// </summary>
        public string GetStatusDescription()
        {
            return $"生命值: {currentHealth:F1}/{maxHealth:F1} " +
                   $"San值: {currentSan:F1}/{maxSan:F1} " +
                   $"攻击力: {attack:F1} " +
                   $"防御力: {defense:F1} " +
                   $"移速: {moveSpeed:F1}";
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        public override string ToString()
        {
            return GetStatusDescription();
        }
    }
}