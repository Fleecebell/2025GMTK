using UnityEngine;

namespace AI.States
{
    /// <summary>
    /// 追击状态 (RP: 状态可复用)
    /// </summary>
    public class ChaseState : IState<Enemy.Enemy>
    {
        private Transform target;

        public void Enter(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} 进入追击状态");
            // 寻找目标
            target = FindTarget();
        }

        public void Update(Enemy.Enemy owner)
        {
            if (target != null)
            {
                // 向目标移动
                Vector3 direction = (target.position - owner.transform.position).normalized;
                owner.transform.Translate(direction * Time.deltaTime);

                // 检查是否进入攻击范围
                float distance = Vector3.Distance(owner.transform.position, target.position);
                if (distance <= 2f) // 攻击范围
                {
                    // 切换到攻击状态
                }
            }
        }

        public void Exit(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} 退出追击状态");
        }

        private Transform FindTarget()
        {
            // 寻找玩家目标
            Player.Player player = Object.FindObjectOfType<Player.Player>();
            return player?.transform;
        }
    }
}