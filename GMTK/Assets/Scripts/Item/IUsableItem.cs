namespace Item
{
    /// <summary>
    /// 可使用道具接口
    /// </summary>
    public interface IUsableItem : IItem
    {
        bool CanUse();
        void Use(Player.Player player);
    }
}