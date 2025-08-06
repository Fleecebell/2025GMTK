using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;

    private Rigidbody2D rb; // 2D刚体
    private CapsuleCollider2D capsuleCollider; // 2D碰撞器
    private SpriteRenderer spriteRenderer; // 添加SpriteRenderer组件

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取2D刚体组件
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // 获取2D碰撞器组件
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取SpriteRenderer组件

    }

    void Update()
    {
        PlayerMove();
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
}