namespace Player
{
    /// <summary>
    /// 玩家理智系统接口 (ISP: 分离不同属性关注点)
    /// </summary>
    public interface ISanitySystem
    {
        int CurrentSan { get; }
        int MaxSan { get; }
        void LoseSanity(int amount);
        void RestoreSanity(int amount);
    }
}