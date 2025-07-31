namespace Combat
{
    /// <summary>
    /// ¹¥»÷Õß½Ó¿Ú
    /// </summary>
    public interface IAttacker
    {
        int AttackDamage { get; }
        DamageType AttackType { get; }
        void PerformAttack(IDamageable target);
    }
}