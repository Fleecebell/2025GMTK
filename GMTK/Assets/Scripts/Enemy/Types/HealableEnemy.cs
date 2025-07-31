using UnityEngine;

namespace Enemy.Types
{
    /// <summary>
    /// 可治疗敌人 (OCP: 新敌人类型)
    /// </summary>
    public class HealableEnemy : Enemy
    {
        [SerializeField] private float healCooldown = 5f;
        [SerializeField] private int healAmount = 20;
        [SerializeField] private float lastHealTime;

        public override void Initialize(EnemyData data)
        {
            enemyData = data;
            currentHealth = data.maxHealth;
            
            Debug.Log($"初始化可治疗敌人：{data.enemyName}");
        }

        protected override void Update()
        {
            base.Update();
            
            // 自动治疗逻辑
            if (Time.time - lastHealTime >= healCooldown && currentHealth < enemyData.maxHealth)
            {
                SelfHeal();
            }
        }

        private void SelfHeal()
        {
            Heal(healAmount);
            lastHealTime = Time.time;
            Debug.Log($"{enemyData.enemyName} 自我治疗");
        }
    }
}