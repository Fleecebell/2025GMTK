using UnityEngine;

namespace InventorySystem.Data
{
    /// <summary>
    /// 属性修改器
    /// 用于定义装备对角色属性的影响
    /// </summary>
    [System.Serializable]
    public class AttributeModifier
    {
        [Header("属性修改")]
        [SerializeField] public AttributeType attributeType;
        [SerializeField] public float value;
        [SerializeField] public bool isPercentage;

        // 属性访问器
        public AttributeType AttributeType => attributeType;
        public float Value => value;
        public bool IsPercentage => isPercentage;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AttributeModifier()
        {
            attributeType = AttributeType.power;
            value = 0f;
            isPercentage = false;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public AttributeModifier(AttributeType type, float val, bool isPercent = false)
        {
            attributeType = type;
            value = val;
            isPercentage = isPercent;
        }

        /// <summary>
        /// 获取属性修改器的描述
        /// </summary>
        public string GetDescription()
        {
            string attributeName = GetAttributeDisplayName();
            string valueStr;

            if (isPercentage || attributeType == AttributeType.critical)
            {
                // 百分比显示
                valueStr = value > 0 ? $"+{(value * 100):F1}%" : $"{(value * 100):F1}%";
            }
            else
            {
                // 普通数值显示
                valueStr = value > 0 ? $"+{value:F1}" : value.ToString("F1");
            }

            return $"{attributeName}: {valueStr}";
        }

        /// <summary>
        /// 获取属性类型的显示名称
        /// </summary>
        private string GetAttributeDisplayName()
        {
            return attributeType switch
            {
                AttributeType.power => "力量",
                AttributeType.armor => "护甲",
                AttributeType.intelligence => "智力",
                AttributeType.attackSpeed => "攻击速度",
                AttributeType.moveSpeed => "移动速度",
                AttributeType.critical => "暴击率",
                _ => attributeType.ToString()
            };
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        public override string ToString()
        {
            return GetDescription();
        }

        /// <summary>
        /// 检查修改器是否有效
        /// </summary>
        public bool IsValid()
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}