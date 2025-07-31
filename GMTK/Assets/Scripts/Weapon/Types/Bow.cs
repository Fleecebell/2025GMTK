using UnityEngine;

namespace Weapon.Types
{
    /// <summary>
    /// 弓类武器 (OCP: 新武器只需派生)
    /// </summary>
    [CreateAssetMenu(fileName = "Bow", menuName = "Weapons/Bow")]
    public class Bow : Weapon
    {
        [SerializeField] private float arrowSpeed = 10f;
        [SerializeField] private GameObject arrowPrefab;

        public override void Attack(Vector2 direction)
        {
            Debug.Log($"使用{Name}射箭，方向：{direction}");
            // 射箭攻击逻辑
            if (arrowPrefab != null)
            {
                // 生成箭矢
            }
        }
    }
}