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
    public GameObject muzzleFlashEffect;        // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    public Transform casingSpawnPoint;          // ź�� ���� ��ġ
    public Transform bulletSpawnPoint;          // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    public AudioClip audioClipTakeOutWeapon;    // ���� ���� ����
    public AudioClip audioClipFire;             // ���� ����
    public AudioClip audioClipReload;           // ������ ����

    [Header("Weapon Setting")]
    public WeaponSetting weaponSetting;         // ���⼳��

    [Header("Aim UI")]
    public Image imageAim;                      // default/aim ��忡 ���� Aim �̹��� Ȱ��/��Ȱ��

    private float lastAttackTime = 0;           // ������ �߻�ð� üũ
    private bool isReload = false;              // ������ ������ üũ
    private bool isAttack = false;              // ���� ���� üũ��
    private bool isModeChange = false;          // ��� ��ȯ ���� üũ��
    private float defaultModeFOV = 60;          // �⺻��忡���� ī�޶� FOV
    private float aimModeFOV = 30;              // AIM��忡���� ī�޶� FOV

    private AudioSource audioSource;            // ���� ��� ������Ʈ
    private PlayerAnim animator;                // �ִϸ��̼� ��� ����
    private CasingMemoryPool casingMemoryPool;  // ź�� ���� �� Ȱ��/��Ȱ�� ����
    private ImpactMemoryPool impactMemoryPool;  // ���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
    private Camera mainCamera;                  // ���� �߻�

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();              // AudioSource ������Ʈ�� �����´�
        animator = GetComponentInParent<PlayerAnim>();          // PlayerAnim��ũ��Ʈ�� ���� �θ� ������Ʈ�� ������Ʈ�� �����´�
        casingMemoryPool = GetComponent<CasingMemoryPool>();    // CasingMemoryPool��ũ��Ʈ ������Ʈ�� �����´�
        impactMemoryPool = GetComponent<ImpactMemoryPool>();    // ImpactMemoryPool��ũ��Ʈ ������Ʈ�� �����´�
        mainCamera = Camera.main;                               // Camera ������Ʈ�� �����´�

        // ó�� źâ ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // ó�� ź ���� �ִ�� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // ���� ������ Ȱ��ȭ �ɶ����� ���� ���
        Debug.Log("��Ĭ");
        PlaySound(audioClipTakeOutWeapon);
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ �����Ѵ�
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����Ѵ�
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }

    // ���� ��� ����
    public void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼��� �� �� ����.
        if (isReload == true) return;

        // ��� ��ȯ���̸� ���� �׼��� �� �� ����
        if (isModeChange == true) return;

        // ���콺 ���� Ŭ�� (���� ����)
        if(type == 0)
        {
            // ���� ����
            if(weaponSetting.isAutomaticAttack == true)
            {
                Debug.Log("���Ӱ���");
                StartCoroutine("OnAttackLoop");
            }
            // �ܹ� ����
            else
            {
                Debug.Log("���");
                OnAttack();
            }
        }
        // ���콺 ������ Ŭ�� (��� ��ȯ)
        else
        {
            // ���� ���� ���� ��� ��ȯ�� �� �� ����
            if (isAttack == true) return;

            StartCoroutine("OnModeChange");
        }
    }

    // ���� ��� ����
    public void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ�� X (���� ����)
        if(type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        // ���� ������ ���̸� ������ �Ұ���
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        // ���� �׼� ���߿� 'r'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();

        Debug.Log("��Ĭ");
        StartCoroutine("OnReload");
    }

    // �����ϴ� OnAttack�� �ݺ������� ��Ÿ��
    private IEnumerator OnAttackLoop()
    {
        while(true)
        {
            OnAttack();

            yield return null;
        }
    }

    // ������ ����
    public void OnAttack()
    {
        //
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // �޸��� ���� ���� ������ �� ����.
            if (animator.MoveSpeed > 0.5f)
                return;

            // ���� �ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            // ź ���� ������ ���� �Ұ���
            if(weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // ���� �� currentAmmo 1 ����
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ���
            // ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���
            animator.Play("Fire", -1, 0);
            // ���� �ִϸ��̼� ��� (��忡 ���� AimFire or Fire �ִϸ��̼� ���)
            // animator.Play("Fire", -1, 0);
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);
            // �ѱ� ����Ʈ ��� (�򶧸���)
            Debug.Log("��¦");
            StartCoroutine("OnMuzzleFlashEffect");
            // ���� ���� ���
            Debug.Log(audioClipFire);
            PlaySound(audioClipFire);
            // ź�� ����
            Debug.Log("ź�� �εε�");
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // ������ �߻��� ���ϴ� ��ġ ���� (+Impact Effect)
            TwoStepRaycast();
        }
    }

    // ������ ���ݼӵ����� ������ muzzleFlashEffect�� Ȱ��ȭ�� �� ��Ȱ��ȭ�Ѵ�.
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * .3f);

        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        isReload = true;

        // ������ �ִϸ��̼�, ���� ���
        animator.OnReload();
        PlaySound(audioClipReload);

        while(true)
        {
            // ���尡 ������� �ƴϰ�, ���� �ִϸ��̼��� Movement�̸�
            // ������ �ִϸ��̼�(, ����) ����� ����Ǿ��ٴ� ��
            if (audioSource.isPlaying == false && animator.CurrentAnimaionIs("Movement"))
            {
                isReload = false;

                // ���� źâ ���� 1 ���ҽ�Ű��, �ٲ� źâ ������ Text UI�� ������Ʈ
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // ���� ź ���� �ִ�� �����ϰ�, �ٲ� ź �� ������ Text UI�� ������Ʈ
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    // �ѱ��� ��ġ���� �ٷ� �������� ����
    // �츮�� ���� �������� �����ִ� ��ġ���� Ÿ���� �� �� ���� ������
    // ������ 2�ܰ�� ������ �߻��Ѵ�
    // 1�ܰ迡�� aim�� �����ϴ� ȭ�� �߾ӿ� ��� ������ �߻��ϰ�
    // 2�ܰ迡�� 1�ܰ� ������ ���� ���� ��ġ������ �������� �ѱ����� �ٽ� ������ �߻��Ѵ�
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // ȭ���� �߾� ��ǥ (Aim �������� Raycast ����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * .5f);
        // ���� ��Ÿ�(attackDistance) �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� ������ �ε��� ��ġ
        if(Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        // ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
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

        animator.AimModeIs = !animator.AimModeIs;   // animator.AimModeIs�� ���̸� �����̵���, �����̸� ���̵���
        imageAim.enabled = !imageAim.enabled;       // imageAim.enabled�� ���̸� ������ �ʵ���, ������ ������ �Ⱥ��̵���

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        isModeChange = true;    // ��� �����߿��� ������ �� �� ������

        // start���� end���� �ݺ��Ѵ�
        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            // mode�� ���� ī�޶��� �þ߰��� ����
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;   // �� �ൿ�� �������� isModeChange�� �������� �ٲ��༭ ���� �����ϰ�
    }

    // bool���� �ʱ�ȭ ��Ŵ
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }

    // ���� ���
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // ������ ������� ���带 �����ϰ�,
        audioSource.clip = clip;    // ���ο� ���� clip���� ��ü ��
        audioSource.Play();         // ���� ���
    }
}
