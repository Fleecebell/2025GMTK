using UnityEngine;

    /// <summary>
    /// 子弹脚本 - 用于枪械类武器的弹药
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        [Header("子弹设置")]
        [SerializeField] private float lifeTime = 5f;          // 子弹生存时间
        [SerializeField] private LayerMask targetLayerMask = -1; // 目标图层
        [SerializeField] private bool destroyOnHit = true;      // 击中后是否销毁

        // 子弹属性
        private float damage;
        private Vector2 direction;
        private float speed;
        private bool isInitialized = false;

        // 组件引用
        private Rigidbody2D rb;
        private Collider2D bulletCollider;

        /// <summary>
        /// 初始化子弹
        /// </summary>
        /// <param name="bulletDamage">子弹伤害</param>
        /// <param name="bulletDirection">子弹方向</param>
        /// <param name="bulletSpeed">子弹速度</param>
        public void Initialize(float bulletDamage, Vector2 bulletDirection, float bulletSpeed)
        {
            damage = bulletDamage;
            direction = bulletDirection.normalized;
            speed = bulletSpeed;
            isInitialized = true;

            // 获取组件
            rb = GetComponent<Rigidbody2D>();
            bulletCollider = GetComponent<Collider2D>();

            // 如果没有Rigidbody2D，添加一个
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; // 子弹不受重力影响
            }

            // 如果没有Collider2D，添加一个
            if (bulletCollider == null)
            {
                bulletCollider = gameObject.AddComponent<CircleCollider2D>();
                ((CircleCollider2D)bulletCollider).radius = 0.1f;
                bulletCollider.isTrigger = true;
            }

            // 设置子弹运动
            rb.velocity = direction * speed;

            // 设置子弹朝向
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // 设置生存时间
            Destroy(gameObject, lifeTime);

            Debug.Log($"子弹初始化: 伤害={damage}, 速度={speed}, 方向={direction}");
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        /// <param name="other">碰撞对象</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isInitialized) return;

            // 检查是否为目标图层
            if (((1 << other.gameObject.layer) & targetLayerMask) == 0) return;

            // 对目标造成伤害
            var health = other.GetComponent<IHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log($"子弹击中 {other.name}，造成 {damage} 点伤害");
            }

            // 生成击中特效
            SpawnHitEffect(other.transform.position);

            // 如果设置为击中后销毁，则销毁子弹
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 生成击中特效
        /// </summary>
        /// <param name="hitPosition">击中位置</param>
        private void SpawnHitEffect(Vector3 hitPosition)
        {
            // 这里可以添加击中特效的生成逻辑
            // 例如：粒子效果、音效等
            Debug.Log($"子弹在 {hitPosition} 处击中目标");
        }

        /// <summary>
        /// 更新子弹位置（如果不使用物理系统）
        /// </summary>
        private void Update()
        {
            // 如果没有使用Rigidbody2D，手动移动子弹
            if (rb == null && isInitialized)
            {
                transform.Translate(direction * speed * Time.deltaTime, Space.World);
            }
        }

        /// <summary>
        /// 设置子弹目标图层
        /// </summary>
        /// <param name="layerMask">图层遮罩</param>
        public void SetTargetLayerMask(LayerMask layerMask)
        {
            targetLayerMask = layerMask;
        }

        /// <summary>
        /// 获取子弹信息
        /// </summary>
        /// <returns>子弹信息字符串</returns>
        public string GetBulletInfo()
        {
            return $"子弹伤害: {damage}, 速度: {speed}, 方向: {direction}";
        }
    }

