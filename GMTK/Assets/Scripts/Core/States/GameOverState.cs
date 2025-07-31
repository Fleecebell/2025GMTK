using UnityEngine;

namespace GameCore.States
{
    /// <summary>
    /// 游戏结束状态实现 (OCP: 新增状态只需派生)
    /// </summary>
    public class GameOverState : IGameState
    {
        public void Enter()
        {
            Debug.Log("进入游戏结束状态");
            // 游戏结束状态进入逻辑
        }

        public void Update()
        {
            // 游戏结束状态更新逻辑
        }

        public void Exit()
        {
            Debug.Log("退出游戏结束状态");
            // 游戏结束状态退出逻辑
        }
    }
}