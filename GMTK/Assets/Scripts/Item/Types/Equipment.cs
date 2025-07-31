using UnityEngine;

namespace Item.Types
{
    /// <summary>
    /// 装备基类 (OCP: 新道具只需派生)
    /// </summary>
    public abstract class Equipment : Item, IEquippableItem
    {
        [SerializeField] protected EquipmentSlot slot;
        [SerializeField] protected int statBonus;

        public EquipmentSlot Slot => slot;

        public virtual void OnEquip(Player.Player player)
        {
            Debug.Log($"装备{Name}");
            // 装备效果逻辑
        }

        public virtual void OnUnequip(Player.Player player)
        {
            Debug.Log($"卸下{Name}");
            // 卸下装备效果逻辑
        }
    }
}