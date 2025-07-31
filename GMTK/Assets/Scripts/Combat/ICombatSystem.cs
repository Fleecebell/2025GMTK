using System.Collections.Generic;

namespace Combat
{
    /// <summary>
    /// 战斗系统接口 (ISP: 分离战斗关注点)
    /// </summary>
    public interface ICombatSystem
    {
        void StartCombat(List<Enemy.Enemy> enemies);
        void EndCombat();
        bool IsInCombat { get; }
    }
}