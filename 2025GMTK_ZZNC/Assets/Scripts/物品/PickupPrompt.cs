using UnityEngine;
using InventorySystem.Items;
using TMPro;


public class PickupPrompt : MonoBehaviour
{
    [Header("通用 UI")]
    public TextMeshProUGUI nameText;

    [HideInInspector] public bool isUsed { get; private set; } = false;

    private BaseItemData curItemData = null;
    [SerializeField] private CanvasGroup canvasGroup;

    /// <summary>
    /// 外部调用：设置要显示的物品数据
    /// </summary>
    public void SetPickupPrompt(BaseItemData data)
    {
        curItemData = data;
        isUsed = true;
        DisplayData();
        canvasGroup.alpha = 1;
    }

    private void DisplayData()
    {
        if (curItemData == null) return;

        // 通用部分
        if (nameText) nameText.text = curItemData.itemName;
        transform.SetAsFirstSibling();  // 放到最后（最先渲染）
    }
    public void ClearPickupPrompt()
    {
        isUsed = false;
        canvasGroup.alpha = 0;
    }
}
