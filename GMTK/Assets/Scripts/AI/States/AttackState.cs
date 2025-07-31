using UnityEngine;

namespace AI.States
{
    /// <summary>
    /// ¹¥»÷×´Ì¬ (RP: ×´Ì¬¿É¸´ÓÃ)
    /// </summary>
    public class AttackState : IState<Enemy.Enemy>
    {
        private float lastAttackTime;
        private float attackCooldown = 1f;

        public void Enter(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} ½øÈë¹¥»÷×´Ì¬");
        }

        public void Update(Enemy.Enemy owner)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                PerformAttack(owner);
                lastAttackTime = Time.time;
            }

            // ¼ì²éÄ¿±êÊÇ·ñÀë¿ª¹¥»÷·¶Î§
            Player.Player player = Object.FindObjectOfType<Player.Player>();
            if (player != null)
            {
                float distance = Vector3.Distance(owner.transform.position, player.transform.position);
                if (distance > 2f) // ¹¥»÷·¶Î§
                {
                    // ÇÐ»»»Ø×·»÷×´Ì¬
                }
            }
        }

        public void Exit(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} ÍË³ö¹¥»÷×´Ì¬");
        }

        private void PerformAttack(Enemy.Enemy owner)
        {
            Debug.Log($"{owner.name} Ö´ÐÐ¹¥»÷");
            // ¹¥»÷Âß¼­
        }
    }
}