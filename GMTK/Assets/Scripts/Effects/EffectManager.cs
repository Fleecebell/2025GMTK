using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    /// <summary>
    /// 效果管理器 (SRP: 只负责效果管理)
    /// </summary>
    public class EffectManager : MonoBehaviour
    {
        private Dictionary<GameObject, List<IEffect>> activeEffects = new Dictionary<GameObject, List<IEffect>>();

        /// <summary>
        /// 添加效果
        /// </summary>
        /// <param name="effect">效果</param>
        /// <param name="target">目标对象</param>
        public void AddEffect(IEffect effect, GameObject target)
        {
            if (!activeEffects.ContainsKey(target))
            {
                activeEffects[target] = new List<IEffect>();
            }

            activeEffects[target].Add(effect);
            effect.Apply(target);
            Debug.Log($"对 {target.name} 施加效果：{effect.Name}");
        }

        /// <summary>
        /// 移除效果
        /// </summary>
        /// <param name="effect">效果</param>
        /// <param name="target">目标对象</param>
        public void RemoveEffect(IEffect effect, GameObject target)
        {
            if (activeEffects.ContainsKey(target))
            {
                if (activeEffects[target].Contains(effect))
                {
                    activeEffects[target].Remove(effect);
                    effect.Remove(target);
                    Debug.Log($"从 {target.name} 移除效果：{effect.Name}");
                }
            }
        }

        /// <summary>
        /// 获取目标的活跃效果
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <returns>活跃效果列表</returns>
        public List<IEffect> GetActiveEffects(GameObject target)
        {
            if (activeEffects.ContainsKey(target))
            {
                return new List<IEffect>(activeEffects[target]);
            }
            return new List<IEffect>();
        }

        private void Update()
        {
            // 检查过期效果
            List<GameObject> targetsToRemove = new List<GameObject>();
            
            foreach (var kvp in activeEffects)
            {
                GameObject target = kvp.Key;
                List<IEffect> effects = kvp.Value;
                
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    if (effects[i].IsExpired())
                    {
                        effects[i].Remove(target);
                        effects.RemoveAt(i);
                    }
                }
                
                if (effects.Count == 0)
                {
                    targetsToRemove.Add(target);
                }
            }
            
            foreach (GameObject target in targetsToRemove)
            {
                activeEffects.Remove(target);
            }
        }
    }
}