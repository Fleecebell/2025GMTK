using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI.Panels
{
    /// <summary>
    /// 背包UI面板 (OCP: 新面板只需派生)
    /// </summary>
    public class InventoryUI : UIPanel
    {
        [Header("UI组件")]
        [SerializeField] private Transform itemContainer;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private Button closeButton;

        private Inventory.Inventory inventory;
        private List<GameObject> itemSlots = new List<GameObject>();

        public override void Initialize()
        {
            inventory = FindObjectOfType<Inventory.Inventory>();
            CreateItemSlots();
        }

        protected override void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        public override void Show()
        {
            base.Show();
            RefreshInventory();
        }

        private void CreateItemSlots()
        {
            if (itemContainer == null || itemSlotPrefab == null) return;

            // 创建物品槽
            for (int i = 0; i < 20; i++) // 假设背包有20个槽位
            {
                GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
                itemSlots.Add(slot);
            }
        }

        private void RefreshInventory()
        {
            if (inventory == null) return;

            for (int i = 0; i < itemSlots.Count; i++)
            {
                GameObject slot = itemSlots[i];
                Item.IItem item = inventory.GetItem(i);

                if (item != null)
                {
                    // 显示物品
                    Image itemIcon = slot.GetComponentInChildren<Image>();
                    if (itemIcon != null && item.Icon != null)
                    {
                        itemIcon.sprite = item.Icon;
                        itemIcon.gameObject.SetActive(true);
                    }
                }
                else
                {
                    // 隐藏物品
                    Image itemIcon = slot.GetComponentInChildren<Image>();
                    if (itemIcon != null)
                    {
                        itemIcon.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}