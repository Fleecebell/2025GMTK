namespace Data
{
    /// <summary>
    /// 数据接口 (DIP: 依赖抽象)
    /// </summary>
    public interface IDataProvider
    {
        T LoadData<T>(string key);
        void SaveData<T>(string key, T data);
    }
}