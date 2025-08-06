using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss2 : MonoBehaviour
{
    //GameObject数组
    public GameObject[] fire;
    public GameObject fireparent;

    // 技能冷却时间
    private float skill3CooldownTime = 10f;
    private float nextSkill3Time = 0f;

    // 技能1和2的冷却时间
    public float skill1CooldownTime = 6f;
    private float nextSkill1Time = 0f;

    public float skill2CooldownTime = 6f;
    private float nextSkill2Time = 0f;

    public GameObject Collider1;
    public GameObject Collider2;

    // 动画控制器
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //将fireparent的所有子物体存进数组fire
        fire = new GameObject[fireparent.transform.childCount];
        for (int i = 0; i < fireparent.transform.childCount; i++)
        {
            fire[i] = fireparent.transform.GetChild(i).gameObject;
        }

        // 启动协程
        StartCoroutine(ActivateRandomFireAfterSkill3());
    }

    // Update is called once per frame
    void Update()
    {
        // 检查技能3是否可以使用
        if (Time.time >= nextSkill3Time)
        {
            UseSkill3();
        }
        else if (Time.time >= nextSkill1Time && Time.time >= nextSkill2Time)
        {
            // 随机选择技能1或技能2
            UseRandomSkill();
        }
    }

    void UseSkill3()
    {
        // 设置下一次技能3的冷却时间
        nextSkill3Time = Time.time + skill3CooldownTime;

        // 播放技能3的动画
        animator.SetTrigger("3");

        // 重新启动协程
        StartCoroutine(ActivateRandomFireAfterSkill3());
    }

    IEnumerator ActivateRandomFireAfterSkill3()
    {
        // 等待3秒
        yield return new WaitForSeconds(3f);

        // 随机激活fire数组中的2个不同对象
        if (fire.Length >= 2)
        {
            // 使用一个HashSet来确保选择的索引是唯一的
            HashSet<int> indices = new HashSet<int>();

            // 随机选择2个不同的索引
            while (indices.Count < 2)
            {
                int randomIndex = Random.Range(0, fire.Length);
                indices.Add(randomIndex);
            }

            // 激活这三个对象
            foreach (int index in indices)
            {
                fire[index].SetActive(true);
            }
        }
    }

    void UseRandomSkill()
    {
        // 使用50%的概率来决定使用技能1还是技能2
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
        // 设置下一次技能1的冷却时间
        nextSkill1Time = Time.time + skill1CooldownTime;

        // 播放技能1的动画
        animator.SetTrigger("1");
    }

    void UseSkill2()
    {
        // 设置下一次技能2的冷却时间
        nextSkill2Time = Time.time + skill2CooldownTime;

        // 播放技能2的动画
        animator.SetTrigger("2");
    }

    public void Collider1D_(int co1)
    {
        if (co1 == 1)
        {
            Collider1.SetActive(true);
            Debug.Log("collider1D");
        }
        else if (co1 == 0)
        {
            Collider1.SetActive(false);
        }
    }

    public void Collider2D_(int co1)
    {
        if (co1 == 1)
        {
            Collider2.SetActive(true);
            Debug.Log("collider2D");
        }
        else if (co1 == 0)
        {
            Collider2.SetActive(false);
        }
    }
}
