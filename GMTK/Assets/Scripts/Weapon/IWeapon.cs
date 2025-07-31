using UnityEngine;

namespace Weapon
{
    /// <summary>
    /// 武器接口 (ISP: 武器行为分离)
    /// </summary>
    public interface IWeapon
    {
        string Name { get; }
        int Damage { get; }
        float AttackSpeed { get; }
        void Attack(Vector2 direction);
    }
}