namespace Combat
{
    /// <summary>
    /// 治疗者接口
    /// </summary>
    public interface IHealer
    {
        int HealAmount { get; }
        void PerformHeal(IHealable target);
    }
}