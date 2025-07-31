namespace Skill
{
    /// <summary>
    /// 技能接口 (ISP: 技能类型分离)
    /// </summary>
    public interface ISkill
    {
        string Name { get; }
        bool CanUse();
        void Use();
    }
}