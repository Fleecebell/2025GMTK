namespace Skill
{
    /// <summary>
    /// 被动技能接口
    /// </summary>
    public interface IPassiveSkill : ISkill
    {
        void OnEquip();
        void OnUnequip();
    }
}