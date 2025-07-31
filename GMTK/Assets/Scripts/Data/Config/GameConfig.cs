using UnityEngine;

namespace Data.Config
{
    /// <summary>
    /// 游戏配置 (SRP: 各系统配置分离)
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("玩家配置")]
        public PlayerConfig playerConfig;

        [Header("地图配置")]
        public Map.MapConfig mapConfig;

        [Header("敌人配置")]
        public EnemyConfig[] enemyConfigs;

        [Header("游戏设置")]
        public float gameSpeed = 1f;
        public bool debugMode = false;
    }
}