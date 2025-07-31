using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// 背包实现 (SRP: 只负责物品存储)
    /// </summary>
    public class Inventory : MonoBehaviour, IInventory
    {
        [SerializeField] private List<Item.IItem> items = new List<Item.IItem>();
        [SerializeField] private int maxCapacity = 20;

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <returns>是否成功添加</returns>
        public bool AddItem(Item.IItem item)
        {
            if (HasSpace())
            {
                items.Add(item);
                Debug.Log($"添加物品：{item.Name}");
                return true;
            }
            
            Debug.Log("背包已满，无法添加物品");
            return false;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="item">要移除的物品</param>
        /// <returns>是否成功移除</returns>
        public bool RemoveItem(Item.IItem item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                Debug.Log($"移除物品：{item.Name}");
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// 获取指定索引的物品
        /// </summary>
        /// <param name="index">物品索引</param>
        /// <returns>物品</returns>
        public Item.IItem GetItem(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                return items[index];
            }
            
            return null;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <returns>物品数量</returns>
        public int GetItemCount()
        {
            return items.Count;
        }

        /// <summary>
        /// 检查是否有空间
        /// </summary>
        /// <returns>是否有空间</returns>
        public bool HasSpace()
        {
            return items.Count < maxCapacity;
        }

        /// <summary>
        /// 整理物品
        /// </summary>
        public void SortItems()
        {
            // 物品排序逻辑
            Debug.Log("整理背包");
        }
    }
}