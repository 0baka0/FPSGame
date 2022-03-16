using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator; // �ִϸ�Ƽ��

    public float MoveSpeed
    {
        get => animator.GetFloat("movementSpeed");
        set => animator.SetFloat("movementSpeed", value);
    }

    private void Awake()
    {
        // "Player" ������Ʈ �������� �ڽ� ������Ʈ��
        // "arms_assault-rifle_01" ������Ʈ�� Animator ������Ʈ�� �ִ�
        animator = GetComponentInChildren<Animator>();
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }
}
