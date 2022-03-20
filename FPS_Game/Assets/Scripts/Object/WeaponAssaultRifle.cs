using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public Transform bulletSpawnPoint;          // 총알 생성 위치

    [Header("Audio Clips")]
    public AudioClip audioClipTakeOutWeapon;    // 무기 장착 사운드
    public AudioClip audioClipFire;             // 공격 사운드
    public AudioClip audioClipReload;           // 재장전 사운드

    [Header("Weapon Setting")]
    public WeaponSetting weaponSetting;         // 무기설정

    [Header("Aim UI")]
    public Image imageAim;                      // default/aim 모드에 따라 Aim 이미지 활성/비활성

    private float lastAttackTime = 0;           // 마지막 발사시간 체크
    private bool isReload = false;              // 재장전 중인지 체크
    private bool isAttack = false;              // 공격 여부 체크용
    private bool isModeChange = false;          // 모드 전환 여부 체크용
    private float defaultModeFOV = 60;          // 기본모드에서의 카메라 FOV
    private float aimModeFOV = 30;              // AIM모드에서의 카메라 FOV

    private AudioSource audioSource;            // 사운드 재생 컴포넌트
    private PlayerAnim animator;                // 애니메이션 재생 제어
    private CasingMemoryPool casingMemoryPool;  // 탄피 생성 후 활성/비활성 관리
    private ImpactMemoryPool impactMemoryPool;  // 공격 효과 생성 후 활성/비활성 관리
    private Camera mainCamera;                  // 광선 발사

    // 외부에서 필요한 정보를 열람하기 위해 정의한 Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();              // AudioSource 컴포넌트를 가져온다
        animator = GetComponentInParent<PlayerAnim>();          // PlayerAnim스크립트를 가진 부모 오브젝트의 컴포넌트를 가져온다
        casingMemoryPool = GetComponent<CasingMemoryPool>();    // CasingMemoryPool스크립트 컴포넌트를 가져온다
        impactMemoryPool = GetComponent<ImpactMemoryPool>();    // ImpactMemoryPool스크립트 컴포넌트를 가져온다
        mainCamera = Camera.main;                               // Camera 컴포넌트를 가져온다

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

        ResetVariables();
    }

    // 공격 모션 시작
    public void StartWeaponAction(int type = 0)
    {
        // 재장전 중일 때는 무기 액션을 할 수 없다.
        if (isReload == true) return;

        // 모드 전환주이면 무기 액션을 할 수 없다
        if (isModeChange == true) return;

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
        // 마우스 오른쪽 클릭 (모드 전환)
        else
        {
            // 공격 중일 때는 모드 전환을 할 수 없다
            if (isAttack == true) return;

            StartCoroutine("OnModeChange");
        }
    }

    // 공격 모션 종료
    public void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭 X (공격 종료)
        if(type == 0)
        {
            isAttack = false;
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
            // 무기 애니메이션 재생 (모드에 따라 AimFire or Fire 애니메이션 재생)
            // animator.Play("Fire", -1, 0);
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);
            // 총구 이펙트 재생 (쏠때마다)
            Debug.Log("반짝");
            StartCoroutine("OnMuzzleFlashEffect");
            // 공격 사운드 재생
            Debug.Log(audioClipFire);
            PlaySound(audioClipFire);
            // 탄피 생성
            Debug.Log("탄피 두두둥");
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // 광선을 발사해 원하는 위치 공격 (+Impact Effect)
            TwoStepRaycast();
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

                // 현재 탄창 수를 1 감소시키고, 바뀐 탄창 정보를 Text UI에 업데이트
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

    // 총구의 위치에서 바로 공격했을 떄는
    // 우리가 현재 에임으로 보고있는 위치에는 타격을 할 수 없기 때문에
    // 광선을 2단계로 나눠서 발사한다
    // 1단계에서 aim을 관통하는 화면 중앙에 쏘는 광선을 발사하고
    // 2단계에서 1단계 광선을 통해 얻은 위치정보를 바탕으로 총구에서 다시 광선을 발사한다
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // 화면의 중앙 좌표 (Aim 기준으로 Raycast 연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * .5f);
        // 공격 사거리(attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        if(Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = .35f;

        animator.AimModeIs = !animator.AimModeIs;   // animator.AimModeIs가 참이면 거짓이도록, 거짓이면 참이도록
        imageAim.enabled = !imageAim.enabled;       // imageAim.enabled가 보이면 보이지 않도록, 보이지 않으면 안보이도록

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        isModeChange = true;    // 모드 변경중에는 공격을 할 수 없도록

        // start부터 end까지 반복한다
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            // mode에 따라 카메라의 시야각을 변경
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;   // 위 행동이 끝났으면 isModeChange를 거짓으로 바꿔줘서 공격 가능하게
    }

    // bool값을 초기화 시킴
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }

    // 사운드 재생
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // 기존에 재생중인 사운드를 정지하고,
        audioSource.clip = clip;    // 새로운 사운드 clip으로 교체 후
        audioSource.Play();         // 사운드 재생
    }
}
