using UnityEngine;

namespace AI.States
{
    /// <summary>
    /// 空闲状态 (RP: 状态可复用)
    /// </summary>
    public class IdleState : IState<Enemy.Enemy>
    {
        public void Enter(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} 进入空闲状态");
        }

        public void Update(Enemy.Enemy owner)
        {
            // 空闲状态更新逻辑
            // 检测玩家是否在范围内，如果是则切换到追击状态
        }

        public void Exit(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} 退出空闲状态");
        }
    }
}