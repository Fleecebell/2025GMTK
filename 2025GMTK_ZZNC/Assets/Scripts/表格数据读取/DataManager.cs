using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using InventorySystem.Items;
using InventorySystem.Data;
using System.Linq;

public class DataManager : MonoBehaviour
{
    public TextAsset csvTextAsset;

    [ContextMenu("转换数据")]
    public void CreateData()
    {
        if (csvTextAsset == null)
        {
            Debug.LogError("CSV TextAsset is not assigned!");
            return;
        }

        List<BaseItemData> itemDataList = ReadCsvData();
        SaveItemDataToFiles(itemDataList);
    }

    private List<BaseItemData> ReadCsvData()
    {
        List<BaseItemData> itemDataList = new List<BaseItemData>();

        // 处理换行符，兼容不同格式的换行
        string[] lines = csvTextAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
        {
            Debug.LogError("CSV文件为空");
            return itemDataList;
        }

        Debug.Log($"CSV表头: {lines[0]}");

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] values = SplitCsvLine(line);

            // 确保至少有12列数据
            if (values.Length < 12)
            {
                Debug.LogError($"第{i + 1}行数据列数不足，实际:{values.Length}列，预期:至少12列 | 内容:{line}");
                continue;
            }

            // 解析所有字段（均为字符串）
            string itemName = values[0].Trim();
            string typeStr = values[1].Trim();  // 类型字符串（原本是数字字符串）
            string qualityStr = values[2].Trim();
            string powerStr = values[3].Trim();
            string armorStr = values[4].Trim();
            string intelligenceStr = values[5].Trim();
            string beliefStr = values[6].Trim();
            string attackSpeedStr = values[7].Trim();
            string moveSpeedStr = values[8].Trim();
            string criticalStr = values[9].Trim();
            string effectStr = values[10].Trim();

            string iconStr = values[11].Trim();   // 新增

            string description = values[12].Trim();

            string priceStr = values[13].Trim();   // 新增

            // 解析类型（字符串转整数）
            if (!int.TryParse(typeStr, out int typeInt))
            {
                Debug.LogError($"第{i + 1}行类型解析失败，类型字符串为:'{typeStr}'");
                continue;
            }

            ItemType itemType = GetItemType(typeInt);
            Quality quality = GetQuality(qualityStr, typeInt);

            if (itemType == ItemType.Weapon)
            {
                WeaponData weaponData = new WeaponData();
                weaponData.itemName = itemName;
                weaponData.quality = quality;
                weaponData.iconID = iconStr;
                weaponData.price = priceStr;
                weaponData.description = description;
                weaponData.attackType = GetAttackType(typeInt);
                weaponData.power = ParseStringToFloat(powerStr);
                weaponData.belief = ParseStringToFloat(beliefStr);
                weaponData.attackSpeed = ParseStringToFloat(attackSpeedStr);
                weaponData.critical = ParseCriticalString(criticalStr);
                // 绑定图标
                AssignIcon(weaponData);

                itemDataList.Add(weaponData);
            }
            else if (itemType == ItemType.Equipment)
            {
                EquipmentData equipmentData = new EquipmentData();
                equipmentData.itemName = itemName;
                equipmentData.itemType = itemType;
                equipmentData.quality = quality;
                equipmentData.iconID = iconStr;
                equipmentData.price = priceStr;
                equipmentData.description = description;
                equipmentData.specialEffect = effectStr;

                // 添加属性修饰符
                AddAttributeModifier(equipmentData, AttributeType.power, powerStr);
                AddAttributeModifier(equipmentData, AttributeType.armor, armorStr);
                AddAttributeModifier(equipmentData, AttributeType.intelligence, intelligenceStr);
                AddAttributeModifier(equipmentData, AttributeType.attackSpeed, attackSpeedStr);
                AddAttributeModifier(equipmentData, AttributeType.moveSpeed, moveSpeedStr);
                AddAttributeModifier(equipmentData, AttributeType.critical, criticalStr, true);
                // 绑定图标
                AssignIcon(equipmentData);

                itemDataList.Add(equipmentData);
            }
            else if (itemType == ItemType.Consumable)
            {
                ConsumableData consumableData = new ConsumableData();
                consumableData.itemName = itemName;
                consumableData.itemType = itemType;
                consumableData.quality = quality;
                consumableData.iconID = iconStr;
                consumableData.price = priceStr;
                consumableData.description = description;
                consumableData.effectDescription = effectStr;
                // 绑定图标
                AssignIcon(consumableData);

                itemDataList.Add(consumableData);
            }
        }

        Debug.Log($"数据解析完成，共解析{itemDataList.Count}条有效数据");
        return itemDataList;
    }

    // 解析CSV行（处理字符串类型数据）
    private string[] SplitCsvLine(string line)
    {
        List<string> values = new List<string>();
        bool inQuotes = false;
        System.Text.StringBuilder currentValue = new System.Text.StringBuilder();

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        // 添加最后一个值
        values.Add(currentValue.ToString());

        return values.ToArray();
    }

    // 将字符串转换为float（处理特殊符号和空值）
    private float ParseStringToFloat(string valueStr)
    {
        // 处理空值或特殊符号（如"——"表示无值）
        if (string.IsNullOrEmpty(valueStr) || valueStr == "——" || valueStr == "无")
        {
            return 0f;
        }

        if (float.TryParse(valueStr, out float result))
        {
            return result;
        }

        Debug.LogWarning($"无法转换为float: '{valueStr}'，返回0");
        return 0f;
    }

    // 解析暴击率字符串（处理百分比符号）
    private float ParseCriticalString(string criticalStr)
    {
        if (string.IsNullOrEmpty(criticalStr)) return 0f;

        // 移除百分号并转换
        string cleaned = criticalStr.TrimEnd('%');
        if (float.TryParse(cleaned, out float value))
        {
            return value / 100f; // 转换为小数形式
        }

        Debug.LogWarning($"无法解析暴击率: '{criticalStr}'，返回0");
        return 0f;
    }

    // 根据类型整数获取物品类型
    private ItemType GetItemType(int typeInt)
    {
        switch (typeInt)
        {
            case 1:
            case 2:
            case 3:
                return ItemType.Weapon;
            case 4:
            case 5:
                return ItemType.Equipment;
            case 6:
            case 7:
                return ItemType.Consumable;
            default:
                Debug.LogError($"未知类型整数: {typeInt}");
                return ItemType.Consumable;
        }
    }

    // 获取品质
    private Quality GetQuality(string qualityStr, int typeInt)
    {
        // 装备类型（4,5）固定为Artifact
        if (typeInt == 4 || typeInt == 5)
        {
            return Quality.Artifact;
        }

        // 武器类型根据品质字符串判断
        switch (qualityStr)
        {
            case "蓝":
                return Quality.Rare;
            case "紫":
                return Quality.Epic;
            case "金":
                return Quality.Legendary;
            default:
                Debug.LogWarning($"未知品质字符串: '{qualityStr}'，默认返回Rare");
                return Quality.Rare;
        }
    }

    // 获取武器攻击类型
    private AttackType GetAttackType(int typeInt)
    {
        switch (typeInt)
        {
            case 1: // 武器-长枪
                return AttackType.Thrust;
            case 2: // 武器-剑
                return AttackType.Slash;
            case 3: // 武器-弓
                return AttackType.Firearm;
            default:
                Debug.LogWarning($"未知武器类型整数: {typeInt}，默认返回Slash");
                return AttackType.Slash;
        }
    }

    // 为装备添加属性修饰符
    private void AddAttributeModifier(EquipmentData equipmentData, AttributeType type, string valueStr, bool isCritical = false)
    {
        float value = isCritical ? ParseCriticalString(valueStr) : ParseStringToFloat(valueStr);

        // 即使值为0也添加（如果明确设置了0）
        if (value != 0 || (valueStr == "0" || valueStr == "0%"))
        {
            AttributeModifier modifier = new AttributeModifier();
            modifier.attributeType = type;
            modifier.value = value;
            equipmentData.attributeModifiers.Add(modifier);
        }
    }

    // 保存数据到文件
    private void SaveItemDataToFiles(List<BaseItemData> itemDataList)
    {
        foreach (BaseItemData itemData in itemDataList)
        {
            string folderPath = GetFolderPath(itemData.itemType);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 处理文件名中的特殊字符
            string safeFileName = Regex.Replace(itemData.itemName, @"[\/:*?""<>|]", "_");
            string filePath = Path.Combine(folderPath, safeFileName + ".asset");

            // 创建Asset文件
            if (itemData is WeaponData weaponData)
            {
                UnityEditor.AssetDatabase.CreateAsset(weaponData, filePath);
            }
            else if (itemData is EquipmentData equipmentData)
            {
                UnityEditor.AssetDatabase.CreateAsset(equipmentData, filePath);
            }
            else if (itemData is ConsumableData consumableData)
            {
                UnityEditor.AssetDatabase.CreateAsset(consumableData, filePath);
            }
        }

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log($"数据保存完成，共保存{itemDataList.Count}个资产文件");
    }

    // 获取保存路径
    private string GetFolderPath(ItemType itemType)
    {
        string basePath = "Assets/Resources/Data/Items";
        switch (itemType)
        {
            case ItemType.Weapon:
                return Path.Combine(basePath, "武器");
            case ItemType.Equipment:
                return Path.Combine(basePath, "神器");
            case ItemType.Consumable:
                return Path.Combine(basePath, "消耗品");
            default:
                return basePath;
        }
    }
    private void AssignIcon(BaseItemData itemData)
    {
        var sprite = Resources.Load<Sprite>($"Sprites/{itemData.iconID}");
        if (sprite == null)
        {
            Debug.LogWarning($"未找到对应 Sprite：{itemData.iconID}");
            return;
        }
        itemData.icon = sprite;
    }
}