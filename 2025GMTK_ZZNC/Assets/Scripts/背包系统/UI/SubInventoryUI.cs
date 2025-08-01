using UnityEngine;
using UnityEngine.UI;
using InventorySystem.Core;
using InventorySystem.Data;
using InventorySystem.Managers;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace InventorySystem.UI
{
    /// <summary>
    /// 子背包UI管理器 - 管理单个子背包的显示和滑动功能
    /// </summary>
    public class SubInventoryUI : MonoBehaviour
    {
        [SerializeField] private InventoryUI inventoryUI;
        [Header("UI组件")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform contentContainer;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private ContentSizeFitter contentSizeFitter;
        [SerializeField] private GameObject slotPrefab;

        [Header("配置")]
        [SerializeField] private ItemType itemType;
        [SerializeField] private int maxDisplaySlots = 20; // 最多显示的格子数
        [SerializeField] private int expandSlotsAmount = 5; // 每次扩充的格子数
        [SerializeField] private int initialSlots = 10; // 初始格子数

        [Header("统计信息")]
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI capacityText;

        // 私有变量
        private List<InventorySlotUI> slotUIs;
        private InventoryManager inventoryManager;
        private int currentSlotCount;
        private bool isInitialized = false;

        // 属性
        public ItemType ItemType => itemType;
        public int CurrentSlotCount => currentSlotCount;
        public int MaxDisplaySlots => maxDisplaySlots;

        /// <summary>
        /// 初始化子背包UI
        /// </summary>
        private void Start()
        {
            InitializeSubInventory();
        }

        /// <summary>
        /// 初始化子背包
        /// </summary>
        private void InitializeSubInventory()
        {
            // 获取背包管理器
            inventoryManager = InventoryManager.Instance;
            if (inventoryManager == null)
            {
                Debug.LogError($"找不到InventoryManager实例，无法初始化{itemType}子背包");
                return;
            }

            // 初始化数据结构
            slotUIs = new List<InventorySlotUI>();
            currentSlotCount = initialSlots;

            // 创建初始格子
            CreateSlots(initialSlots);

            // 设置滑动面板
            SetupScrollRect();

            isInitialized = true;

            Debug.Log($"{itemType}子背包UI初始化完成，初始格子数: {initialSlots}");
        }

        /// <summary>
        /// 设置滑动面板
        /// </summary>
        private void SetupScrollRect()
        {
            if (scrollRect != null)
            {
                // 设置滑动面板属性
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                scrollRect.inertia = true;
                scrollRect.decelerationRate = 0.135f;
                scrollRect.scrollSensitivity = 20f;
            }

            if (contentSizeFitter != null)
            {
                // 设置内容大小适配器
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        /// <summary>
        /// 创建格子
        /// </summary>
        /// <param name="count">格子数量</param>
        private void CreateSlots(int count)
        {
            if (contentContainer == null || slotPrefab == null)
            {
                Debug.LogError("内容容器或格子预制体未设置");
                return;
            }

            int existingSlots = slotUIs.Count;
            int newSlotsNeeded = count - existingSlots;

            if (newSlotsNeeded <= 0) return;

            // 创建新格子
            for (int i = 0; i < newSlotsNeeded; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, contentContainer);
                InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
                
                if (slotUI != null)
                {
                    int slotIndex = existingSlots + i;
                    slotUI.Initialize(slotIndex, inventoryUI);
                    slotUIs.Add(slotUI);
                }
            }

            Debug.Log($"为{itemType}子背包创建了{newSlotsNeeded}个新格子，总计{slotUIs.Count}个格子");
        }

        /// <summary>
        /// 刷新子背包显示
        /// </summary>
        public void RefreshDisplay()
        {
            if (!isInitialized || inventoryManager == null) return;

            var allSlots = inventoryManager.GetAllSlotsByType(itemType);

            // 检查是否需要扩充格子
            CheckAndExpandSlots(allSlots.Count);

            // 更新格子显示
            for (int i = 0; i < slotUIs.Count; i++)
            {
                if (i < allSlots.Count)
                {
                    var slot = allSlots[i];
                    slotUIs[i].SetSlot(slot);
                    slotUIs[i].gameObject.SetActive(true);
                }
                else
                {
                    slotUIs[i].ClearSlot();
                    //slotUIs[i].gameObject.SetActive(false);
                }
            }

            // 更新统计信息
            UpdateStats(allSlots.Count);
        }

        /// <summary>
        /// 检查并扩充格子
        /// </summary>
        /// <param name="requiredSlots">需要的格子数</param>
        private void CheckAndExpandSlots(int requiredSlots)
        {
            if (requiredSlots > currentSlotCount)
            {
                int newSlotCount = currentSlotCount + expandSlotsAmount;
                currentSlotCount = newSlotCount;
                
                CreateSlots(newSlotCount);
                
                Debug.Log($"扩充{itemType}子背包格子，从{currentSlotCount - expandSlotsAmount}增加到{currentSlotCount}");
            }
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        /// <param name="usedSlots">已使用的格子数</param>
        private void UpdateStats(int usedSlots)
        {
            if (statsText != null)
            {
                statsText.text = $"{GetItemTypeDisplayName(itemType)}: {usedSlots}/{currentSlotCount}";
            }

            if (capacityText != null)
            {
                float capacityPercentage = currentSlotCount > 0 ? (float)usedSlots / currentSlotCount * 100f : 0f;
                capacityText.text = $"容量: {capacityPercentage:F1}%";
            }
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

        /// <summary>
        /// 设置子背包可见性
        /// </summary>
        /// <param name="visible">是否可见</param>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// 滚动到指定格子
        /// </summary>
        /// <param name="slotIndex">格子索引</param>
        public void ScrollToSlot(int slotIndex)
        {
            if (scrollRect == null || slotIndex < 0 || slotIndex >= slotUIs.Count) return;

            // 计算目标位置
            float normalizedPosition = 1f - (float)slotIndex / slotUIs.Count;
            scrollRect.verticalNormalizedPosition = normalizedPosition;
        }

        /// <summary>
        /// 滚动到顶部
        /// </summary>
        public void ScrollToTop()
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        /// <summary>
        /// 滚动到底部
        /// </summary>
        public void ScrollToBottom()
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// 获取指定索引的格子UI
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>格子UI</returns>
        public InventorySlotUI GetSlotUI(int index)
        {
            if (index >= 0 && index < slotUIs.Count)
            {
                return slotUIs[index];
            }
            return null;
        }

        /// <summary>
        /// 获取所有格子UI
        /// </summary>
        /// <returns>格子UI列表</returns>
        public List<InventorySlotUI> GetAllSlotUIs()
        {
            return new List<InventorySlotUI>(slotUIs);
        }

        /// <summary>
        /// 清空所有格子
        /// </summary>
        public void ClearAllSlots()
        {
            foreach (var slotUI in slotUIs)
            {
                slotUI.ClearSlot();
            }
        }

        /// <summary>
        /// 设置格子交互性
        /// </summary>
        /// <param name="interactable">是否可交互</param>
        public void SetSlotsInteractable(bool interactable)
        {
            foreach (var slotUI in slotUIs)
            {
                slotUI.SetInteractable(interactable);
            }
        }

        /// <summary>
        /// 获取子背包统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetSubInventoryStats()
        {
            if (inventoryManager == null) return "背包管理器未初始化";

            var allSlots = inventoryManager.GetAllSlotsByType(itemType);
            int usedSlots = allSlots.Count(slot => !slot.IsEmpty);
            int totalSlots = allSlots.Count;

            return $"{GetItemTypeDisplayName(itemType)}背包: {usedSlots}个物品 ({totalSlots}个格子)";
        }
    }
} 