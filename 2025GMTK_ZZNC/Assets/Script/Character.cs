using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Character : MonoBehaviour,IHealth
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;

    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = value;
    }


    [Header("无敌")]
    public bool invulnerable;
    public float invulnerableDuration;//无敌时间

    public UnityEvent OnHurt;
    public UnityEvent OnDie;

    protected virtual void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if(invulnerable)
        {
            return;
        }
        
        CurrentHealth -= damage;

        StartCoroutine(InvulnerableCoroutine());//启动无敌时间协程
        //执行角色受伤动画
        OnHurt?.Invoke();
        
        if (CurrentHealth <= 0f)
        {
            //死亡
            Die();
        }
    }
public virtual void Die()
    {
        CurrentHealth = 0f;
        
        //执行角色死亡动画
        OnDie?.Invoke();
    }

    //无敌
    protected virtual IEnumerator InvulnerableCoroutine()
    {
        invulnerable = true;
        //等待无敌时间
        yield return new WaitForSeconds(invulnerableDuration);
        invulnerable = false;
    }
    
}
