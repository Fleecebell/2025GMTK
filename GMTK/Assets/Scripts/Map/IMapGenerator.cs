namespace Map
{
    /// <summary>
    /// 地图生成接口 (DIP: 依赖抽象)
    /// </summary>
    public interface IMapGenerator
    {
        MapData GenerateMap(MapConfig config);
    }
}