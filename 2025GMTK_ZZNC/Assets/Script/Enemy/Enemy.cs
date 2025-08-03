using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Enemy : Character
{
    [Header("导航设置")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float rotationSpeed = 5f; // 转向速度

    [Header("目标与范围")]
    [SerializeField] private Transform player;
    [SerializeField] private float chaseDistance = 3f;
    [SerializeField] private float attackDistance = 0.8f;

    [Header("攻击设置")]
    public float meleeAttackDamage;
    public UnityEvent OnAttack;

    [Header("行为参数")]
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;
    private EnemyController controller;

    private void Awake()
    {
        // 获取组件引用
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        controller = GetComponent<EnemyController>();

        // 2D导航设置
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        player=GameObject.Find("player").transform;
    }

    private void Update()
    {
        if (player == null || agent == null || controller == null || controller.IsDead)
            return;

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceToPlayer < chaseDistance)
        {
            if (distanceToPlayer <= attackDistance)
            {
                // 攻击范围内
                agent.ResetPath();
                //FaceTarget(player.position);
                TryAttack();
            }
            else
            {
                // 追击玩家
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // 超出范围，停止移动
            agent.ResetPath();
        }

        // 更新控制器的移动方向（用于动画）
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            controller.MovementDirection = new Vector2(agent.velocity.x, agent.velocity.y).normalized;
        }
        else
        {
            controller.MovementDirection = Vector2.zero;
        }
    }

    // 面向目标
    private void FaceTarget(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown && !controller.IsHurt)
        {
            OnAttack?.Invoke();
            lastAttackTime = Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
    