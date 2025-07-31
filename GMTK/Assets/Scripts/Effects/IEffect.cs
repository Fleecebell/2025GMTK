using UnityEngine;

namespace Effects
{
    /// <summary>
    /// 效果接口 (ISP: 效果类型分离)
    /// </summary>
    public interface IEffect
    {
        string Name { get; }
        float Duration { get; }
        void Apply(GameObject target);
        void Remove(GameObject target);
        bool IsExpired();
    }
}