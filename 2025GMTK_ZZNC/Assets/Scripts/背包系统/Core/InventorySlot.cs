using System;
using UnityEngine;
using InventorySystem.Items;

namespace InventorySystem.Core
{
    /// <summary>
    /// 背包槽位类 - 管理单个槽位的物品和数量
    /// 遵循单一职责原则，只负责槽位数据管理
    /// </summary>
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private BaseItemData itemData;
        [SerializeField] private int quantity;
        [SerializeField] private int slotIndex;

        // 事件 - 当槽位内容改变时触发
        public event Action<InventorySlot> OnSlotChanged;

        // 只读属性
        public BaseItemData ItemData => itemData;
        public int Quantity => quantity;
        public int SlotIndex => slotIndex;
        public bool IsEmpty => itemData == null || quantity <= 0;
        public bool IsFull => itemData != null && quantity >= itemData.MaxStackSize;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">槽位索引</param>
        public InventorySlot(int index)
        {
            slotIndex = index;
            itemData = null;
            quantity = 0;
        }

        /// <summary>
        /// 设置物品数据
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="amount">数量</param>
        /// <returns>是否设置成功</returns>
        public bool SetItem(BaseItemData item, int amount = 1)
        {
            if (item == null || amount <= 0)
            {
                ClearSlot();
                return true;
            }

            // 检查是否可以设置该物品
            if (!CanAcceptItem(item))
            {
                return false;
            }

            itemData = item;
            quantity = Mathf.Min(amount, item.MaxStackSize);
            
            NotifySlotChanged();
            return true;
        }

        /// <summary>
        /// 添加物品到槽位
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="amount">数量</param>
        /// <returns>实际添加的数量</returns>
        public int AddItem(BaseItemData item, int amount = 1)
        {
            if (item == null || amount <= 0)
                return 0;

            // 如果槽位为空，直接设置物品
            if (IsEmpty)
            {
                int addAmount = Mathf.Min(amount, item.MaxStackSize);
                SetItem(item, addAmount);
                return addAmount;
            }

            // 如果物品类型不匹配，无法添加
            if (itemData != item)
                return 0;

            // 计算可以添加的数量
            int maxCanAdd = itemData.MaxStackSize - quantity;
            int actualAddAmount = Mathf.Min(amount, maxCanAdd);

            if (actualAddAmount > 0)
            {
                quantity += actualAddAmount;
                NotifySlotChanged();
            }

            return actualAddAmount;
        }

        /// <summary>
        /// 从槽位移除物品
        /// </summary>
        /// <param name="amount">移除数量</param>
        /// <returns>实际移除的数量</returns>
        public int RemoveItem(int amount = 1)
        {
            if (IsEmpty || amount <= 0)
                return 0;

            int actualRemoveAmount = Mathf.Min(amount, quantity);
            quantity -= actualRemoveAmount;

            // 如果数量为0，清空槽位
            if (quantity <= 0)
            {
                ClearSlot();
            }
            else
            {
                NotifySlotChanged();
            }

            return actualRemoveAmount;
        }

        /// <summary>
        /// 清空槽位
        /// </summary>
        public void ClearSlot()
        {
            itemData = null;
            quantity = 0;
            NotifySlotChanged();
        }

        /// <summary>
        /// 检查是否可以接受指定物品
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <returns>是否可以接受</returns>
        public bool CanAcceptItem(BaseItemData item)
        {
            if (item == null)
                return false;

            // 如果槽位为空，可以接受任何物品
            if (IsEmpty)
                return true;

            // 如果物品类型相同且未满，可以接受
            return itemData == item && !IsFull;
        }

        /// <summary>
        /// 获取可以添加的最大数量
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <returns>可添加的最大数量</returns>
        public int GetMaxAddableAmount(BaseItemData item)
        {
            if (!CanAcceptItem(item))
                return 0;

            if (IsEmpty)
                return item.MaxStackSize;

            return itemData.MaxStackSize - quantity;
        }

        /// <summary>
        /// 使用槽位中的物品
        /// </summary>
        /// <param name="user">使用者</param>
        /// <returns>是否使用成功</returns>
        public bool UseItem(GameObject user)
        {
            if (IsEmpty || user == null)
                return false;

            bool success = itemData.Use(user);
            
            // 如果是消耗品且使用成功，减少数量
            if (success && itemData.IsConsumable)
            {
                RemoveItem(1);
            }

            return success;
        }

        /// <summary>
        /// 获取槽位信息的字符串表示
        /// </summary>
        /// <returns>槽位信息</returns>
        public override string ToString()
        {
            if (IsEmpty)
                return $"槽位 {slotIndex}: 空";
            
            return $"槽位 {slotIndex}: {itemData.ItemName} x{quantity}";
        }

        /// <summary>
        /// 通知槽位内容改变
        /// </summary>
        private void NotifySlotChanged()
        {
            OnSlotChanged?.Invoke(this);
        }

        /// <summary>
        /// 复制槽位数据
        /// </summary>
        /// <returns>槽位数据的副本</returns>
        public InventorySlot Clone()
        {
            var clone = new InventorySlot(slotIndex);
            if (!IsEmpty)
            {
                clone.SetItem(itemData, quantity);
            }
            return clone;
        }
    }
}