// 新增文件: EnemyRemoteAttack.cs
using UnityEngine;

[RequireComponent(typeof(Enemy), typeof(EnemyController))]
public class EnemyRemoteAttack : MonoBehaviour
{
    [Header("远程攻击设置")]
    [SerializeField] private float remoteAttackDistance = 8f; // 远程攻击范围
    [SerializeField] private float remoteAttackDamage = 2f;   // 远程攻击伤害
    [SerializeField] private float remoteAttackInterval = 1f; // 远程攻击间隔
    [SerializeField] private GameObject projectilePrefab;     //  projectile预制体
    [SerializeField] private Transform firePoint;             // 发射点

    private Enemy enemy;
    private EnemyController enemyController;
    private float lastRemoteAttackTime;
    private Transform player;

    private void Awake()
    {
        // 获取关联组件
        enemy = GetComponent<Enemy>();
        enemyController = GetComponent<EnemyController>();
        player = enemy.player; // 复用Enemy中已有的player引用
    }

    private void Update()
    {
        // 基础状态检查
        if (player == null || enemyController.IsDead || enemyController.IsHurt)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 远程攻击逻辑（在追逐范围内但超出近战范围时生效）
        if (distanceToPlayer <= remoteAttackDistance &&
            distanceToPlayer > enemy.attackDistance &&
            Time.time >= lastRemoteAttackTime + remoteAttackInterval)
        {
            PerformRemoteAttack();
        }
    }

    /// <summary>
    /// 执行远程攻击
    /// </summary>
    private void PerformRemoteAttack()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("远程攻击配置不完整");
            return;
        }

        // 触发远程攻击动画
        enemyController.RemoteAttack();

        // 生成 projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            // 设置 projectile 属性
            projectileScript.SetDamage(remoteAttackDamage);
            projectileScript.SetDirection((player.position - firePoint.position).normalized);
        }

        // 更新冷却时间
        lastRemoteAttackTime = Time.time;
    }

    // 绘制远程攻击范围 gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, remoteAttackDistance);
    }
}