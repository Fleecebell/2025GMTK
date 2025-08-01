using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Data;
using System.Collections.Generic;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 增强示例物品创建器 - 创建完整的示例物品数据，支持指定Resources路径
    /// 支持图标自动加载和完整的物品属性配置
    /// </summary>
    public class EnhancedSampleItemCreator : MonoBehaviour
    {
        [Header("创建设置")]
        [SerializeField] private bool createOnStart = false;
        [SerializeField] private bool showCreationMessages = true;
        [SerializeField] private bool overwriteExisting = false;

        [Header("路径设置")]
        [SerializeField] private string weaponDataPath = "Assets/Resources/Items/Weapons/";
        [SerializeField] private string equipmentDataPath = "Assets/Resources/Items/Equipment/";
        [SerializeField] private string consumableDataPath = "Assets/Resources/Items/Consumables/";
        
        [Header("图标路径设置")]
        [SerializeField] private string weaponSpritePath = "Assets/Resources/Sprites/Weapons/";
        [SerializeField] private string equipmentSpritePath = "Assets/Resources/Sprites/Equipment/";
        [SerializeField] private string consumableSpritePath = "Assets/Resources/Sprites/Consumables/";

        // 物品基础结构
        [System.Serializable]
        private struct ItemDefinition
        {
            public string name;
            public string description;
            public string iconName;
        }

        // 武器定义结构
        [System.Serializable]
        private struct WeaponDefinition
        {
            public ItemDefinition baseInfo;
            public AttackType attackType;
            public float damage;
            public float attackSpeed;
        }

        // 装备定义结构
        [System.Serializable]
        private struct EquipmentDefinition
        {
            public ItemDefinition baseInfo;
            public List<AttributeModifierData> attributes;
            public string specialEffect;
        }

        // 消耗品定义结构
        [System.Serializable]
        private struct ConsumableDefinition
        {
            public ItemDefinition baseInfo;
            public AttributeType targetAttribute;
            public float effectValue;
            public float cooldownTime;
        }

        // 属性修改器数据结构
        [System.Serializable]
        private struct AttributeModifierData
        {
            public AttributeType attributeType;
            public float value;
            public bool isPercentage;
        }

        private void Start()
        {
            if (createOnStart)
            {
                CreateAllItemsWithFullData();
            }
        }

        /// <summary>
        /// 创建所有增强示例物品数据
        /// </summary>
        [ContextMenu("创建所有增强示例物品数据")]
        public void CreateAllItemsWithFullData()
        {
            if (showCreationMessages)
                Debug.Log("=== 开始创建增强示例物品数据 ===");

            CreateWeaponsWithFullData();
            CreateEquipmentsWithFullData();
            CreateConsumablesWithFullData();

            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif

            if (showCreationMessages)
                Debug.Log("=== 增强示例物品数据创建完成 ===");
        }

        /// <summary>
        /// 创建武器数据
        /// </summary>
        [ContextMenu("创建增强武器数据")]
        public void CreateWeaponsWithFullData()
        {
            if (showCreationMessages)
                Debug.Log("开始创建武器数据...");

            var weaponDefinitions = GetWeaponDefinitions();
            foreach (var weaponDef in weaponDefinitions)
            {
                CreateWeaponData(weaponDef);
            }

            if (showCreationMessages)
                Debug.Log($"武器数据创建完成，共创建 {weaponDefinitions.Count} 个武器");
        }

        /// <summary>
        /// 创建装备数据
        /// </summary>
        [ContextMenu("创建增强装备数据")]
        public void CreateEquipmentsWithFullData()
        {
            if (showCreationMessages)
                Debug.Log("开始创建装备数据...");

            var equipmentDefinitions = GetEquipmentDefinitions();
            foreach (var equipmentDef in equipmentDefinitions)
            {
                CreateEquipmentData(equipmentDef);
            }

            if (showCreationMessages)
                Debug.Log($"装备数据创建完成，共创建 {equipmentDefinitions.Count} 个装备");
        }

        /// <summary>
        /// 创建消耗品数据
        /// </summary>
        [ContextMenu("创建增强消耗品数据")]
        public void CreateConsumablesWithFullData()
        {
            if (showCreationMessages)
                Debug.Log("开始创建消耗品数据...");

            var consumableDefinitions = GetConsumableDefinitions();
            foreach (var consumableDef in consumableDefinitions)
            {
                CreateConsumableData(consumableDef);
            }

            if (showCreationMessages)
                Debug.Log($"消耗品数据创建完成，共创建 {consumableDefinitions.Count} 个消耗品");
        }

        /// <summary>
        /// 创建武器数据
        /// </summary>
        private void CreateWeaponData(WeaponDefinition weaponDef)
        {
            #if UNITY_EDITOR
            string fileName = $"{weaponDef.baseInfo.name}.asset";
            string fullPath = weaponDataPath + fileName;

            // 检查是否已存在且不允许覆盖
            if (!overwriteExisting && UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponData>(fullPath) != null)
            {
                if (showCreationMessages)
                    Debug.Log($"武器 {weaponDef.baseInfo.name} 已存在，跳过创建");
                return;
            }

            // 确保目录存在
            EnsureDirectoryExists(weaponDataPath);

            // 创建武器数据
            var weapon = ScriptableObject.CreateInstance<WeaponData>();
            
            // 设置基础数据
            SetBaseItemData(weapon, weaponDef.baseInfo, ItemType.Weapon, 1, false);
            
            // 设置武器特定数据
            SetWeaponSpecificData(weapon, weaponDef);
            
            // 设置图标
            Sprite icon = LoadIcon(weaponSpritePath, weaponDef.baseInfo.iconName);
            SetItemIcon(weapon, icon);

            // 保存资源
            UnityEditor.AssetDatabase.CreateAsset(weapon, fullPath);
            
            if (showCreationMessages)
                Debug.Log($"创建武器: {weaponDef.baseInfo.name}");
            #endif
        }

        /// <summary>
        /// 创建装备数据
        /// </summary>
        private void CreateEquipmentData(EquipmentDefinition equipmentDef)
        {
            #if UNITY_EDITOR
            string fileName = $"{equipmentDef.baseInfo.name}.asset";
            string fullPath = equipmentDataPath + fileName;

            // 检查是否已存在且不允许覆盖
            if (!overwriteExisting && UnityEditor.AssetDatabase.LoadAssetAtPath<EquipmentData>(fullPath) != null)
            {
                if (showCreationMessages)
                    Debug.Log($"装备 {equipmentDef.baseInfo.name} 已存在，跳过创建");
                return;
            }

            // 确保目录存在
            EnsureDirectoryExists(equipmentDataPath);

            // 创建装备数据
            var equipment = ScriptableObject.CreateInstance<EquipmentData>();
            
            // 设置基础数据
            SetBaseItemData(equipment, equipmentDef.baseInfo, ItemType.Equipment, 1, false);
            
            // 设置装备特定数据
            SetEquipmentSpecificData(equipment, equipmentDef);
            
            // 设置图标
            Sprite icon = LoadIcon(equipmentSpritePath, equipmentDef.baseInfo.iconName);
            SetItemIcon(equipment, icon);

            // 保存资源
            UnityEditor.AssetDatabase.CreateAsset(equipment, fullPath);
            
            if (showCreationMessages)
                Debug.Log($"创建装备: {equipmentDef.baseInfo.name}");
            #endif
        }

        /// <summary>
        /// 创建消耗品数据
        /// </summary>
        private void CreateConsumableData(ConsumableDefinition consumableDef)
        {
            #if UNITY_EDITOR
            string fileName = $"{consumableDef.baseInfo.name}.asset";
            string fullPath = consumableDataPath + fileName;

            // 检查是否已存在且不允许覆盖
            if (!overwriteExisting && UnityEditor.AssetDatabase.LoadAssetAtPath<ConsumableData>(fullPath) != null)
            {
                if (showCreationMessages)
                    Debug.Log($"消耗品 {consumableDef.baseInfo.name} 已存在，跳过创建");
                return;
            }

            // 确保目录存在
            EnsureDirectoryExists(consumableDataPath);

            // 创建消耗品数据
            var consumable = ScriptableObject.CreateInstance<ConsumableData>();
            
            // 设置基础数据
            SetBaseItemData(consumable, consumableDef.baseInfo, ItemType.Consumable, 99, true);
            
            // 设置消耗品特定数据
            SetConsumableSpecificData(consumable, consumableDef);
            
            // 设置图标
            Sprite icon = LoadIcon(consumableSpritePath, consumableDef.baseInfo.iconName);
            SetItemIcon(consumable, icon);

            // 保存资源
            UnityEditor.AssetDatabase.CreateAsset(consumable, fullPath);
            
            if (showCreationMessages)
                Debug.Log($"创建消耗品: {consumableDef.baseInfo.name}");
            #endif
        }

        /// <summary>
        /// 设置基础物品数据
        /// </summary>
        private void SetBaseItemData(BaseItemData item, ItemDefinition baseInfo, ItemType type, int maxStack, bool consumable)
        {
            #if UNITY_EDITOR
            var serializedObject = new UnityEditor.SerializedObject(item);
            
            serializedObject.FindProperty("itemName").stringValue = baseInfo.name;
            serializedObject.FindProperty("description").stringValue = baseInfo.description;
            serializedObject.FindProperty("itemType").enumValueIndex = (int)type;
            serializedObject.FindProperty("maxStackSize").intValue = maxStack;
            serializedObject.FindProperty("isConsumable").boolValue = consumable;
            
            serializedObject.ApplyModifiedProperties();
            #endif
        }

        /// <summary>
        /// 设置物品图标
        /// </summary>
        private void SetItemIcon(BaseItemData item, Sprite icon)
        {
            #if UNITY_EDITOR
            if (icon != null)
            {
                var serializedObject = new UnityEditor.SerializedObject(item);
                serializedObject.FindProperty("icon").objectReferenceValue = icon;
                serializedObject.ApplyModifiedProperties();
            }
            #endif
        }

        /// <summary>
        /// 设置武器特定数据
        /// </summary>
        private void SetWeaponSpecificData(WeaponData weapon, WeaponDefinition weaponDef)
        {
            #if UNITY_EDITOR
            var serializedObject = new UnityEditor.SerializedObject(weapon);
            
            serializedObject.FindProperty("attackType").enumValueIndex = (int)weaponDef.attackType;
            serializedObject.FindProperty("damage").floatValue = weaponDef.damage;
            serializedObject.FindProperty("attackSpeed").floatValue = weaponDef.attackSpeed;
            
            serializedObject.ApplyModifiedProperties();
            #endif
        }

        /// <summary>
        /// 设置装备特定数据
        /// </summary>
        private void SetEquipmentSpecificData(EquipmentData equipment, EquipmentDefinition equipmentDef)
        {
            #if UNITY_EDITOR
            var serializedObject = new UnityEditor.SerializedObject(equipment);
            
            // 设置特殊效果
            serializedObject.FindProperty("specialEffect").stringValue = equipmentDef.specialEffect;
            
            // 设置属性修改器
            var attributeModifiersProperty = serializedObject.FindProperty("attributeModifiers");
            attributeModifiersProperty.ClearArray();
            
            for (int i = 0; i < equipmentDef.attributes.Count; i++)
            {
                attributeModifiersProperty.InsertArrayElementAtIndex(i);
                var element = attributeModifiersProperty.GetArrayElementAtIndex(i);
                
                element.FindPropertyRelative("attributeType").enumValueIndex = (int)equipmentDef.attributes[i].attributeType;
                element.FindPropertyRelative("value").floatValue = equipmentDef.attributes[i].value;
                element.FindPropertyRelative("isPercentage").boolValue = equipmentDef.attributes[i].isPercentage;
            }
            
            serializedObject.ApplyModifiedProperties();
            #endif
        }

        /// <summary>
        /// 设置消耗品特定数据
        /// </summary>
        private void SetConsumableSpecificData(ConsumableData consumable, ConsumableDefinition consumableDef)
        {
            #if UNITY_EDITOR
            var serializedObject = new UnityEditor.SerializedObject(consumable);
            
            serializedObject.FindProperty("targetAttribute").enumValueIndex = (int)consumableDef.targetAttribute;
            serializedObject.FindProperty("effectValue").floatValue = consumableDef.effectValue;
            serializedObject.FindProperty("cooldownTime").floatValue = consumableDef.cooldownTime;
            
            serializedObject.ApplyModifiedProperties();
            #endif
        }

        /// <summary>
        /// 加载图标
        /// </summary>
        private Sprite LoadIcon(string spritePath, string iconName)
        {
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(iconName))
                return null;

            string fullPath = spritePath + iconName + ".png";
            Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            
            if (sprite == null)
            {
                // 尝试其他格式
                fullPath = spritePath + iconName + ".jpg";
                sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            }
            
            if (sprite == null)
            {
                Debug.LogWarning($"无法加载图标: {fullPath}");
            }
            
            return sprite;
            #else
            return null;
            #endif
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        private void EnsureDirectoryExists(string path)
        {
            #if UNITY_EDITOR
            if (!UnityEditor.AssetDatabase.IsValidFolder(path))
            {
                string[] pathParts = path.Split('/');
                string currentPath = pathParts[0];
                
                for (int i = 1; i < pathParts.Length; i++)
                {
                    string nextPath = currentPath + "/" + pathParts[i];
                    if (!UnityEditor.AssetDatabase.IsValidFolder(nextPath))
                    {
                        UnityEditor.AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                    }
                    currentPath = nextPath;
                }
            }
            #endif
        }

        /// <summary>
        /// 获取武器定义列表
        /// </summary>
        private List<WeaponDefinition> GetWeaponDefinitions()
        {
            return new List<WeaponDefinition>
            {
                new WeaponDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "铁剑",
                        description = "一把普通的铁剑，适合新手使用",
                        iconName = "iron_sword"
                    },
                    attackType = AttackType.Slash,
                    damage = 15f,
                    attackSpeed = 1.2f
                },
                new WeaponDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "长弓",
                        description = "远程武器，可以攻击远处的敌人",
                        iconName = "long_bow"
                    },
                    attackType = AttackType.Firearm,
                    damage = 12f,
                    attackSpeed = 0.8f
                },
                new WeaponDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "法杖",
                        description = "魔法武器，可以施展法术",
                        iconName = "magic_staff"
                    },
                    attackType = AttackType.Thrust,
                    damage = 18f,
                    attackSpeed = 1.0f
                },
                new WeaponDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "匕首",
                        description = "轻便的短武器，攻击速度快",
                        iconName = "dagger"
                    },
                    attackType = AttackType.Slash,
                    damage = 10f,
                    attackSpeed = 1.5f
                }
            };
        }

        /// <summary>
        /// 获取装备定义列表
        /// </summary>
        private List<EquipmentDefinition> GetEquipmentDefinitions()
        {
            return new List<EquipmentDefinition>
            {
                new EquipmentDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "铁头盔",
                        description = "提供基础防御的头盔",
                        iconName = "iron_helmet"
                    },
                    attributes = new List<AttributeModifierData>
                    {
                        new AttributeModifierData { attributeType = AttributeType.Defense, value = 5f, isPercentage = false },
                        new AttributeModifierData { attributeType = AttributeType.Health, value = 20f, isPercentage = false }
                    },
                    specialEffect = "减少头部受到的伤害"
                },
                new EquipmentDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "皮甲",
                        description = "轻便的护甲，不影响移动",
                        iconName = "leather_armor"
                    },
                    attributes = new List<AttributeModifierData>
                    {
                        new AttributeModifierData { attributeType = AttributeType.Defense, value = 8f, isPercentage = false },
                        new AttributeModifierData { attributeType = AttributeType.MoveSpeed, value = 0.5f, isPercentage = false }
                    },
                    specialEffect = "提升移动速度"
                },
                new EquipmentDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "疾行之靴",
                        description = "提升移动速度的靴子",
                        iconName = "speed_boots"
                    },
                    attributes = new List<AttributeModifierData>
                    {
                        new AttributeModifierData { attributeType = AttributeType.MoveSpeed, value = 2f, isPercentage = false }
                    },
                    specialEffect = "大幅提升移动速度"
                },
                new EquipmentDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "力量戒指",
                        description = "增强攻击力的戒指",
                        iconName = "strength_ring"
                    },
                    attributes = new List<AttributeModifierData>
                    {
                        new AttributeModifierData { attributeType = AttributeType.Attack, value = 10f, isPercentage = false },
                        new AttributeModifierData { attributeType = AttributeType.CriticalRate, value = 0.05f, isPercentage = false }
                    },
                    specialEffect = "提升攻击力和暴击率"
                }
            };
        }

        /// <summary>
        /// 获取消耗品定义列表
        /// </summary>
        private List<ConsumableDefinition> GetConsumableDefinitions()
        {
            return new List<ConsumableDefinition>
            {
                new ConsumableDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "生命药水",
                        description = "恢复生命值的药水",
                        iconName = "health_potion"
                    },
                    targetAttribute = AttributeType.Health,
                    effectValue = 50f,
                    cooldownTime = 5f
                },
                new ConsumableDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "魔法药水",
                        description = "恢复San值的药水",
                        iconName = "mana_potion"
                    },
                    targetAttribute = AttributeType.San,
                    effectValue = 30f,
                    cooldownTime = 3f
                },
                new ConsumableDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "速度药水",
                        description = "临时提升移动速度",
                        iconName = "speed_potion"
                    },
                    targetAttribute = AttributeType.MoveSpeed,
                    effectValue = 3f,
                    cooldownTime = 10f
                },
                new ConsumableDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "力量药水",
                        description = "临时提升攻击力",
                        iconName = "strength_potion"
                    },
                    targetAttribute = AttributeType.Attack,
                    effectValue = 15f,
                    cooldownTime = 8f
                },
                new ConsumableDefinition
                {
                    baseInfo = new ItemDefinition
                    {
                        name = "面包",
                        description = "基础食物，恢复少量生命值",
                        iconName = "bread"
                    },
                    targetAttribute = AttributeType.Health,
                    effectValue = 20f,
                    cooldownTime = 2f
                }
            };
        }

        /// <summary>
        /// 验证资源路径
        /// </summary>
        [ContextMenu("验证资源路径")]
        public void ValidateResources()
        {
            Debug.Log("=== 验证资源路径 ===");
            
            // 验证数据路径
            ValidatePath("武器数据路径", weaponDataPath);
            ValidatePath("装备数据路径", equipmentDataPath);
            ValidatePath("消耗品数据路径", consumableDataPath);
            
            // 验证图标路径
            ValidatePath("武器图标路径", weaponSpritePath);
            ValidatePath("装备图标路径", equipmentSpritePath);
            ValidatePath("消耗品图标路径", consumableSpritePath);
            
            Debug.Log("=== 资源路径验证完成 ===");
        }

        /// <summary>
        /// 验证路径
        /// </summary>
        private void ValidatePath(string pathName, string path)
        {
            #if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.IsValidFolder(path))
            {
                Debug.Log($"{pathName}: ? 有效");
            }
            else
            {
                Debug.LogWarning($"{pathName}: ? 无效，将自动创建");
            }
            #endif
        }

        /// <summary>
        /// 验证图标
        /// </summary>
        private bool ValidateIcon(string spritePath, string iconName)
        {
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(iconName))
                return false;

            string fullPath = spritePath + iconName + ".png";
            Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            
            if (sprite == null)
            {
                fullPath = spritePath + iconName + ".jpg";
                sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            }
            
            return sprite != null;
            #else
            return false;
            #endif
        }
    }
}