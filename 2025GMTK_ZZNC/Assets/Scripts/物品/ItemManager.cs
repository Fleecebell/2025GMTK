using System.Collections.Generic;
using UnityEngine;
using InventorySystem.Items;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    [Header("物品管理")]
    [SerializeField] private GameObject itemPrefab; // 物品预制体

    // 物品池
    private List<BaseItemData> itemPool = new List<BaseItemData>();
    private Dictionary<string, BaseItemData> itemDict = new Dictionary<string, BaseItemData>();
    
    // 分类物品池（优化查询性能）
    private List<WeaponData> weaponPool = new List<WeaponData>();
    private List<EquipmentData> equipmentPool = new List<EquipmentData>();
    private List<ConsumableData> consumablePool = new List<ConsumableData>();

    // 预制体池
    private Dictionary<string, List<GameObject>> prefabPools = new Dictionary<string, List<GameObject>>();

    // 单例模式
    public static ItemManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeItemPool();
            InitializePrefabPools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeItemPool()
    {
        BaseItemData[] loadedItems = Resources.LoadAll<BaseItemData>("Data/Items");

        foreach (BaseItemData item in loadedItems)
        {
            if (item != null)
            {
                itemPool.Add(item);
                itemDict[item.itemName] = item;
                
                // 分类存入对应池
                if (item is WeaponData weapon)
                {
                    weaponPool.Add(weapon);
                }
                else if (item is EquipmentData equipment)
                {
                    equipmentPool.Add(equipment);
                }
                else if (item is ConsumableData consumable)
                {
                    consumablePool.Add(consumable);
                }

                Debug.Log($"已加载物品: {item.itemName}");
            }
        }

        Debug.Log($"物品池初始化完成，共加载 {itemPool.Count} 个物品");
        Debug.Log($"  武器: {weaponPool.Count} 个");
        Debug.Log($"  神器: {equipmentPool.Count} 个");
        Debug.Log($"  消耗品: {consumablePool.Count} 个");
    }

    private void InitializePrefabPools()
    {
        // 加载所有物品预制体
        prefabPools["Weapon"] = Resources.LoadAll<GameObject>("Items/Weapon").ToList();
        prefabPools["Equipment"] = Resources.LoadAll<GameObject>("Items/Equipment").ToList();
        prefabPools["Consumable"] = Resources.LoadAll<GameObject>("Items/Consumable").ToList();

        Debug.Log($"预制体池初始化完成，共加载：");
        foreach (var kvp in prefabPools)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value.Count} 个预制体");
        }
    }

    public GameObject GetPrefabByName(string itemName)
    {
        foreach (var kvp in prefabPools)
        {
            foreach (var prefab in kvp.Value)
            {
                if (prefab.name == itemName)
                {
                    return prefab;
                }
            }
        }
        return null;
    }

    public GameObject DropItem(BaseItemData itemData, Vector3 dropPosition,bool isGoods=false)
    {
        if (itemData == null)
        {
            Debug.LogError("掉落物品数据为空！");
            return null;
        }

        // 从预制体池中获取对应预制体
        GameObject prefab = GetPrefabByName(itemData.itemName);
        if (prefab == null)
        {
            Debug.LogError($"未找到预制体: {itemData.itemName}");
            return null;
        }

        GameObject droppedItem = Instantiate(prefab, dropPosition, Quaternion.identity);
        Item itemComponent = droppedItem.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.SetItemData(itemData,isGoods);
        }
        else
        {
            Debug.LogError("物品预制体缺少Item组件！");
        }

        Debug.Log($"已掉落物品: {itemData.itemName} 在位置: {dropPosition}");
        return droppedItem;
    }

    public BaseItemData GetItemByID(string itemID)
    {
        itemDict.TryGetValue(itemID, out BaseItemData item);
        return item;
    }

    public BaseItemData GetRandomItem()
    {
        if (itemPool.Count == 0)
        {
            Debug.LogWarning("物品池为空！");
            return null;
        }

        int randomIndex = Random.Range(0, itemPool.Count);
        return itemPool[randomIndex];
    }

    public WeaponData GetRandomWeapon()
    {
        if (weaponPool.Count == 0)
        {
            Debug.LogWarning("武器池为空！");
            return null;
        }

        // 随机选择一个武器
        return weaponPool[Random.Range(0, weaponPool.Count)];
    }

    public EquipmentData GetRandomEquipment()
    {
        if (equipmentPool.Count == 0)
        {
            Debug.LogWarning("装备池为空！");
            return null;
        }


        return equipmentPool[Random.Range(0, equipmentPool.Count)];
    }

    /// <summary>
    /// 随机掉落一个武器
    /// </summary>
    /// <param name="dropPosition">掉落位置</param>
    /// <param name="preferredAttackType">优先的攻击类型</param>
    /// <returns>掉落的武器游戏对象</returns>
    public GameObject DropRandomWeapon(Vector3 dropPosition,bool isGoods=false)
    {
        WeaponData randomWeapon = GetRandomWeapon();
        if (randomWeapon != null)
        {
            return DropItem(randomWeapon, dropPosition,isGoods);
        }
        return null;
    }

    /// <summary>
    /// 随机掉落一个装备
    /// </summary>
    /// <param name="dropPosition">掉落位置</param>
    /// <param name="minQuality">最低品质要求</param>
    /// <returns>掉落的装备游戏对象</returns>
    public GameObject DropRandomEquipment(Vector3 dropPosition,bool isGoods=false)
    {
        EquipmentData randomEquipment = GetRandomEquipment();
        if (randomEquipment != null)
        {
            return DropItem(randomEquipment, dropPosition,isGoods);
        }
        return null;
    }

    public GameObject DropItemByRule(Vector3 dropPosition)
    {
        float randomChance = Random.value; // [0, 1)

        // 10% 概率掉落“蜜酿”（ConsumableData）
        if (randomChance < 0.1f)
        {
            BaseItemData honey = consumablePool.Find(item => item.itemName == "蜜酿");
            if (honey != null)
            {
                return DropItem(honey, dropPosition);
            }
        }

        // 3% 概率掉落任意神器（EquipmentData）
        else if (randomChance < 0.13f) // 0.1 + 0.03
        {
            if (equipmentPool.Count > 0)
            {
                BaseItemData randomEquipment = equipmentPool[Random.Range(0, equipmentPool.Count)];
                return DropItem(randomEquipment, dropPosition);
            }
        }

        Debug.Log("未掉落任何物品");
        return null;
    }
}
    