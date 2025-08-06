using UnityEngine;
using InventorySystem.Items;

public class TreasureChest : MonoBehaviour
{
    [Header("宝箱设置")]
    [SerializeField] private bool 是否已开启 = false; // 宝箱是否已经开启
    private bool canOpen = false;

    [Header("提示UI")]
    [SerializeField] private GameObject 提示UI; // 提示UI的预制体

    [Header("物品掉落")]
    [SerializeField] private Vector3 掉落位置偏移 = new Vector3(0, 1, 0); // 物品掉落位置的偏移量

    private Animator 宝箱动画器; // 宝箱的动画器

    private void Start()
    {
        // 获取宝箱的动画器
        宝箱动画器 = this.GetComponent<Animator>();
        if (宝箱动画器 == null)
        {
            Debug.LogError("宝箱模型上没有找到Animator组件！");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return; // 只在玩家进入触发器时触发
        canOpen = true;
        显示提示();
        Debug.Log("玩家进入宝箱触发器范围，可以开启宝箱！");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return; // 只在玩家离开触发器时触发
        canOpen = false;
        隐藏提示();
        Debug.Log("玩家离开宝箱触发器范围，无法开启宝箱！");
    }

    private void Update()
    {
        if (canOpen && Input.GetKeyDown(KeyCode.E) && !是否已开启)
        {
            Debug.Log("玩家按下E键，尝试开启宝箱！");
            开启宝箱();
        }
    }

    private void 显示提示()
    {
        if (提示UI != null)
        {
            提示UI.SetActive(true);
            Debug.Log("提示UI已显示！");
        }
        else
        {
            Debug.LogError("提示UI未设置，无法显示！");
        }
    }

    private void 隐藏提示()
    {
        if (提示UI != null)
        {
            提示UI.SetActive(false);
            Debug.Log("提示UI已隐藏！");
        }
    }

    private void 开启宝箱()
    {
        Debug.Log("开始开启宝箱...");

        // 播放宝箱开启的动画
        if (宝箱动画器 != null)
        {
            宝箱动画器.SetTrigger("Open"); // 触发动画器的Open动作
            Debug.Log("宝箱开启动画已播放！");
        }
        else
        {
            Debug.LogError("宝箱动画器未找到，无法播放开启动画！");
        }

        // 掉落物品
        Debug.Log("开始掉落物品...");
        // 掉落一个随机装备
        Vector3 装备掉落位置 = transform.position + 掉落位置偏移;
        ItemManager.Instance.DropRandomEquipment(装备掉落位置);
        Debug.Log("随机装备已掉落！");

        // 掉落一个固定消耗品“记忆碎片”
        Vector3 消耗品掉落位置 = transform.position + new Vector3(掉落位置偏移.x, 掉落位置偏移.y, 掉落位置偏移.z + 0.5f); // 稍微偏移避免重叠
        BaseItemData 记忆碎片 = ItemManager.Instance.GetItemByID("记忆碎片");
        if (记忆碎片 != null)
        {
            ItemManager.Instance.DropItem(记忆碎片, 消耗品掉落位置);
            Debug.Log("记忆碎片已掉落！");
        }
        else
        {
            Debug.LogError("未找到记忆碎片物品！");
        }

        // 标记宝箱已开启
        是否已开启 = true;
        Debug.Log("宝箱已标记为已开启！");

        // 隐藏提示UI
        隐藏提示();
    }
}