using UnityEngine;

public class boss1_2Collider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 确保碰撞体被设置为触发器
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 触发器检测方�?
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否带�? "Player" 标签
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("二技能击中玩�?");
        }
    }
}
