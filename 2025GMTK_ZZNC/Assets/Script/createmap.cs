using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class createmap : MonoBehaviour
{
    public GameObject[] roomPrefabs;

    // 地图大小
    public int mapWidth = 20;
    public int mapHeight = 20;
     public NavMeshSurface navMeshSurface;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 随机选择一个房间预制体
                GameObject selectedRoomPrefab = SelectRoomPrefab();

                // 实例化房间预制体
                GameObject roomInstance = Instantiate(selectedRoomPrefab, new Vector3(x * 18.135f, y * 10.355f, 0), Quaternion.identity);

                // 如果需要，可以添加到某个父对象�?
                roomInstance.transform.parent = transform;
            }
        }
    }

    GameObject SelectRoomPrefab()
    {
        int randomIndex = Random.Range(0, 100); // 生成0�?99的随机数

        if (randomIndex < 55) // 45%的概率选择普通房�?
        {
            int roomIndex = Random.Range(0, 6); // 选择索引0�?5的普通房�?
            return roomPrefabs[roomIndex];
        }
        else if (randomIndex < 80) // 25%的概率选择商店（索�?6�?
        {
            return roomPrefabs[6];
        }
        else // 20%的概率选择宝箱房（索引7�?
        {
            return roomPrefabs[7];
        }
    }
}
