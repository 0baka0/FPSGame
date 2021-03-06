using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator animator; // 애니메티어

    public float MoveSpeed
    {
        get => animator.GetFloat("movementSpeed");
        set => animator.SetFloat("movementSpeed", value);
    }

    // Assault Rifle 마우스 오른쪽 클릭 액션 (default/aim mode)
    public bool AimModeIs
    {
        get => animator.GetBool("isAimMode");
        set => animator.SetBool("isAimMode", value);
    }

    private void Awake()
    {
        // "Player" 오브젝트 기준으로 자식 오브젝트인
        // "arms_assault-rifle_01" 오브젝트에 Animator 컴포넌트가 있다
        animator = GetComponentInChildren<Animator>();
    }

    public void OnReload()
    {
        animator.SetTrigger("onReload");
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    public bool CurrentAnimaionIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}
