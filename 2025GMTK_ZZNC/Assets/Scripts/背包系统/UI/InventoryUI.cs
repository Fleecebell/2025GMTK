using UnityEngine;
using UnityEngine.UI;
using InventorySystem.Managers;
using InventorySystem.Core;
using InventorySystem.Data;
using InventorySystem.Items;
using TMPro;
using System.Collections.Generic;

namespace InventorySystem.UI
{
    /// <summary>
    /// 背包UI管理器 - 负责三个子背包的显示和交互
    /// 每个子背包最多显示20个格子，通过滑动面板查看更多
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button minimizeButton;

        [Header("角色面板")]
        [SerializeField] private Transform characterPanel;
        [SerializeField] private Image characterPortrait;
        [SerializeField] private Transform weaponSlot;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private Button weaponSlotButton;

        [Header("子背包UI")]
        [SerializeField] private SubInventoryUI weaponSubInventory;
        [SerializeField] private SubInventoryUI equipmentSubInventory;
        [SerializeField] private SubInventoryUI consumableSubInventory;

        [Header("标签页")]
        [SerializeField] private Button weaponTabButton;
        [SerializeField] private Button equipmentTabButton;
        [SerializeField] private Button consumableTabButton;

        [Header("物品信息")]
        [SerializeField] private GameObject itemInfoPanel;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private Image itemIconImage;

        // 私有变量
        private InventoryManager inventoryManager;
        private ItemType currentTab = ItemType.Weapon;
        private bool isInventoryOpen = false;
        private CanvasGroup itemInfoCanvasGroup;
        private CanvasGroup inventoryCanvasGroup;

        /// <summary>
        /// 初始化UI系统
        /// </summary>
        private void Start()
        {
            InitializeUI();
            BindEvents();
            RefreshUI();
        }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        private void InitializeUI()
        {
            // 获取背包管理器
            inventoryManager = InventoryManager.Instance;
            if (inventoryManager == null)
            {
                Debug.LogError("找不到InventoryManager实例");
                return;
            }

            // 初始化物品信息面板CanvasGroup
            InitializeItemInfoPanel();
            
            // 初始化背包面板CanvasGroup
            InitializeInventoryPanel();

            // 设置初始状态
            isInventoryOpen = false;

            // 设置默认标签页
            SetActiveTab(ItemType.Weapon);
        }

        /// <summary>
        /// 初始化物品信息面板
        /// </summary>
        private void InitializeItemInfoPanel()
        {
            if (itemInfoPanel != null)
            {
                // 获取或添加CanvasGroup组件
                itemInfoCanvasGroup = itemInfoPanel.GetComponent<CanvasGroup>();
                if (itemInfoCanvasGroup == null)
                {
                    itemInfoCanvasGroup = itemInfoPanel.AddComponent<CanvasGroup>();
                }

                // 设置初始状态
                itemInfoCanvasGroup.alpha = 0f;
                itemInfoCanvasGroup.interactable = false;
                itemInfoCanvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// 初始化背包面板
        /// </summary>
        private void InitializeInventoryPanel()
        {
            if (inventoryPanel != null)
            {
                // 获取或添加CanvasGroup组件
                inventoryCanvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
                if (inventoryCanvasGroup == null)
                {
                    inventoryCanvasGroup = inventoryPanel.AddComponent<CanvasGroup>();
                }

                // 设置初始状态
                inventoryCanvasGroup.alpha = 0f;
                inventoryCanvasGroup.interactable = false;
                inventoryCanvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        private void BindEvents()
        {
            // 背包按钮事件
            if (inventoryButton != null)
                inventoryButton.onClick.AddListener(ToggleInventory);

            if (minimizeButton != null)
                minimizeButton.onClick.AddListener(CloseInventory);

            // 标签页按钮事件
            if (weaponTabButton != null)
                weaponTabButton.onClick.AddListener(() => SetActiveTab(ItemType.Weapon));

            if (equipmentTabButton != null)
                equipmentTabButton.onClick.AddListener(() => SetActiveTab(ItemType.Equipment));

            if (consumableTabButton != null)
                consumableTabButton.onClick.AddListener(() => SetActiveTab(ItemType.Consumable));

            // 武器槽位事件
            if (weaponSlotButton != null)
                weaponSlotButton.onClick.AddListener(OnWeaponSlotClicked);

            // 背包管理器事件
            if (inventoryManager != null)
            {
                inventoryManager.OnInventoryChanged += RefreshUI;
                inventoryManager.EquipmentManager.OnWeaponEquipped += OnWeaponEquipped;
                inventoryManager.EquipmentManager.OnWeaponUnequipped += OnWeaponUnequipped;
            }
        }

        /// <summary>
        /// 切换背包显示状态
        /// </summary>
        public void ToggleInventory()
        {
            if (isInventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        /// <summary>
        /// 打开背包
        /// </summary>
        public void OpenInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
                isInventoryOpen = true;

                if (inventoryCanvasGroup != null)
                {
                    inventoryCanvasGroup.alpha = 1f;
                    inventoryCanvasGroup.interactable = true;
                    inventoryCanvasGroup.blocksRaycasts = true;
                }

                RefreshUI();
            }
        }

        /// <summary>
        /// 关闭背包
        /// </summary>
        public void CloseInventory()
        {
            if (inventoryPanel != null)
            {
                isInventoryOpen = false;

                if (inventoryCanvasGroup != null)
                {
                    inventoryCanvasGroup.alpha = 0f;
                    inventoryCanvasGroup.interactable = false;
                    inventoryCanvasGroup.blocksRaycasts = false;
                }

                inventoryPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 设置活动标签页
        /// </summary>
        /// <param name="itemType">物品类型</param>
        public void SetActiveTab(ItemType itemType)
        {
            currentTab = itemType;
            UpdateTabButtons();
            UpdateSubInventoryVisibility();
        }

        /// <summary>
        /// 更新标签页按钮状态
        /// </summary>
        private void UpdateTabButtons()
        {
            SetButtonState(weaponTabButton, currentTab == ItemType.Weapon);
            SetButtonState(equipmentTabButton, currentTab == ItemType.Equipment);
            SetButtonState(consumableTabButton, currentTab == ItemType.Consumable);
        }

        /// <summary>
        /// 设置按钮状态
        /// </summary>
        /// <param name="button">按钮</param>
        /// <param name="isActive">是否激活</param>
        private void SetButtonState(Button button, bool isActive)
        {
            if (button != null)
            {
                var colors = button.colors;
                colors.normalColor = isActive ? Color.white : Color.gray;
                button.colors = colors;
            }
        }

        /// <summary>
        /// 更新子背包可见性
        /// </summary>
        private void UpdateSubInventoryVisibility()
        {
            if (weaponSubInventory != null)
                weaponSubInventory.SetVisible(currentTab == ItemType.Weapon);

            if (equipmentSubInventory != null)
                equipmentSubInventory.SetVisible(currentTab == ItemType.Equipment);

            if (consumableSubInventory != null)
                consumableSubInventory.SetVisible(currentTab == ItemType.Consumable);
        }

        /// <summary>
        /// 刷新UI
        /// </summary>
        public void RefreshUI()
        {
            RefreshCharacterPanel();
            RefreshSubInventories();
        }

        /// <summary>
        /// 刷新角色面板
        /// </summary>
        private void RefreshCharacterPanel()
        {
            if (inventoryManager == null) return;

            // 更新武器显示
            var currentWeapon = inventoryManager.EquipmentManager.CurrentWeapon;
            if (weaponIcon != null)
            {
                if (currentWeapon != null && currentWeapon.Icon != null)
                {
                    weaponIcon.sprite = currentWeapon.Icon;
                    weaponIcon.enabled = true;
                }
                else
                {
                    weaponIcon.sprite = null;
                    weaponIcon.enabled = false;
                }
            }
        }

        /// <summary>
        /// 刷新所有子背包
        /// </summary>
        private void RefreshSubInventories()
        {
            if (weaponSubInventory != null)
                weaponSubInventory.RefreshDisplay();

            if (equipmentSubInventory != null)
                equipmentSubInventory.RefreshDisplay();

            if (consumableSubInventory != null)
                consumableSubInventory.RefreshDisplay();
        }

        /// <summary>
        /// 格子点击事件
        /// </summary>
        /// <param name="slotIndex">格子索引</param>
        /// <param name="isRightClick">是否右键点击</param>
        public void OnSlotClicked(int slotIndex, bool isRightClick = false)
        {
            var slot = GetSlotByDisplayIndex(slotIndex);
            if (slot == null) return;

            if (isRightClick)
            {
                // 右键使用物品
                UseItem(slot);
            }
            else
            {
                // 左键显示物品信息
                if (!slot.IsEmpty)
                {
                    ShowItemInfo(slot.ItemData);
                }
            }
        }

        /// <summary>
        /// 根据显示索引获取格子
        /// </summary>
        /// <param name="displayIndex">显示索引</param>
        /// <returns>格子</returns>
        private InventorySlot GetSlotByDisplayIndex(int displayIndex)
        {
            if (inventoryManager == null) return null;

            var allSlots = inventoryManager.GetAllSlotsByType(currentTab);
            if (displayIndex >= 0 && displayIndex < allSlots.Count)
            {
                return allSlots[displayIndex];
            }

            return null;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="slot">格子</param>
        private void UseItem(InventorySlot slot)
        {
            if (slot == null || slot.IsEmpty) return;

            var item = slot.ItemData;
            Debug.Log($"使用物品: {item.ItemName}");

            try
            {
                // 获取所有槽位并查找索引
                var allSlots = inventoryManager.GetAllSlotsByType(currentTab);
                Debug.Log($"获取到的槽位总数: {allSlots?.Count ?? 0}");

                int slotIndex = allSlots?.IndexOf(slot) ?? -1;
                Debug.Log($"槽位索引: {slotIndex}");

                if (slotIndex < 0)
                {
                    Debug.LogError($"未找到槽位索引，slot为null: {slot == null}, allSlots为null: {allSlots == null}");
                    return;
                }

                // 根据物品类型执行不同操作
                switch (item.ItemType)
                {
                    case ItemType.Weapon:
                        Debug.Log("处理武器类型物品");
                        var weaponData = item as WeaponData;
                        if (weaponData != null)
                        {
                            Debug.Log($"调用inventoryManager.UseItem - currentTab: {currentTab}, slotIndex: {slotIndex}");
                            // 记录当前装备的武器
                            var currentWeapon = inventoryManager.EquipmentManager.CurrentWeapon;

                            // 如果原本有装备武器，则放回背包
                            if (currentWeapon != null)
                            {
                                inventoryManager.AddItem(currentWeapon, 1);
                                Debug.Log($"将原装备武器放回背包: {currentWeapon.ItemName}");
                            }

                            // 先从背包移除该武器
                            int removed = inventoryManager.RemoveItem(weaponData, 1);
                            Debug.Log($"从背包移除武器: {weaponData.ItemName}，移除数量: {removed}");

                            // 装备新武器
                            bool useResult = inventoryManager.UseWeapon(weaponData);
                            Debug.Log($"武器使用结果: {useResult}");
                        }
                        else
                        {
                            Debug.LogError("武器数据转换失败");
                        }
                        break;

                    case ItemType.Equipment:
                        Debug.Log("处理装备类型物品");
                        var equipmentData = item as EquipmentData;
                        if (equipmentData != null)
                        {
                            Debug.Log($"调用inventoryManager.UseItem - currentTab: {currentTab}, slotIndex: {slotIndex}");
                            bool useResult = inventoryManager.UseEquipment(equipmentData);
                            Debug.Log($"装备使用结果: {useResult}");
                        }
                        else
                        {
                            Debug.LogError("装备数据转换失败");
                        }
                        break;

                    case ItemType.Consumable:
                        Debug.Log("处理消耗品类型物品");
                        var consumableData = item as ConsumableData;
                        if (consumableData != null)
                        {
                            Debug.Log($"调用inventoryManager.UseItem - currentTab: {currentTab}, slotIndex: {slotIndex}");

                            bool useResult = inventoryManager.UseConsumable(slot);
                            Debug.Log($"消耗品使用结果: {useResult}");
                        }
                        else
                        {
                            Debug.LogError("消耗品数据转换失败");
                        }
                        break;

                    default:
                        Debug.LogWarning($"未知物品类型: {item.ItemType}");
                        break;
                }

                Debug.Log("开始刷新UI");
                // 刷新UI
                RefreshUI();
                Debug.Log("UI刷新完成");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"UseItem异常: {ex.Message}\n{ex.StackTrace}");
                Debug.LogError($"异常发生时的状态 - inventoryManager为null: {inventoryManager == null}, currentTab: {currentTab}, item: {item?.ItemName}");
            }
        }

        /// <summary>
        /// 武器槽位点击事件
        /// </summary>
        private void OnWeaponSlotClicked()
        {
            if (inventoryManager == null) return;

            var currentWeapon = inventoryManager.EquipmentManager.CurrentWeapon;
            if (currentWeapon != null)
            {
                UnequipWeapon();
            }
        }

        /// <summary>
        /// 卸下武器
        /// </summary>
        private void UnequipWeapon()
        {
            if (inventoryManager == null) return;

            var unequippedWeapon = inventoryManager.EquipmentManager.UnequipWeapon();
            if (unequippedWeapon != null)
            {
                // 将武器放回背包
                inventoryManager.AddItem(unequippedWeapon, 1);
                Debug.Log($"卸下武器: {unequippedWeapon.ItemName}");
            }

            RefreshUI();
        }

        /// <summary>
        /// 显示物品信息
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="slotPosition">格子位置</param>
        public void ShowItemInfo(BaseItemData item, Vector3? slotPosition = null)
        {
            if (itemInfoPanel == null || item == null) return;

            // 设置物品信息
            if (itemNameText != null)
                itemNameText.text = item.ItemName;

            if (itemDescriptionText != null)
                itemDescriptionText.text = item.Description;

            if (itemIconImage != null && item.Icon != null)
            {
                itemIconImage.sprite = item.Icon;
                itemIconImage.enabled = true;
            }

            // 显示面板
            itemInfoPanel.SetActive(true);

            // 设置位置
            if (slotPosition.HasValue)
            {
                itemInfoPanel.transform.position = slotPosition.Value;
            }
            else
            {
                PositionInfoPanelAtMouse();
            }

            // 设置CanvasGroup
            if (itemInfoCanvasGroup != null)
            {
                itemInfoCanvasGroup.alpha = 1f;
                itemInfoCanvasGroup.interactable = true;
                itemInfoCanvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// 在鼠标位置显示信息面板
        /// </summary>
        private void PositionInfoPanelAtMouse()
        {
            if (itemInfoPanel == null) return;

            Vector3 mousePosition = Input.mousePosition;
            Vector3 panelPosition = mousePosition;

            // 获取面板尺寸
            RectTransform panelRect = itemInfoPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                Vector2 panelSize = panelRect.sizeDelta;
                
                // 确保面板不会超出屏幕边界
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;

                if (mousePosition.x + panelSize.x > screenWidth)
                {
                    panelPosition.x = screenWidth - panelSize.x - 10f;
                }

                if (mousePosition.y + panelSize.y > screenHeight)
                {
                    panelPosition.y = screenHeight - panelSize.y - 10f;
                }

                if (panelPosition.x < 0)
                    panelPosition.x = 10f;

                if (panelPosition.y < 0)
                    panelPosition.y = 10f;
            }

            itemInfoPanel.transform.position = panelPosition;
        }

        /// <summary>
        /// 隐藏物品信息
        /// </summary>
        public void HideItemInfo()
        {
            if (itemInfoPanel != null)
            {
                if (itemInfoCanvasGroup != null)
                {
                    itemInfoCanvasGroup.alpha = 0f;
                    itemInfoCanvasGroup.interactable = false;
                    itemInfoCanvasGroup.blocksRaycasts = false;
                }
            }
        }

        /// <summary>
        /// 带动画显示物品信息
        /// </summary>
        /// <param name="item">物品数据</param>
        /// <param name="duration">动画时长</param>
        public void ShowItemInfoWithAnimation(BaseItemData item, float duration = 0.2f)
        {
            if (itemInfoPanel == null || item == null) return;

            // 设置物品信息
            if (itemNameText != null)
                itemNameText.text = item.ItemName;

            if (itemDescriptionText != null)
                itemDescriptionText.text = item.Description;

            if (itemIconImage != null && item.Icon != null)
            {
                itemIconImage.sprite = item.Icon;
                itemIconImage.enabled = true;
            }

            // 显示面板
            itemInfoPanel.SetActive(true);
            PositionInfoPanelAtMouse();

            // 开始淡入动画
            StartCoroutine(FadeInInfoPanel(duration));
        }

        /// <summary>
        /// 带动画隐藏物品信息
        /// </summary>
        /// <param name="duration">动画时长</param>
        public void HideItemInfoWithAnimation(float duration = 0.15f)
        {
            StartCoroutine(FadeOutInfoPanel(duration));
        }

        /// <summary>
        /// 淡入信息面板
        /// </summary>
        /// <param name="duration">动画时长</param>
        private System.Collections.IEnumerator FadeInInfoPanel(float duration)
        {
            if (itemInfoCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            float startAlpha = itemInfoCanvasGroup.alpha;

            itemInfoCanvasGroup.interactable = true;
            itemInfoCanvasGroup.blocksRaycasts = true;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                itemInfoCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, progress);
                yield return null;
            }

            itemInfoCanvasGroup.alpha = 1f;
        }

        /// <summary>
        /// 淡出信息面板
        /// </summary>
        /// <param name="duration">动画时长</param>
        private System.Collections.IEnumerator FadeOutInfoPanel(float duration)
        {
            if (itemInfoCanvasGroup == null) yield break;

            float elapsedTime = 0f;
            float startAlpha = itemInfoCanvasGroup.alpha;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                itemInfoCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
                yield return null;
            }

            itemInfoCanvasGroup.alpha = 0f;
            itemInfoCanvasGroup.interactable = false;
            itemInfoCanvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 武器装备事件
        /// </summary>
        /// <param name="weapon">武器数据</param>
        private void OnWeaponEquipped(WeaponData weapon)
        {
            RefreshCharacterPanel();
        }

        /// <summary>
        /// 武器卸下事件
        /// </summary>
        /// <param name="weapon">武器数据</param>
        private void OnWeaponUnequipped(WeaponData weapon)
        {
            RefreshCharacterPanel();
        }

        /// <summary>
        /// 获取标签页显示名称
        /// </summary>
        /// <param name="itemType">物品类型</param>
        /// <returns>显示名称</returns>
        private string GetTabDisplayName(ItemType itemType)
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
        /// 销毁时清理事件
        /// </summary>
        private void OnDestroy()
        {
            if (inventoryManager != null)
            {
                inventoryManager.OnInventoryChanged -= RefreshUI;
                if (inventoryManager.EquipmentManager != null)
                {
                    inventoryManager.EquipmentManager.OnWeaponEquipped -= OnWeaponEquipped;
                    inventoryManager.EquipmentManager.OnWeaponUnequipped -= OnWeaponUnequipped;
                }
            }
        }
    }
}