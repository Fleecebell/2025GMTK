namespace GameCore
{
    /// <summary>
    /// 游戏状态接口 (ISP: 状态接口分离)
    /// </summary>
    public interface IGameState
    {
        void Enter();
        void Update();
        void Exit();
    }
}