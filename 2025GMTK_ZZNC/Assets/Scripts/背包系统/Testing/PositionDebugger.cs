using UnityEngine;
using UnityEngine.UI;
using InventorySystem.UI;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 位置调试器 - 用于调试UI坐标转换问题
    /// </summary>
    public class PositionDebugger : MonoBehaviour
    {
        [Header("调试设置")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private GameObject testPanel;

        private InventoryUI inventoryUI;

        private void Start()
        {
            inventoryUI = FindObjectOfType<InventoryUI>();
            
            if (inventoryUI == null)
            {
                Debug.LogError("找不到InventoryUI组件");
            }
        }

        private void Update()
        {
            // F6键 - 测试鼠标位置转换
            if (Input.GetKeyDown(KeyCode.F6))
            {
                TestMousePositionConversion();
            }

            // F7键 - 测试槽位位置获取
            if (Input.GetKeyDown(KeyCode.F7))
            {
                TestSlotPositions();
            }
        }

        /// <summary>
        /// 测试鼠标位置转换
        /// </summary>
        [ContextMenu("测试鼠标位置转换")]
        public void TestMousePositionConversion()
        {
            if (!enableDebugLogs) return;

            Vector3 mousePosition = Input.mousePosition;
            Debug.Log($"=== 鼠标位置转换测试 ===");
            Debug.Log($"鼠标屏幕坐标: {mousePosition}");

            // 获取Canvas
            Canvas canvas = inventoryUI?.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("找不到Canvas");
                return;
            }

            Debug.Log($"Canvas渲染模式: {canvas.renderMode}");

            // 转换为Canvas本地坐标
            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePosition,
                canvas.worldCamera,
                out localPosition);

            Debug.Log($"Canvas本地坐标: {localPosition}");

            // 如果有测试面板，移动到该位置
            if (testPanel != null)
            {
                RectTransform testRect = testPanel.GetComponent<RectTransform>();
                if (testRect != null)
                {
                    testRect.anchoredPosition = localPosition;
                    testPanel.SetActive(true);
                    Debug.Log($"测试面板已移动到: {localPosition}");
                }
            }
        }

        /// <summary>
        /// 测试槽位位置获取
        /// </summary>
        [ContextMenu("测试槽位位置获取")]
        public void TestSlotPositions()
        {
            if (!enableDebugLogs) return;

            Debug.Log($"=== 槽位位置获取测试 ===");

            // 查找所有槽位UI
            InventorySlotUI[] slotUIs = FindObjectsOfType<InventorySlotUI>();
            
            Debug.Log($"找到 {slotUIs.Length} 个槽位UI");

            for (int i = 0; i < Mathf.Min(5, slotUIs.Length); i++) // 只测试前5个
            {
                InventorySlotUI slotUI = slotUIs[i];
                if (slotUI != null)
                {
                    Vector3 worldPos = slotUI.GetSlotPosition();
                    Vector3 transformPos = slotUI.transform.position;
                    
                    Debug.Log($"槽位 {i}:");
                    Debug.Log($"  GetSlotPosition(): {worldPos}");
                    Debug.Log($"  transform.position: {transformPos}");
                    
                    // 测试RectTransform坐标
                    RectTransform rectTransform = slotUI.transform as RectTransform;
                    if (rectTransform != null)
                    {
                        Vector3[] worldCorners = new Vector3[4];
                        rectTransform.GetWorldCorners(worldCorners);
                        Vector3 center = (worldCorners[0] + worldCorners[2]) / 2f;
                        
                        Debug.Log($"  RectTransform中心: {center}");
                        Debug.Log($"  anchoredPosition: {rectTransform.anchoredPosition}");
                    }
                }
            }
        }

        /// <summary>
        /// 创建测试面板
        /// </summary>
        [ContextMenu("创建测试面板")]
        public void CreateTestPanel()
        {
            if (testPanel != null)
            {
                DestroyImmediate(testPanel);
            }

            // 找到Canvas
            Canvas canvas = inventoryUI?.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("找不到Canvas");
                return;
            }

            // 创建测试面板
            testPanel = new GameObject("TestPanel");
            testPanel.transform.SetParent(canvas.transform);

            // 添加RectTransform
            RectTransform rectTransform = testPanel.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);

            // 添加Image组件
            Image image = testPanel.AddComponent<Image>();
            image.color = Color.red;

            // 添加Text组件
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(testPanel.transform);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObj.AddComponent<Text>();
            text.text = "TEST";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            testPanel.SetActive(false);
            Debug.Log("测试面板已创建");
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        [ContextMenu("显示帮助")]
        public void ShowHelp()
        {
            Debug.Log("=== PositionDebugger 使用说明 ===");
            Debug.Log("F6键: 测试鼠标位置转换");
            Debug.Log("F7键: 测试槽位位置获取");
            Debug.Log("右键点击组件 -> 创建测试面板: 创建红色测试面板");
            Debug.Log("右键点击组件 -> 测试鼠标位置转换: 将测试面板移动到鼠标位置");
        }
    }
}