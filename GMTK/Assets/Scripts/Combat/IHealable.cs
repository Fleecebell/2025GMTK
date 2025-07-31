namespace Combat
{
    /// <summary>
    /// 可治疗接口
    /// </summary>
    public interface IHealable
    {
        bool CanBeHealed { get; }
        void Heal(int amount);
    }
}