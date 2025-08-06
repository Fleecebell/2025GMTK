using UnityEngine;

namespace InventorySystem.Character
{
    [System.Serializable]
    public class CharacterData
    {
        [Header("特殊属性")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private float maxSan = 100f;
        [SerializeField] private float currentSan = 100f;

        [Header("基础属性")]
        [SerializeField] private float power = 10f;              // 力量
        [SerializeField] private float armor = 5f;               // 护甲
        [SerializeField] private float intelligence = 10f;       // 智力
        [SerializeField] private float attackSpeed = 1f;         // 攻击速度
        [SerializeField] private float moveSpeed = 1f;           // 移动速度
        [SerializeField] private float criticalRate = 0.05f;     // 暴击率
        [SerializeField] private float criticalDamage = 1.5f;    // 暴击伤害

        [Header("技能属性")]
        [SerializeField] private int skillPoints = 0;

        // 基础属性访问器
        public float Power
        {
            get => power;
            set => power = Mathf.Max(0, value);
        }

        public float Armor
        {
            get => armor;
            set => armor = Mathf.Max(0, value);
        }

        public float Intelligence
        {
            get => intelligence;
            set => intelligence = Mathf.Max(0, value);
        }

        public float AttackSpeed
        {
            get => attackSpeed;
            set => attackSpeed = Mathf.Max(0.1f, value);
        }

        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0.1f, value);
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

        // 其他属性访问器保持不变
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
        
        public int SkillPoints 
        { 
            get => skillPoints; 
            set => skillPoints = Mathf.Max(0, value); 
        }

        // 构造函数更新
        public CharacterData()
        {
            // 使用默认值
        }

        public CharacterData(float maxHealth, float maxSan, float power, float armor, float intelligence)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
            this.maxSan = maxSan;
            this.currentSan = maxSan;
            this.power = power;
            this.armor = armor;
            this.intelligence = intelligence;
        }

        public CharacterData(CharacterData other)
        {
            if (other != null)
            {
                maxHealth = other.maxHealth;
                currentHealth = other.currentHealth;
                maxSan = other.maxSan;
                currentSan = other.currentSan;
                power = other.power;
                armor = other.armor;
                intelligence = other.intelligence;
                attackSpeed = other.attackSpeed;
                moveSpeed = other.moveSpeed;
                criticalRate = other.criticalRate;
                criticalDamage = other.criticalDamage;
                skillPoints = other.skillPoints;
            }
        }

        // 计算实际伤害的方法更新
        public void TakeDamage(float damage)
        {
            float actualDamage = Mathf.Max(0, damage - armor);
            CurrentHealth -= actualDamage;
        }

        // 其他方法保持不变
        public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;
        public float SanPercentage => maxSan > 0 ? currentSan / maxSan : 0f;
        public void RestoreHealth(float amount) => CurrentHealth += amount;
        public void RestoreSan(float amount) => CurrentSan += amount;
        public void ConsumeSan(float amount) => CurrentSan -= amount;
        public void FullRestore()
        {
            currentHealth = maxHealth;
            currentSan = maxSan;
        }

        public bool IsAlive => currentHealth > 0;
        public bool IsSane => currentSan > 0;

        // 状态描述更新
        public string GetStatusDescription()
        {
            return $"生命值: {currentHealth:F1}/{maxHealth:F1}\n" +
                   $"San值: {currentSan:F1}/{maxSan:F1}\n" +
                   $"力量: {power:F1}\n" +
                   $"护甲: {armor:F1}\n" +
                   $"智力: {intelligence:F1}\n" +
                   $"攻击速度: {attackSpeed:P0}\n" +
                   $"移动速度: {moveSpeed:P0}\n" +
                   $"暴击率: {criticalRate:P0}\n" +
                   $"暴击伤害: {criticalDamage:P0}";
        }

        public override string ToString() => GetStatusDescription();

        // 新增：属性值限制方法
        public void ClampAttributes()
        {
            power = Mathf.Max(0, power);
            armor = Mathf.Max(0, armor);
            intelligence = Mathf.Max(0, intelligence);
            attackSpeed = Mathf.Max(0.1f, attackSpeed);
            moveSpeed = Mathf.Max(0.1f, moveSpeed);
            criticalRate = Mathf.Clamp01(criticalRate);
            criticalDamage = Mathf.Max(1f, criticalDamage);
        }
    }
}