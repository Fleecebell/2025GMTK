using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    /// <summary>
    /// 技能管理器
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private List<ISkill> learnedSkills = new List<ISkill>();
        [SerializeField] private List<ISkill> equippedSkills = new List<ISkill>();

        /// <summary>
        /// 学习技能
        /// </summary>
        /// <param name="skill">要学习的技能</param>
        public void LearnSkill(ISkill skill)
        {
            if (!learnedSkills.Contains(skill))
            {
                learnedSkills.Add(skill);
                Debug.Log($"学会技能：{skill.Name}");
            }
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="skillIndex">技能索引</param>
        public void UseSkill(int skillIndex)
        {
            if (skillIndex >= 0 && skillIndex < equippedSkills.Count)
            {
                ISkill skill = equippedSkills[skillIndex];
                if (skill.CanUse())
                {
                    skill.Use();
                }
                else
                {
                    Debug.Log($"技能 {skill.Name} 无法使用");
                }
            }
        }

        /// <summary>
        /// 获取可用技能列表
        /// </summary>
        /// <returns>可用技能列表</returns>
        public List<ISkill> GetAvailableSkills()
        {
            return new List<ISkill>(learnedSkills);
        }

        /// <summary>
        /// 装备技能
        /// </summary>
        /// <param name="skill">要装备的技能</param>
        public void EquipSkill(ISkill skill)
        {
            if (learnedSkills.Contains(skill) && !equippedSkills.Contains(skill))
            {
                equippedSkills.Add(skill);
                
                if (skill is IPassiveSkill passiveSkill)
                {
                    passiveSkill.OnEquip();
                }
            }
        }

        /// <summary>
        /// 卸下技能
        /// </summary>
        /// <param name="skill">要卸下的技能</param>
        public void UnequipSkill(ISkill skill)
        {
            if (equippedSkills.Contains(skill))
            {
                equippedSkills.Remove(skill);
                
                if (skill is IPassiveSkill passiveSkill)
                {
                    passiveSkill.OnUnequip();
                }
            }
        }
    }
}