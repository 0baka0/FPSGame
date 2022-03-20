using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class HPEvent : UnityEvent<int, int> { }

public class Status : MonoBehaviour
{
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    public float walkSpeed; // 걷는 속도
    public float runSpeed;  // 달리는 속도

    [Header("HP")]
    public int maxHP = 100;
    private int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = currentHP;

        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        onHPEvent.Invoke(previousHP, currentHP);

        if(currentHP == 0)
        {
            return true;
        }

        return false;
    }
}
