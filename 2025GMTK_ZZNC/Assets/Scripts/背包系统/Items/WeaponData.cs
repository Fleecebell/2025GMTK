using UnityEngine;
using InventorySystem.Data;
using InventorySystem.Managers;

namespace InventorySystem.Items
{
    /// <summary>
    /// 武器数据类 - 继承自BaseItemData
    /// 遵循OOP继承原则，专门处理武器相关数据
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Inventory/Weapon Data")]
    public class WeaponData : BaseItemData
    {
        [Header("武器属性")]
        [SerializeField] public AttackType attackType;
        [SerializeField] public float power;//力量
        [SerializeField] public float belief;//信仰
        [SerializeField] public float attackSpeed;
        [SerializeField] public float critical;//暴击率

        [Header("视觉效果")]
        public GameObject attackInstance;
        [SerializeField] private GameObject attackEffect;
        [SerializeField] private AudioClip attackSound;

        // 只读属性访问器
        public AttackType AttackType => attackType;
        public float Damage => power;
        public float AttackSpeed => attackSpeed;
        public GameObject AttackEffect => attackEffect;
        public AudioClip AttackSound => attackSound;

        /// <summary>
        /// 在编辑器中验证数据时调用
        /// </summary>
        private void OnValidate()
        {
            itemType = ItemType.Weapon;
            maxStackSize = 1; // 武器不可堆叠
        }

        /// <summary>
        /// 重写获取详细信息方法
        /// </summary>
        /// <returns>包含武器属性的详细信息</returns>
        public override string GetDetailedInfo()
        {
            string baseInfo = base.GetDetailedInfo();
            string weaponInfo = $"\n<color=yellow>攻击方式:</color> {GetAttackTypeDisplayName()}" +
                               $"\n<color=red>伤害:</color> {power}" +
                               $"\n<color=green>攻击速度:</color> {attackSpeed}";
            
            return baseInfo + weaponInfo;
        }

        /// <summary>
        /// 武器使用方法 - 装备武器
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否装备成功</returns>
        public override bool Use(GameObject user)
        {
            // 获取玩家的装备管理器
            var equipmentManager = user.GetComponent<EquipmentManager>();
            if (equipmentManager != null)
            {
                return equipmentManager.EquipWeapon(this);
            }
            
            Debug.LogWarning($"无法在 {user.name} 上找到装备管理器");
            return false;
        }

        /// <summary>
        /// 验证武器数据有效性
        /// </summary>
        /// <returns>是否有效</returns>
        public override bool IsValid()
        {
            return base.IsValid() && 
                   power > 0 && 
                   attackSpeed > 0;
        }

        /// <summary>
        /// 获取攻击方式的显示名称
        /// </summary>
        /// <returns>本地化的攻击方式名称</returns>
        private string GetAttackTypeDisplayName()
        {
            return attackType switch
            {
                AttackType.Slash => "挥砍",
                AttackType.Thrust => "刺击",
                AttackType.Firearm => "枪械",
                _ => attackType.ToString()
            };
        }

        /// <summary>
        /// 计算武器的DPS（每秒伤害）
        /// </summary>
        /// <returns>DPS值</returns>
        public float GetDPS()
        {
            return power * attackSpeed;
        }
    }
}