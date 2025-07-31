using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapcamera1 : MonoBehaviour
{
    public GameObject roomCamera;
    public GameObject unexploredBlack;

    private static GameObject activeCamera; // 静态变量跟踪当前活动的相机

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
        }
    }
}