using System.Collections.Generic;
using UnityEngine;
using InventorySystem.Items;

public class ItemManager : MonoBehaviour
{
    [Header("物品管理")]
    [SerializeField] private GameObject itemPrefab; // 物品预制体

    // 物品池
    private List<BaseItemData> itemPool = new List<BaseItemData>();
    private Dictionary<string, BaseItemData> itemDict = new Dictionary<string, BaseItemData>();

    // 单例模式
    public static ItemManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeItemPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化物品池：从Assets/Resources/Data/Items文件夹中加载所有类型为BaseItemData的对象
    /// </summary>
    private void InitializeItemPool()
    {
        // 从Resources/Data/Items文件夹加载所有BaseItemData类型的资源（包括子文件夹）
        BaseItemData[] loadedItems = Resources.LoadAll<BaseItemData>("Data/Items");

        foreach (BaseItemData item in loadedItems)
        {
            if (item != null)
            {
                itemPool.Add(item);
                itemDict[item.itemName] = item; // 使用ID作为键方便查找
                Debug.Log($"已加载物品: {item.ItemName} ");
            }
        }

        Debug.Log($"物品池初始化完成，共加载 {itemPool.Count} 个物品");
    }

    /// <summary>
    /// 掉落道具：在特定位置掉落指定物品
    /// </summary>
    /// <param name="itemData">要掉落的物品数据</param>
    /// <param name="dropPosition">掉落位置</param>
    /// <returns>创建的物品游戏对象</returns>
    public GameObject DropItem(BaseItemData itemData, Vector3 dropPosition)
    {
        if (itemData == null)
        {
            Debug.LogError("掉落物品数据为空！");
            return null;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("物品预制体未设置！");
            return null;
        }

        // 在指定位置创建物品实例
        GameObject droppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);

        // 获取Item组件并设置物品数据
        Item itemComponent = droppedItem.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.SetItemData(itemData);
        }
        else
        {
            Debug.LogError("物品预制体缺少Item组件！");
        }

        Debug.Log($"已掉落物品: {itemData.ItemName} 在位置: {dropPosition}");
        return droppedItem;
    }

    /// <summary>
    /// 根据物品ID获取物品数据
    /// </summary>
    /// <param name="itemID">物品ID</param>
    /// <returns>物品数据</returns>
    public BaseItemData GetItemByID(string itemID)
    {
        itemDict.TryGetValue(itemID, out BaseItemData item);
        return item;
    }

    /// <summary>
    /// 获取随机物品
    /// </summary>
    /// <returns>随机物品数据</returns>
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
    
}