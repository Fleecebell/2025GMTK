using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// 房间类型枚举
    /// </summary>
    public enum RoomType
    {
        Normal,
        Battle,
        Treasure,
        Shop,
        Boss
    }

    /// <summary>
    /// 敌人生成数据
    /// </summary>
    [System.Serializable]
    public class EnemySpawnData
    {
        public string enemyType;
        public Vector2 spawnPosition;
        public int level;
    }

    /// <summary>
    /// 房间数据结构 (SRP: 只存储房间信息)
    /// </summary>
    [System.Serializable]
    public class RoomData
    {
        public RoomType Type { get; set; }
        public Vector2Int Position { get; set; }
        public List<EnemySpawnData> Enemies { get; set; }

        public RoomData()
        {
            Enemies = new List<EnemySpawnData>();
        }
    }
}