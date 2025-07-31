using Combat;

namespace Enemy
{
    /// <summary>
    /// 敌人接口 (ISP: 敌人行为分离)
    /// </summary>
    public interface IEnemy : IDamageable, IHealable
    {
        void Initialize(EnemyData data);
        void StartAI();
        void StopAI();
    }
}