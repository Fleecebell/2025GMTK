using UnityEngine;

namespace Map.Generators
{
    /// <summary>
    /// 预定义地图生成器 (OCP: 可扩展不同算法)
    /// </summary>
    public class PredefinedMapGenerator : IMapGenerator
    {
        [SerializeField] private MapData[] predefinedMaps;

        public MapData GenerateMap(MapConfig config)
        {
            if (predefinedMaps != null && predefinedMaps.Length > 0)
            {
                int randomIndex = Random.Range(0, predefinedMaps.Length);
                return predefinedMaps[randomIndex];
            }

            // 如果没有预定义地图，返回空地图
            return new MapData();
        }

        public void SetPredefinedMaps(MapData[] maps)
        {
            predefinedMaps = maps;
        }
    }
}