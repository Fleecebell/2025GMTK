using UnityEngine;

namespace Enemy.Types
{
    /// <summary>
    /// 基础敌人 (OCP: 新敌人类型)
    /// </summary>
    public class BasicEnemy : Enemy
    {
        public override void Initialize(EnemyData data)
        {
            enemyData = data;
            currentHealth = data.maxHealth;
            
            Debug.Log($"初始化基础敌人：{data.enemyName}");
        }

        public override void StartAI()
        {
            base.StartAI();
            // 基础敌人特有的AI启动逻辑
        }
    }
}