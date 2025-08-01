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
        [SerializeField] private AttributeType attributeType;
        [SerializeField] private float value;
        [SerializeField] private bool isPercentage;

        // 属性访问器
        public AttributeType AttributeType => attributeType;
        public float Value => value;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AttributeModifier()
        {
            attributeType = AttributeType.Attack;
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
            isPercentage= isPercent;
        }

        /// <summary>
        /// 获取属性修改器的描述
        /// </summary>
        public string GetDescription()
        {
            string attributeName = GetAttributeDisplayName();
            string valueStr = value > 0 ? $"+{value:F1}" : value.ToString("F1");
            
            // 对于百分比属性，显示百分比
            if (attributeType == AttributeType.CriticalRate || attributeType == AttributeType.CriticalDamage)
            {
                valueStr = value > 0 ? $"+{(value * 100):F1}%" : $"{(value * 100):F1}%";
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
                AttributeType.Health => "生命值",
                AttributeType.San => "San值",
                AttributeType.Attack => "攻击力",
                AttributeType.Defense => "防御力",
                AttributeType.MoveSpeed => "移动速度",
                AttributeType.CriticalRate => "暴击率",
                AttributeType.CriticalDamage => "暴击伤害",
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