using UnityEngine;

namespace Weapon.Types
{
    /// <summary>
    /// 剑类武器 (OCP: 新武器只需派生)
    /// </summary>
    [CreateAssetMenu(fileName = "Sword", menuName = "Weapons/Sword")]
    public class Sword : Weapon
    {
        [SerializeField] private float slashRange = 2f;

        public override void Attack(Vector2 direction)
        {
            Debug.Log($"使用{Name}进行近战攻击，方向：{direction}");
            // 近战攻击逻辑
        }
    }
}