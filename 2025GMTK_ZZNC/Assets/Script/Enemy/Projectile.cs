using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 4f;
    private float damage;
    private Vector2 direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("投射物已生成，将在 " + lifetime + " 秒后销毁");
        Destroy(gameObject, lifetime); // 自动销毁
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = direction * speed;
            Debug.Log("投射物移动方向: " + direction + ", 速度: " + speed);
        }
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
        Debug.Log("投射物伤害设置为: " + damage);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Debug.Log("投射物方向设置为: " + direction + ", 旋转角度: " + angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"投射物触发碰撞到: {other.gameObject.name}, Tag: {other.gameObject.tag}");

        // 检测玩家碰撞
        if (other.CompareTag("Player"))
        {
            IHealth health = other.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("投射物击中玩家，造成 " + damage + " 伤害");
            }
            else
            {
                Debug.LogError("玩家对象上没有找到 IHealth 组件！");
            }
            Destroy(gameObject);
        }
    }
}