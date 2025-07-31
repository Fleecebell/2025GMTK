using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    /// <summary>
    /// 技能UI面板 (OCP: 新面板只需派生)
    /// </summary>
    public class SkillUI : UIPanel
    {
        [Header("UI组件")]
        [SerializeField] private Transform skillContainer;
        [SerializeField] private GameObject skillSlotPrefab;
        [SerializeField] private Button closeButton;

        private Skill.SkillManager skillManager;

        public override void Initialize()
        {
            skillManager = FindObjectOfType<Skill.SkillManager>();
            CreateSkillSlots();
        }

        protected override void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        public override void Show()
        {
            base.Show();
            RefreshSkills();
        }

        private void CreateSkillSlots()
        {
            if (skillContainer == null || skillSlotPrefab == null) return;

            // 创建技能槽
            for (int i = 0; i < 6; i++) // 假设有6个技能槽
            {
                GameObject slot = Instantiate(skillSlotPrefab, skillContainer);
                
                // 绑定技能使用事件
                Button skillButton = slot.GetComponent<Button>();
                if (skillButton != null)
                {
                    int skillIndex = i; // 捕获索引
                    skillButton.onClick.AddListener(() => UseSkill(skillIndex));
                }
            }
        }

        private void RefreshSkills()
        {
            if (skillManager == null) return;

            var availableSkills = skillManager.GetAvailableSkills();
            
            for (int i = 0; i < skillContainer.childCount; i++)
            {
                Transform slot = skillContainer.GetChild(i);
                
                if (i < availableSkills.Count)
                {
                    // 显示技能
                    Text skillName = slot.GetComponentInChildren<Text>();
                    if (skillName != null)
                    {
                        skillName.text = availableSkills[i].Name;
                    }
                    
                    slot.gameObject.SetActive(true);
                }
                else
                {
                    slot.gameObject.SetActive(false);
                }
            }
        }

        private void UseSkill(int skillIndex)
        {
            skillManager?.UseSkill(skillIndex);
        }
    }
}