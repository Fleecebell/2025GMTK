using UnityEngine;

namespace Map
{
    /// <summary>
    /// 地图管理器 (SRP: 只负责地图管理)
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MapData currentMapData;
        [SerializeField] private RoomData currentRoom;

        /// <summary>
        /// 加载地图
        /// </summary>
        /// <param name="mapData">地图数据</param>
        public void LoadMap(MapData mapData)
        {
            currentMapData = mapData;
            // 地图加载逻辑
        }

        /// <summary>
        /// 卸载当前地图
        /// </summary>
        public void UnloadCurrentMap()
        {
            currentMapData = null;
            currentRoom = null;
            // 地图卸载逻辑
        }

        /// <summary>
        /// 获取当前房间
        /// </summary>
        /// <returns>当前房间数据</returns>
        public RoomData GetCurrentRoom()
        {
            return currentRoom;
        }

        /// <summary>
        /// 设置当前房间
        /// </summary>
        /// <param name="room">房间数据</param>
        public void SetCurrentRoom(RoomData room)
        {
            currentRoom = room;
        }
    }
}