using UnityEngine;

namespace Weapon
{
    /// <summary>
    /// 武器管理器
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private IWeapon currentWeapon;

        /// <summary>
        /// 装备武器
        /// </summary>
        /// <param name="weapon">要装备的武器</param>
        public void EquipWeapon(IWeapon weapon)
        {
            currentWeapon = weapon;
            Debug.Log($"装备武器：{weapon.Name}");
        }

        /// <summary>
        /// 卸下武器
        /// </summary>
        public void UnequipWeapon()
        {
            if (currentWeapon != null)
            {
                Debug.Log($"卸下武器：{currentWeapon.Name}");
                currentWeapon = null;
            }
        }

        /// <summary>
        /// 获取当前武器
        /// </summary>
        /// <returns>当前装备的武器</returns>
        public IWeapon GetCurrentWeapon()
        {
            return currentWeapon;
        }

        /// <summary>
        /// 使用当前武器攻击
        /// </summary>
        /// <param name="direction">攻击方向</param>
        public void AttackWithCurrentWeapon(Vector2 direction)
        {
            currentWeapon?.Attack(direction);
        }
    }
}