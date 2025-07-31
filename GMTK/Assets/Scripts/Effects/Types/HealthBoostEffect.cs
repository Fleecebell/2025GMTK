using UnityEngine;

namespace Effects.Types
{
    /// <summary>
    /// 生命值提升效果 (OCP: 新效果只需派生)
    /// </summary>
    public class HealthBoostEffect : IEffect
    {
        private string name = "生命值提升";
        private float duration;
        private float startTime;
        private int boostAmount;

        public string Name => name;
        public float Duration => duration;

        public HealthBoostEffect(float duration, int boostAmount)
        {
            this.duration = duration;
            this.boostAmount = boostAmount;
            this.startTime = Time.time;
        }

        public void Apply(GameObject target)
        {
            Player.Player player = target.GetComponent<Player.Player>();
            if (player != null)
            {
                // 提升最大生命值
                Debug.Log($"对 {target.name} 施加生命值提升效果，提升 {boostAmount} 点");
            }
        }

        public void Remove(GameObject target)
        {
            Player.Player player = target.GetComponent<Player.Player>();
            if (player != null)
            {
                // 移除生命值提升
                Debug.Log($"从 {target.name} 移除生命值提升效果");
            }
        }

        public bool IsExpired()
        {
            return Time.time - startTime >= duration;
        }
    }
}