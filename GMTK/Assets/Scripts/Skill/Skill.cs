using UnityEngine;

namespace Skill
{
    /// <summary>
    /// 技能基类 (Template Method Pattern)
    /// </summary>
    public abstract class Skill : ScriptableObject, ISkill
    {
        [SerializeField] protected string skillName;
        [SerializeField] protected string description;

        public string Name => skillName;
        public string Description => description;

        public abstract bool CanUse();
        public abstract void Use();
    }
}