namespace Item
{
    /// <summary>
    /// 装备槽位枚举
    /// </summary>
    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Accessory,
        Ring
    }

    /// <summary>
    /// 可装备道具接口
    /// </summary>
    public interface IEquippableItem : IItem
    {
        EquipmentSlot Slot { get; }
        void OnEquip(Player.Player player);
        void OnUnequip(Player.Player player);
    }
}