using System.Collections.Generic;
using UnityEngine;

namespace Map.Generators
{
    /// <summary>
    /// 随机地图生成器 (OCP: 可扩展不同算法)
    /// </summary>
    public class RandomMapGenerator : IMapGenerator
    {
        public MapData GenerateMap(MapConfig config)
        {
            MapData mapData = new MapData();
            mapData.width = config.width;
            mapData.height = config.height;

            int roomCount = Random.Range(config.minRooms, config.maxRooms + 1);
            
            for (int i = 0; i < roomCount; i++)
            {
                RoomData room = new RoomData();
                room.Position = new Vector2Int(
                    Random.Range(0, config.width),
                    Random.Range(0, config.height)
                );
                
                // 根据概率决定房间类型
                float rand = Random.value;
                if (rand < config.battleRoomProbability)
                    room.Type = RoomType.Battle;
                else if (rand < config.battleRoomProbability + config.treasureRoomProbability)
                    room.Type = RoomType.Treasure;
                else if (rand < config.battleRoomProbability + config.treasureRoomProbability + config.shopRoomProbability)
                    room.Type = RoomType.Shop;
                else
                    room.Type = RoomType.Normal;

                mapData.rooms.Add(room);
            }

            return mapData;
        }
    }
}