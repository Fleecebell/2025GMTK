using UnityEngine;
using UnityEngine.UI;

namespace UI.Panels
{
    /// <summary>
    /// 游戏结束UI面板 (OCP: 新面板只需派生)
    /// </summary>
    public class GameOverUI : UIPanel
    {
        [Header("UI组件")]
        [SerializeField] private Text gameOverText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        public override void Initialize()
        {
            // 初始化游戏结束UI
        }

        protected override void BindEvents()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }

            // 监听玩家死亡事件
            if (Events.EventManager.Instance != null)
            {
                Events.EventManager.Instance.Subscribe<Events.PlayerDeathEvent>(OnPlayerDeath);
            }
        }

        private void OnPlayerDeath(Events.PlayerDeathEvent eventData)
        {
            Show();
            if (gameOverText != null)
            {
                gameOverText.text = "游戏结束";
            }
        }

        private void RestartGame()
        {
            Hide();
            // 重新开始游戏逻辑
            GameCore.GameManager gameManager = FindObjectOfType<GameCore.GameManager>();
            gameManager?.RestartLevel();
        }

        private void ReturnToMainMenu()
        {
            Hide();
            // 返回主菜单逻辑
            GameCore.GameManager gameManager = FindObjectOfType<GameCore.GameManager>();
            gameManager?.ChangeState(new GameCore.States.MenuState());
        }

        private void OnDestroy()
        {
            if (Events.EventManager.Instance != null)
            {
                Events.EventManager.Instance.Unsubscribe<Events.PlayerDeathEvent>(OnPlayerDeath);
            }
        }
    }
}