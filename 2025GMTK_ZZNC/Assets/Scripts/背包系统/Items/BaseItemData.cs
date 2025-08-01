using UnityEngine;
using InventorySystem.Data;

namespace InventorySystem.Items
{
    /// <summary>
    /// 物品基础数据类 - 遵循OOP原则的抽象基类
    /// 实现了数据与方法分离，提供良好的可读性
    /// </summary>
    [CreateAssetMenu(fileName = "BaseItem", menuName = "Inventory/Base Item")]
    public abstract class BaseItemData : ScriptableObject
    {
        [Header("基础信息")]
        [SerializeField] protected string itemName;
        [SerializeField] protected string description;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected ItemType itemType;
        [SerializeField] protected int maxStackSize = 1;
        [SerializeField] protected bool isConsumable = false;

        // 只读属性，保证数据封装性
        public string ItemName => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public ItemType ItemType => itemType;
        public int MaxStackSize => maxStackSize;
        public bool IsConsumable => isConsumable;

        /// <summary>
        /// 获取物品的详细信息 - 虚方法，子类可重写
        /// </summary>
        /// <returns>格式化的物品信息</returns>
        public virtual string GetDetailedInfo()
        {
            return $"<b>{itemName}</b>\n{description}";
        }

        /// <summary>
        /// 验证物品数据的有效性
        /// </summary>
        /// <returns>是否有效</returns>
        public virtual bool IsValid()
        {
            return !string.IsNullOrEmpty(itemName) && 
                   !string.IsNullOrEmpty(description) && 
                   icon != null;
        }

        /// <summary>
        /// 物品使用方法 - 抽象方法，强制子类实现
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否使用成功</returns>
        public abstract bool Use(GameObject user);

        /// <summary>
        /// 获取物品类型的本地化名称
        /// </summary>
        /// <returns>类型名称</returns>
        public string GetTypeDisplayName()
        {
            return itemType switch
            {
                ItemType.Weapon => "武器",
                ItemType.Equipment => "装备",
                ItemType.Consumable => "道具",
                _ => itemType.ToString()
            };
        }
    }
}