using UnityEngine;
using Events;

namespace Player.Components
{
    /// <summary>
    /// 生命值组件
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int currentHP;

        public int MaxHP => maxHP;
        public int CurrentHP => currentHP;

        private void Start()
        {
            currentHP = maxHP;
        }

        public void TakeDamage(int damage)
        {
            currentHP = Mathf.Max(0, currentHP - damage);
            
            if (currentHP <= 0)
            {
                EventManager.Instance.Publish(new PlayerDeathEvent());
            }
        }

        public void Heal(int amount)
        {
            currentHP = Mathf.Min(maxHP, currentHP + amount);
        }

        public void SetMaxHP(int newMaxHP)
        {
            maxHP = newMaxHP;
            currentHP = Mathf.Min(currentHP, maxHP);
        }
    }
}