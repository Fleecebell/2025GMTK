using UnityEngine;

namespace Item.Types
{
    /// <summary>
    /// 武器道具 (OCP: 新道具只需派生)
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Items/WeaponItem")]
    public class WeaponItem : Equipment
    {
        [SerializeField] private int attackDamage;
        [SerializeField] private float attackSpeed;

        public int AttackDamage => attackDamage;
        public float AttackSpeed => attackSpeed;

        public override void OnEquip(Player.Player player)
        {
            base.OnEquip(player);
            // 武器装备特殊逻辑
        }

        public override void OnUnequip(Player.Player player)
        {
            base.OnUnequip(player);
            // 武器卸下特殊逻辑
        }
    }
}