using System.Collections.Generic;

namespace Map
{
    /// <summary>
    /// µØÍ¼Êý¾Ý
    /// </summary>
    [System.Serializable]
    public class MapData
    {
        public List<RoomData> rooms;
        public int width;
        public int height;

        public MapData()
        {
            rooms = new List<RoomData>();
        }
    }
}