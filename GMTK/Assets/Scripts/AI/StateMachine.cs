namespace AI
{
    /// <summary>
    /// 状态机组件 (Composition over Inheritance)
    /// </summary>
    public class StateMachine<T> where T : class
    {
        private IState<T> currentState;
        private T owner;

        public StateMachine()
        {
        }

        public void Initialize(T owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="newState">新状态</param>
        public void ChangeState(IState<T> newState)
        {
            currentState?.Exit(owner);
            currentState = newState;
            currentState?.Enter(owner);
        }

        /// <summary>
        /// 更新当前状态
        /// </summary>
        public void Update()
        {
            currentState?.Update(owner);
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns>当前状态</returns>
        public IState<T> GetCurrentState()
        {
            return currentState;
        }
    }
}