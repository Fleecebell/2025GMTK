using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBullet : MonoBehaviour
{
    [Header("子弹属性")]
    [SerializeField] public float speed = 15f;         // 飞行速度
    [SerializeField] private float lifetime = 3f;       // 生命周期（超时自动销毁）
    [SerializeField] private int damage = 10;           // 基础伤害值
    [SerializeField] public LayerMask targetLayer;     // 可攻击目标图层
    [SerializeField] private GameObject hitEffect;      // 命中特效（可选）

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 确保碰撞体是触发器
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        // 自动销毁超时子弹
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// 初始化子弹参数
    /// </summary>
    public void Initialize(Vector2 direction, int customDamage = -1)
    {
        moveDirection = direction.normalized;
        rb.velocity = moveDirection * speed;

        // 应用自定义伤害（如果有）
        if (customDamage > 0)
            damage = customDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否命中目标图层
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            // 尝试对目标造成伤害
            IHealth health = other.GetComponent<IHealth>();
            health?.TakeDamage(damage);

            // 生成命中特效（如果有）
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        // 碰到其他碰撞体（如墙壁）也销毁
        else if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}