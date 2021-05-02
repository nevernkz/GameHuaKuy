using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusSystem : MonoBehaviour
{
    public float zombieHP = 100.0f;

    [Range(0,100)]
    public float playerHP;

    public delegate void DelegateHandleTakeDamage(GameObject damageFrom, float inDamage);
    public delegate void DelegateHandleDead();

    public event DelegateHandleTakeDamage OnTakeDamage;
    public event DelegateHandleDead OnDead;

    public void TakeDamage(GameObject damageFrom,float inDamage)
    {
        if (zombieHP > 0)
        {
            zombieHP -= inDamage;
            if (OnTakeDamage != null)
            {
                OnTakeDamage(damageFrom, inDamage);
            }

            if (zombieHP <= 0)
            {
                if (OnDead != null)
                {
                    OnDead();
                }
            }
        }
    }

    public bool IsAlive()
    {
        return zombieHP > 0;
    }

}
