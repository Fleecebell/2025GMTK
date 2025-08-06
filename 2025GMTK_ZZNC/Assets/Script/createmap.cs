using System.Collections;
using UnityEngine;
using NavMeshPlus.Components;

public class createmap : MonoBehaviour
{
    public GameObject[] roomPrefabs;

    // 地图大小
    public int mapWidth = 20;
    public int mapHeight = 20;

    public NavMeshSurface navMeshSurface;

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
                GameObject roomInstance = Instantiate(selectedRoomPrefab, new Vector3(x * 18.135f, y * 10.355f, 0), Quaternion.identity);

                // 如果需要，可以添加到某个父对象�?
                roomInstance.transform.parent = transform;
            }
        }
        // 生成导航网格
        StartCoroutine(BuildNavMeshAfterDelay());
    }

    IEnumerator BuildNavMeshAfterDelay()
    {
        // 等待一段时间以确保所有房间都已实例化并渲�?
        yield return new WaitForSeconds(1f);

        // 生成导航网格
        navMeshSurface.BuildNavMesh();
        Debug.Log("导航网格已烘�?");
    }
}
