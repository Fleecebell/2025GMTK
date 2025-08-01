using UnityEngine;
using InventorySystem.UI;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 物品信息面板位置测试器
    /// 用于测试物品详情面板是否正确定位到槽位位置
    /// </summary>
    public class ItemInfoPositionTester : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private bool enableDebugVisualization = true;
        [SerializeField] private Color debugLineColor = Color.red;
        [SerializeField] private float debugLineDuration = 2f;

        private InventoryUI inventoryUI;
        private InventorySlotUI[] slotUIs;

        /// <summary>
        /// 初始化测试器
        /// </summary>
        private void Start()
        {
            // 查找InventoryUI组件
            inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI == null)
            {
                Debug.LogError("ItemInfoPositionTester: 找不到InventoryUI组件");
                return;
            }

            // 查找所有InventorySlotUI组件
            slotUIs = FindObjectsOfType<InventorySlotUI>();
            
            Debug.Log($"ItemInfoPositionTester: 找到 {slotUIs.Length} 个槽位UI组件");
        }

        /// <summary>
        /// 更新测试
        /// </summary>
        private void Update()
        {
            // F4键 - 测试物品信息面板定位
            if (Input.GetKeyDown(KeyCode.F4))
            {
                TestItemInfoPositioning();
            }

            // F5键 - 切换调试可视化
            if (Input.GetKeyDown(KeyCode.F5))
            {
                enableDebugVisualization = !enableDebugVisualization;
                Debug.Log($"调试可视化: {(enableDebugVisualization ? "开启" : "关闭")}");
            }
        }

        /// <summary>
        /// 测试物品信息面板定位
        /// </summary>
        [ContextMenu("测试物品信息面板定位")]
        public void TestItemInfoPositioning()
        {
            if (inventoryUI == null || slotUIs == null)
            {
                Debug.LogError("测试组件未正确初始化");
                return;
            }

            Debug.Log("=== 开始测试物品信息面板定位 ===");

            int testedSlots = 0;
            
            foreach (var slotUI in slotUIs)
            {
                if (slotUI != null && !slotUI.IsEmpty)
                {
                    TestSlotPositioning(slotUI);
                    testedSlots++;
                }
            }

            if (testedSlots == 0)
            {
                Debug.LogWarning("没有找到包含物品的槽位进行测试");
                Debug.Log("请先使用ItemSpawner添加一些物品到背包中");
            }
            else
            {
                Debug.Log($"完成测试，共测试了 {testedSlots} 个槽位");
            }
        }

        /// <summary>
        /// 测试单个槽位的定位
        /// </summary>
        /// <param name="slotUI">槽位UI</param>
        private void TestSlotPositioning(InventorySlotUI slotUI)
        {
            if (slotUI == null || slotUI.CurrentSlot == null)
                return;

            // 获取槽位位置
            Vector3 slotPosition = GetSlotWorldPosition(slotUI);
            
            // 模拟显示物品信息
            inventoryUI.ShowItemInfo(slotUI.CurrentSlot.ItemData, slotPosition);
            
            Debug.Log($"测试槽位 {slotUI.SlotIndex}: {slotUI.CurrentSlot.ItemData.ItemName}");
            Debug.Log($"槽位位置: {slotPosition}");

            // 绘制调试线条
            if (enableDebugVisualization)
            {
                DrawDebugVisualization(slotPosition);
            }

            // 延迟隐藏面板
            Invoke(nameof(HideItemInfo), debugLineDuration);
        }

        /// <summary>
        /// 获取槽位的世界坐标位置
        /// </summary>
        /// <param name="slotUI">槽位UI</param>
        /// <returns>世界坐标位置</returns>
        private Vector3 GetSlotWorldPosition(InventorySlotUI slotUI)
        {
            RectTransform rectTransform = slotUI.transform as RectTransform;
            if (rectTransform != null)
            {
                Vector3[] worldCorners = new Vector3[4];
                rectTransform.GetWorldCorners(worldCorners);
                return (worldCorners[0] + worldCorners[2]) / 2f;
            }
            
            return slotUI.transform.position;
        }

        /// <summary>
        /// 绘制调试可视化
        /// </summary>
        /// <param name="slotPosition">槽位位置</param>
        private void DrawDebugVisualization(Vector3 slotPosition)
        {
            // 在Scene视图中绘制调试信息
            Debug.DrawRay(slotPosition, Vector3.up * 50f, debugLineColor, debugLineDuration);
            Debug.DrawRay(slotPosition, Vector3.right * 50f, debugLineColor, debugLineDuration);
            Debug.DrawRay(slotPosition, Vector3.down * 50f, debugLineColor, debugLineDuration);
            Debug.DrawRay(slotPosition, Vector3.left * 50f, debugLineColor, debugLineDuration);
        }

        /// <summary>
        /// 隐藏物品信息面板
        /// </summary>
        private void HideItemInfo()
        {
            if (inventoryUI != null)
            {
                inventoryUI.HideItemInfo();
            }
        }

        /// <summary>
        /// 测试边界情况
        /// </summary>
        [ContextMenu("测试边界情况")]
        public void TestBoundaryConditions()
        {
            if (inventoryUI == null)
            {
                Debug.LogError("InventoryUI未找到");
                return;
            }

            Debug.Log("=== 测试边界情况 ===");

            // 获取Canvas尺寸
            Canvas canvas = inventoryUI.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("找不到Canvas组件");
                return;
            }

            RectTransform canvasRect = canvas.transform as RectTransform;
            Vector2 canvasSize = canvasRect.sizeDelta;

            // 测试四个角落的位置
            Vector3[] testPositions = {
                new Vector3(-canvasSize.x/2 + 50, canvasSize.y/2 - 50, 0),    // 左上角
                new Vector3(canvasSize.x/2 - 50, canvasSize.y/2 - 50, 0),     // 右上角
                new Vector3(-canvasSize.x/2 + 50, -canvasSize.y/2 + 50, 0),   // 左下角
                new Vector3(canvasSize.x/2 - 50, -canvasSize.y/2 + 50, 0)     // 右下角
            };

            string[] positionNames = { "左上角", "右上角", "左下角", "右下角" };

            for (int i = 0; i < testPositions.Length; i++)
            {
                Debug.Log($"测试 {positionNames[i]} 位置: {testPositions[i]}");
                
                // 创建一个测试物品数据（如果有的话）
                if (slotUIs != null && slotUIs.Length > 0)
                {
                    var testSlot = System.Array.Find(slotUIs, s => s != null && !s.IsEmpty);
                    if (testSlot != null)
                    {
                        inventoryUI.ShowItemInfo(testSlot.CurrentSlot.ItemData, testPositions[i]);
                        
                        // 等待一段时间后隐藏
                        Invoke(nameof(HideItemInfo), 1f + i * 0.5f);
                    }
                }
            }
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        [ContextMenu("显示帮助")]
        public void ShowHelp()
        {
            Debug.Log("=== ItemInfoPositionTester 使用说明 ===");
            Debug.Log("F4键: 测试物品信息面板定位");
            Debug.Log("F5键: 切换调试可视化");
            Debug.Log("右键点击组件 -> 测试物品信息面板定位: 手动测试");
            Debug.Log("右键点击组件 -> 测试边界情况: 测试面板在屏幕边缘的表现");
            Debug.Log("注意: 需要先添加物品到背包中才能进行测试");
        }

        /// <summary>
        /// 在Scene视图中显示调试信息
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!enableDebugVisualization || slotUIs == null)
                return;

            Gizmos.color = debugLineColor;
            
            foreach (var slotUI in slotUIs)
            {
                if (slotUI != null && !slotUI.IsEmpty)
                {
                    Vector3 slotPosition = GetSlotWorldPosition(slotUI);
                    
                    // 绘制槽位位置标记
                    Gizmos.DrawWireCube(slotPosition, Vector3.one * 20f);
                    
                    // 绘制槽位索引
                    #if UNITY_EDITOR
                    UnityEditor.Handles.Label(slotPosition + Vector3.up * 30f, 
                        $"Slot {slotUI.SlotIndex}");
                    #endif
                }
            }
        }
    }
}