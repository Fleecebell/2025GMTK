using UnityEngine;
using Combat;

namespace Enemy
{
    /// <summary>
    /// 敌人数据
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("基础属性")]
        public string enemyName;
        public int maxHealth;
        public int attackDamage;
        public float moveSpeed;
        public DamageType attackType;

        [Header("AI设置")]
        public float detectionRange = 5f;
        public float attackRange = 2f;
        public float attackCooldown = 1f;
    }
}