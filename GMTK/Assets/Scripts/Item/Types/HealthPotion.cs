using UnityEngine;

namespace Item.Types
{
    /// <summary>
    /// 生命药水 (OCP: 新道具只需派生)
    /// </summary>
    [CreateAssetMenu(fileName = "HealthPotion", menuName = "Items/HealthPotion")]
    public class HealthPotion : Item, IUsableItem
    {
        [SerializeField] private int healAmount = 50;

        public bool CanUse()
        {
            // 检查是否可以使用
            return true;
        }

        public void Use(Player.Player player)
        {
            if (player != null && CanUse())
            {
                player.Heal(healAmount);
                Debug.Log($"使用{Name}，恢复{healAmount}生命值");
            }
        }
    }
}