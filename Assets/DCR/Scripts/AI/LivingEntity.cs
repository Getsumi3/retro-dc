using UnityEngine;
using System.Collections;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{

    public float startingHealth;
    public float health { get; set; }
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }

    public delegate void Die();

    [ContextMenu("Self Destruct")]
    public virtual void DieFunc()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        //GameObject.Destroy(gameObject);
    }

    public virtual void TakeHit (float damage)
    {
        // Do some stuff here with hit var
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            DieFunc();
        }
    }
}
