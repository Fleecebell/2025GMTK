using UnityEngine;

using InventorySystem.Data;
using InventorySystem.Managers;

namespace InventorySystem.Items
{
    /// <summary>
    /// 道具数据类 - 继承自BaseItemData
    /// 处理可消耗道具的效果和使用逻辑
    /// </summary>
    [CreateAssetMenu(fileName = "ConsumableData", menuName = "Inventory/Consumable Data")]
    public class ConsumableData : BaseItemData
    {
        [Header("道具效果")]
        [SerializeField] private string effectDescription;
        [SerializeField] private float effectValue;
        [SerializeField] private AttributeType targetAttribute;
        
        [Header("使用设置")]
        [SerializeField] private bool isInstantUse = true;
        [SerializeField] private float cooldownTime = 0f;
        
        [Header("视觉效果")]
        [SerializeField] private GameObject useEffect;
        [SerializeField] private AudioClip useSound;

        // 只读属性访问器
        public string EffectDescription => effectDescription;
        public float EffectValue => effectValue;
        public AttributeType TargetAttribute => targetAttribute;
        public bool IsInstantUse => isInstantUse;
        public float CooldownTime => cooldownTime;
        public GameObject UseEffect => useEffect;
        public AudioClip UseSound => useSound;

        /// <summary>
        /// 在编辑器中验证数据时调用
        /// </summary>
        private void OnValidate()
        {
            itemType = ItemType.Consumable;
            isConsumable = true;
            maxStackSize = 99; // 道具可堆叠
        }

        /// <summary>
        /// 重写获取详细信息方法
        /// </summary>
        /// <returns>包含道具效果的详细信息</returns>
        public override string GetDetailedInfo()
        {
            string baseInfo = base.GetDetailedInfo();
            string consumableInfo = "";

            if (!string.IsNullOrEmpty(effectDescription))
            {
                consumableInfo += $"\n<color=green>效果:</color> {effectDescription}";
            }

            if (effectValue != 0)
            {
                string attributeName = GetAttributeDisplayName(targetAttribute);
                string sign = effectValue > 0 ? "+" : "";
                consumableInfo += $"\n<color=yellow>{attributeName}:</color> {sign}{effectValue}";
            }

            if (cooldownTime > 0)
            {
                consumableInfo += $"\n<color=orange>冷却时间:</color> {cooldownTime}秒";
            }

            return baseInfo + consumableInfo;
        }

        /// <summary>
        /// 道具使用方法 - 应用道具效果
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否使用成功</returns>
        public override bool Use(GameObject user)
        {
            // 检查冷却时间
            var consumableManager = user.GetComponent<ConsumableManager>();
            if (consumableManager != null && !consumableManager.CanUseItem(this))
            {
                Debug.Log($"道具 {itemName} 还在冷却中");
                return false;
            }

            // 应用道具效果
            bool success = ApplyEffect(user);
            
            if (success)
            {
                // 播放使用效果
                PlayUseEffects(user);
                
                // 设置冷却时间
                if (consumableManager != null && cooldownTime > 0)
                {
                    consumableManager.SetItemCooldown(this, cooldownTime);
                }
                
                Debug.Log($"使用道具: {itemName}");
            }

            return success;
        }

        /// <summary>
        /// 应用道具效果的具体实现
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否应用成功</returns>
        private bool ApplyEffect(GameObject user)
        {
            // 根据目标属性类型应用不同效果
            switch (targetAttribute)
            {
                case AttributeType.Health:
                    return ApplyHealthEffect(user);
                case AttributeType.Attack:
                    return ApplyAttackEffect(user);
                case AttributeType.Defense:
                    return ApplyDefenseEffect(user);
                case AttributeType.MoveSpeed:
                    return ApplySpeedEffect(user);
                default:
                    Debug.LogWarning($"未实现的属性类型: {targetAttribute}");
                    return false;
            }
        }

        /// <summary>
        /// 应用生命值效果
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否应用成功</returns>
        private bool ApplyHealthEffect(GameObject user)
        {
            // 这里应该调用玩家的生命值系统
            // 暂时用Debug.Log模拟
            Debug.Log($"恢复生命值: {effectValue}");
            return true;
        }

        /// <summary>
        /// 应用攻击力效果
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否应用成功</returns>
        private bool ApplyAttackEffect(GameObject user)
        {
            Debug.Log($"增加攻击力: {effectValue}");
            return true;
        }

        /// <summary>
        /// 应用防御力效果
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否应用成功</returns>
        private bool ApplyDefenseEffect(GameObject user)
        {
            Debug.Log($"增加防御力: {effectValue}");
            return true;
        }

        /// <summary>
        /// 应用速度效果
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否应用成功</returns>
        private bool ApplySpeedEffect(GameObject user)
        {
            Debug.Log($"增加速度: {effectValue}");
            return true;
        }

        /// <summary>
        /// 播放使用效果
        /// </summary>
        /// <param name="user">使用者</param>
        private void PlayUseEffects(GameObject user)
        {
            // 播放视觉效果
            if (useEffect != null)
            {
                Instantiate(useEffect, user.transform.position, Quaternion.identity);
            }

            // 播放音效
            if (useSound != null)
            {
                AudioSource.PlayClipAtPoint(useSound, user.transform.position);
            }
        }

        /// <summary>
        /// 获取属性类型的显示名称
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>本地化的属性名称</returns>
        private string GetAttributeDisplayName(AttributeType attributeType)
        {
            return attributeType switch
            {
                AttributeType.Health => "生命值",
                AttributeType.Attack => "攻击力",
                AttributeType.Defense => "防御力",
                AttributeType.MoveSpeed => "速度",
                AttributeType.CriticalRate => "暴击率",
                AttributeType.CriticalDamage => "暴击伤害",
                _ => attributeType.ToString()
            };
        }

        /// <summary>
        /// 验证道具数据有效性
        /// </summary>
        /// <returns>是否有效</returns>
        public override bool IsValid()
        {
            return base.IsValid() && 
                   !string.IsNullOrEmpty(effectDescription);
        }
    }
}