using UnityEngine;
using Map;

namespace Room
{
    /// <summary>
    /// 房间控制器 (SRP: 只负责房间逻辑)
    /// </summary>
    public class Room : MonoBehaviour
    {
        [SerializeField] private RoomType type;
        [SerializeField] private IRoomBehavior roomBehavior;
        [SerializeField] private bool isCleared = false;

        public RoomType Type { get; private set; }

        private void Start()
        {
            Type = type;
            SetRoomBehavior();
        }

        /// <summary>
        /// 玩家进入房间
        /// </summary>
        public void OnPlayerEnter()
        {
            roomBehavior?.OnEnter(FindObjectOfType<Player.Player>());
        }

        /// <summary>
        /// 玩家离开房间
        /// </summary>
        public void OnPlayerExit()
        {
            roomBehavior?.OnExit(FindObjectOfType<Player.Player>());
        }

        /// <summary>
        /// 房间是否已清理
        /// </summary>
        /// <returns>是否已清理</returns>
        public bool IsCleared()
        {
            return isCleared;
        }

        /// <summary>
        /// 设置房间已清理
        /// </summary>
        public void SetCleared(bool cleared)
        {
            isCleared = cleared;
        }

        /// <summary>
        /// 根据房间类型设置行为
        /// </summary>
        private void SetRoomBehavior()
        {
            switch (type)
            {
                case RoomType.Battle:
                    roomBehavior = new BattleRoomBehavior();
                    break;
                case RoomType.Treasure:
                    roomBehavior = new TreasureRoomBehavior();
                    break;
                case RoomType.Shop:
                    roomBehavior = new ShopRoomBehavior();
                    break;
                default:
                    roomBehavior = null;
                    break;
            }
        }
    }
}