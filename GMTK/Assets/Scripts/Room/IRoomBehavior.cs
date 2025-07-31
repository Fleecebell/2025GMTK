namespace Room
{
    /// <summary>
    /// 房间类型策略接口 (Strategy Pattern + OCP)
    /// </summary>
    public interface IRoomBehavior
    {
        void OnEnter(Player.Player player);
        void OnExit(Player.Player player);
    }
}