using UnityEngine;

namespace GameCore.States
{
    /// <summary>
    /// 游戏进行状态实现 (OCP: 新增状态只需派生)
    /// </summary>
    public class PlayingState : IGameState
    {
        public void Enter()
        {
            Debug.Log("进入游戏状态");
            // 游戏状态进入逻辑
        }

        public void Update()
        {
            // 游戏状态更新逻辑
        }

        public void Exit()
        {
            Debug.Log("退出游戏状态");
            // 游戏状态退出逻辑
        }
    }
}