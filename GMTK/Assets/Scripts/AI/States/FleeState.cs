using UnityEngine;

namespace AI.States
{
    /// <summary>
    /// 逃跑状态 (RP: 状态可复用)
    /// </summary>
    public class FleeState : IState<Enemy.Enemy>
    {
        private Transform target;

        public void Enter(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} 进入逃跑状态");
            target = FindTarget();
        }

        public void Update(Enemy.Enemy owner)
        {
            if (target != null)
            {
                // 远离目标
                Vector3 direction = (owner.transform.position - target.position).normalized;
                owner.transform.Translate(direction * Time.deltaTime);

                // 检查是否已经逃离足够远
                float distance = Vector3.Distance(owner.transform.position, target.position);
                if (distance >= 10f) // 安全距离
                {
                    // 切换回空闲状态
                }
            }
        }

        public void Exit(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} 退出逃跑状态");
        }

        private Transform FindTarget()
        {
            // 寻找威胁目标
            Player.Player player = Object.FindObjectOfType<Player.Player>();
            return player?.transform;
        }
    }
}