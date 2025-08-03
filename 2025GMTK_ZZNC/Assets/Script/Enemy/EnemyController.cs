using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("移动属性")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("攻击设置")]
    [SerializeField] private float attackCoolDuration = 1f;      // 近战冷却
    [SerializeField] private float remoteAttackCoolDuration = 2f; // 远程冷却
    private bool canAttack = true;
    private bool canRemoteAttack = true;

    [Header("击退设置")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.1f;

    // 状态变量
    public bool IsHurt { get; private set; }
    public bool IsDead { get; private set; }
    public Vector2 MovementDirection { get; set; }

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private SpriteRenderer sr;
    private Animator anim;
    private NavMeshAgent agent;
    private Character character; // 引用Character组件

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<Character>();

        // 初始化导航速度
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        // 绑定Character事件
        if (character != null)
        {
            character.OnHurt.AddListener(HandleHurt);
            character.OnDie.AddListener(HandleDie);
        }
    }

    private void OnDestroy()
    {
        // 解除事件绑定，避免内存泄漏
        if (character != null)
        {
            character.OnHurt.RemoveListener(HandleHurt);
            character.OnDie.RemoveListener(HandleDie);
        }
    }

    private void FixedUpdate()
    {
        if (IsDead || IsHurt)
        {
            return;
        }

        HandleNavigationMovement();
        UpdateAnimation();
    }

    // 处理导航移动
    private void HandleNavigationMovement()
    {
        if (agent != null && agent.velocity.sqrMagnitude > 0.1f)
        {
            UpdateFacingDirection(agent.velocity);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    // 更新朝向（左右翻转）
    private void UpdateFacingDirection(Vector2 movement)
    {
        if (movement.x < 0)
        {
            sr.flipX = true;
        }
        else if (movement.x > 0)
        {
            sr.flipX = false;
        }
    }

    // 近战攻击处理
    public void Attack()
    {
        if (canAttack && !IsHurt && !IsDead)
        {
            canAttack = false;
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        anim.SetTrigger("Attack");
        agent?.ResetPath();
        yield return new WaitForSeconds(attackCoolDuration);
        canAttack = true;
    }

    // 远程攻击处理
    public void RemoteAttack()
    {
        if (canRemoteAttack && !IsHurt && !IsDead)
        {
            canRemoteAttack = false;
            StartCoroutine(RemoteAttackCoroutine());
        }
    }

    private IEnumerator RemoteAttackCoroutine()
    {
        anim.SetTrigger("RemoteAttack");
        agent?.ResetPath();
        yield return new WaitForSeconds(remoteAttackCoolDuration);
        canRemoteAttack = true;
    }

    // 处理受伤（由Character的OnHurt事件触发）
    public void HandleHurt()
    {
        if (IsDead) return;

        IsHurt = true;
        anim.SetTrigger("Hurt");
        agent?.ResetPath();
        StartCoroutine(RecoverFromHurt());
    }

    // 从受伤状态恢复
    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(0.3f);
        IsHurt = false;
    }

    // 击退处理
    public void ApplyKnockback(Vector3 attackerPosition)
    {
        if (IsDead) return;

        Vector2 direction = (transform.position - attackerPosition).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCoroutine());
    }

    private IEnumerator KnockbackCoroutine()
    {
        float timer = 0;
        while (timer < knockbackDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector2.zero;
    }

    // 处理死亡（由Character的OnDie事件触发）
    public void HandleDie()
    {
        if (IsDead) return;

        IsDead = true;
        anim.SetBool("isDie", true);
        agent.enabled = false;
        rb.velocity = Vector2.zero;
        enemyCollider.enabled = false;
    }

    // 更新动画状态
    private void UpdateAnimation()
    {
        bool isMoving = agent != null && agent.velocity.sqrMagnitude > 0.1f;
        anim.SetBool("isWalk", isMoving);
        anim.SetBool("isDie", IsDead);
    }

    // 销毁敌人
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}