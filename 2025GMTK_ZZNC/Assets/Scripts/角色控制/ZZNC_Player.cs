using System.Collections;
using UnityEngine;

public class ZZNC_Player : MonoBehaviour
{
    // 当前持有的武器实�?
    public GameObject currentWeaponInstance { get; private set; }
    // 当前持有武器的Weapon组件
    public Weapon currentWeaponComponent { get; private set; }

    public float speed = 5.0f;
    public float thrustPower = 10.0f; // 突刺的力�?
    public float thrustTime = 1.0f; // 突刺的时�?

    private Rigidbody2D rb; // 2D刚体
    private CapsuleCollider2D capsuleCollider; // 2D碰撞�?
    private SpriteRenderer spriteRenderer; // 添加SpriteRenderer组件
    private SpriteRenderer weaponSpriteRenderer; // 武器的SpriteRenderer组件
    [Header("世界边界")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取2D刚体组件
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // 获取2D碰撞器组�?
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件

        // 初始化时没有武器
        currentWeaponInstance = null;
        currentWeaponComponent = null;
        weaponSpriteRenderer = null;
    }

    void Update()
    {
        PlayerMove();

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("isAttack");
            // 启动协程，改变速度
            StartCoroutine(ThrustSpeed());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("isHurt");
            // 扣血
            Debug.Log("扣血");
            blood.HP -= 10;
        }

        if (blood.HP <= 0)
        {
            animator.SetTrigger("isDie");
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.localPosition;

        // X 轴环�?
        if (pos.x > maxX) pos.x = minX + 0.5f;
        if (pos.x < minX) pos.x = maxX - 0.5f;

        // Y 轴环�?
        if (pos.y > maxY) pos.y = minY + 0.5f;
        if (pos.y < minY) pos.y = maxY - 0.5f;
        transform.localPosition = pos;
    }

    private void PlayerMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(horizontal, vertical);
        rb.velocity = move * speed; // 设置速度

        // 翻转SpriteRenderer的x�?
        if (horizontal < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void SetPlayerWeapon(GameObject obj = null, Weapon weapon = null)
    {
        currentWeaponInstance = obj;
        currentWeaponComponent = weapon;

        // 获取武器的SpriteRenderer组件
        if (currentWeaponInstance != null)
        {
            weaponSpriteRenderer = currentWeaponInstance.GetComponent<SpriteRenderer>();
        }
        else
        {
            weaponSpriteRenderer = null;
        }
    }

    // 协程：改变速度
    private IEnumerator ThrustSpeed()
    {
        float originalSpeed = speed; // 保存原始速度
        speed = thrustPower; // 设置突刺速度

        yield return new WaitForSeconds(thrustTime); // 等待1�?

        speed = originalSpeed; // 恢复原始速度
    }
}
