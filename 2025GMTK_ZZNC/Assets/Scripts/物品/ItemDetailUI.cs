using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem.Items;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class ItemDetailUI : MonoBehaviour
{
    [Header("通用 UI")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    [Header("消耗品额外字段")]
    public TextMeshProUGUI effectDescText;

    [Header("装备额外字段")]
    public TextMeshProUGUI modifiersText;
    public TextMeshProUGUI specialEffectText;

    [Header("武器额外字段")]
    public TextMeshProUGUI weaponStatsText;

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
        if (iconImage) iconImage.sprite = curItemData.icon;
        if (nameText) nameText.text = curItemData.itemName;
        if (descText) descText.text = curItemData.description;

        // 隐藏所有额外面板
        if (effectDescText) effectDescText.gameObject.SetActive(false);
        if (modifiersText) modifiersText.gameObject.SetActive(false);
        if (specialEffectText) specialEffectText.gameObject.SetActive(false);
        if (weaponStatsText) weaponStatsText.gameObject.SetActive(false);

        // 根据具体类型显示
        switch (curItemData)
        {
            case ConsumableData cd:
                ShowConsumable(cd);
                break;

            case EquipmentData ed:
                ShowEquipment(ed);
                break;

            case WeaponData wd:
                ShowWeapon(wd);
                break;
        }
        transform.SetAsLastSibling();   // 放到最前（最后渲染）
    }

    private void ShowConsumable(ConsumableData cd)
    {
        if (effectDescText)
        {
            effectDescText.gameObject.SetActive(true);
            effectDescText.text = cd.effectDescription;
        }
    }

    private void ShowEquipment(EquipmentData ed)
    {
        // 属性修饰符
        if (modifiersText)
        {
            modifiersText.gameObject.SetActive(true);
            var sb = new StringBuilder();
            foreach (var m in ed.attributeModifiers)
            {
                sb.AppendLine($"{m.attributeType}: {m.value}" + (m.isPercentage ? "%" : ""));
            }
            modifiersText.text = sb.ToString();
        }

        // 特殊效果
        if (specialEffectText)
        {
            specialEffectText.gameObject.SetActive(true);
            specialEffectText.text = ed.specialEffect;
        }
    }

    private void ShowWeapon(WeaponData wd)
    {
        if (weaponStatsText)
        {
            weaponStatsText.gameObject.SetActive(true);
            weaponStatsText.text =
                $"攻击类型: {wd.attackType}\n" +
                $"力量: {wd.power}\n" +
                $"信仰: {wd.belief}\n" +
                $"攻速: {wd.attackSpeed}\n" +
                $"暴击率: {wd.critical * 100:F1}%";
        }
    }
    public void ClearPickupPrompt()
    {
        isUsed = false;
        canvasGroup.alpha = 0;
    }
}
