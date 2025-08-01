using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InventorySystem.Core;
using InventorySystem.Items;
using TMPro;

namespace InventorySystem.UI
{
    /// <summary>
    /// 背包槽位UI组件 - 处理单个槽位的显示和交互
    /// 遵循单一职责原则，专门处理槽位UI逻辑
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI组件")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Image backgroundImage;

        [Header("视觉设置")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private Color emptyColor = Color.gray;

        // 私有变量
        private int slotIndex;
        private InventorySlot currentSlot;
        private InventoryUI parentInventoryUI;
        private bool isInitialized = false;

        // 只读属性
        public int SlotIndex => slotIndex;
        public bool IsEmpty => currentSlot == null || currentSlot.IsEmpty;
        public InventorySlot CurrentSlot => currentSlot;

        /// <summary>
        /// 初始化槽位UI
        /// </summary>
        /// <param name="index">槽位索引</param>
        /// <param name="inventoryUI">父级背包UI</param>
        public void Initialize(int index, InventoryUI inventoryUI)
        {
            slotIndex = index;
            parentInventoryUI = inventoryUI;
            Debug.Log("parentInventoryUI:"+parentInventoryUI!=null);
            isInitialized = true;

            // 初始化UI组件
            InitializeComponents();

            // 设置初始状态
            ClearSlot();
        }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        private void InitializeComponents()
        {
            // 如果组件未设置，尝试自动获取
            if (itemIcon == null)
                itemIcon = transform.Find("ItemIcon")?.GetComponent<Image>();

            if (quantityText == null)
                quantityText = transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();
        }

        /// <summary>
        /// 设置槽位数据
        /// </summary>
        /// <param name="slot">背包槽位</param>
        public void SetSlot(InventorySlot slot)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("槽位UI未初始化");
                return;
            }

            currentSlot = slot;

            if (slot == null || slot.IsEmpty)
            {
                ClearSlot();
                return;
            }

            // 更新物品图标
            UpdateItemIcon(slot.ItemData);

            // 更新数量显示
            UpdateQuantityDisplay(slot.Quantity, slot.ItemData.MaxStackSize);

            // 更新背景颜色
            UpdateBackgroundColor(normalColor);
        }

        /// <summary>
        /// 清空槽位显示
        /// </summary>
        public void ClearSlot()
        {
            currentSlot = null;

            // 隐藏物品图标
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.gameObject.SetActive(true);
            }

            // 隐藏数量文本
            if (quantityText != null)
            {
                quantityText.gameObject.SetActive(true);
            }

            // 设置空槽位背景色
            UpdateBackgroundColor(emptyColor);
        }

        /// <summary>
        /// 更新物品图标
        /// </summary>
        /// <param name="itemData">物品数据</param>
        private void UpdateItemIcon(BaseItemData itemData)
        {
            if (itemIcon == null || itemData == null) return;

            itemIcon.sprite = itemData.Icon;
            itemIcon.gameObject.SetActive(itemData.Icon != null);
        }

        /// <summary>
        /// 更新数量显示
        /// </summary>
        /// <param name="quantity">当前数量</param>
        /// <param name="maxStack">最大堆叠数</param>
        private void UpdateQuantityDisplay(int quantity, int maxStack)
        {
            if (quantityText == null) return;

            // 只有堆叠数量大于1时才显示数量
            if (maxStack > 1 && quantity > 1)
            {
                quantityText.text = quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 更新背景颜色
        /// </summary>
        /// <param name="color">颜色</param>
        private void UpdateBackgroundColor(Color color)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
        }



        /// <summary>
        /// 处理鼠标点击事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click");
            if (!isInitialized || parentInventoryUI == null) return;

            bool isRightClick = eventData.button == PointerEventData.InputButton.Right;
            parentInventoryUI.OnSlotClicked(slotIndex, isRightClick);
        }

        /// <summary>
        /// 处理鼠标进入事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Enter");
            if (!IsEmpty)
            {
                // 高亮显示
                UpdateBackgroundColor(highlightColor);

                // 显示物品信息，面板将自动定位到鼠标位置上方
                try
                {
                    // 详细检查每个条件
                    Debug.Log($"parentInventoryUI is null: {parentInventoryUI == null}");
                    Debug.Log($"currentSlot is null: {currentSlot == null}");

                    if (parentInventoryUI != null && currentSlot != null)
                    {
                        Debug.Log($"currentSlot.ItemData is null: {currentSlot.ItemData == null}");
                        Debug.Log("显示");
                        parentInventoryUI.ShowItemInfo(currentSlot.ItemData);
                    }
                    else
                    {
                        Debug.LogWarning("条件不满足 - parentInventoryUI或currentSlot为null");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"OnPointerEnter异常: {ex.Message}\n{ex.StackTrace}");
                }
            }
            else
            {
                Debug.Log("IsEmpty为true，跳过显示逻辑");
            }
        }



        /// <summary>
        /// 处理鼠标离开事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Exit");
            if (!IsEmpty)
            {
                // 恢复正常颜色
                UpdateBackgroundColor(normalColor);
            }
            else
            {
                // 恢复空槽位颜色
                UpdateBackgroundColor(emptyColor);
            }

            // 隐藏物品信息面板
            if (parentInventoryUI != null)
            {
                parentInventoryUI.HideItemInfo();
            }
        }



        /// <summary>
        /// 获取槽位信息的字符串表示
        /// </summary>
        /// <returns>槽位信息</returns>
        public override string ToString()
        {
            if (IsEmpty)
                return $"槽位UI {slotIndex}: 空";

            return $"槽位UI {slotIndex}: {currentSlot.ItemData.ItemName} x{currentSlot.Quantity}";
        }

        /// <summary>
        /// 设置槽位是否可交互
        /// </summary>
        /// <param name="interactable">是否可交互</param>
        public void SetInteractable(bool interactable)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.interactable = interactable;
            canvasGroup.alpha = interactable ? 1f : 0.5f;
        }

        /// <summary>
        /// 播放槽位动画效果
        /// </summary>
        /// <param name="animationType">动画类型</param>
        public void PlayAnimation(string animationType)
        {
            // 这里可以添加各种动画效果
            switch (animationType)
            {
                case "ItemAdded":
                    StartCoroutine(ScaleAnimation(1.2f, 0.2f));
                    break;
                case "ItemRemoved":
                    StartCoroutine(FadeAnimation(0.5f, 0.3f));
                    break;
                case "ItemUsed":
                    StartCoroutine(FlashAnimation(Color.green, 0.5f));
                    break;
            }
        }

        /// <summary>
        /// 缩放动画协程
        /// </summary>
        /// <param name="targetScale">目标缩放</param>
        /// <param name="duration">持续时间</param>
        private System.Collections.IEnumerator ScaleAnimation(float targetScale, float duration)
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScaleVector = originalScale * targetScale;

            float elapsed = 0f;
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (duration / 2);
                transform.localScale = Vector3.Lerp(originalScale, targetScaleVector, progress);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (duration / 2);
                transform.localScale = Vector3.Lerp(targetScaleVector, originalScale, progress);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// 淡入淡出动画协程
        /// </summary>
        /// <param name="targetAlpha">目标透明度</param>
        /// <param name="duration">持续时间</param>
        private System.Collections.IEnumerator FadeAnimation(float targetAlpha, float duration)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            float originalAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, targetAlpha, progress);
                yield return null;
            }

            canvasGroup.alpha = originalAlpha;
        }

        /// <summary>
        /// 闪烁动画协程
        /// </summary>
        /// <param name="flashColor">闪烁颜色</param>
        /// <param name="duration">持续时间</param>
        private System.Collections.IEnumerator FlashAnimation(Color flashColor, float duration)
        {
            Color originalColor = backgroundImage.color;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.PingPong(elapsed * 4, 1);
                backgroundImage.color = Color.Lerp(originalColor, flashColor, progress);
                yield return null;
            }

            backgroundImage.color = originalColor;
        }
        /// <summary>
        /// 获取槽位位置（公共方法）
        /// </summary>
        /// <returns>槽位的世界坐标位置</returns>
        public Vector3 GetSlotPosition()
        {
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null)
            {
                // 获取RectTransform的世界坐标
                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);

                // 返回中心点位置
                return (worldCorners[0] + worldCorners[2]) / 2f;
            }

            return transform.position;
        }
    }
}