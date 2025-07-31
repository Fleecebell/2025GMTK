using UnityEngine;

namespace Item
{
    /// <summary>
    /// 道具接口 (ISP: 道具功能分离)
    /// </summary>
    public interface IItem
    {
        string Name { get; }
        string Description { get; }
        Sprite Icon { get; }
    }
}