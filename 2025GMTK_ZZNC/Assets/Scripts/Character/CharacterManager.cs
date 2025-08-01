using UnityEngine;
using System;

namespace InventorySystem.Character
{
    /// <summary>
    /// 角色管理器
    /// 管理角色属性和装备效果
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        [Header("角色基础数据")]
        [SerializeField] private CharacterDataConfig characterDataConfig;
        
        // 当前角色数据（包含装备加成）
        private CharacterData currentCharacterData;
        
        // 装备提供的属性加成
        private EquipmentAttributes totalEquipmentBonus;

        // 事件
        public event Action<CharacterData> OnCharacterDataChanged;
        public event Action<float, float> OnHealthChanged;  // 当前血量, 最大血量
        public event Action<float, float> OnSanChanged;     // 当前San值, 最大San值

        // 单例模式
        public static CharacterManager Instance { get; private set; }

        // 属性访问器
        public CharacterData CurrentCharacterData => currentCharacterData;
        public CharacterDataConfig CharacterDataConfig => characterDataConfig;
        public EquipmentAttributes TotalEquipmentBonus => totalEquipmentBonus;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            // 单例模式
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCharacter();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初始化角色数据
        /// </summary>
        private void InitializeCharacter()
        {
            // 如果没有配置数据，使用默认数据
            if (characterDataConfig == null)
            {
                Debug.LogWarning("未设置角色数据配置，使用默认配置");
                characterDataConfig = CreateDefaultConfig();
            }

            // 从配置创建基础角色数据
            currentCharacterData = characterDataConfig.CreateCharacterData();
            
            // 初始化装备加成
            totalEquipmentBonus = new EquipmentAttributes();

            Debug.Log($"角色数据初始化完成: {currentCharacterData}");
        }

        /// <summary>
        /// 创建默认配置
        /// </summary>
        /// <returns>默认角色配置</returns>
        private CharacterDataConfig CreateDefaultConfig()
        {
            var config = ScriptableObject.CreateInstance<CharacterDataConfig>();
            config.ResetToDefaults();
            return config;
        }

        /// <summary>
        /// 应用装备属性加成
        /// </summary>
        /// <param name="equipmentAttributes">装备属性</param>
        /// <param name="isEquipping">是否为装备</param>
        public void ApplyEquipmentBonus(EquipmentAttributes equipmentAttributes, bool isEquipping = true)
        {
            if (equipmentAttributes == null)
            {
                Debug.LogWarning("装备属性为空，无法应用");
                return;
            }

            // 记录变化前的血量和San值
            float healthRatio = currentCharacterData.HealthPercentage;
            float sanRatio = currentCharacterData.SanPercentage;

            // 应用装备属性加成
            if (isEquipping)
            {
                totalEquipmentBonus.Add(equipmentAttributes);
                Debug.Log($"装备属性加成: {equipmentAttributes.GetBonusDescription()}");
            }
            else
            {
                totalEquipmentBonus.Subtract(equipmentAttributes);
                Debug.Log($"卸下装备属性: {equipmentAttributes.GetBonusDescription()}");
            }

            // 通知角色数据变化
            RecalculateCharacterData();

            // 根据装备状态调整当前血量和San值
            if (isEquipping)
            {
                currentCharacterData.CurrentHealth = currentCharacterData.MaxHealth * healthRatio;
                currentCharacterData.CurrentSan = currentCharacterData.MaxSan * sanRatio;
            }
            else
            {
                // 卸下装备时，确保当前值不超过最大值
                currentCharacterData.CurrentHealth = Mathf.Min(currentCharacterData.CurrentHealth, currentCharacterData.MaxHealth);
                currentCharacterData.CurrentSan = Mathf.Min(currentCharacterData.CurrentSan, currentCharacterData.MaxSan);
            }

            // 触发事件
            NotifyCharacterDataChanged();
        }

        /// <summary>
        /// 重新计算角色数据
        /// </summary>
        private void RecalculateCharacterData()
        {
            // 从基础配置重新创建当前角色数据
            currentCharacterData = characterDataConfig.CreateCharacterData();

            // 应用装备属性加成
            totalEquipmentBonus.ApplyToCharacter(currentCharacterData, true);
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        public void RestoreHealth(float amount)
        {
            float oldHealth = currentCharacterData.CurrentHealth;
            currentCharacterData.RestoreHealth(amount);
            
            if (Mathf.Abs(oldHealth - currentCharacterData.CurrentHealth) > 0.01f)
            {
                OnHealthChanged?.Invoke(currentCharacterData.CurrentHealth, currentCharacterData.MaxHealth);
                Debug.Log($"恢复生命值 {amount:F1}，当前: {currentCharacterData.CurrentHealth:F1}/{currentCharacterData.MaxHealth:F1}");
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(float damage)
        {
            float oldHealth = currentCharacterData.CurrentHealth;
            currentCharacterData.TakeDamage(damage);
            
            if (Mathf.Abs(oldHealth - currentCharacterData.CurrentHealth) > 0.01f)
            {
                OnHealthChanged?.Invoke(currentCharacterData.CurrentHealth, currentCharacterData.MaxHealth);
                Debug.Log($"受到伤害 {damage:F1}，当前: {currentCharacterData.CurrentHealth:F1}/{currentCharacterData.MaxHealth:F1}");
            }
        }

        /// <summary>
        /// 恢复San值
        /// </summary>
        public void RestoreSan(float amount)
        {
            float oldSan = currentCharacterData.CurrentSan;
            currentCharacterData.RestoreSan(amount);
            
            if (Mathf.Abs(oldSan - currentCharacterData.CurrentSan) > 0.01f)
            {
                OnSanChanged?.Invoke(currentCharacterData.CurrentSan, currentCharacterData.MaxSan);
                Debug.Log($"恢复San值 {amount:F1}，当前: {currentCharacterData.CurrentSan:F1}/{currentCharacterData.MaxSan:F1}");
            }
        }

        /// <summary>
        /// 消耗San值
        /// </summary>
        public void ConsumeSan(float amount)
        {
            float oldSan = currentCharacterData.CurrentSan;
            currentCharacterData.ConsumeSan(amount);
            
            if (Mathf.Abs(oldSan - currentCharacterData.CurrentSan) > 0.01f)
            {
                OnSanChanged?.Invoke(currentCharacterData.CurrentSan, currentCharacterData.MaxSan);
                Debug.Log($"消耗San值 {amount:F1}，当前: {currentCharacterData.CurrentSan:F1}/{currentCharacterData.MaxSan:F1}");
            }
        }

        /// <summary>
        /// 完全恢复
        /// </summary>
        public void FullRestore()
        {
            currentCharacterData.FullRestore();
            OnHealthChanged?.Invoke(currentCharacterData.CurrentHealth, currentCharacterData.MaxHealth);
            OnSanChanged?.Invoke(currentCharacterData.CurrentSan, currentCharacterData.MaxSan);
            Debug.Log("角色完全恢复");
        }

        /// <summary>
        /// 设置角色数据配置
        /// </summary>
        public void SetCharacterDataConfig(CharacterDataConfig newConfig)
        {
            if (newConfig == null)
            {
                Debug.LogWarning("角色数据配置不能为空");
                return;
            }

            characterDataConfig = newConfig;
            RecalculateCharacterData();
            NotifyCharacterDataChanged();
            
            Debug.Log($"设置角色数据配置: {newConfig.GetConfigDescription()}");
        }

        /// <summary>
        /// 通知角色数据变化
        /// </summary>
        private void NotifyCharacterDataChanged()
        {
            OnCharacterDataChanged?.Invoke(currentCharacterData);
            OnHealthChanged?.Invoke(currentCharacterData.CurrentHealth, currentCharacterData.MaxHealth);
            OnSanChanged?.Invoke(currentCharacterData.CurrentSan, currentCharacterData.MaxSan);
        }

        /// <summary>
        /// 获取角色状态描述
        /// </summary>
        public string GetCharacterStatusDescription()
        {
            var description = new System.Text.StringBuilder();
            description.AppendLine("=== 角色状态 ===");
            description.AppendLine(currentCharacterData.GetStatusDescription());
            
            if (totalEquipmentBonus.HasAnyBonus())
            {
                description.AppendLine("\n=== 装备加成 ===");
                description.AppendLine(totalEquipmentBonus.GetBonusDescription());
            }

            return description.ToString();
        }

        /// <summary>
        /// 测试装备效果
        /// </summary>
        [ContextMenu("打印角色状态")]
        public void PrintCharacterStatus()
        {
            Debug.Log(GetCharacterStatusDescription());
        }

        /// <summary>
        /// 测试装备效果
        /// </summary>
        [ContextMenu("测试装备效果")]
        public void TestEquipmentEffect()
        {
            var testEquipment = new EquipmentAttributes(20f, 10f, 5f, 3f, 1f);
            ApplyEquipmentBonus(testEquipment, true);
        }

        /// <summary>
        /// 销毁时清理
        /// </summary>
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}