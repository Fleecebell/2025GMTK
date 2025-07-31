using UnityEngine;

namespace GameCore
{
    /// <summary>
    /// 游戏主循环管理器 (SRP: 只负责游戏状态流转)
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private IGameState currentState;
        
        /// <summary>
        /// 管理游戏状态切换
        /// </summary>
        public void UpdateGameState()
        {
            currentState?.Update();
        }
        
        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            // 游戏结束逻辑
        }
        
        /// <summary>
        /// 重新开始关卡
        /// </summary>
        public void RestartLevel()
        {
            // 重新开始关卡逻辑
        }
        
        /// <summary>
        /// 切换游戏状态
        /// </summary>
        /// <param name="newState">新状态</param>
        public void ChangeState(IGameState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }
    }
}