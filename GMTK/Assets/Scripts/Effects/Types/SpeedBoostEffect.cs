using UnityEngine;

namespace Effects.Types
{
    /// <summary>
    /// 速度提升效果 (OCP: 新效果只需派生)
    /// </summary>
    public class SpeedBoostEffect : IEffect
    {
        private string name = "速度提升";
        private float duration;
        private float startTime;
        private float speedMultiplier;

        public string Name => name;
        public float Duration => duration;

        public SpeedBoostEffect(float duration, float speedMultiplier)
        {
            this.duration = duration;
            this.speedMultiplier = speedMultiplier;
            this.startTime = Time.time;
        }

        public void Apply(GameObject target)
        {
            Debug.Log($"对 {target.name} 施加速度提升效果，倍率：{speedMultiplier}");
            // 提升移动速度逻辑
        }

        public void Remove(GameObject target)
        {
            Debug.Log($"从 {target.name} 移除速度提升效果");
            // 恢复原始移动速度逻辑
        }

        public bool IsExpired()
        {
            return Time.time - startTime >= duration;
        }
    }
}