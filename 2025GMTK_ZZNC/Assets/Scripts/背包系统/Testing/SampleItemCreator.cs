using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Data;
using System.Collections.Generic;
using System.Reflection;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 示例物品创建器 - 用于快速创建完整配置的物品数据
    /// 自动设置属性、图标、描述等完整信息
    /// </summary>
    public class SampleItemCreator : MonoBehaviour
    {
        [Header("创建设置")]
        [SerializeField] private bool createOnStart = false;
        [SerializeField] private bool showDetailedLog = true;


        // 资源路径常量
        private const string WEAPON_DATA_PATH = "Assets/Resources/Items/Weapons";
        private const string EQUIPMENT_DATA_PATH = "Assets/Resources/Items/Equipment";
        private const string CONSUMABLE_DATA_PATH = "Assets/Resources/Items/Consumables";

        private const string WEAPON_SPRITE_PATH = "Assets/Resources/Sprites/Weapons";
        private const string EQUIPMENT_SPRITE_PATH = "Assets/Resources/Sprites/Equipment";
        private const string CONSUMABLE_SPRITE_PATH = "Assets/Resources/Sprites/Consumables";

        [Header("保存文件夹（相对Assets路径）")]
        [SerializeField] private string saveFolder = "Assets/Resources/Items";

        private void Start()
        {
            if (createOnStart)
            {
                CreateAllSampleItems();
            }
        }

        /// <summary>
        /// 创建所有示例物品
        /// </summary>
        [ContextMenu("创建所有示例物品")]
        public void CreateAllSampleItems()
        {
            {
                // 确保文件夹存在
                string weaponFolder = $"{saveFolder}/Weapons";
                if (!UnityEditor.AssetDatabase.IsValidFolder(weaponFolder))
                {
                    System.IO.Directory.CreateDirectory(weaponFolder);
                    UnityEditor.AssetDatabase.Refresh();
                }

                // 创建剑
                var sword = ScriptableObject.CreateInstance<WeaponData>();
                UnityEditor.AssetDatabase.CreateAsset(sword, $"{weaponFolder}/铁剑.asset");

                // 创建弓
                var bow = ScriptableObject.CreateInstance<WeaponData>();
                UnityEditor.AssetDatabase.CreateAsset(bow, $"{weaponFolder}/长弓.asset");

                // 创建法杖
                var staff = ScriptableObject.CreateInstance<WeaponData>();
                UnityEditor.AssetDatabase.CreateAsset(staff, $"{weaponFolder}/法师杖.asset");

                // 创建匕首
                var dagger = ScriptableObject.CreateInstance<WeaponData>();
                UnityEditor.AssetDatabase.CreateAsset(dagger, $"{weaponFolder}/精钢匕首.asset");

                UnityEditor.AssetDatabase.SaveAssets();
                Debug.Log("示例武器创建完成！");
            }
            {
                CreateSampleEquipments();
            }
            {
                CreateSampleConsumables();
            }

        }
        /// <summary>
        /// 创建示例装备
        /// </summary>
        [ContextMenu("创建示例装备")]
        public void CreateSampleEquipments()
        {
            // 确保文件夹存在
            string equipmentFolder = $"{saveFolder}/Equipment";
            if (!UnityEditor.AssetDatabase.IsValidFolder(equipmentFolder))
            {
                System.IO.Directory.CreateDirectory(equipmentFolder);
                UnityEditor.AssetDatabase.Refresh();
            }

            // 创建头盔
            var helmet = ScriptableObject.CreateInstance<EquipmentData>();
            UnityEditor.AssetDatabase.CreateAsset(helmet, $"{equipmentFolder}/铁制头盔.asset");

            // 创建护甲
            var armor = ScriptableObject.CreateInstance<EquipmentData>();
            UnityEditor.AssetDatabase.CreateAsset(armor, $"{equipmentFolder}/皮革护甲.asset");

            // 创建靴子
            var boots = ScriptableObject.CreateInstance<EquipmentData>();
            UnityEditor.AssetDatabase.CreateAsset(boots, $"{equipmentFolder}/旅行者之靴.asset");

            // 创建饰品
            var accessory = ScriptableObject.CreateInstance<EquipmentData>();
            UnityEditor.AssetDatabase.CreateAsset(accessory, $"{equipmentFolder}/力量戒指.asset");

            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log("示例装备创建完成！");
        }

        /// <summary>
        /// 创建示例消耗品
        /// </summary>
        [ContextMenu("创建示例消耗品")]
        public void CreateSampleConsumables()
        {
            // 确保文件夹存在
            string consumableFolder = $"{saveFolder}/Consumables";
            if (!UnityEditor.AssetDatabase.IsValidFolder(consumableFolder))
            {
                System.IO.Directory.CreateDirectory(consumableFolder);
                UnityEditor.AssetDatabase.Refresh();
            }

            // 创建生命药水
            var healthPotion = ScriptableObject.CreateInstance<ConsumableData>();
            UnityEditor.AssetDatabase.CreateAsset(healthPotion, $"{consumableFolder}/生命药水.asset");

            // 创建魔法药水
            var manaPotion = ScriptableObject.CreateInstance<ConsumableData>();
            UnityEditor.AssetDatabase.CreateAsset(manaPotion, $"{consumableFolder}/魔法药水.asset");

            // 创建速度药水
            var speedPotion = ScriptableObject.CreateInstance<ConsumableData>();
            UnityEditor.AssetDatabase.CreateAsset(speedPotion, $"{consumableFolder}/速度药水.asset");

            // 创建力量药水
            var strengthPotion = ScriptableObject.CreateInstance<ConsumableData>();
            UnityEditor.AssetDatabase.CreateAsset(strengthPotion, $"{consumableFolder}/力量药水.asset");

            // 创建面包
            var bread = ScriptableObject.CreateInstance<ConsumableData>();
            UnityEditor.AssetDatabase.CreateAsset(bread, $"{consumableFolder}/黑面包.asset");

            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log("示例消耗品创建完成！");
        }
    }
}