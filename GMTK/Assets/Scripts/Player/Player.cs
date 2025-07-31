using UnityEngine;
using Player.Components;

namespace Player
{
    /// <summary>
    /// 玩家控制器 (SRP: 只负责玩家行为协调)
    /// </summary>
    public class Player : MonoBehaviour, IHealthSystem, ISanitySystem
    {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private SanityComponent sanityComponent;
        [SerializeField] private float moveSpeed = 5f;

        // IHealthSystem 实现
        public int CurrentHP => healthComponent.CurrentHP;
        public int MaxHP => healthComponent.MaxHP;

        // ISanitySystem 实现
        public int CurrentSan => sanityComponent.CurrentSan;
        public int MaxSan => sanityComponent.MaxSan;

        private void Awake()
        {
            if (healthComponent == null)
                healthComponent = GetComponent<HealthComponent>();
            if (sanityComponent == null)
                sanityComponent = GetComponent<SanityComponent>();
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="direction">移动方向</param>
        public void Move(Vector2 direction)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 攻击
        /// </summary>
        public void Attack()
        {
            // 攻击逻辑
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skillIndex">技能索引</param>
        public void UseSkill(int skillIndex)
        {
            // 使用技能逻辑
        }

        // IHealthSystem 委托实现
        public void TakeDamage(int damage)
        {
            healthComponent.TakeDamage(damage);
        }

        public void Heal(int amount)
        {
            healthComponent.Heal(amount);
        }

        // ISanitySystem 委托实现
        public void LoseSanity(int amount)
        {
            sanityComponent.LoseSanity(amount);
        }

        public void RestoreSanity(int amount)
        {
            sanityComponent.RestoreSanity(amount);
        }
    }
}