using UnityEngine;
using Combat;

namespace Effects.Types
{
    /// <summary>
    /// 持续伤害效果 (OCP: 新效果只需派生)
    /// </summary>
    public class DamageOverTimeEffect : IEffect
    {
        private string name = "持续伤害";
        private float duration;
        private float startTime;
        private int damagePerTick;
        private float tickInterval;
        private float lastTickTime;

        public string Name => name;
        public float Duration => duration;

        public DamageOverTimeEffect(float duration, int damagePerTick, float tickInterval = 1f)
        {
            this.duration = duration;
            this.damagePerTick = damagePerTick;
            this.tickInterval = tickInterval;
            this.startTime = Time.time;
            this.lastTickTime = Time.time;
        }

        public void Apply(GameObject target)
        {
            Debug.Log($"对 {target.name} 施加持续伤害效果");
        }

        public void Remove(GameObject target)
        {
            Debug.Log($"从 {target.name} 移除持续伤害效果");
        }

        public bool IsExpired()
        {
            bool expired = Time.time - startTime >= duration;
            
            // 在效果持续期间造成伤害
            if (!expired && Time.time - lastTickTime >= tickInterval)
            {
                // 这里需要获取目标并造成伤害
                lastTickTime = Time.time;
            }
            
            return expired;
        }
    }
}