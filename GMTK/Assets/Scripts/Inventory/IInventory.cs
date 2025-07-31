namespace Inventory
{
    /// <summary>
    /// 背包接口 (ISP: 背包操作分离)
    /// </summary>
    public interface IInventory
    {
        bool AddItem(Item.IItem item);
        bool RemoveItem(Item.IItem item);
        Item.IItem GetItem(int index);
        int GetItemCount();
    }
}