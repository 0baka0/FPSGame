using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssaultRifle : MonoBehaviour
{
    [Header("Fire Effects")]
    public GameObject muzzleFlashEffect;        // 총구 이펙트 (On/Off)

    [Header("Audio Clips")]
    public AudioClip audioClipTakeOutWeapon;    // 무기 장착 사운드
    public AudioClip audioClipFire;             // 공격 사운드

    [Header("Weapon Setting")]
    public WeaponSetting weaponSetting;         // 무기설정
    private float lastAttackTime = 0;           // 마지막 발사시간 체크

    private AudioSource audioSource;            // 사운드 재생 컴포넌트
    private PlayerAnim animator;                // 애니메이션 재생 제어

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();      // AudioSource 컴포넌트를 가져온다.
        animator = GetComponentInParent<PlayerAnim>();  //
    }

    private void OnEnable()
    {
        // 무기 장착이 활성화 될때마다 사운드 재생
        Debug.Log("찰칵");
        PlaySound(audioClipTakeOutWeapon);
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);
    }

    // 공격 모션 시작
    public void StartWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 시작)
        if(type == 0)
        {
            // 연속 공격
            if(weaponSetting.isAutomaticAttack == true)
            {
                Debug.Log("연속공격");
                StartCoroutine("OnAttackLoop");
            }
            // 단발 공격
            else
            {
                Debug.Log("드득");
                OnAttack();
            }
        }
    }

    // 공격 모션 종료
    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 (공격 종료)
        if(type == 0)
        {
            StopCoroutine("OnAttackLoop");
        }
    }

    // 공격하는 OnAttack을 반복적으로 나타냄
    private IEnumerator OnAttackLoop()
    {
        while(true)
        {
            OnAttack();

            yield return null;
        }
    }

    // 실질적 공격
    public void OnAttack()
    {
        //
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // 달리고 있을 때는 공격할 수 없다.
            if (animator.MoveSpeed > 0.5f)
                return;

            // 공격 주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
            lastAttackTime = Time.time;

            // 무기 애니메이션 재생
            // 같은 애니메이션을 반복할 때 애니메이션을 끊고 처음부터 다시 재생
            animator.Play("Fire", -1, 0);
            // 총구 이펙트 재생 (쏠때마다)
            Debug.Log("반짝");
            StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            Debug.Log(audioClipFire);
            PlaySound(audioClipFire);
        }
    }

    // 무기의 공격속도보다 빠르게 muzzleFlashEffect를 활성화한 후 비활성화한다.
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * .3f);

        muzzleFlashEffect.SetActive(false);
    }

    // 사운드 재생
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // 기존에 재생중인 사운드를 정지하고,
        audioSource.clip = clip;    // 새로운 사운드 clip으로 교체 후
        audioSource.Play();         // 사운드 재생
    }
}
