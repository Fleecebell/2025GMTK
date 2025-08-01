using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InventorySystem.Items;
using InventorySystem.Data;

namespace InventorySystem.Core
{
    /// <summary>
    /// 背包核心类 - 管理三个独立的子背包系统
    /// 每个子背包都有无限容量和动态扩充功能
    /// </summary>
    [System.Serializable]
    public class Inventory
    {
        [Header("子背包设置")]
        [SerializeField] private List<InventorySlot> weaponSlots = new List<InventorySlot>();
        [SerializeField] private List<InventorySlot> equipmentSlots = new List<InventorySlot>();
        [SerializeField] private List<InventorySlot> consumableSlots = new List<InventorySlot>();

        // 事件系统
        public event Action<InventorySlot> OnSlotChanged;
        public event Action<BaseItemData, int> OnItemAdded;
        public event Action<BaseItemData, int> OnItemRemoved;
        public event Action OnInventoryChanged;

        // 只读属性
        public int TotalUsedSlots => weaponSlots.Count(slot => !slot.IsEmpty) + 
                                    equipmentSlots.Count(slot => !slot.IsEmpty) + 
                                    consumableSlots.Count(slot => !slot.IsEmpty);
        
        public int TotalSlots => weaponSlots.Count + equipmentSlots.Count + consumableSlots.Count;
        
        public bool IsFull => false; // 无限容量，永远不会满
        
        // 各子背包的格子列表
        public List<InventorySlot> WeaponSlots => new List<InventorySlot>(weaponSlots);
        public List<InventorySlot> EquipmentSlots => new List<InventorySlot>(equipmentSlots);
        public List<InventorySlot> ConsumableSlots => new List<InventorySlot>(consumableSlots);

        /// <summary>
        /// 构造函数
        /// </summary>
        public Inventory()
        {
            InitializeSubInventories();
        }

        /// <summary>
        /// 初始化子背包
        /// </summary>
        private void InitializeSubInventories()
        {
            weaponSlots = new List<InventorySlot>();
            equipmentSlots = new List<InventorySlot>();
            consumableSlots = new List<InventorySlot>();
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        /// <returns>实际添加的数量</returns>
        public int AddItem(BaseItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
                return 0;

            // 根据物品类型选择对应的子背包
            var targetSlots = GetSlotsByItemType(item.ItemType);
            if (targetSlots == null)
            {
                Debug.LogError($"未知物品类型: {item.ItemType}");
                return 0;
            }

            int remainingQuantity = quantity;
            int totalAdded = 0;

            // 首先尝试添加到已有的相同物品的格子
            foreach (var slot in targetSlots)
            {
                if (remainingQuantity <= 0)
                    break;

                if (!slot.IsEmpty && slot.ItemData == item && !slot.IsFull)
                {
                    int added = slot.AddItem(item, remainingQuantity);
                    remainingQuantity -= added;
                    totalAdded += added;
                }
            }

            // 然后尝试添加到空格子
            foreach (var slot in targetSlots)
            {
                if (remainingQuantity <= 0)
                    break;

                if (slot.IsEmpty)
                {
                    int added = slot.AddItem(item, remainingQuantity);
                    remainingQuantity -= added;
                    totalAdded += added;
                }
            }

            // 如果还有剩余数量，创建新格子
            while (remainingQuantity > 0)
            {
                var newSlot = new InventorySlot(targetSlots.Count);
                newSlot.OnSlotChanged += HandleSlotChanged;
                targetSlots.Add(newSlot);

                int added = newSlot.AddItem(item, remainingQuantity);
                remainingQuantity -= added;
                totalAdded += added;
            }

            // 触发事件
            if (totalAdded > 0)
            {
                OnItemAdded?.Invoke(item, totalAdded);
                OnInventoryChanged?.Invoke();
            }

            return totalAdded;
        }

        /// <summary>
        /// 从背包移除物品
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        /// <returns>实际移除的数量</returns>
        public int RemoveItem(BaseItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
                return 0;

            var targetSlots = GetSlotsByItemType(item.ItemType);
            if (targetSlots == null)
                return 0;

            int remainingQuantity = quantity;
            int totalRemoved = 0;

            // 从后往前移除，保持格子顺序
            for (int i = targetSlots.Count - 1; i >= 0; i--)
            {
                if (remainingQuantity <= 0)
                    break;

                var slot = targetSlots[i];
                if (!slot.IsEmpty && slot.ItemData == item)
                {
                    int removed = slot.RemoveItem(remainingQuantity);
                    remainingQuantity -= removed;
                    totalRemoved += removed;

                    // 如果格子空了，移除该格子
                    if (slot.IsEmpty)
                    {
                        targetSlots.RemoveAt(i);
                    }
                }
            }

            // 触发事件
            if (totalRemoved > 0)
            {
                OnItemRemoved?.Invoke(item, totalRemoved);
                OnInventoryChanged?.Invoke();
            }

            return totalRemoved;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <returns>物品数量</returns>
        public int GetItemCount(BaseItemData item)
        {
            if (item == null)
                return 0;

            var targetSlots = GetSlotsByItemType(item.ItemType);
            if (targetSlots == null)
                return 0;

            return targetSlots.Where(slot => !slot.IsEmpty && slot.ItemData == item)
                           .Sum(slot => slot.Quantity);
        }

        /// <summary>
        /// 检查是否有指定数量的物品
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否有足够数量</returns>
        public bool HasItem(BaseItemData item, int quantity = 1)
        {
            return GetItemCount(item) >= quantity;
        }

        /// <summary>
        /// 获取指定索引的格子
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <param name="index">格子索引</param>
        /// <returns>格子</returns>
        public InventorySlot GetSlot(ItemType itemType, int index)
        {
            var targetSlots = GetSlotsByItemType(itemType);
            if (targetSlots == null || index < 0 || index >= targetSlots.Count)
                return null;

            return targetSlots[index];
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <param name="slotIndex">格子索引</param>
        /// <param name="user">使用者</param>
        /// <returns>是否使用成功</returns>
        public bool UseItem(ItemType itemType, int slotIndex, GameObject user = null)
        {
            var slot = GetSlot(itemType, slotIndex);
            if (slot == null || slot.IsEmpty)
                return false;

            bool success = slot.UseItem(user);
            if (success)
            {
                OnInventoryChanged?.Invoke();
            }

            return success;
        }

        /// <summary>
        /// 根据类型获取物品格子
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>物品格子列表</returns>
        public List<InventorySlot> GetItemsByType(ItemType itemType)
        {
            var targetSlots = GetSlotsByItemType(itemType);
            if (targetSlots == null)
                return new List<InventorySlot>();

            return targetSlots.Where(slot => !slot.IsEmpty && slot.ItemData.ItemType == itemType)
                           .ToList();
        }

        /// <summary>
        /// 获取指定类型的所有格子（包括空格子）
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>所有格子列表</returns>
        public List<InventorySlot> GetAllSlotsByType(ItemType itemType)
        {
            var targetSlots = GetSlotsByItemType(itemType);
            return targetSlots != null ? new List<InventorySlot>(targetSlots) : new List<InventorySlot>();
        }

        /// <summary>
        /// 获取非空格子
        /// </summary>
        /// <returns>非空格子列表</returns>
        public List<InventorySlot> GetNonEmptySlots()
        {
            var allSlots = new List<InventorySlot>();
            allSlots.AddRange(weaponSlots);
            allSlots.AddRange(equipmentSlots);
            allSlots.AddRange(consumableSlots);

            return allSlots.Where(slot => !slot.IsEmpty).ToList();
        }

        /// <summary>
        /// 清空指定类型的背包
        /// </summary>
        /// <param name="itemType">物品类型</param>
        public void ClearSubInventory(ItemType itemType)
        {
            var targetSlots = GetSlotsByItemType(itemType);
            if (targetSlots == null)
                return;

            foreach (var slot in targetSlots)
            {
                slot.ClearSlot();
            }

            targetSlots.Clear();
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// 清空所有背包
        /// </summary>
        public void ClearInventory()
        {
            ClearSubInventory(ItemType.Weapon);
            ClearSubInventory(ItemType.Equipment);
            ClearSubInventory(ItemType.Consumable);
        }

        /// <summary>
        /// 排序指定类型的背包
        /// </summary>
        /// <param name="itemType">物品类型</param>
        public void SortSubInventory(ItemType itemType)
        {
            var targetSlots = GetSlotsByItemType(itemType);
            if (targetSlots == null)
                return;

            // 获取非空格子
            var nonEmptySlots = targetSlots.Where(slot => !slot.IsEmpty).ToList();

            // 清空所有格子
            targetSlots.Clear();

            // 重新添加非空格子
            foreach (var slot in nonEmptySlots)
            {
                targetSlots.Add(slot);
            }

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// 排序所有背包
        /// </summary>
        public void SortInventory()
        {
            SortSubInventory(ItemType.Weapon);
            SortSubInventory(ItemType.Equipment);
            SortSubInventory(ItemType.Consumable);
        }

        /// <summary>
        /// 检查是否可以添加物品
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否可以添加</returns>
        public bool CanAddItem(BaseItemData item, int quantity = 1)
        {
            // 无限容量，总是可以添加
            return true;
        }

        /// <summary>
        /// 获取指定类型的格子列表
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>格子列表</returns>
        private List<InventorySlot> GetSlotsByItemType(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return weaponSlots;
                case ItemType.Equipment:
                    return equipmentSlots;
                case ItemType.Consumable:
                    return consumableSlots;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 处理格子变化事件
        /// </summary>
        /// <param name="slot">变化的格子</param>
        private void HandleSlotChanged(InventorySlot slot)
        {
            OnSlotChanged?.Invoke(slot);
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// 获取背包统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetInventoryStats()
        {
            int weaponCount = weaponSlots.Count(slot => !slot.IsEmpty);
            int equipmentCount = equipmentSlots.Count(slot => !slot.IsEmpty);
            int consumableCount = consumableSlots.Count(slot => !slot.IsEmpty);
            int totalSlots = weaponSlots.Count + equipmentSlots.Count + consumableSlots.Count;

            return $"背包统计:\n" +
                   $"武器背包: {weaponCount} 个物品 ({weaponSlots.Count} 个格子)\n" +
                   $"装备背包: {equipmentCount} 个物品 ({equipmentSlots.Count} 个格子)\n" +
                   $"道具背包: {consumableCount} 个物品 ({consumableSlots.Count} 个格子)\n" +
                   $"总计: {weaponCount + equipmentCount + consumableCount} 个物品 ({totalSlots} 个格子)";
        }

        /// <summary>
        /// 获取指定类型的背包统计信息
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>统计信息字符串</returns>
        public string GetSubInventoryStats(ItemType itemType)
        {
            var targetSlots = GetSlotsByItemType(itemType);
            if (targetSlots == null)
                return $"未知背包类型: {itemType}";

            int usedSlots = targetSlots.Count(slot => !slot.IsEmpty);
            int totalSlots = targetSlots.Count;

            return $"{GetItemTypeDisplayName(itemType)}背包: {usedSlots} 个物品 ({totalSlots} 个格子)";
        }

        /// <summary>
        /// 获取物品类型显示名称
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>显示名称</returns>
        private string GetItemTypeDisplayName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    return "武器";
                case ItemType.Equipment:
                    return "装备";
                case ItemType.Consumable:
                    return "道具";
                default:
                    return itemType.ToString();
            }
        }
    }
}