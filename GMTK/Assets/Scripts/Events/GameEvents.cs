namespace Events
{
    /// <summary>
    /// 玩家死亡事件
    /// </summary>
    public struct PlayerDeathEvent
    {
        // 可以添加相关数据
    }

    /// <summary>
    /// 理智值变化事件
    /// </summary>
    public struct SanValueChangedEvent
    {
        public int NewValue;
        
        public SanValueChangedEvent(int newValue)
        {
            NewValue = newValue;
        }
    }

    /// <summary>
    /// 敌人被击败事件
    /// </summary>
    public struct EnemyDefeatedEvent
    {
        public Enemy.Enemy enemy;
        
        public EnemyDefeatedEvent(Enemy.Enemy defeatedEnemy)
        {
            enemy = defeatedEnemy;
        }
    }
}