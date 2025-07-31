namespace AI
{
    /// <summary>
    /// AI×´Ì¬½Ó¿Ú
    /// </summary>
    public interface IState<T>
    {
        void Enter(T owner);
        void Update(T owner);
        void Exit(T owner);
    }
}