using UnityEngine;

namespace Enemy.Types
{
    /// <summary>
    /// Boss敌人 (OCP: 新敌人类型)
    /// </summary>
    public class BossEnemy : Enemy
    {
        [SerializeField] private int phaseCount = 3;
        [SerializeField] private int currentPhase = 1;

        public override void Initialize(EnemyData data)
        {
            enemyData = data;
            currentHealth = data.maxHealth;
            
            Debug.Log($"初始化Boss敌人：{data.enemyName}");
        }

        public override void StartAI()
        {
            base.StartAI();
            // Boss特有的AI启动逻辑
        }

        protected override void Die()
        {
            // Boss死亡特殊逻辑
            Debug.Log($"Boss {enemyData.enemyName} 被击败！");
            base.Die();
        }

        private void CheckPhaseTransition()
        {
            float healthPercentage = (float)currentHealth / enemyData.maxHealth;
            int newPhase = Mathf.CeilToInt(healthPercentage * phaseCount);
            
            if (newPhase != currentPhase)
            {
                currentPhase = newPhase;
                OnPhaseChange();
            }
        }

        private void OnPhaseChange()
        {
            Debug.Log($"Boss进入第{currentPhase}阶段");
            // 阶段转换逻辑
        }
    }
}