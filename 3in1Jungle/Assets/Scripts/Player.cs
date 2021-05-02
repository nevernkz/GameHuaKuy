using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float hp;
    [Range(0, 100)]
    public float playerHp;

    public delegate void DelegateHandleTakeDamage(GameObject dmFrom, float inDm);
    public delegate void DelegateHandleDead();

    public event DelegateHandleTakeDamage takeDm;
    public event DelegateHandleDead dead;

    public void TakeDamage(GameObject dmFrom, float inDm)
    {
        if (hp > 0)
        {
            hp -= inDm;

            if (takeDm != null)
            {
                takeDm(dmFrom, inDm);
            }

            if (hp <= 0)
            {
                if (dead != null)
                {
                    dead();
                }
            }
        }
    }

    //Ai
    public bool IsAlive()
    {
        return hp > 0;
    }
}
