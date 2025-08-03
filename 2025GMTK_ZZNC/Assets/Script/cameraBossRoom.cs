using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBossRoom : MonoBehaviour
{
    public Transform playerTransform; // 角色的Transform
    public float offsetY = 0f; // 相机与角色的偏移量，如果有需要的话
    public float yUpperLimit = -7f; // Y轴的上限
    public float yLowerLimit = -15f; // Y轴的下限
    public Camera cameraBoss;
    public GameObject MiniMap;

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            // 计算相机的新位置，只改变Y轴
            float newY = playerTransform.position.y + offsetY;
            // 确保Y轴的位置在上限和下限之间
            newY = Mathf.Clamp(newY, yLowerLimit, yUpperLimit);
            // 将相机移动到新位置
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            playerTransform.position = new Vector3(-67.7699966f, -11.9700003f, 0f);
            cameraBoss.depth = 3;
            MiniMap.SetActive(false);
        }
    }
}
