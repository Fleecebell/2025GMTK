using UnityEngine;

namespace Item
{
    /// <summary>
    /// 道具基类 (LSP: 保证替换性)
    /// </summary>
    public abstract class Item : ScriptableObject, IItem
    {
        [SerializeField] protected string itemName;
        [SerializeField] protected string description;
        [SerializeField] protected Sprite icon;

        public string Name { get => itemName; protected set => itemName = value; }
        public string Description { get => description; protected set => description = value; }
        public Sprite Icon { get => icon; protected set => icon = value; }
    }
}