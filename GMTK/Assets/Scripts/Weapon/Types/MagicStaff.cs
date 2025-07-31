using UnityEngine;

namespace Weapon.Types
{
    /// <summary>
    /// 魔法杖类武器 (OCP: 新武器只需派生)
    /// </summary>
    [CreateAssetMenu(fileName = "MagicStaff", menuName = "Weapons/MagicStaff")]
    public class MagicStaff : Weapon
    {
        [SerializeField] private int manaCost = 10;
        [SerializeField] private GameObject magicProjectilePrefab;

        public override void Attack(Vector2 direction)
        {
            Debug.Log($"使用{Name}施放魔法，方向：{direction}");
            // 魔法攻击逻辑
            if (magicProjectilePrefab != null)
            {
                // 生成魔法弹
            }
        }
    }
}