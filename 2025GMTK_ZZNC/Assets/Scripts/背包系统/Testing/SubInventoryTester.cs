using UnityEngine;
using InventorySystem.Managers;
using InventorySystem.Items;
using InventorySystem.Data;
using InventorySystem.UI;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 子背包系统测试器 - 测试三个子背包的功能
    /// </summary>
    public class SubInventoryTester : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private bool enableAutoTest = false;
        [SerializeField] private float testInterval = 2f;

        [Header("测试物品")]
        [SerializeField] private WeaponData testWeapon;
        [SerializeField] private EquipmentData testEquipment;
        [SerializeField] private ConsumableData testConsumable;

        private InventoryManager inventoryManager;
        private float lastTestTime;

        private void Start()
        {
            Invoke(nameof(Initialize), 0.1f);
        }

        private void Initialize()
        {
            inventoryManager = InventoryManager.Instance;
            
            if (inventoryManager != null)
            {
                Debug.Log("SubInventoryTester 已初始化");
                
                // 订阅背包事件
                inventoryManager.OnItemAdded += OnItemAdded;
                inventoryManager.OnItemRemoved += OnItemRemoved;
                inventoryManager.OnInventoryChanged += OnInventoryChanged;
            }
        }

        private void Update()
        {
            // 自动测试
            if (enableAutoTest && Time.time - lastTestTime > testInterval)
            {
                RunAutoTest();
                lastTestTime = Time.time;
            }

            // 手动测试快捷键
            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestAddWeapons();
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                TestAddEquipments();
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                TestAddConsumables();
            }
            
            if (Input.GetKeyDown(KeyCode.F4))
            {
                TestSubInventoryExpansion();
            }
            
            if (Input.GetKeyDown(KeyCode.F5))
            {
                TestScrollFunctionality();
            }
        }

        /// <summary>
        /// 运行自动测试
        /// </summary>
        private void RunAutoTest()
        {
            Debug.Log("=== 运行自动测试 ===");
            
            // 随机添加不同类型的物品
            int randomType = Random.Range(0, 3);
            switch (randomType)
            {
                case 0:
                    TestAddWeapons();
                    break;
                case 1:
                    TestAddEquipments();
                    break;
                case 2:
                    TestAddConsumables();
                    break;
            }
        }

        /// <summary>
        /// 测试添加武器
        /// </summary>
        [ContextMenu("测试添加武器")]
        public void TestAddWeapons()
        {
            if (inventoryManager == null) return;

            Debug.Log("=== 测试添加武器 ===");

            // 查找武器数据
            #if UNITY_EDITOR
            string[] weaponGuids = UnityEditor.AssetDatabase.FindAssets("t:WeaponData");
            if (weaponGuids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(weaponGuids[0]);
                WeaponData weapon = UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponData>(path);
                
                if (weapon != null)
                {
                    int added = inventoryManager.AddItem(weapon, 1);
                    Debug.Log($"添加武器: {weapon.ItemName}, 实际添加: {added}");
                }
            }
            #endif
        }

        /// <summary>
        /// 测试添加装备
        /// </summary>
        [ContextMenu("测试添加装备")]
        public void TestAddEquipments()
        {
            if (inventoryManager == null) return;

            Debug.Log("=== 测试添加装备 ===");

            // 查找装备数据
            #if UNITY_EDITOR
            string[] equipmentGuids = UnityEditor.AssetDatabase.FindAssets("t:EquipmentData");
            if (equipmentGuids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(equipmentGuids[0]);
                EquipmentData equipment = UnityEditor.AssetDatabase.LoadAssetAtPath<EquipmentData>(path);
                
                if (equipment != null)
                {
                    int added = inventoryManager.AddItem(equipment, 1);
                    Debug.Log($"添加装备: {equipment.ItemName}, 实际添加: {added}");
                }
            }
            #endif
        }

        /// <summary>
        /// 测试添加消耗品
        /// </summary>
        [ContextMenu("测试添加消耗品")]
        public void TestAddConsumables()
        {
            if (inventoryManager == null) return;

            Debug.Log("=== 测试添加消耗品 ===");

            // 查找消耗品数据
            #if UNITY_EDITOR
            string[] consumableGuids = UnityEditor.AssetDatabase.FindAssets("t:ConsumableData");
            if (consumableGuids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(consumableGuids[0]);
                ConsumableData consumable = UnityEditor.AssetDatabase.LoadAssetAtPath<ConsumableData>(path);
                
                if (consumable != null)
                {
                    int added = inventoryManager.AddItem(consumable, Random.Range(1, 5));
                    Debug.Log($"添加消耗品: {consumable.ItemName}, 实际添加: {added}");
                }
            }
            #endif
        }

        /// <summary>
        /// 测试子背包扩充功能
        /// </summary>
        [ContextMenu("测试子背包扩充")]
        public void TestSubInventoryExpansion()
        {
            if (inventoryManager == null) return;

            Debug.Log("=== 测试子背包扩充 ===");

            // 获取当前各子背包的格子数
            var weaponSlots = inventoryManager.GetAllSlotsByType(ItemType.Weapon);
            var equipmentSlots = inventoryManager.GetAllSlotsByType(ItemType.Equipment);
            var consumableSlots = inventoryManager.GetAllSlotsByType(ItemType.Consumable);

            Debug.Log($"当前格子数 - 武器: {weaponSlots.Count}, 装备: {equipmentSlots.Count}, 消耗品: {consumableSlots.Count}");

            // 添加大量物品来触发扩充
            #if UNITY_EDITOR
            string[] allItemGuids = UnityEditor.AssetDatabase.FindAssets("t:BaseItemData");
            
            for (int i = 0; i < 25; i++) // 添加25个物品来测试扩充
            {
                if (i < allItemGuids.Length)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(allItemGuids[i]);
                    BaseItemData item = UnityEditor.AssetDatabase.LoadAssetAtPath<BaseItemData>(path);
                    
                    if (item != null)
                    {
                        int added = inventoryManager.AddItem(item, 1);
                        Debug.Log($"添加物品 {i + 1}: {item.ItemName}, 实际添加: {added}");
                    }
                }
            }
            #endif

            // 再次检查格子数
            weaponSlots = inventoryManager.GetAllSlotsByType(ItemType.Weapon);
            equipmentSlots = inventoryManager.GetAllSlotsByType(ItemType.Equipment);
            consumableSlots = inventoryManager.GetAllSlotsByType(ItemType.Consumable);

            Debug.Log($"扩充后格子数 - 武器: {weaponSlots.Count}, 装备: {equipmentSlots.Count}, 消耗品: {consumableSlots.Count}");
        }

        /// <summary>
        /// 测试滑动功能
        /// </summary>
        [ContextMenu("测试滑动功能")]
        public void TestScrollFunctionality()
        {
            Debug.Log("=== 测试滑动功能 ===");

            // 查找子背包UI组件
            var subInventories = FindObjectsOfType<SubInventoryUI>();
            
            foreach (var subInventory in subInventories)
            {
                Debug.Log($"测试 {subInventory.ItemType} 子背包滑动功能");
                
                // 测试滚动到顶部
                subInventory.ScrollToTop();
                Debug.Log($"{subInventory.ItemType} 子背包滚动到顶部");
                
                // 测试滚动到底部
                subInventory.ScrollToBottom();
                Debug.Log($"{subInventory.ItemType} 子背包滚动到底部");
                
                // 测试滚动到指定格子
                subInventory.ScrollToSlot(5);
                Debug.Log($"{subInventory.ItemType} 子背包滚动到第5个格子");
            }
        }

        /// <summary>
        /// 测试子背包统计信息
        /// </summary>
        [ContextMenu("测试子背包统计")]
        public void TestSubInventoryStats()
        {
            if (inventoryManager == null) return;

            Debug.Log("=== 测试子背包统计 ===");

            // 获取各子背包的统计信息
            string weaponStats = inventoryManager.GetSubInventoryStats(ItemType.Weapon);
            string equipmentStats = inventoryManager.GetSubInventoryStats(ItemType.Equipment);
            string consumableStats = inventoryManager.GetSubInventoryStats(ItemType.Consumable);

            Debug.Log($"武器背包: {weaponStats}");
            Debug.Log($"装备背包: {equipmentStats}");
            Debug.Log($"消耗品背包: {consumableStats}");

            // 获取总体统计
            string totalStats = inventoryManager.GetInventoryStats();
            Debug.Log($"总体统计: {totalStats}");
        }

        /// <summary>
        /// 清空所有子背包
        /// </summary>
        [ContextMenu("清空所有子背包")]
        public void ClearAllSubInventories()
        {
            if (inventoryManager == null) return;

            Debug.Log("=== 清空所有子背包 ===");

            inventoryManager.ClearInventory();
            Debug.Log("所有子背包已清空");
        }

        /// <summary>
        /// 物品添加事件处理
        /// </summary>
        private void OnItemAdded(BaseItemData item, int quantity)
        {
            Debug.Log($"[事件] 物品已添加: {item.ItemName} x{quantity} (类型: {item.ItemType})");
        }

        /// <summary>
        /// 物品移除事件处理
        /// </summary>
        private void OnItemRemoved(BaseItemData item, int quantity)
        {
            Debug.Log($"[事件] 物品已移除: {item.ItemName} x{quantity} (类型: {item.ItemType})");
        }

        /// <summary>
        /// 背包变化事件处理
        /// </summary>
        private void OnInventoryChanged()
        {
            Debug.Log("[事件] 背包内容已变化");
        }

        private void OnDestroy()
        {
            if (inventoryManager != null)
            {
                inventoryManager.OnItemAdded -= OnItemAdded;
                inventoryManager.OnItemRemoved -= OnItemRemoved;
                inventoryManager.OnInventoryChanged -= OnInventoryChanged;
            }
        }
    }
} 