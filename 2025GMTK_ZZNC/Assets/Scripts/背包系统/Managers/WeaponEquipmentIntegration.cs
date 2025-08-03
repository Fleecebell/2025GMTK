using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem.Items;

namespace InventorySystem.Managers
{
    /// <summary>
    /// 武器装备集成脚本
    /// 展示如何将WeaponInstanceManager与装备系统集成
    /// </summary>
    public class WeaponEquipmentIntegration : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private EquipmentManager equipmentManager;
        [SerializeField] private WeaponInstanceManager weaponInstanceManager;
        [SerializeField] private ZZNC_Player playerController;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Start()
        {
            InitializeIntegration();
        }

        /// <summary>
        /// 初始化集成
        /// </summary>
        private void InitializeIntegration()
        {
            // 获取组件引用
            if (equipmentManager == null)
                equipmentManager = GetComponent<EquipmentManager>();

            if (weaponInstanceManager == null)
                weaponInstanceManager = GetComponent<WeaponInstanceManager>();

            // 绑定事件
            if (equipmentManager != null)
            {
                equipmentManager.OnWeaponEquipped += OnWeaponEquipped;
                equipmentManager.OnWeaponUnequipped += OnWeaponUnequipped;
            }
        }

        /// <summary>
        /// 武器装备事件处理
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        private void OnWeaponEquipped(WeaponData weaponData)
        {
            if (weaponInstanceManager != null)
            {
                // 创建武器实例
                bool success = weaponInstanceManager.CreateWeaponInstance(weaponData);

                if (success)
                {
                    Debug.Log($"武器实例创建成功: {weaponData.ItemName}");
                    playerController.SetPlayerWeapon(weaponInstanceManager.CurrentWeaponInstance,weaponInstanceManager.CurrentWeaponScript);
                }
                else
                {
                    Debug.LogError($"武器实例创建失败: {weaponData.ItemName}");
                }
            }
        }

        /// <summary>
        /// 武器卸下事件处理
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        private void OnWeaponUnequipped(WeaponData weaponData)
        {
            if (weaponInstanceManager != null)
            {
                // 销毁武器实例
                weaponInstanceManager.DestroyCurrentWeaponInstance();
                Debug.Log($"武器实例已销毁: {weaponData.ItemName}");
                playerController.SetPlayerWeapon(null);
            }
        }

        /// <summary>
        /// 清理事件绑定
        /// </summary>
        private void OnDestroy()
        {
            if (equipmentManager != null)
            {
                equipmentManager.OnWeaponEquipped -= OnWeaponEquipped;
                equipmentManager.OnWeaponUnequipped -= OnWeaponUnequipped;
            }
        }
    }
}