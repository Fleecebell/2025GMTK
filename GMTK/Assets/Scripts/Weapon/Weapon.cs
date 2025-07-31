using UnityEngine;

namespace Weapon
{
    /// <summary>
    /// 武器基类 (LSP: 子类可替换父类)
    /// </summary>
    public abstract class Weapon : ScriptableObject, IWeapon
    {
        [SerializeField] protected string weaponName;
        [SerializeField] protected int damage;
        [SerializeField] protected float attackSpeed;

        public string Name => weaponName;
        public int Damage => damage;
        public float AttackSpeed => attackSpeed;

        public abstract void Attack(Vector2 direction);
    }
}