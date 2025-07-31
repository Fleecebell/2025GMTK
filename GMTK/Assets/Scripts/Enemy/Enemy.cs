using UnityEngine;
using Combat;
using AI;
using Events;

namespace Enemy
{
    /// <summary>
    /// 敌人基类 (Template Method + LSP)
    /// </summary>
    public abstract class Enemy : MonoBehaviour, IEnemy
    {
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected int currentHealth;
        [SerializeField] protected bool isDead = false;
        
        protected StateMachine<Enemy> stateMachine;

        public bool IsDead => isDead;
        public bool CanBeHealed => !isDead && currentHealth < enemyData.maxHealth;

        protected virtual void Awake()
        {
            stateMachine = new StateMachine<Enemy>();
        }

        public abstract void Initialize(EnemyData data);

        public virtual void StartAI()
        {
            // 启动AI逻辑
            Debug.Log($"{enemyData.enemyName} AI启动");
        }

        public virtual void StopAI()
        {
            // 停止AI逻辑
            Debug.Log($"{enemyData.enemyName} AI停止");
        }

        public virtual void TakeDamage(int damage, DamageType type)
        {
            if (isDead) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            Debug.Log($"{enemyData.enemyName} 受到 {damage} 点伤害");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Heal(int amount)
        {
            if (isDead) return;

            currentHealth = Mathf.Min(enemyData.maxHealth, currentHealth + amount);
            Debug.Log($"{enemyData.enemyName} 恢复 {amount} 点生命值");
        }

        protected virtual void Die()
        {
            isDead = true;
            StopAI();
            EventManager.Instance.Publish(new EnemyDefeatedEvent(this));
            Debug.Log($"{enemyData.enemyName} 死亡");
        }

        protected virtual void Update()
        {
            if (!isDead)
            {
                stateMachine?.Update();
            }
        }
    }
}