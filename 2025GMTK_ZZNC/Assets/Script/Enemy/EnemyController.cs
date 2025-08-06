using System.Collections;
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
    public float attackPower;

    [Header("击退设置")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.1f;

    [Header("攻击范围")]
    [SerializeField] private Collider2D attackRangeCollider; // 攻击范围触发器
    [Header("货币奖励")]
    [SerializeField]private int minCurrency = 1;
    [SerializeField]private int maxCurrency = 3;
    //召唤蓝魂
    [Tooltip("敌人死亡时生成的特殊预制体")]
    public GameObject specialEnemyPrefab;

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

        // 检测攻击范围内的玩家
        Collider2D[] hitColliders = new Collider2D[10];
        int hitCount = Physics2D.OverlapCollider(attackRangeCollider, new ContactFilter2D(), hitColliders);

        Debug.Log($"攻击范围内检测到的碰撞体数量: {hitCount}");

        for (int i = 0; i < hitCount; i++)
        {
            var hitCollider = hitColliders[i];
            if (hitCollider != null)
            {
                Debug.Log($"检测到的碰撞体: {hitCollider.name}, Tag: {hitCollider.tag}");

                if (hitCollider.CompareTag("Player"))
                {
                    var playerHealth = hitCollider.GetComponent<IHealth>();
                    if (playerHealth != null)
                    {
                        Debug.Log($"对玩家造成伤害: {attackPower}");
                        playerHealth.TakeDamage(attackPower); // 假设每次攻击造成 attackPower 点伤害
                    }
                    else
                    {
                        Debug.LogError("玩家对象上没有找到 IHealth 组件！");
                    }
                }
            }
        }

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
        ItemManager.Instance.DropItemByRule(this.transform.position);
        int randomCurrency = Random.Range(minCurrency, maxCurrency + 1);
        Debug.Log($"随机生成的货币数: {randomCurrency}");
        CurrencyManager.Instance.GainCurrency(randomCurrency);
        Debug.Log("敌人死亡");
    }

    // 更新动画状态
    private void UpdateAnimation()
    {
        bool isMoving = agent != null && agent.velocity.sqrMagnitude > 0.1f;
        anim.SetBool("isWalk", isMoving);
        anim.SetBool("isDie", IsDead);
    }

    // 销毁敌人,并且召唤蓝魂
    public void DestroyEnemy()
    {
        // 检查是否已分配特殊预制体
        if (specialEnemyPrefab != null)
        {
            // 在当前敌人位置创建特殊预制体
            // 使用当前对象的位置和旋转
            Instantiate(specialEnemyPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("未分配特殊敌人预制体，请在Inspector中设置");
        }
        Destroy(gameObject);
    }

}