using UnityEngine;
using Events;

namespace Player.Components
{
    /// <summary>
    /// 理智值组件
    /// </summary>
    public class SanityComponent : MonoBehaviour
    {
        [SerializeField] private int maxSan = 100;
        [SerializeField] private int currentSan;

        public int MaxSan => maxSan;
        public int CurrentSan => currentSan;

        private void Start()
        {
            currentSan = maxSan;
        }

        public void LoseSanity(int amount)
        {
            currentSan = Mathf.Max(0, currentSan - amount);
            EventManager.Instance.Publish(new SanValueChangedEvent(currentSan));
        }

        public void RestoreSanity(int amount)
        {
            currentSan = Mathf.Min(maxSan, currentSan + amount);
            EventManager.Instance.Publish(new SanValueChangedEvent(currentSan));
        }

        public void SetMaxSan(int newMaxSan)
        {
            maxSan = newMaxSan;
            currentSan = Mathf.Min(currentSan, maxSan);
        }
    }
}