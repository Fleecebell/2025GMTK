using System;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Data;
using InventorySystem.Character;

namespace InventorySystem.Managers
{
    /// <summary>
    /// 装备管理器 - 管理玩家当前装备和装备效果
    /// 遵循单一职责原则，专门处理装备相关逻辑
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        [Header("装备槽位")]
        [SerializeField] private WeaponData currentWeapon;
        [SerializeField] private List<EquipmentData> equippedItems = new List<EquipmentData>();

        // 事件系统
        public event Action<WeaponData> OnWeaponEquipped;
        public event Action<WeaponData> OnWeaponUnequipped;
        public event Action<EquipmentData> OnEquipmentEquipped;
        public event Action<EquipmentData> OnEquipmentUnequipped;
        public event Action OnEquipmentChanged;

        // 只读属性
        public WeaponData CurrentWeapon => currentWeapon;
        public List<EquipmentData> EquippedItems => new List<EquipmentData>(equippedItems);

        /// <summary>
        /// 初始化装备管理器
        /// </summary>
        private void Awake()
        {
            equippedItems = new List<EquipmentData>();
        }

        /// <summary>
        /// 装备武器
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        /// <returns>是否装备成功</returns>
        public bool EquipWeapon(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogWarning("武器数据为空");
                return false;
            }

            // 卸下当前武器
            WeaponData previousWeapon = currentWeapon;
            if (previousWeapon != null)
            {
                UnequipWeapon();
            }

            // 装备新武器
            currentWeapon = weaponData;
            ApplyWeaponEffects(weaponData);

            // 触发事件（WeaponInstanceManager会监听这个事件来创建武器实例）
            OnWeaponEquipped?.Invoke(weaponData);
            OnEquipmentChanged?.Invoke();

            Debug.Log($"装备武器: {weaponData.ItemName}");
            return true;
        }

        /// <summary>
        /// 卸下武器
        /// </summary>
        /// <returns>被卸下的武器</returns>
        public WeaponData UnequipWeapon()
        {
            if (currentWeapon == null)
                return null;

            WeaponData unequippedWeapon = currentWeapon;
            
            // 移除武器效果
            RemoveWeaponEffects(currentWeapon);
            
            // 清空当前武器
            currentWeapon = null;

            // 触发事件（WeaponInstanceManager会监听这个事件来销毁武器实例）
            OnWeaponUnequipped?.Invoke(unequippedWeapon);
            OnEquipmentChanged?.Invoke();

            Debug.Log($"卸下武器: {unequippedWeapon.ItemName}");
            return unequippedWeapon;
        }

        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="equipmentData">装备数据</param>
        /// <returns>是否装备成功</returns>
        public bool EquipItem(EquipmentData equipmentData)
        {
            if (equipmentData == null)
            {
                Debug.LogWarning("装备数据为空");
                return false;
            }

            // 添加到装备列表
            equippedItems.Add(equipmentData);
            ApplyEquipmentEffects(equipmentData);

            // 触发事件
            OnEquipmentEquipped?.Invoke(equipmentData);
            OnEquipmentChanged?.Invoke();

            Debug.Log($"装备物品: {equipmentData.ItemName}");
            return true;
        }

        /// <summary>
        /// 卸下装备（已禁用）
        /// </summary>
        /// <param name="equipmentData">要卸下的装备</param>
        /// <returns>null，因为装备无法卸下</returns>
        public EquipmentData UnequipItem(EquipmentData equipmentData)
        {
            Debug.LogWarning($"装备 {equipmentData?.ItemName} 无法卸下，装备系统已设置为自动装备模式");
            return null;
        }

        /// <summary>
        /// 获取所有已装备的物品
        /// </summary>
        /// <returns>已装备的物品列表</returns>
        public List<EquipmentData> GetAllEquippedItems()
        {
            return new List<EquipmentData>(equippedItems);
        }

        /// <summary>
        /// 获取指定属性的总加成值
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>总加成值</returns>
        public float GetTotalAttributeBonus(AttributeType attributeType)
        {
            float totalBonus = 0f;

            // 计算装备加成
            foreach (var equipment in equippedItems)
            {
                if (equipment != null)
                {
                    totalBonus += equipment.GetAttributeBonus(attributeType);
                }
            }

            return totalBonus;
        }

        /// <summary>
        /// 应用武器效果
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        private void ApplyWeaponEffects(WeaponData weaponData)
        {
            // 武器暂时不提供属性加成，只记录装备状态
            Debug.Log($"应用武器效果: {weaponData.ItemName}");
        }

        /// <summary>
        /// 移除武器效果
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        private void RemoveWeaponEffects(WeaponData weaponData)
        {
            // 武器暂时不提供属性加成，只记录装备状态
            Debug.Log($"移除武器效果: {weaponData.ItemName}");
        }

        /// <summary>
        /// 应用装备效果
        /// </summary>
        /// <param name="equipmentData">装备数据</param>
        private void ApplyEquipmentEffects(EquipmentData equipmentData)
        {
            var characterManager = CharacterManager.Instance;
            if (characterManager != null)
            {
                var equipmentAttributes = equipmentData.GetEquipmentAttributes();
                characterManager.ApplyEquipmentBonus(equipmentAttributes, true);
            }
        }

        /// <summary>
        /// 移除装备效果
        /// </summary>
        /// <param name="equipmentData">装备数据</param>
        private void RemoveEquipmentEffects(EquipmentData equipmentData)
        {
            var characterManager = CharacterManager.Instance;
            if (characterManager != null)
            {
                var equipmentAttributes = equipmentData.GetEquipmentAttributes();
                characterManager.ApplyEquipmentBonus(equipmentAttributes, false);
            }
        }

        /// <summary>
        /// 获取装备统计信息
        /// </summary>
        /// <returns>装备统计信息字符串</returns>
        public string GetEquipmentStats()
        {
            var stats = new System.Text.StringBuilder();
            stats.AppendLine("=== 装备状态 ===");
            
            if (currentWeapon != null)
            {
                stats.AppendLine($"武器: {currentWeapon.ItemName}");
            }
            else
            {
                stats.AppendLine("武器: 无");
            }

            if (equippedItems.Count > 0)
            {
                stats.AppendLine($"装备数量: {equippedItems.Count}");
                foreach (var equipment in equippedItems)
                {
                    stats.AppendLine($"  - {equipment.ItemName}");
                }
            }
            else
            {
                stats.AppendLine("装备: 无");
            }

            return stats.ToString();
        }

        /// <summary>
        /// 清空所有装备（已禁用）
        /// </summary>
        public void ClearAllEquipment()
        {
            Debug.LogWarning("无法清空装备，装备系统已设置为自动装备模式");
        }

        /// <summary>
        /// 调试用：打印装备状态
        /// </summary>
        [ContextMenu("打印装备状态")]
        public void PrintEquipmentStatus()
        {
            Debug.Log(GetEquipmentStats());
        }

        /// <summary>
        /// 调试用：测试武器装备
        /// </summary>
        [ContextMenu("测试武器装备")]
        public void TestWeaponEquipment()
        {
            if (currentWeapon != null)
            {
                Debug.Log($"当前武器: {currentWeapon.ItemName}");
                Debug.Log("武器实例管理由WeaponInstanceManager负责");
            }
            else
            {
                Debug.Log("当前没有装备武器");
            }
        }
    }
}