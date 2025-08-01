using UnityEngine;
using InventorySystem.Managers;
using InventorySystem.Items;
using InventorySystem.Data;

namespace InventorySystem.Testing
{
    /// <summary>
    /// 物品生成器 - 用于快速测试背包系统
    /// 按A获得随机武器，按S获得随机装备，按D获得随机道具
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        [Header("测试物品池")]
        [SerializeField] private WeaponData[] weaponPool;
        [SerializeField] private EquipmentData[] equipmentPool;
        [SerializeField] private ConsumableData[] consumablePool;

        [Header("生成设置")]
        [SerializeField] private int minConsumableQuantity = 1;
        [SerializeField] private int maxConsumableQuantity = 5;
        [SerializeField] private bool showSpawnMessages = true;

        private InventoryManager inventoryManager;

        /// <summary>
        /// 初始化物品生成器
        /// </summary>
        private void Start()
        {
            // 延迟获取背包管理器，确保其已初始化
            Invoke(nameof(InitializeSpawner), 0.1f);
        }

        /// <summary>
        /// 初始化生成器
        /// </summary>
        private void InitializeSpawner()
        {
            inventoryManager = InventoryManager.Instance;
            
            if (inventoryManager == null)
            {
                Debug.LogError("ItemSpawner: 找不到InventoryManager实例");
                enabled = false;
                return;
            }

            // 如果没有设置物品池，尝试自动查找
            if (weaponPool == null || weaponPool.Length == 0)
            {
                AutoFindWeapons();
            }
            
            if (equipmentPool == null || equipmentPool.Length == 0)
            {
                AutoFindEquipments();
            }
            
            if (consumablePool == null || consumablePool.Length == 0)
            {
                AutoFindConsumables();
            }

            if (showSpawnMessages)
            {
                Debug.Log("ItemSpawner初始化完成！");
                Debug.Log("按A键获得随机武器");
                Debug.Log("按S键获得随机装备");
                Debug.Log("按D键获得随机道具");
                Debug.Log($"武器池: {weaponPool?.Length ?? 0}个");
                Debug.Log($"装备池: {equipmentPool?.Length ?? 0}个");
                Debug.Log($"道具池: {consumablePool?.Length ?? 0}个");
            }
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        private void Update()
        {
            if (inventoryManager == null) return;

            // A键 - 生成随机武器
            if (Input.GetKeyDown(KeyCode.A))
            {
                SpawnRandomWeapon();
            }

            // S键 - 生成随机装备
            if (Input.GetKeyDown(KeyCode.S))
            {
                SpawnRandomEquipment();
            }

            // D键 - 生成随机道具
            if (Input.GetKeyDown(KeyCode.D))
            {
                SpawnRandomConsumable();
            }
        }

        /// <summary>
        /// 生成随机武器
        /// </summary>
        [ContextMenu("生成随机武器")]
        public void SpawnRandomWeapon()
        {
            if (weaponPool == null || weaponPool.Length == 0)
            {
                if (showSpawnMessages)
                    Debug.LogWarning("武器池为空，无法生成武器");
                return;
            }

            var randomWeapon = weaponPool[Random.Range(0, weaponPool.Length)];
            if (randomWeapon != null)
            {
                if (showSpawnMessages)
                {
                    Debug.Log($"尝试添加武器: {randomWeapon.ItemName}, 类型: {randomWeapon.ItemType}, 最大堆叠: {randomWeapon.MaxStackSize}");
                    
                    // 检查背包状态
                    var inventory = inventoryManager.Inventory;
                    //Debug.Log($"背包状态: {inventory.UsedSlots}/{inventory.MaxSlots}, 是否已满: {inventory.IsFull}");
                    
                    // 检查是否可以添加
                    bool canAdd = inventory.CanAddItem(randomWeapon, 1);
                    Debug.Log($"是否可以添加: {canAdd}");
                }
                
                int added = inventoryManager.AddItem(randomWeapon, 1);
                
                if (showSpawnMessages)
                {
                    if (added > 0)
                    {
                        Debug.Log($"? 成功获得武器: {randomWeapon.ItemName} (添加了 {added} 个)");
                    }
                    else
                    {
                        Debug.LogWarning($"? 无法添加武器: {randomWeapon.ItemName} (添加了 {added} 个)");
                        
                        // 详细诊断为什么无法添加
                        var inventory = inventoryManager.Inventory;
                        if (inventory.IsFull)
                        {
                            Debug.LogWarning("原因: 背包已满");
                        }
                        else
                        {
                            Debug.LogWarning("原因: 未知错误，请检查物品数据");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成随机装备
        /// </summary>
        [ContextMenu("生成随机装备")]
        public void SpawnRandomEquipment()
        {
            if (equipmentPool == null || equipmentPool.Length == 0)
            {
                if (showSpawnMessages)
                    Debug.LogWarning("装备池为空，无法生成装备");
                return;
            }

            var randomEquipment = equipmentPool[Random.Range(0, equipmentPool.Length)];
            if (randomEquipment != null)
            {
                int added = inventoryManager.AddItem(randomEquipment, 1);
                if (added > 0 && showSpawnMessages)
                {
                    Debug.Log($"获得装备: {randomEquipment.ItemName}");
                }
                else if (showSpawnMessages)
                {
                    Debug.LogWarning($"背包已满，无法添加装备: {randomEquipment.ItemName}");
                }
            }
        }

        /// <summary>
        /// 生成随机道具
        /// </summary>
        [ContextMenu("生成随机道具")]
        public void SpawnRandomConsumable()
        {
            if (consumablePool == null || consumablePool.Length == 0)
            {
                if (showSpawnMessages)
                    Debug.LogWarning("道具池为空，无法生成道具");
                return;
            }

            var randomConsumable = consumablePool[Random.Range(0, consumablePool.Length)];
            if (randomConsumable != null)
            {
                int quantity = Random.Range(minConsumableQuantity, maxConsumableQuantity + 1);
                int added = inventoryManager.AddItem(randomConsumable, quantity);
                if (added > 0 && showSpawnMessages)
                {
                    Debug.Log($"获得道具: {randomConsumable.ItemName} x{added}");
                }
                else if (showSpawnMessages)
                {
                    Debug.LogWarning($"背包已满，无法添加道具: {randomConsumable.ItemName}");
                }
            }
        }

        /// <summary>
        /// 自动查找项目中的武器数据
        /// </summary>
        private void AutoFindWeapons()
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:WeaponData", new[] {"Assets/Resources/Items/Weapons"});
            weaponPool = new WeaponData[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                weaponPool[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponData>(path);
            }
            if (showSpawnMessages && weaponPool.Length > 0)
            {
                Debug.Log($"自动找到 {weaponPool.Length} 个武器数据");
            }
            #endif
        }

        /// <summary>
        /// 自动查找项目中的装备数据
        /// </summary>
        private void AutoFindEquipments()
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:EquipmentData", new[] {"Assets/Resources/Items/Equipment"});
            equipmentPool = new EquipmentData[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                equipmentPool[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<EquipmentData>(path);
            }
            if (showSpawnMessages && equipmentPool.Length > 0)
            {
                Debug.Log($"自动找到 {equipmentPool.Length} 个装备数据");
            }
            #endif
        }

        /// <summary>
        /// 自动查找项目中的道具数据
        /// </summary>
        private void AutoFindConsumables()
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ConsumableData", new[] {"Assets/Resources/Items/Consumables"});
            consumablePool = new ConsumableData[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                consumablePool[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<ConsumableData>(path);
            }
            if (showSpawnMessages && consumablePool.Length > 0)
            {
                Debug.Log($"自动找到 {consumablePool.Length} 个道具数据");
            }
            #endif
        }

        /// <summary>
        /// 生成所有类型的随机物品
        /// </summary>
        [ContextMenu("生成随机物品包")]
        public void SpawnRandomItemPack()
        {
            SpawnRandomWeapon();
            SpawnRandomEquipment();
            SpawnRandomConsumable();
            
            if (showSpawnMessages)
            {
                Debug.Log("生成了一个随机物品包！");
            }
        }

        /// <summary>
        /// 批量生成物品
        /// </summary>
        /// <param name="count">生成数量</param>
        [ContextMenu("批量生成物品")]
        public void SpawnMultipleItems(int count = 5)
        {
            for (int i = 0; i < count; i++)
            {
                int itemType = Random.Range(0, 3);
                switch (itemType)
                {
                    case 0:
                        SpawnRandomWeapon();
                        break;
                    case 1:
                        SpawnRandomEquipment();
                        break;
                    case 2:
                        SpawnRandomConsumable();
                        break;
                }
            }
            
            if (showSpawnMessages)
            {
                Debug.Log($"批量生成了 {count} 个随机物品！");
            }
        }

        /// <summary>
        /// 清空背包并生成新物品
        /// </summary>
        [ContextMenu("重置并生成新物品")]
        public void ResetAndSpawnNew()
        {
            if (inventoryManager != null)
            {
                inventoryManager.ClearInventory();
                SpawnRandomItemPack();
                
                if (showSpawnMessages)
                {
                    Debug.Log("背包已重置并生成新物品！");
                }
            }
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        [ContextMenu("显示帮助")]
        public void ShowHelp()
        {
            Debug.Log("=== ItemSpawner 使用说明 ===");
            Debug.Log("A键: 获得随机武器");
            Debug.Log("S键: 获得随机装备");
            Debug.Log("D键: 获得随机道具");
            Debug.Log("右键点击组件 -> 生成随机物品包: 一次获得所有类型物品");
            Debug.Log("右键点击组件 -> 批量生成物品: 生成多个随机物品");
            Debug.Log("右键点击组件 -> 重置并生成新物品: 清空背包并生成新物品");
        }

        /// <summary>
        /// 获取物品池统计信息
        /// </summary>
        /// <returns>统计信息字符串</returns>
        public string GetPoolStats()
        {
            return $"物品池统计:\n" +
                   $"武器: {weaponPool?.Length ?? 0}\n" +
                   $"装备: {equipmentPool?.Length ?? 0}\n" +
                   $"道具: {consumablePool?.Length ?? 0}";
        }

        /// <summary>
        /// 在Inspector中显示统计信息
        /// </summary>
        private void OnValidate()
        {
            // 这个方法在Inspector值改变时调用，可以用来验证设置
            if (maxConsumableQuantity < minConsumableQuantity)
            {
                maxConsumableQuantity = minConsumableQuantity;
            }
        }

        /// <summary>
        /// 在Scene视图中显示帮助信息
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // 在Scene视图中选中此对象时显示帮助信息
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2, 
                "ItemSpawner\nA-武器 S-装备 D-道具");
            #endif
        }
    }
}