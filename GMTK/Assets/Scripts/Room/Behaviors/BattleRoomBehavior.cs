using UnityEngine;

namespace Room
{
    /// <summary>
    /// 战斗房间行为
    /// </summary>
    public class BattleRoomBehavior : IRoomBehavior
    {
        public void OnEnter(Player.Player player)
        {
            Debug.Log("进入战斗房间");
            // 开始战斗逻辑
        }

        public void OnExit(Player.Player player)
        {
            Debug.Log("离开战斗房间");
            // 结束战斗逻辑
        }
    }
}