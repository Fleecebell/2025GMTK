using UnityEngine;

namespace GameCore.States
{
    /// <summary>
    /// 菜单状态实现 (OCP: 新增状态只需派生)
    /// </summary>
    public class MenuState : IGameState
    {
        public void Enter()
        {
            Debug.Log("进入菜单状态");
            // 菜单状态进入逻辑
        }

        public void Update()
        {
            // 菜单状态更新逻辑
        }

        public void Exit()
        {
            Debug.Log("退出菜单状态");
            // 菜单状态退出逻辑
        }
    }
}