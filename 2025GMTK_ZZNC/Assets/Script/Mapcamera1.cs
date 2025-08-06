using UnityEngine;

public class Mapcamera1 : MonoBehaviour
{
    public GameObject roomCamera;
    public GameObject unexploredBlack;

    private static GameObject activeCamera; // 静态变量跟踪当前活动的相机

    [SerializeField] private GameObject[] enemys;
    [SerializeField] private Transform createPoint;

    [SerializeField] private Transform[] transforms;

    [SerializeField] private EventType eventType; // 房间事件类型

    private bool isEnterRoom = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测是否是玩家进入
        if (other.CompareTag("Player"))
        {
            unexploredBlack.SetActive(false); // 隐藏未探索区域
            // 如果有当前活动的相机，禁用它
            if (activeCamera != null)
            {
                activeCamera.SetActive(false);
            }

            // 启用当前房间的相机
            if (roomCamera != null)
            {
                roomCamera.SetActive(true);
                activeCamera = roomCamera; // 更新当前活动的相机
            }

            // 开启房间事件
            if (!isEnterRoom)
            {
                StartRoomEvent();
            }
        }
    }

    private void StartRoomEvent()
    {
        switch (eventType)
        {
            case EventType.Enemies:
                SpawnEnemies();
                break;
            case EventType.Merchant:
                SpawnMerchant();
                break;
            case EventType.Treasure:
                SpawnTreasure();
                break;
            case EventType.Boss:
                SpawnBoss();
                break;
        }

        isEnterRoom = true;
    }

    private void SpawnEnemies()
    {
        if (enemys == null || enemys.Length == 0)
        {
            Debug.Log("没有敌人预制体，房间事件结束");
            return;
        }

        foreach (var enemyPrefab in enemys)
        {
            if (enemyPrefab == null) continue;

            // 在圆形范围内随机生成位置
            Vector2 randomPosition = RandomPointInCircle(createPoint.position, 3f); // 半径为3
            Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        }
    }

    private void SpawnMerchant()
    {
        Debug.Log("生成商品");
        if (transforms == null || transforms.Length != 6)
        {
            Debug.LogError("生成位置数量不正确，需要恰好6个位置！");
            return;
        }

        // 在 transforms[0] 和 transforms[1] 生成武器
        for (int i = 0; i < 2; i++)
        {
            ItemManager.Instance.DropRandomWeapon(transforms[i].position,true);
            Debug.Log("生成武器");
        }

        // 在 transforms[2] 到 transforms[5] 生成装备
        for (int i = 2; i < transforms.Length; i++)
        {
            ItemManager.Instance.DropRandomEquipment(transforms[i].position,true);
            Debug.Log("生成装备");
        }
    }

    private void SpawnTreasure()
    {
        Debug.Log("生成宝箱");
        // 在指定位置生成宝箱
        Instantiate(enemys[0], createPoint.position, Quaternion.identity);
    }

    private void SpawnBoss()
    {
        Debug.Log("生成Boss");
        // 在指定位置生成Boss
        if (transforms.Length > 2)
        {
            Instantiate(enemys[0], transforms[2].position, Quaternion.identity);
        }
    }

    // 生成圆形范围内随机点
    private Vector2 RandomPointInCircle(Vector2 center, float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float distance = Random.Range(0f, radius);
        return center + new Vector2(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance);
    }
}

// 定义房间事件类型
public enum EventType
{
    Enemies,    // 敌人
    Merchant,   // 商人
    Treasure,   // 宝箱
    Boss        // Boss
}