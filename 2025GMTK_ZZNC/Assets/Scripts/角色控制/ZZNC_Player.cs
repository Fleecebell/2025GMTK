using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZZNC_Player : MonoBehaviour
{
    // 当前持有的武器实例
    public GameObject currentWeaponInstance { get; private set; }
    // 当前持有武器的Weapon组件
    public Weapon currentWeaponComponent { get; private set; }

    public float speed = 5.0f;

    private Rigidbody2D rb; // 2D刚体
    private CapsuleCollider2D capsuleCollider; // 2D碰撞器
    private SpriteRenderer spriteRenderer; // 添加SpriteRenderer组件
    private SpriteRenderer weaponSpriteRenderer; // 武器的SpriteRenderer组件
    [Header("世界边界")]
[SerializeField] private float minX ;
[SerializeField] private float maxX;
[SerializeField] private float minY;
    [SerializeField] private float maxY;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取2D刚体组件
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // 获取2D碰撞器组件
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件

        // 初始化时没有武器
        currentWeaponInstance = null;
        currentWeaponComponent = null;
        weaponSpriteRenderer = null;
    }

    void Update()
    {
        PlayerMove();
    }
    private void LateUpdate()
    {
        Vector3 pos = transform.localPosition;

        // X 轴环绕
        if (pos.x > maxX) pos.x = minX + 0.5f;
        Debug.Log(pos.x);
        if (pos.x < minX) pos.x = maxX - 0.5f;
        Debug.Log(pos.x);

        // Y 轴环绕
        if (pos.y > maxY) pos.y = minY + 0.5f;
        Debug.Log(pos.y);
        if (pos.y < minY) pos.y = maxY - 0.5f;
        Debug.Log(pos.y);

        transform.localPosition = pos;
    }


    private void PlayerMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(horizontal, vertical);
        rb.velocity = move * speed; // 设置速度

        // 翻转SpriteRenderer的x轴
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
}