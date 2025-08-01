using UnityEngine;
using UnityEngine.UI;
using InventorySystem.UI;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 简单位置测试 - 验证物品信息面板定位功能
    /// </summary>
    public class SimplePositionTest : MonoBehaviour
    {
        [Header("测试组件")]
        [SerializeField] private GameObject testInfoPanel;
        [SerializeField] private Text testText;

        private InventoryUI inventoryUI;
        private Canvas canvas;

        private void Start()
        {
            inventoryUI = FindObjectOfType<InventoryUI>();
            canvas = GetComponentInParent<Canvas>();
            
            CreateTestPanel();
        }

        private void Update()
        {
            // F8键 - 简单位置测试
            if (Input.GetKeyDown(KeyCode.F8))
            {
                TestSimplePositioning();
            }
        }

        /// <summary>
        /// 创建测试面板
        /// </summary>
        private void CreateTestPanel()
        {
            if (canvas == null) return;

            // 创建测试面板
            testInfoPanel = new GameObject("TestInfoPanel");
            testInfoPanel.transform.SetParent(canvas.transform);

            // 设置RectTransform
            RectTransform rect = testInfoPanel.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 100);

            // 添加背景
            Image bg = testInfoPanel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.8f);

            // 添加文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(testInfoPanel.transform);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            testText = textObj.AddComponent<Text>();
            testText.text = "测试面板";
            testText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            testText.alignment = TextAnchor.MiddleCenter;
            testText.color = Color.white;

            testInfoPanel.SetActive(false);
        }

        /// <summary>
        /// 简单位置测试
        /// </summary>
        [ContextMenu("简单位置测试")]
        public void TestSimplePositioning()
        {
            // 查找第一个非空的槽位
            InventorySlotUI[] slots = FindObjectsOfType<InventorySlotUI>();
            InventorySlotUI targetSlot = null;

            foreach (var slot in slots)
            {
                if (slot != null && !slot.IsEmpty)
                {
                    targetSlot = slot;
                    break;
                }
            }

            if (targetSlot == null)
            {
                Debug.LogWarning("没有找到包含物品的槽位");
                return;
            }

            Debug.Log($"测试槽位: {targetSlot.SlotIndex}");

            // 获取槽位的RectTransform
            RectTransform slotRect = targetSlot.transform as RectTransform;
            if (slotRect == null) return;

            // 显示测试面板
            testInfoPanel.SetActive(true);
            RectTransform panelRect = testInfoPanel.GetComponent<RectTransform>();

            // 方法1：直接使用anchoredPosition
            Vector2 slotPosition = slotRect.anchoredPosition;
            Vector2 panelPosition = slotPosition + new Vector2(220f, 0f); // 右侧偏移
            
            panelRect.anchoredPosition = panelPosition;
            testText.text = $"方法1\n槽位: {slotPosition}\n面板: {panelPosition}";

            Debug.Log($"槽位anchoredPosition: {slotPosition}");
            Debug.Log($"面板设置位置: {panelPosition}");

            // 3秒后隐藏
            Invoke(nameof(HideTestPanel), 3f);
        }

        /// <summary>
        /// 测试方法2：使用世界坐标转换
        /// </summary>
        [ContextMenu("测试世界坐标转换")]
        public void TestWorldCoordinateConversion()
        {
            // 查找第一个非空的槽位
            InventorySlotUI[] slots = FindObjectsOfType<InventorySlotUI>();
            InventorySlotUI targetSlot = null;

            foreach (var slot in slots)
            {
                if (slot != null && !slot.IsEmpty)
                {
                    targetSlot = slot;
                    break;
                }
            }

            if (targetSlot == null)
            {
                Debug.LogWarning("没有找到包含物品的槽位");
                return;
            }

            // 显示测试面板
            testInfoPanel.SetActive(true);
            RectTransform panelRect = testInfoPanel.GetComponent<RectTransform>();

            // 方法2：使用世界坐标转换
            Vector3 worldPos = targetSlot.GetSlotPosition();
            
            // 转换为Canvas本地坐标
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPos),
                canvas.worldCamera,
                out localPos);

            Vector2 panelPosition = localPos + new Vector2(220f, 0f);
            panelRect.anchoredPosition = panelPosition;
            
            testText.text = $"方法2\n世界坐标: {worldPos}\n本地坐标: {localPos}\n面板: {panelPosition}";

            Debug.Log($"世界坐标: {worldPos}");
            Debug.Log($"本地坐标: {localPos}");
            Debug.Log($"面板位置: {panelPosition}");

            // 3秒后隐藏
            Invoke(nameof(HideTestPanel), 3f);
        }

        /// <summary>
        /// 隐藏测试面板
        /// </summary>
        private void HideTestPanel()
        {
            if (testInfoPanel != null)
            {
                testInfoPanel.SetActive(false);
            }
        }

        /// <summary>
        /// 显示帮助
        /// </summary>
        [ContextMenu("显示帮助")]
        public void ShowHelp()
        {
            Debug.Log("=== SimplePositionTest 使用说明 ===");
            Debug.Log("F8键: 简单位置测试（方法1）");
            Debug.Log("右键点击组件 -> 测试世界坐标转换: 测试方法2");
            Debug.Log("需要先添加一些物品到背包中进行测试");
        }
    }
}