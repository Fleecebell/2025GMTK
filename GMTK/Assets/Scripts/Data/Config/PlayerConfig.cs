using UnityEngine;

namespace Data.Config
{
    /// <summary>
    /// 玩家配置
    /// </summary>
    [System.Serializable]
    public class PlayerConfig
    {
        [Header("基础属性")]
        public int maxHealth = 100;
        public int maxSanity = 100;
        public float moveSpeed = 5f;

        [Header("战斗属性")]
        public int baseDamage = 10;
        public float attackSpeed = 1f;
    }
}