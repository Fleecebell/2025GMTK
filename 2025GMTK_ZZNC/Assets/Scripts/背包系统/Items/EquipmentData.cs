using System.Collections.Generic;
using UnityEngine;

using InventorySystem.Data;
using InventorySystem.Managers;
using InventorySystem.Character;

namespace InventorySystem.Items
{
    /// <summary>
    /// 装备数据类 - 继承自BaseItemData
    /// 包含装备属性加成和特殊效果
    /// </summary>
    [CreateAssetMenu(fileName = "EquipmentData", menuName = "Inventory/Equipment Data")]
    public class EquipmentData : BaseItemData
    {
        [Header("装备属性")]
        [SerializeField] public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
        
        [Header("特殊效果")]
        [SerializeField] public string specialEffect;
        [SerializeField] public GameObject equipEffect;

        // 只读属性访问器
        public List<AttributeModifier> AttributeModifiers => new List<AttributeModifier>(attributeModifiers);
        public string SpecialEffect => specialEffect;
        public GameObject EquipEffect => equipEffect;

        /// <summary>
        /// 在编辑器中验证数据时调用
        /// </summary>
        private void OnValidate()
        {
            itemType = ItemType.Equipment;
            maxStackSize = 1; // 装备不可堆叠
        }

        /// <summary>
        /// 重写获取详细信息方法
        /// </summary>
        /// <returns>包含装备属性的详细信息</returns>
        public override string GetDetailedInfo()
        {
            string baseInfo = base.GetDetailedInfo();

            // 添加属性加成信息
            if (attributeModifiers.Count > 0)
            {
                baseInfo += "\n<color=yellow>属性加成:</color>";
                foreach (var modifier in attributeModifiers)
                {
                    baseInfo += $"\n  {modifier.GetDescription()}";
                }
            }

            // 添加特殊效果信息
            if (!string.IsNullOrEmpty(specialEffect))
            {
                baseInfo += $"\n<color=purple>特殊效果:</color> {specialEffect}";
            }

            return baseInfo;
        }

        /// <summary>
        /// 装备使用方法 - 装备到装备栏
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否装备成功</returns>
        public override bool Use(GameObject user)
        {
            var equipmentManager = user.GetComponent<EquipmentManager>();
            if (equipmentManager != null)
            {
                return equipmentManager.EquipItem(this);
            }
            
            Debug.LogWarning($"无法在 {user.name} 找到装备管理器");
            return false;
        }

        /// <summary>
        /// 验证装备数据有效性
        /// </summary>
        /// <returns>是否有效</returns>
        public override bool IsValid()
        {
            return base.IsValid() && attributeModifiers != null;
        }

        /// <summary>
        /// 获取指定属性类型的总加成值
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>加成值</returns>
        public float GetAttributeBonus(AttributeType attributeType)
        {
            float totalBonus = 0f;
            foreach (var modifier in attributeModifiers)
            {
                if (modifier.AttributeType == attributeType)
                {
                    totalBonus += modifier.Value;
                }
            }
            return totalBonus;
        }

        /// <summary>
        /// 检查是否有指定属性的加成
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>是否有该属性加成</returns>
        public bool HasAttributeBonus(AttributeType attributeType)
        {
            return attributeModifiers.Exists(modifier => modifier.AttributeType == attributeType);
        }

        /// <summary>
        /// 将装备属性修改器转换为装备属性定义
        /// </summary>
        /// <returns>装备属性定义</returns>
        public EquipmentAttributes GetEquipmentAttributes()
        {
            var equipmentAttributes = new EquipmentAttributes();

            foreach (var modifier in attributeModifiers)
            {
                switch (modifier.AttributeType)
                {
                    case AttributeType.power:
                        equipmentAttributes.PowerBonus = modifier.Value;
                        break;
                    case AttributeType.armor:
                        equipmentAttributes.ArmorBonus = modifier.Value;
                        break;
                    case AttributeType.intelligence:
                        equipmentAttributes.IntelligenceBonus = modifier.Value;
                        break;
                    case AttributeType.attackSpeed:
                        equipmentAttributes.AttackSpeedBonus = modifier.Value;
                        break;
                    case AttributeType.moveSpeed:
                        equipmentAttributes.MoveSpeedBonus = modifier.Value;
                        break;
                    case AttributeType.critical:
                        equipmentAttributes.CriticalRateBonus = modifier.Value;
                        break;
                }
            }

            return equipmentAttributes;
        }
    }
}