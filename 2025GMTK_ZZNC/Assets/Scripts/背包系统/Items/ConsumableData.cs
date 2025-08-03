using UnityEngine;
using InventorySystem.Data;
using InventorySystem.Managers;

namespace InventorySystem.Items
{
    [CreateAssetMenu(fileName = "ConsumableData", menuName = "Inventory/Consumable Data")]
    public class ConsumableData : BaseItemData
    {
        [Header("道具效果")]
        [SerializeField] public string effectDescription;
        [SerializeField] public float effectValue;
        [SerializeField] public AttributeType targetAttribute;
        
        [Header("使用设置")]
        [SerializeField] public bool isInstantUse = true;
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

        private void OnValidate()
        {
            itemType = ItemType.Consumable;
            isConsumable = true;
            maxStackSize = 99;
        }

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
                string valueStr;
                
                // 对于百分比属性（如暴击率），显示百分比
                if (targetAttribute == AttributeType.critical || 
                    targetAttribute == AttributeType.attackSpeed || 
                    targetAttribute == AttributeType.moveSpeed)
                {
                    valueStr = effectValue > 0 ? $"+{(effectValue * 100):F1}%" : $"{(effectValue * 100):F1}%";
                }
                else
                {
                    valueStr = effectValue > 0 ? $"+{effectValue:F1}" : $"{effectValue:F1}";
                }
                
                consumableInfo += $"\n<color=yellow>{attributeName}:</color> {valueStr}";
            }

            if (cooldownTime > 0)
            {
                consumableInfo += $"\n<color=orange>冷却时间:</color> {cooldownTime}秒";
            }

            return baseInfo + consumableInfo;
        }

        public override bool Use(GameObject user)
        {
            var consumableManager = user.GetComponent<ConsumableManager>();
            if (consumableManager != null && !consumableManager.CanUseItem(this))
            {
                Debug.Log($"道具 {itemName} 还在冷却中");
                return false;
            }

            bool success = ApplyEffect(user);
            
            if (success)
            {
                PlayUseEffects(user);
                
                if (consumableManager != null && cooldownTime > 0)
                {
                    consumableManager.SetItemCooldown(this, cooldownTime);
                }
                
                Debug.Log($"使用道具: {itemName}");
            }

            return success;
        }

        private bool ApplyEffect(GameObject user)
        {
            switch (targetAttribute)
            {
                case AttributeType.power:
                    return ApplyPowerEffect(user);
                case AttributeType.armor:
                    return ApplyArmorEffect(user);
                case AttributeType.intelligence:
                    return ApplyIntelligenceEffect(user);
                case AttributeType.attackSpeed:
                    return ApplyAttackSpeedEffect(user);
                case AttributeType.moveSpeed:
                    return ApplyMoveSpeedEffect(user);
                case AttributeType.critical:
                    return ApplyCriticalEffect(user);
                default:
                    Debug.LogWarning($"未实现的属性类型: {targetAttribute}");
                    return false;
            }
        }

        private bool ApplyPowerEffect(GameObject user)
        {
            Debug.Log($"增加力量: {effectValue}");
            return true;
        }

        private bool ApplyArmorEffect(GameObject user)
        {
            Debug.Log($"增加护甲: {effectValue}");
            return true;
        }

        private bool ApplyIntelligenceEffect(GameObject user)
        {
            Debug.Log($"增加智力: {effectValue}");
            return true;
        }

        private bool ApplyAttackSpeedEffect(GameObject user)
        {
            Debug.Log($"增加攻击速度: {effectValue * 100}%");
            return true;
        }

        private bool ApplyMoveSpeedEffect(GameObject user)
        {
            Debug.Log($"增加移动速度: {effectValue * 100}%");
            return true;
        }

        private bool ApplyCriticalEffect(GameObject user)
        {
            Debug.Log($"增加暴击率: {effectValue * 100}%");
            return true;
        }

        private void PlayUseEffects(GameObject user)
        {
            if (useEffect != null)
            {
                Instantiate(useEffect, user.transform.position, Quaternion.identity);
            }

            if (useSound != null)
            {
                AudioSource.PlayClipAtPoint(useSound, user.transform.position);
            }
        }

        private string GetAttributeDisplayName(AttributeType attributeType)
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

        public override bool IsValid()
        {
            return base.IsValid() && !string.IsNullOrEmpty(effectDescription);
        }
    }
}