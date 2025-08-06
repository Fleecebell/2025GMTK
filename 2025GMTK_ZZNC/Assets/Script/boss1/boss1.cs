using UnityEngine;

public class boss1 : MonoBehaviour
{
    // 技能冷却时�?
    private float skill3CooldownTime = 21f;
    private float nextSkill3Time = 0f;

    // 技�?1�?2的冷却时�?
    public float skill1CooldownTime = 8f;
    private float nextSkill1Time = 0f;

    public float skill2CooldownTime = 8f;
    private float nextSkill2Time = 0f;

    public GameObject Collider1;
    public GameObject Collider2;

    public GameObject kulou;


    // 动画控制�?
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 检查技�?3是否可以使用
        if (Time.time >= nextSkill3Time)
        {
            UseSkill3();
        }
        else if (Time.time >= nextSkill1Time && Time.time >= nextSkill2Time)
        {
            // 随机选择技�?1或技�?2
            UseRandomSkill();
        }

    }

    void UseSkill3()
    {
        // 设置下一次技�?3的冷却时�?
        nextSkill3Time = Time.time + skill3CooldownTime;

        // 播放技�?3的动�?
        animator.SetTrigger("3");
    }

    void UseRandomSkill()
    {
        // 使用50%的概率来决定使用技�?1还是技�?2
        if (Random.value < 0.5f)
        {
            UseSkill1();
        }
        else
        {
            UseSkill2();
        }
    }

    void UseSkill1()
    {
        // 设置下一次技�?1的冷却时�?
        nextSkill1Time = Time.time + skill1CooldownTime;

        // 播放技�?1的动�?
        animator.SetTrigger("1");
    }

    void UseSkill2()
    {
        // 设置下一次技�?2的冷却时�?
        nextSkill2Time = Time.time + skill2CooldownTime;

        // 播放技�?2的动�?
        animator.SetTrigger("2");
    }



    public void HookedCharacter()
    {
        // 当技�?2钩到角色后，立刻使用技�?1
        UseSkill1();
    }

    public void Collider1D_(int co1)
    {
        if (co1 == 1)
        {
            Collider1.SetActive(true);
            Enemy.speed = 30;

        }
        else if (co1 == 0)
        {
            Collider1.SetActive(false);
            Enemy.speed = 2;
        }
    }

    public void Collider2D_(int co1)
    {
        if (co1 == 1)
        {
            Collider2.SetActive(true);
        }
        else if (co1 == 0)
        {
            Collider2.SetActive(false);
        }
    }

    public void Collider3D_(int co1)
    {
        if (co1 == 1)
        {
            //kulou是预制体，在Vector3(0.709999979,2.8599999,0)位置生成kulou
            Instantiate(kulou, new Vector3(-64.3639374f, -4.5985899f, -0.479532301f), Quaternion.identity);
            Instantiate(kulou, new Vector3(-57.2638512f, -4.22774315f, -0.479532301f), Quaternion.identity);
            Instantiate(kulou, new Vector3(-55.8f, -5.5f, 0), Quaternion.identity);

        }
    }
}
