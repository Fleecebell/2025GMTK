using UnityEngine;
using Combat;

namespace Data.Config
{
    /// <summary>
    /// 敌人配置
    /// </summary>
    [System.Serializable]
    public class EnemyConfig
    {
        [Header("基础信息")]
        public string enemyName;
        public GameObject enemyPrefab;

        [Header("属性")]
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