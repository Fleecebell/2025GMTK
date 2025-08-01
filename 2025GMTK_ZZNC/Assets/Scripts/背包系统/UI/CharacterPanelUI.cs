using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InventorySystem.Character;

namespace InventorySystem.UI
{
    /// <summary>
    /// 角色面板UI
    /// 显示角色属性信息并监听属性变化
    /// </summary>
    public class CharacterPanelUI : MonoBehaviour
    {
        [Header("生命值显示")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI maxHealthText;

        [Header("San值显示")]
        [SerializeField] private Slider sanSlider;
        [SerializeField] private TextMeshProUGUI sanText;
        [SerializeField] private TextMeshProUGUI maxSanText;

        [Header("属性显示")]
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI defenseText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI criticalRateText;
        [SerializeField] private TextMeshProUGUI criticalDamageText;

        [Header("角色头像")]
        [SerializeField] private Image characterPortrait;

        // 私有变量
        private CharacterManager characterManager;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Start()
        {
            InitializeCharacterPanel();
        }

        /// <summary>
        /// 初始化角色面板
        /// </summary>
        private void InitializeCharacterPanel()
        {
            // 获取角色管理器
            characterManager = CharacterManager.Instance;
            if (characterManager == null)
            {
                Debug.LogError("找不到CharacterManager实例");
                return;
            }

            // 绑定事件
            BindEvents();

            // 初始化显示
            UpdateCharacterDisplay(characterManager.CurrentCharacterData);
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        private void BindEvents()
        {
            if (characterManager != null)
            {
                characterManager.OnCharacterDataChanged += UpdateCharacterDisplay;
                characterManager.OnHealthChanged += UpdateHealthDisplay;
                characterManager.OnSanChanged += UpdateSanDisplay;
            }
        }

        /// <summary>
        /// 更新角色显示
        /// </summary>
        /// <param name="characterData">角色数据</param>
        private void UpdateCharacterDisplay(CharacterData characterData)
        {
            if (characterData == null) return;

            // 更新生命值显示
            UpdateHealthDisplay(characterData.CurrentHealth, characterData.MaxHealth);

            // 更新San值显示
            UpdateSanDisplay(characterData.CurrentSan, characterData.MaxSan);

            // 更新属性显示
            UpdateAttributeDisplay(characterData);

            Debug.Log("角色面板UI已更新");
        }

        /// <summary>
        /// 更新生命值显示
        /// </summary>
        /// <param name="currentHealth">当前生命值</param>
        /// <param name="maxHealth">最大生命值</param>
        private void UpdateHealthDisplay(float currentHealth, float maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{currentHealth:F0}";
            }

            if (maxHealthText != null)
            {
                maxHealthText.text = $"/{maxHealth:F0}";
            }

            // 根据生命值百分比改变颜色
            UpdateHealthColor(currentHealth / maxHealth);
        }

        /// <summary>
        /// 更新San值显示
        /// </summary>
        /// <param name="currentSan">当前San值</param>
        /// <param name="maxSan">最大San值</param>
        private void UpdateSanDisplay(float currentSan, float maxSan)
        {
            if (sanSlider != null)
            {
                sanSlider.maxValue = maxSan;
                sanSlider.value = currentSan;
            }

            if (sanText != null)
            {
                sanText.text = $"{currentSan:F0}";
            }

            if (maxSanText != null)
            {
                maxSanText.text = $"/{maxSan:F0}";
            }

            // 根据San值百分比改变颜色
            UpdateSanColor(currentSan / maxSan);
        }

        /// <summary>
        /// 更新属性显示
        /// </summary>
        /// <param name="characterData">角色数据</param>
        private void UpdateAttributeDisplay(CharacterData characterData)
        {
            if (attackText != null)
            {
                attackText.text = $"攻击{characterData.Attack:F1}";
            }

            if (defenseText != null)
            {
                defenseText.text = $"防御{characterData.Defense:F1}";
            }

            if (moveSpeedText != null)
            {
                moveSpeedText.text = $"移速{characterData.MoveSpeed:F1}";
            }

            if (criticalRateText != null)
            {
                criticalRateText.text = $"暴击率{(characterData.CriticalRate * 100):F1}%";
            }

            if (criticalDamageText != null)
            {
                criticalDamageText.text = $"爆伤{(characterData.CriticalDamage * 100):F0}%";
            }
        }

        /// <summary>
        /// 更新生命值颜色
        /// </summary>
        /// <param name="healthPercentage">生命值百分比</param>
        private void UpdateHealthColor(float healthPercentage)
        {
            if (healthSlider?.fillRect?.GetComponent<Image>() == null) return;

            Image fillImage = healthSlider.fillRect.GetComponent<Image>();
            
            if (healthPercentage > 0.6f)
            {
                fillImage.color = Color.green;
            }
            else if (healthPercentage > 0.3f)
            {
                fillImage.color = Color.yellow;
            }
            else
            {
                fillImage.color = Color.red;
            }
        }

        /// <summary>
        /// 更新San值颜色
        /// </summary>
        /// <param name="sanPercentage">San值百分比</param>
        private void UpdateSanColor(float sanPercentage)
        {
            if (sanSlider?.fillRect?.GetComponent<Image>() == null) return;

            Image fillImage = sanSlider.fillRect.GetComponent<Image>();
            
            if (sanPercentage > 0.6f)
            {
                fillImage.color = Color.cyan;
            }
            else if (sanPercentage > 0.3f)
            {
                fillImage.color = Color.blue;
            }
            else
            {
                fillImage.color = Color.magenta;
            }
        }

        /// <summary>
        /// 设置角色头像
        /// </summary>
        /// <param name="portrait">头像精灵</param>
        public void SetCharacterPortrait(Sprite portrait)
        {
            if (characterPortrait != null && portrait != null)
            {
                characterPortrait.sprite = portrait;
            }
        }

        /// <summary>
        /// 调试用：测试属性变化
        /// </summary>
        [ContextMenu("测试生命值变化")]
        public void TestHealthChange()
        {
            if (characterManager != null)
            {
                characterManager.TakeDamage(10f);
            }
        }

        /// <summary>
        /// 调试用：测试San值变化
        /// </summary>
        [ContextMenu("测试San值变化")]
        public void TestSanChange()
        {
            if (characterManager != null)
            {
                characterManager.ConsumeSan(15f);
            }
        }

        /// <summary>
        /// 调试用：完全恢复
        /// </summary>
        [ContextMenu("完全恢复")]
        public void TestFullRestore()
        {
            if (characterManager != null)
            {
                characterManager.FullRestore();
            }
        }

        /// <summary>
        /// 获取角色状态摘要
        /// </summary>
        /// <returns>状态摘要字符串</returns>
        public string GetCharacterSummary()
        {
            if (characterManager?.CurrentCharacterData == null)
                return "角色数据未加载";

            var data = characterManager.CurrentCharacterData;
            return $"生命: {data.CurrentHealth:F0}/{data.MaxHealth:F0} " +
                   $"San: {data.CurrentSan:F0}/{data.MaxSan:F0} " +
                   $"攻击: {data.Attack:F1} " +
                   $"防御: {data.Defense:F1}";
        }

        /// <summary>
        /// 清理事件绑定
        /// </summary>
        private void OnDestroy()
        {
            if (characterManager != null)
            {
                characterManager.OnCharacterDataChanged -= UpdateCharacterDisplay;
                characterManager.OnHealthChanged -= UpdateHealthDisplay;
                characterManager.OnSanChanged -= UpdateSanDisplay;
            }
        }
    }
}