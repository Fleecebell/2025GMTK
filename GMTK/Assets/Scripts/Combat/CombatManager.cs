using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 战斗管理器 (SRP: 只负责战斗流程)
    /// </summary>
    public class CombatManager : MonoBehaviour, ICombatSystem
    {
        [SerializeField] private bool isInCombat = false;
        [SerializeField] private List<Enemy.Enemy> currentEnemies;

        public bool IsInCombat => isInCombat;

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="enemies">敌人列表</param>
        public void StartCombat(List<Enemy.Enemy> enemies)
        {
            isInCombat = true;
            currentEnemies = enemies;
            Debug.Log("战斗开始");
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        public void EndCombat()
        {
            isInCombat = false;
            currentEnemies?.Clear();
            Debug.Log("战斗结束");
        }

        /// <summary>
        /// 处理攻击
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">目标</param>
        public void ProcessAttack(IAttacker attacker, IDamageable target)
        {
            if (attacker != null && target != null)
            {
                attacker.PerformAttack(target);
            }
        }

        /// <summary>
        /// 处理治疗
        /// </summary>
        /// <param name="healer">治疗者</param>
        /// <param name="target">目标</param>
        public void ProcessHeal(IHealer healer, IHealable target)
        {
            if (healer != null && target != null && target.CanBeHealed)
            {
                healer.PerformHeal(target);
            }
        }
    }
}