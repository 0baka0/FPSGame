using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AmmoEvent : UnityEvent<int, int> { }
[Serializable]
public class MagazineEvet : UnityEvent<int> { }

public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvet onMagazineEvent = new MagazineEvet();

    [Header("Fire Effects")]
    public GameObject muzzleFlashEffect;        // 총구 이펙트 (On/Off)

    [Header("Spawn Points")]
    public Transform casingSpawnPoint;          // 탄피 생성 위치

    [Header("Audio Clips")]
    public AudioClip audioClipTakeOutWeapon;    // 무기 장착 사운드
    public AudioClip audioClipFire;             // 공격 사운드
    public AudioClip audioClipReload;           // 재장전 사운드

    [Header("Weapon Setting")]
    public WeaponSetting weaponSetting;         // 무기설정

    private float lastAttackTime = 0;           // 마지막 발사시간 체크
    private bool isReload = false;              // 재장전 중인지 체크

    private AudioSource audioSource;            // 사운드 재생 컴포넌트
    private PlayerAnim animator;                // 애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool;  // 탄피 생성 후 활성/비활성 관리

    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();              // AudioSource 컴포넌트를 가져온다.
        animator = GetComponentInParent<PlayerAnim>();          // PlayerAnim스크립트를 가진 부모 오브젝트의 컴포넌트를 가져온다.
        casingMemoryPool = GetComponent<CasingMemoryPool>();    // CasingMemoryPool스크립트 컴포넌트를 가져온다.

        // 처음 탄창 수는 최대로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // 처음 탄 수는 최대로 설정
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // 무기 장착이 활성화 될때마다 사운드 재생
        Debug.Log("찰칵");
        PlaySound(audioClipTakeOutWeapon);
        // 총구 이펙트 오브젝트 비활성화
        muzzleFlashEffect.SetActive(false);

        // 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신한다
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신한다
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    // 공격 모션 시작
    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다.
        if (isReload == true) return;

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

    public void StartReload()
    {
        // 현재 재정잔 중이면 재장전 불가능
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        // 무기 액션 도중에 'r'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        Debug.Log("찰칵");
        StartCoroutine("OnReload");
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

            // 탄 수가 없으면 공격 불가능
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // 공격 시 currentAmmo 1 감소
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // 무기 애니메이션 재생
            // 같은 애니메이션을 반복할 때 애니메이션을 끊고 처음부터 다시 재생
            animator.Play("Fire", -1, 0);
            // 총구 이펙트 재생 (쏠때마다)
            Debug.Log("반짝");
            StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            Debug.Log(audioClipFire);
            PlaySound(audioClipFire);
            // 탄피 생성
            Debug.Log("탄피 두두둥");
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
        }
    }

    // 무기의 공격속도보다 빠르게 muzzleFlashEffect를 활성화한 후 비활성화한다.
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * .3f);

        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        isReload = true;

        // 재장전 애니메이션, 사운드 재생
        animator.OnReload();
        PlaySound(audioClipReload);

        while(true)
        {
            // 사운드가 재생중이 아니고, 현재 애니메이션이 Movement이면
            // 재장전 애니메이션(, 사운드) 재생이 종료되었다는 뜻
            if (audioSource.isPlaying == false && animator.CurrentAnimaionIs("Movement"))
            {
                isReload = false;

                // 현재 탕창 수를 1 감소시키고, 바뀐 탄창 정보를 Text UI에 업데이트
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // 현재 탄 수를 최대로 설정하고, 바뀐 탄 수 정보를 Text UI에 업데이트
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    // 사운드 재생
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // 기존에 재생중인 사운드를 정지하고,
        audioSource.clip = clip;    // 새로운 사운드 clip으로 교체 후
        audioSource.Play();         // 사운드 재생
    }
}
