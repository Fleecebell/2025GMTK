using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Data;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 物品数据修复器 - 用于修复物品数据的常见问题
    /// </summary>
    public class ItemDataFixer : MonoBehaviour
    {
        [Header("修复设置")]
        [SerializeField] private bool autoFixOnStart = true;
        [SerializeField] private bool showFixMessages = true;

        private void Start()
        {
            if (autoFixOnStart)
            {
                Invoke(nameof(FixAllItemData), 0.1f);
            }
        }

        /// <summary>
        /// 修复所有物品数据
        /// </summary>
        [ContextMenu("修复所有物品数据")]
        public void FixAllItemData()
        {
            if (showFixMessages)
                Debug.Log("=== 开始修复物品数据 ===");

            #if UNITY_EDITOR
            int fixedCount = 0;

            // 修复武器数据
            string[] weaponGuids = UnityEditor.AssetDatabase.FindAssets("t:WeaponData");
            foreach (string guid in weaponGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                WeaponData weapon = UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponData>(path);
                if (weapon != null)
                {
                    bool needsFix = false;
                    
                    // 检查并修复物品名称
                    if (string.IsNullOrEmpty(weapon.ItemName))
                    {
                        // 使用反射设置私有字段
                        var itemNameField = typeof(BaseItemData).GetField("itemName", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (itemNameField != null)
                        {
                            itemNameField.SetValue(weapon, weapon.name);
                            needsFix = true;
                        }
                    }
                    
                    // 检查并修复描述
                    if (string.IsNullOrEmpty(weapon.Description))
                    {
                        var descriptionField = typeof(BaseItemData).GetField("description", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (descriptionField != null)
                        {
                            descriptionField.SetValue(weapon, $"一把{weapon.name}");
                            needsFix = true;
                        }
                    }
                    
                    // 检查并修复类型和堆叠数
                    if (weapon.ItemType != ItemType.Weapon || weapon.MaxStackSize != 1)
                    {
                        var itemTypeField = typeof(BaseItemData).GetField("itemType", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var maxStackSizeField = typeof(BaseItemData).GetField("maxStackSize", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (itemTypeField != null && maxStackSizeField != null)
                        {
                            itemTypeField.SetValue(weapon, ItemType.Weapon);
                            maxStackSizeField.SetValue(weapon, 1);
                            needsFix = true;
                        }
                    }
                    
                    if (needsFix)
                    {
                        UnityEditor.EditorUtility.SetDirty(weapon);
                        fixedCount++;
                        if (showFixMessages)
                            Debug.Log($"修复武器数据: {weapon.name}");
                    }
                }
            }

            // 修复装备数据
            string[] equipmentGuids = UnityEditor.AssetDatabase.FindAssets("t:EquipmentData");
            foreach (string guid in equipmentGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                EquipmentData equipment = UnityEditor.AssetDatabase.LoadAssetAtPath<EquipmentData>(path);
                if (equipment != null)
                {
                    bool needsFix = false;
                    
                    // 检查并修复物品名称
                    if (string.IsNullOrEmpty(equipment.ItemName))
                    {
                        var itemNameField = typeof(BaseItemData).GetField("itemName", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (itemNameField != null)
                        {
                            itemNameField.SetValue(equipment, equipment.name);
                            needsFix = true;
                        }
                    }
                    
                    // 检查并修复描述
                    if (string.IsNullOrEmpty(equipment.Description))
                    {
                        var descriptionField = typeof(BaseItemData).GetField("description", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (descriptionField != null)
                        {
                            descriptionField.SetValue(equipment, $"一件{equipment.name}");
                            needsFix = true;
                        }
                    }
                    
                    // 检查并修复类型和堆叠数
                    if (equipment.ItemType != ItemType.Equipment || equipment.MaxStackSize != 1)
                    {
                        var itemTypeField = typeof(BaseItemData).GetField("itemType", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var maxStackSizeField = typeof(BaseItemData).GetField("maxStackSize", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (itemTypeField != null && maxStackSizeField != null)
                        {
                            itemTypeField.SetValue(equipment, ItemType.Equipment);
                            maxStackSizeField.SetValue(equipment, 1);
                            needsFix = true;
                        }
                    }
                    
                    if (needsFix)
                    {
                        UnityEditor.EditorUtility.SetDirty(equipment);
                        fixedCount++;
                        if (showFixMessages)
                            Debug.Log($"修复装备数据: {equipment.name}");
                    }
                }
            }

            // 修复消耗品数据
            string[] consumableGuids = UnityEditor.AssetDatabase.FindAssets("t:ConsumableData");
            foreach (string guid in consumableGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ConsumableData consumable = UnityEditor.AssetDatabase.LoadAssetAtPath<ConsumableData>(path);
                if (consumable != null)
                {
                    bool needsFix = false;
                    
                    // 检查并修复物品名称
                    if (string.IsNullOrEmpty(consumable.ItemName))
                    {
                        var itemNameField = typeof(BaseItemData).GetField("itemName", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (itemNameField != null)
                        {
                            itemNameField.SetValue(consumable, consumable.name);
                            needsFix = true;
                        }
                    }
                    
                    // 检查并修复描述
                    if (string.IsNullOrEmpty(consumable.Description))
                    {
                        var descriptionField = typeof(BaseItemData).GetField("description", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (descriptionField != null)
                        {
                            descriptionField.SetValue(consumable, $"一个{consumable.name}");
                            needsFix = true;
                        }
                    }
                    
                    // 检查并修复类型和堆叠数
                    if (consumable.ItemType != ItemType.Consumable || consumable.MaxStackSize <= 1)
                    {
                        var itemTypeField = typeof(BaseItemData).GetField("itemType", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var maxStackSizeField = typeof(BaseItemData).GetField("maxStackSize", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var isConsumableField = typeof(BaseItemData).GetField("isConsumable", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (itemTypeField != null && maxStackSizeField != null && isConsumableField != null)
                        {
                            itemTypeField.SetValue(consumable, ItemType.Consumable);
                            maxStackSizeField.SetValue(consumable, 99);
                            isConsumableField.SetValue(consumable, true);
                            needsFix = true;
                        }
                    }
                    
                    if (needsFix)
                    {
                        UnityEditor.EditorUtility.SetDirty(consumable);
                        fixedCount++;
                        if (showFixMessages)
                            Debug.Log($"修复消耗品数据: {consumable.name}");
                    }
                }
            }

            if (fixedCount > 0)
            {
                UnityEditor.AssetDatabase.SaveAssets();
                if (showFixMessages)
                    Debug.Log($"? 物品数据修复完成！共修复了 {fixedCount} 个物品");
            }
            else
            {
                if (showFixMessages)
                    Debug.Log("? 所有物品数据都正常，无需修复");
            }
            #else
            Debug.LogWarning("物品数据修复只能在编辑器中运行");
            #endif
        }

        /// <summary>
        /// 创建并修复示例物品数据
        /// </summary>
        [ContextMenu("创建并修复示例物品")]
        public void CreateAndFixSampleItems()
        {
            #if UNITY_EDITOR
            string sampleFolder = "Assets/ItemData/Samples";
            
            // 确保文件夹存在
            if (!UnityEditor.AssetDatabase.IsValidFolder(sampleFolder))
            {
                System.IO.Directory.CreateDirectory(sampleFolder);
                UnityEditor.AssetDatabase.Refresh();
            }

            // 创建示例武器
            var sword = ScriptableObject.CreateInstance<WeaponData>();
            SetItemData(sword, "铁剑", "一把锋利的铁制长剑", ItemType.Weapon, 1, false);
            UnityEditor.AssetDatabase.CreateAsset(sword, $"{sampleFolder}/铁剑.asset");

            var bow = ScriptableObject.CreateInstance<WeaponData>();
            SetItemData(bow, "长弓", "一把精制的长弓", ItemType.Weapon, 1, false);
            UnityEditor.AssetDatabase.CreateAsset(bow, $"{sampleFolder}/长弓.asset");

            // 创建示例装备
            var helmet = ScriptableObject.CreateInstance<EquipmentData>();
            SetItemData(helmet, "铁制头盔", "一顶坚固的铁制头盔", ItemType.Equipment, 1, false);
            UnityEditor.AssetDatabase.CreateAsset(helmet, $"{sampleFolder}/铁制头盔.asset");

            var armor = ScriptableObject.CreateInstance<EquipmentData>();
            SetItemData(armor, "皮革护甲", "一件轻便的皮革护甲", ItemType.Equipment, 1, false);
            UnityEditor.AssetDatabase.CreateAsset(armor, $"{sampleFolder}/皮革护甲.asset");

            // 创建示例消耗品
            var healthPotion = ScriptableObject.CreateInstance<ConsumableData>();
            SetItemData(healthPotion, "生命药水", "恢复生命值的药水", ItemType.Consumable, 99, true);
            UnityEditor.AssetDatabase.CreateAsset(healthPotion, $"{sampleFolder}/生命药水.asset");

            var manaPotion = ScriptableObject.CreateInstance<ConsumableData>();
            SetItemData(manaPotion, "魔法药水", "恢复魔法值的药水", ItemType.Consumable, 99, true);
            UnityEditor.AssetDatabase.CreateAsset(manaPotion, $"{sampleFolder}/魔法药水.asset");

            UnityEditor.AssetDatabase.SaveAssets();
            
            if (showFixMessages)
                Debug.Log("? 示例物品创建完成！");
            #endif
        }

        #if UNITY_EDITOR
        /// <summary>
        /// 设置物品数据
        /// </summary>
        private void SetItemData(BaseItemData item, string name, string description, ItemType type, int maxStack, bool consumable)
        {
            var itemNameField = typeof(BaseItemData).GetField("itemName", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var descriptionField = typeof(BaseItemData).GetField("description", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var itemTypeField = typeof(BaseItemData).GetField("itemType", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var maxStackSizeField = typeof(BaseItemData).GetField("maxStackSize", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isConsumableField = typeof(BaseItemData).GetField("isConsumable", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            itemNameField?.SetValue(item, name);
            descriptionField?.SetValue(item, description);
            itemTypeField?.SetValue(item, type);
            maxStackSizeField?.SetValue(item, maxStack);
            isConsumableField?.SetValue(item, consumable);
        }
        #endif
    }
}