namespace Combat
{
    /// <summary>
    /// 伤害类型枚举
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magic,
        Fire,
        Ice,
        Lightning
    }

    /// <summary>
    /// 可受伤害接口
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int damage, DamageType type);
        bool IsDead { get; }
    }
}