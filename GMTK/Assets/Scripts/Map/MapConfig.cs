using UnityEngine;

namespace Map
{
    /// <summary>
    /// 地图配置
    /// </summary>
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Game/Map Config")]
    public class MapConfig : ScriptableObject
    {
        [Header("地图尺寸")]
        public int width = 10;
        public int height = 10;
        
        [Header("房间配置")]
        public int minRooms = 5;
        public int maxRooms = 15;
        
        [Header("房间类型概率")]
        [Range(0f, 1f)] public float battleRoomProbability = 0.6f;
        [Range(0f, 1f)] public float treasureRoomProbability = 0.2f;
        [Range(0f, 1f)] public float shopRoomProbability = 0.1f;
    }
}