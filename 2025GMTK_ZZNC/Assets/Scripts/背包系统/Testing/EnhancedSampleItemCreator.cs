using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Data;
using System.Collections.Generic;

namespace InventorySystem.Testing
{
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

        [System.Serializable]
        private struct ItemDefinition
        {
            public string name;
            public string description;
            public string iconName;
            public Quality quality;
        }

        [System.Serializable]
        private struct WeaponDefinition
        {
            public ItemDefinition baseInfo;
            public AttackType attackType;
            public float damage;
            public float attackSpeed;
        }

        [System.Serializable]
        private struct EquipmentDefinition
        {
            public ItemDefinition baseInfo;
            public List<AttributeModifierData> attributes;
            public string specialEffect;
        }

        [System.Serializable]
        private struct ConsumableDefinition
        {
            public ItemDefinition baseInfo;
            public AttributeType targetAttribute;
            public float effectValue;
            public float cooldownTime;
        }

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
                private void CreateWeaponData(WeaponDefinition weaponDef)
        {
            #if UNITY_EDITOR
            string fileName = $"{weaponDef.baseInfo.name}.asset";
            string fullPath = weaponDataPath + fileName;

            if (!overwriteExisting && UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponData>(fullPath) != null)
            {
                if (showCreationMessages)
                    Debug.Log($"武器 {weaponDef.baseInfo.name} 已存在，跳过创建");
                return;
            }

            EnsureDirectoryExists(weaponDataPath);
            var weapon = ScriptableObject.CreateInstance<WeaponData>();
            SetBaseItemData(weapon, weaponDef.baseInfo, ItemType.Weapon, 1, false);
            SetWeaponSpecificData(weapon, weaponDef);
            
            Sprite icon = LoadIcon(weaponSpritePath, weaponDef.baseInfo.iconName);
            SetItemIcon(weapon, icon);

            UnityEditor.AssetDatabase.CreateAsset(weapon, fullPath);
            
            if (showCreationMessages)
                Debug.Log($"创建武器: {weaponDef.baseInfo.name}");
            #endif
        }

        private void CreateEquipmentData(EquipmentDefinition equipmentDef)
        {
            #if UNITY_EDITOR
            string fileName = $"{equipmentDef.baseInfo.name}.asset";
            string fullPath = equipmentDataPath + fileName;

            if (!overwriteExisting && UnityEditor.AssetDatabase.LoadAssetAtPath<EquipmentData>(fullPath) != null)
            {
                if (showCreationMessages)
                    Debug.Log($"装备 {equipmentDef.baseInfo.name} 已存在，跳过创建");
                return;
            }

            EnsureDirectoryExists(equipmentDataPath);
            var equipment = ScriptableObject.CreateInstance<EquipmentData>();
            SetBaseItemData(equipment, equipmentDef.baseInfo, ItemType.Equipment, 1, false);
            SetEquipmentSpecificData(equipment, equipmentDef);
            
            Sprite icon = LoadIcon(equipmentSpritePath, equipmentDef.baseInfo.iconName);
            SetItemIcon(equipment, icon);

            UnityEditor.AssetDatabase.CreateAsset(equipment, fullPath);
            
            if (showCreationMessages)
                Debug.Log($"创建装备: {equipmentDef.baseInfo.name}");
            #endif
        }

        private void CreateConsumableData(ConsumableDefinition consumableDef)
        {
            #if UNITY_EDITOR
            string fileName = $"{consumableDef.baseInfo.name}.asset";
            string fullPath = consumableDataPath + fileName;

            if (!overwriteExisting && UnityEditor.AssetDatabase.LoadAssetAtPath<ConsumableData>(fullPath) != null)
            {
                if (showCreationMessages)
                    Debug.Log($"消耗品 {consumableDef.baseInfo.name} 已存在，跳过创建");
                return;
            }

            EnsureDirectoryExists(consumableDataPath);
            var consumable = ScriptableObject.CreateInstance<ConsumableData>();
            SetBaseItemData(consumable, consumableDef.baseInfo, ItemType.Consumable, 99, true);
            SetConsumableSpecificData(consumable, consumableDef);
            
            Sprite icon = LoadIcon(consumableSpritePath, consumableDef.baseInfo.iconName);
            SetItemIcon(consumable, icon);

            UnityEditor.AssetDatabase.CreateAsset(consumable, fullPath);
            
            if (showCreationMessages)
                Debug.Log($"创建消耗品: {consumableDef.baseInfo.name}");
            #endif
        }

        private void SetBaseItemData(BaseItemData item, ItemDefinition baseInfo, ItemType type, int maxStack, bool consumable)
        {
            #if UNITY_EDITOR
            var serializedObject = new UnityEditor.SerializedObject(item);
            
            serializedObject.FindProperty("itemName").stringValue = baseInfo.name;
            serializedObject.FindProperty("description").stringValue = baseInfo.description;
            serializedObject.FindProperty("itemType").enumValueIndex = (int)type;
            serializedObject.FindProperty("maxStackSize").intValue = maxStack;
            serializedObject.FindProperty("isConsumable").boolValue = consumable;
            serializedObject.FindProperty("quality").enumValueIndex = (int)baseInfo.quality;
            
            serializedObject.ApplyModifiedProperties();
            #endif
        }

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
                private void SetEquipmentSpecificData(EquipmentData equipment, EquipmentDefinition equipmentDef)
        {
            #if UNITY_EDITOR
            var serializedObject = new UnityEditor.SerializedObject(equipment);
            
            var attributesList = serializedObject.FindProperty("attributeModifiers");
            attributesList.ClearArray();
            
            foreach (var attribute in equipmentDef.attributes)
            {
                attributesList.arraySize++;
                var element = attributesList.GetArrayElementAtIndex(attributesList.arraySize - 1);
                element.FindPropertyRelative("attributeType").enumValueIndex = (int)attribute.attributeType;
                element.FindPropertyRelative("value").floatValue = attribute.value;
                element.FindPropertyRelative("isPercentage").boolValue = attribute.isPercentage;
            }
            
            serializedObject.FindProperty("specialEffect").stringValue = equipmentDef.specialEffect;
            serializedObject.ApplyModifiedProperties();
            #endif
        }

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

        private List<WeaponDefinition> GetWeaponDefinitions()
{
    return new List<WeaponDefinition>
    {
        new WeaponDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "钢铁长剑",
                description = "锋利的钢铁制长剑，适合近战斩击",
                iconName = "steel_sword",
                quality = Quality.Rare
            },
            attackType = AttackType.Slash,
            damage = 25f,
            attackSpeed = 1.2f
        },
        new WeaponDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "精灵短剑",
                description = "轻巧的精灵短剑，擅长快速突刺",
                iconName = "elven_dagger",
                quality = Quality.Epic
            },
            attackType = AttackType.Thrust,
            damage = 18f,
            attackSpeed = 1.8f
        },
        new WeaponDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "魔能手枪",
                description = "注入魔法能量的远程武器",
                iconName = "magic_pistol",
                quality = Quality.Legendary
            },
            attackType = AttackType.Firearm,
            damage = 35f,
            attackSpeed = 0.8f
        },
        new WeaponDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "龙鳞巨剑",
                description = "传说中的神器，蕴含古龙之力",
                iconName = "dragon_sword",
                quality = Quality.Artifact
            },
            attackType = AttackType.Slash,
            damage = 50f,
            attackSpeed = 1.0f
        }
    };
}

private List<EquipmentDefinition> GetEquipmentDefinitions()
{
    return new List<EquipmentDefinition>
    {
        new EquipmentDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "力量护腕",
                description = "增强佩戴者力量的护腕",
                iconName = "power_bracer",
                quality = Quality.Rare
            },
            attributes = new List<AttributeModifierData>
            {
                new AttributeModifierData
                {
                    attributeType = AttributeType.power,
                    value = 10f,
                    isPercentage = false
                }
            },
            specialEffect = "力量增强"
        },
        new EquipmentDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "钢铁胸甲",
                description = "坚固的钢铁胸甲，提供优秀的防护",
                iconName = "steel_chestplate",
                quality = Quality.Epic
            },
            attributes = new List<AttributeModifierData>
            {
                new AttributeModifierData
                {
                    attributeType = AttributeType.armor,
                    value = 25f,
                    isPercentage = false
                }
            },
            specialEffect = "物理伤害减免"
        },
        new EquipmentDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "智慧头环",
                description = "提升智力和魔法能力的头环",
                iconName = "wisdom_circlet",
                quality = Quality.Legendary
            },
            attributes = new List<AttributeModifierData>
            {
                new AttributeModifierData
                {
                    attributeType = AttributeType.intelligence,
                    value = 20f,
                    isPercentage = true
                }
            },
            specialEffect = "魔法伤害增幅"
        },
        new EquipmentDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "疾风靴",
                description = "轻如羽毛的靴子，大幅提升移动速度",
                iconName = "wind_boots",
                quality = Quality.Epic
            },
            attributes = new List<AttributeModifierData>
            {
                new AttributeModifierData
                {
                    attributeType = AttributeType.moveSpeed,
                    value = 15f,
                    isPercentage = true
                },
                new AttributeModifierData
                {
                    attributeType = AttributeType.attackSpeed,
                    value = 8f,
                    isPercentage = true
                }
            },
            specialEffect = "疾风加速"
        },
        new EquipmentDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "幸运项链",
                description = "神秘的项链，能提升暴击几率",
                iconName = "lucky_necklace",
                quality = Quality.Artifact
            },
            attributes = new List<AttributeModifierData>
            {
                new AttributeModifierData
                {
                    attributeType = AttributeType.critical,
                    value = 25f,
                    isPercentage = true
                }
            },
            specialEffect = "幸运暴击"
        }
    };
}

        private List<ConsumableDefinition> GetConsumableDefinitions()
        {
            return new List<ConsumableDefinition>
    {
        new ConsumableDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "力量药剂",
                description = "临时提升力量的药剂",
                iconName = "power_potion",
                quality = Quality.Rare
            },
            targetAttribute = AttributeType.power,
            effectValue = 15f,
            cooldownTime = 10f
        },
        new ConsumableDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "护甲强化剂",
                description = "临时增强护甲防御的药剂",
                iconName = "armor_potion",
                quality = Quality.Epic
            },
            targetAttribute = AttributeType.armor,
            effectValue = 20f,
            cooldownTime = 15f
        },
        new ConsumableDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "智慧之水",
                description = "提升智力和魔法能力的神秘药水",
                iconName = "intelligence_potion",
                quality = Quality.Legendary
            },
            targetAttribute = AttributeType.intelligence,
            effectValue = 25f,
            cooldownTime = 20f
        },
        new ConsumableDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "疾行药水",
                description = "大幅提升移动速度的药水",
                iconName = "speed_potion",
                quality = Quality.Rare
            },
            targetAttribute = AttributeType.moveSpeed,
            effectValue = 30f,
            cooldownTime = 8f
        },
        new ConsumableDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "狂暴药剂",
                description = "提升攻击速度的狂暴药剂",
                iconName = "berserk_potion",
                quality = Quality.Epic
            },
            targetAttribute = AttributeType.attackSpeed,
            effectValue = 40f,
            cooldownTime = 12f
        },
        new ConsumableDefinition
        {
            baseInfo = new ItemDefinition
            {
                name = "幸运符咒",
                description = "临时提升暴击率的神秘符咒",
                iconName = "lucky_charm",
                quality = Quality.Artifact
            },
            targetAttribute = AttributeType.critical,
            effectValue = 20f,
            cooldownTime = 25f
        }
    };
        }
        private void EnsureDirectoryExists(string path)
        {
            #if UNITY_EDITOR
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            #endif
        }

        private Sprite LoadIcon(string path, string iconName)
        {
            #if UNITY_EDITOR
            string fullPath = $"{path}{iconName}.png";
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            #else
            return null;
            #endif
        }
    }
}