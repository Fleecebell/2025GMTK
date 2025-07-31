namespace Skill
{
    /// <summary>
    /// 主动技能接口
    /// </summary>
    public interface IActiveSkill : ISkill
    {
        float Cooldown { get; }
        float ManaCost { get; }
    }
}