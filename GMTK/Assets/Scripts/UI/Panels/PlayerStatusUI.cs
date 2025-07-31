using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    /// <summary>
    /// 玩家状态UI面板 (OCP: 新面板只需派生)
    /// </summary>
    public class PlayerStatusUI : UIPanel
    {
        [Header("UI组件")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider sanitySlider;
        [SerializeField] private Text healthText;
        [SerializeField] private Text sanityText;

        private Player.Player player;

        public override void Initialize()
        {
            player = FindObjectOfType<Player.Player>();
            UpdateUI();
        }

        protected override void BindEvents()
        {
            // 绑定玩家状态变化事件
            if (Events.EventManager.Instance != null)
            {
                Events.EventManager.Instance.Subscribe<Events.SanValueChangedEvent>(OnSanityChanged);
            }
        }

        private void Update()
        {
            if (player != null && isVisible)
            {
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (player == null) return;

            // 更新生命值
            if (healthSlider != null)
            {
                healthSlider.value = (float)player.CurrentHP / player.MaxHP;
            }
            if (healthText != null)
            {
                healthText.text = $"{player.CurrentHP}/{player.MaxHP}";
            }

            // 更新理智值
            if (sanitySlider != null)
            {
                sanitySlider.value = (float)player.CurrentSan / player.MaxSan;
            }
            if (sanityText != null)
            {
                sanityText.text = $"{player.CurrentSan}/{player.MaxSan}";
            }
        }

        private void OnSanityChanged(Events.SanValueChangedEvent eventData)
        {
            UpdateUI();
        }

        private void OnDestroy()
        {
            if (Events.EventManager.Instance != null)
            {
                Events.EventManager.Instance.Unsubscribe<Events.SanValueChangedEvent>(OnSanityChanged);
            }
        }
    }
}