using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createmap : MonoBehaviour
{
    public GameObject[] roomPrefabs;

    // 地图大小
    public int mapWidth = 20;
    public int mapHeight = 20;

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
                int randomIndex = Random.Range(0, roomPrefabs.Length);
                GameObject selectedRoomPrefab = roomPrefabs[randomIndex];

                // 实例化房间预制体
                GameObject roomInstance = Instantiate(selectedRoomPrefab, new Vector3(x * 18.13f, y * 10.35f, 0), Quaternion.identity);

                // 如果需要，可以添加到某个父对象下
                roomInstance.transform.parent = transform;
            }
        }
    }
}
