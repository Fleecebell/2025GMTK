namespace Player
{
    /// <summary>
    /// 玩家生命系统接口 (ISP: 分离不同属性关注点)
    /// </summary>
    public interface IHealthSystem
    {
        int CurrentHP { get; }
        int MaxHP { get; }
        void TakeDamage(int damage);
        void Heal(int amount);
    }
}