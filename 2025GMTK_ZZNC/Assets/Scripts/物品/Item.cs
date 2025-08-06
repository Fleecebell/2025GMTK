using UnityEngine;
using TMPro;
using InventorySystem.Items;
using InventorySystem.Managers;

[RequireComponent(typeof(CircleCollider2D))]
public class Item : MonoBehaviour
{
    [Header("物品数据")]
    [SerializeField] private BaseItemData itemData;
    [SerializeField] private bool isGoods;

    [Header("UI")]
    [SerializeField] private Transform pickupUIParent;
    [SerializeField] private GameObject pickupPromptPrefab; // 带 PickupPrompt
    [SerializeField] private GameObject itemDetailPrefab;   // 带 ItemDetailUI

    private GameObject curPrompt;
    private GameObject curDetail;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // 自动添加圆形触发器
        var col = GetComponent<CircleCollider2D>() ?? gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!pickupUIParent)
            pickupUIParent = GameObject.Find("提示框们").transform;

        if (itemData) UpdateItemDisplay();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        HidePrompt();
    }

    private void Update()
    {
        if (curPrompt == null && curDetail == null) return;

        if (Input.GetKeyDown(KeyCode.E) && curPrompt != null)
            PickupItem();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (curDetail == null)
            {
                ShowDetail();
            }
            else if (curDetail != null)
            {
                HideDetail();
            }
        }
    }

    #region UI 管理
    private void ShowPrompt()
    {
        curPrompt = GetOrCreateUI("提示框", pickupPromptPrefab);
        curPrompt.transform.position = transform.position;
        curPrompt.GetComponent<PickupPrompt>()?.SetPickupPrompt(itemData);

        if (isGoods)
        {
            curPrompt.transform.Find("价格").GetComponent<TextMeshProUGUI>().text = $"价格: {itemData.price}";
        }
    }

    private void HidePrompt()
    {
        if (curPrompt)
        {
            curPrompt.GetComponent<PickupPrompt>()?.ClearPickupPrompt();
            curPrompt = null;
        }
        if (curDetail)
        {
            curDetail.GetComponent<ItemDetailUI>()?.ClearPickupPrompt();
            curDetail = null;
        }
        if (curPrompt != null)
        {
            TextMeshProUGUI priceText = curPrompt.transform.Find("价格").GetComponent<TextMeshProUGUI>();
            if (priceText != null)
            {
                priceText.text = null;
            }
        }
    }

    private void ShowDetail()
    {
        curDetail = GetOrCreateUI("物品详情", itemDetailPrefab);
        curDetail.transform.position = transform.position;
        curDetail.GetComponent<ItemDetailUI>()?.SetPickupPrompt(itemData);
    }

    private void HideDetail()
    {
        if (curDetail)
        {
            curDetail.GetComponent<ItemDetailUI>()?.ClearPickupPrompt();
            curDetail = null;
        }
    }

    private GameObject GetOrCreateUI(string name, GameObject prefab)
    {
        foreach (Transform t in pickupUIParent)
        {
            if (t.name == name)
            {
                var script = t.GetComponent<PickupPrompt>() ?? (object)t.GetComponent<ItemDetailUI>() ?? (object)t.GetComponent<TextMeshProUGUI>();
                if (script != null && !(bool)script.GetType().GetProperty("isUsed")?.GetValue(script))
                    return t.gameObject;
            }
        }

        var ui = Instantiate(prefab, pickupUIParent);
        ui.name = name;
        return ui;
    }
    #endregion

    private void PickupItem()
    {
        if (itemData == null) return;

        if (isGoods)
        {
            int price = int.Parse(itemData.price);
            if (CurrencyManager.Instance != null)
            {
                if (CurrencyManager.Instance.HasEnoughCurrency(price))
                {
                    CurrencyManager.Instance.Purchase(price);
                    InventoryManager.Instance?.AddItem(itemData, 1);
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("货币不足，无法购买！");
                }
            }
        }
        else
        {
            InventoryManager.Instance?.AddItem(itemData, 1);
            Destroy(gameObject);
        }
    }

    public void SetItemData(BaseItemData data, bool isGoods = false)
    {
        itemData = data;
        this.isGoods = isGoods;
        UpdateItemDisplay();
    }

    private void UpdateItemDisplay()
    {
        if (itemData && spriteRenderer)
            spriteRenderer.sprite = itemData.icon;
    }

    public BaseItemData GetItemData() => itemData;
}