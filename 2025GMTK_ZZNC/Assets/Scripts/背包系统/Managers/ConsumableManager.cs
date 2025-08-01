using System.Collections.Generic;
using UnityEngine;
using InventorySystem.Items;

namespace InventorySystem.Managers
{
    /// <summary>
    /// 消耗品管理器 - 管理道具的使用和冷却时间
    /// 遵循单一职责原则，专门处理消耗品相关逻辑
    /// </summary>
    public class ConsumableManager : MonoBehaviour
    {
        [Header("冷却设置")]
        [SerializeField] private Dictionary<ConsumableData, float> itemCooldowns;
        [SerializeField] private Dictionary<ConsumableData, float> lastUseTimes;

        /// <summary>
        /// 初始化消耗品管理器
        /// </summary>
        private void Awake()
        {
            itemCooldowns = new Dictionary<ConsumableData, float>();
            lastUseTimes = new Dictionary<ConsumableData, float>();
        }

        /// <summary>
        /// 检查道具是否可以使用（不在冷却中）
        /// </summary>
        /// <param name="consumable">消耗品数据</param>
        /// <returns>是否可以使用</returns>
        public bool CanUseItem(ConsumableData consumable)
        {
            if (consumable == null)
                return false;

            // 如果没有冷却时间，直接可以使用
            if (consumable.CooldownTime <= 0)
                return true;

            // 检查是否在冷却中
            if (lastUseTimes.ContainsKey(consumable))
            {
                float timeSinceLastUse = Time.time - lastUseTimes[consumable];
                return timeSinceLastUse >= consumable.CooldownTime;
            }

            return true;
        }

        /// <summary>
        /// 设置道具冷却时间
        /// </summary>
        /// <param name="consumable">消耗品数据</param>
        /// <param name="cooldownTime">冷却时间</param>
        public void SetItemCooldown(ConsumableData consumable, float cooldownTime)
        {
            if (consumable == null)
                return;

            itemCooldowns[consumable] = cooldownTime;
            lastUseTimes[consumable] = Time.time;
        }

        /// <summary>
        /// 获取道具剩余冷却时间
        /// </summary>
        /// <param name="consumable">消耗品数据</param>
        /// <returns>剩余冷却时间</returns>
        public float GetRemainingCooldown(ConsumableData consumable)
        {
            if (consumable == null || !lastUseTimes.ContainsKey(consumable))
                return 0f;

            float timeSinceLastUse = Time.time - lastUseTimes[consumable];
            float remainingTime = consumable.CooldownTime - timeSinceLastUse;

            return Mathf.Max(0f, remainingTime);
        }

        /// <summary>
        /// 获取道具冷却进度（0-1）
        /// </summary>
        /// <param name="consumable">消耗品数据</param>
        /// <returns>冷却进度</returns>
        public float GetCooldownProgress(ConsumableData consumable)
        {
            if (consumable == null || consumable.CooldownTime <= 0)
                return 1f;

            float remainingTime = GetRemainingCooldown(consumable);
            return 1f - (remainingTime / consumable.CooldownTime);
        }

        /// <summary>
        /// 清除所有冷却时间
        /// </summary>
        public void ClearAllCooldowns()
        {
            itemCooldowns.Clear();
            lastUseTimes.Clear();
        }

        /// <summary>
        /// 清除指定道具的冷却时间
        /// </summary>
        /// <param name="consumable">消耗品数据</param>
        public void ClearItemCooldown(ConsumableData consumable)
        {
            if (consumable == null)
                return;

            itemCooldowns.Remove(consumable);
            lastUseTimes.Remove(consumable);
        }
        public bool UseConsumable(ConsumableData consumableData)
        {
            //道具使用具体实现
            return true;
        }
    }
}