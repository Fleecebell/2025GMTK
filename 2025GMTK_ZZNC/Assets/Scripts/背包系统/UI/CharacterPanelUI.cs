using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InventorySystem.Character;

namespace InventorySystem.UI
{
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

        [Header("基础属性显示")]
        [SerializeField] private TextMeshProUGUI powerText;
        [SerializeField] private TextMeshProUGUI armorText;
        [SerializeField] private TextMeshProUGUI intelligenceText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI criticalRateText;
        [SerializeField] private TextMeshProUGUI criticalDamageText;

        [Header("角色头像")]
        [SerializeField] private Image characterPortrait;

        private CharacterManager characterManager;

        private void Start()
        {
            InitializeCharacterPanel();
        }

        private void InitializeCharacterPanel()
        {
            characterManager = CharacterManager.Instance;
            if (characterManager == null)
            {
                Debug.LogError("找不到CharacterManager实例");
                return;
            }

            BindEvents();
            UpdateCharacterDisplay(characterManager.CurrentCharacterData);
        }

        private void BindEvents()
        {
            if (characterManager != null)
            {
                characterManager.OnCharacterDataChanged += UpdateCharacterDisplay;
                characterManager.OnHealthChanged += UpdateHealthDisplay;
                characterManager.OnSanChanged += UpdateSanDisplay;
            }
        }

        private void UpdateCharacterDisplay(CharacterData characterData)
        {
            if (characterData == null) return;

            UpdateHealthDisplay(characterData.CurrentHealth, characterData.MaxHealth);
            UpdateSanDisplay(characterData.CurrentSan, characterData.MaxSan);
            UpdateAttributeDisplay(characterData);

            Debug.Log("角色面板UI已更新");
        }

        private void UpdateHealthDisplay(float currentHealth, float maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }

            if (healthText != null)
                healthText.text = $"{currentHealth:F0}";

            if (maxHealthText != null)
                maxHealthText.text = $"/{maxHealth:F0}";

            UpdateHealthColor(currentHealth / maxHealth);
        }

        private void UpdateSanDisplay(float currentSan, float maxSan)
        {
            if (sanSlider != null)
            {
                sanSlider.maxValue = maxSan;
                sanSlider.value = currentSan;
            }

            if (sanText != null)
                sanText.text = $"{currentSan:F0}";

            if (maxSanText != null)
                maxSanText.text = $"/{maxSan:F0}";

            UpdateSanColor(currentSan / maxSan);
        }

        private void UpdateAttributeDisplay(CharacterData characterData)
        {
            if (powerText != null)
                powerText.text = $"力量 {characterData.Power:F1}";

            if (armorText != null)
                armorText.text = $"护甲 {characterData.Armor:F1}";

            if (intelligenceText != null)
                intelligenceText.text = $"智力 {characterData.Intelligence:F1}";

            if (attackSpeedText != null)
                attackSpeedText.text = $"攻速 {characterData.AttackSpeed:P0}";

            if (moveSpeedText != null)
                moveSpeedText.text = $"移速 {characterData.MoveSpeed:P0}";

            if (criticalRateText != null)
                criticalRateText.text = $"暴击 {(characterData.CriticalRate * 100):F1}%";

            if (criticalDamageText != null)
                criticalDamageText.text = $"爆伤 {(characterData.CriticalDamage * 100):F0}%";
        }

        private void UpdateHealthColor(float healthPercentage)
        {
            if (healthSlider?.fillRect?.GetComponent<Image>() == null) return;

            Image fillImage = healthSlider.fillRect.GetComponent<Image>();
            fillImage.color = healthPercentage > 0.6f ? Color.green :
                            healthPercentage > 0.3f ? Color.yellow : Color.red;
        }

        private void UpdateSanColor(float sanPercentage)
        {
            if (sanSlider?.fillRect?.GetComponent<Image>() == null) return;

            Image fillImage = sanSlider.fillRect.GetComponent<Image>();
            fillImage.color = sanPercentage > 0.6f ? Color.cyan :
                            sanPercentage > 0.3f ? Color.blue : Color.magenta;
        }

        public void SetCharacterPortrait(Sprite portrait)
        {
            if (characterPortrait != null && portrait != null)
                characterPortrait.sprite = portrait;
        }

        public string GetCharacterSummary()
        {
            if (characterManager?.CurrentCharacterData == null)
                return "角色数据未加载";

            var data = characterManager.CurrentCharacterData;
            return $"生命: {data.CurrentHealth:F0}/{data.MaxHealth:F0}\n" +
                   $"San: {data.CurrentSan:F0}/{data.MaxSan:F0}\n" +
                   $"力量: {data.Power:F1}\n" +
                   $"护甲: {data.Armor:F1}\n" +
                   $"智力: {data.Intelligence:F1}";
        }

        [ContextMenu("测试生命值变化")]
        public void TestHealthChange()
        {
            characterManager?.TakeDamage(10f);
        }

        [ContextMenu("测试San值变化")]
        public void TestSanChange()
        {
            characterManager?.ConsumeSan(15f);
        }

        [ContextMenu("完全恢复")]
        public void TestFullRestore()
        {
            characterManager?.FullRestore();
        }

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