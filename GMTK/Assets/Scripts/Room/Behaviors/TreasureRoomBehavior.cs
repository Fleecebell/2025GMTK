using UnityEngine;

namespace Room
{
    /// <summary>
    /// 宝藏房间行为
    /// </summary>
    public class TreasureRoomBehavior : IRoomBehavior
    {
        public void OnEnter(Player.Player player)
        {
            Debug.Log("进入宝藏房间");
            // 宝藏房间逻辑
        }

        public void OnExit(Player.Player player)
        {
            Debug.Log("离开宝藏房间");
            // 离开宝藏房间逻辑
        }
    }
}