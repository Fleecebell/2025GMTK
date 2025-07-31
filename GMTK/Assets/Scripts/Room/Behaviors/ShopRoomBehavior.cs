using UnityEngine;

namespace Room
{
    /// <summary>
    /// 商店房间行为
    /// </summary>
    public class ShopRoomBehavior : IRoomBehavior
    {
        public void OnEnter(Player.Player player)
        {
            Debug.Log("进入商店房间");
            // 商店房间逻辑
        }

        public void OnExit(Player.Player player)
        {
            Debug.Log("离开商店房间");
            // 离开商店房间逻辑
        }
    }
}