using UnityEngine;
using InventorySystem.Core;
using InventorySystem.Managers;
using InventorySystem.Items;
using InventorySystem.Data;

namespace InventorySystem.Managers
{
    /// <summary>
    /// 背包管理器 - 背包系统的核心管理器
    /// 遵循单一职责原则，协调背包系统的管理
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        [Header("背包设置")]
        [SerializeField] private int maxInventorySlots = 30;
        
        [Header("管理器")]
        [SerializeField] private EquipmentManager equipmentManager;
        [SerializeField] private ConsumableManager consumableManager;

        // 背包核心系统
        private Inventory inventory;

        // 单例模式
        public static InventoryManager Instance { get; private set; }

        // 只读属性
        public Inventory Inventory => inventory;
        public EquipmentManager EquipmentManager => equipmentManager;
        public ConsumableManager ConsumableManager => consumableManager;

        // 事件系统
        public System.Action<BaseItemData, int> OnItemAdded;
        public System.Action<BaseItemData, int> OnItemRemoved;
        public System.Action OnInventoryChanged;

        /// <summary>
        /// 初始化背包管理器
        /// </summary>
        private void Awake()
        {
            // 单例模式检查
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInventorySystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初始化背包系统
        /// </summary>
        private void InitializeInventorySystem()
        {
            // 创建背包实例
            inventory = new Inventory();

            // 获取或创建管理器组件
            if (equipmentManager == null)
                equipmentManager = GetComponent<EquipmentManager>() ?? gameObject.AddComponent<EquipmentManager>();
            
            if (consumableManager == null)
                consumableManager = GetComponent<ConsumableManager>() ?? gameObject.AddComponent<ConsumableManager>();

            // 绑定事件
            BindInventoryEvents();

            Debug.Log("背包系统初始化完成");
        }

        /// <summary>
        /// 绑定背包事件
        /// </summary>
        private void BindInventoryEvents()
        {
            inventory.OnItemAdded += HandleItemAdded;
            inventory.OnItemRemoved += HandleItemRemoved;
            inventory.OnInventoryChanged += HandleInventoryChanged;
        }

        /// <summary>
        /// 向背包添加物品
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        /// <returns>实际添加的数量</returns>
        public int AddItem(BaseItemData item, int quantity = 1)
        {
            if (item == null)
            {
                Debug.LogWarning("无法添加空物品数据");
                return 0;
            }

            // 如果是装备类型，直接装备而不添加到背包
            if (item.ItemType == ItemType.Equipment)
            {
                var equipmentData = item as EquipmentData;
                inventory.AddItem(item, quantity);
                if (equipmentData != null)
                {
                    bool equipped = equipmentManager.EquipItem(equipmentData);
                    if (equipped)
                    {
                        Debug.Log($"自动装备: {item.ItemName}");
                        return quantity; // 返回添加的数量
                    }
                }
            }

            int addedQuantity = inventory.AddItem(item, quantity);
            
            if (addedQuantity > 0)
            {
                Debug.Log($"添加物品: {item.ItemName} x{addedQuantity}");
            }

            return addedQuantity;
        }

        /// <summary>
        /// 从背包移除物品
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        /// <returns>实际移除的数量</returns>
        public int RemoveItem(BaseItemData item, int quantity = 1)
        {
            if (item == null)
            {
                Debug.LogWarning("无法移除空物品数据");
                return 0;
            }

            int removedQuantity = inventory.RemoveItem(item, quantity);
            
            if (removedQuantity > 0)
            {
                Debug.Log($"移除物品: {item.ItemName} x{removedQuantity}");
            }

            return removedQuantity;
        }
        
        /// <summary>
        /// 使用武器
        /// </summary>
        /// <param name="weaponData">武器数据</param>
        /// <returns>是否使用成功</returns>
        public bool UseWeapon(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogWarning("武器数据为空");
                return false;
            }

            // 卸下当前武器
            var currentWeapon = equipmentManager.CurrentWeapon;
            if (currentWeapon != null)
            {
                equipmentManager.UnequipWeapon();
            }

            // 装备新武器
            bool success = equipmentManager.EquipWeapon(weaponData);

            if (success)
            {
                Debug.Log($"装备武器: {weaponData.ItemName}");
                
            }
            else
            {
                Debug.LogWarning($"装备武器失败: {weaponData.ItemName}");
            }

            return success;
        }

        /// <summary>
        /// 使用装备
        /// </summary>
        /// <param name="equipmentData">装备数据</param>
        /// <returns>是否使用成功</returns>
        public bool UseEquipment(EquipmentData equipmentData)
        {
            if (equipmentData == null)
            {
                Debug.LogWarning("装备数据为空");
                return false;
            }

            // 装备系统：装备在获取时自动装备，无法卸下
            // 检查是否已装备该装备
            var equippedItems = equipmentManager.GetAllEquippedItems();
            if (equippedItems.Contains(equipmentData))
            {
                // 装备已装备，无法卸下
                Debug.Log($"装备 {equipmentData.ItemName} 已经装备，无法卸下");
                return true;
            }

            // 装备新装备
            bool success = equipmentManager.EquipItem(equipmentData);
            
            if (success)
            {
                Debug.Log($"装备物品: {equipmentData.ItemName}");
            }
            else
            {
                Debug.LogWarning($"装备物品失败: {equipmentData.ItemName}");
            }

            return success;
        }

        /// <summary>
        /// 使用消耗品
        /// </summary>
        /// <param name="slot">格子</param>
        /// <returns>是否使用成功</returns>
        public bool UseConsumable(InventorySlot slot)
        {
            if (slot == null || slot.IsEmpty)
            {
                Debug.LogWarning("消耗品格子为空");
                return false;
            }

            var consumableData = slot.ItemData as ConsumableData;
            if (consumableData == null)
            {
                Debug.LogWarning("物品不是消耗品类型");
                return false;
            }

            // 使用消耗品
            bool success = consumableManager.UseConsumable(consumableData);
            
            if (success)
            {
                // 移除一个消耗品
                slot.RemoveItem(1);
                Debug.Log($"使用消耗品: {consumableData.ItemName}");
            }
            else
            {
                Debug.LogWarning($"使用消耗品失败: {consumableData.ItemName}");
            }

            return success;
        }

        /// <summary>
        /// 排序背包
        /// </summary>
        public void SortInventory()
        {
            if (inventory != null)
            {
                inventory.SortInventory();
                Debug.Log("背包排序完成");
            }
        }

        /// <summary>
        /// 检查是否有指定数量的物品
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否有足够数量</returns>
        public bool HasItem(BaseItemData item, int quantity = 1)
        {
            return inventory?.HasItem(item, quantity) ?? false;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <returns>物品数量</returns>
        public int GetItemCount(BaseItemData item)
        {
            return inventory?.GetItemCount(item) ?? 0;
        }

        /// <summary>
        /// 根据类型获取物品格子
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>物品格子列表</returns>
        public System.Collections.Generic.List<InventorySlot> GetItemsByType(ItemType itemType)
        {
            return inventory?.GetItemsByType(itemType) ?? new System.Collections.Generic.List<InventorySlot>();
        }

        /// <summary>
        /// 获取指定类型的所有格子（包括空格子）
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>所有格子列表</returns>
        public System.Collections.Generic.List<InventorySlot> GetAllSlotsByType(ItemType itemType)
        {
            return inventory?.GetAllSlotsByType(itemType) ?? new System.Collections.Generic.List<InventorySlot>();
        }

        /// <summary>
        /// 获取指定类型的背包统计信息
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>统计信息字符串</returns>
        public string GetSubInventoryStats(ItemType itemType)
        {
            return inventory?.GetSubInventoryStats(itemType) ?? "背包未初始化";
        }

        /// <summary>
        /// 处理物品添加事件
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        private void HandleItemAdded(BaseItemData item, int quantity)
        {
            OnItemAdded?.Invoke(item, quantity);
            Debug.Log($"物品添加事件: {item.ItemName} x{quantity}");
        }

        /// <summary>
        /// 处理物品移除事件
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        private void HandleItemRemoved(BaseItemData item, int quantity)
        {
            OnItemRemoved?.Invoke(item, quantity);
            Debug.Log($"物品移除事件: {item.ItemName} x{quantity}");
        }

        /// <summary>
        /// 处理背包变化事件
        /// </summary>
        private void HandleInventoryChanged()
        {
            OnInventoryChanged?.Invoke();
            Debug.Log("背包变化事件");
        }

        /// <summary>
        /// 获取背包统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetInventoryStats()
        {
            return inventory?.GetInventoryStats() ?? "背包未初始化";
        }

        /// <summary>
        /// 获取装备统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetEquipmentStats()
        {
            return equipmentManager?.GetEquipmentStats() ?? "装备管理器未初始化";
        }

        /// <summary>
        /// 清空背包
        /// </summary>
        [ContextMenu("清空背包")]
        public void ClearInventory()
        {
            if (inventory != null)
            {
                inventory.ClearInventory();
                Debug.Log("背包已清空");
            }
        }

        /// <summary>
        /// 添加测试物品
        /// </summary>
        [ContextMenu("添加测试物品")]
        public void AddTestItems()
        {
            // 这里可以添加测试物品的逻辑
            Debug.Log("添加测试物品功能");
        }
    }
}