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
    public GameObject muzzleFlashEffect;        // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    public Transform casingSpawnPoint;          // ź�� ���� ��ġ

    [Header("Audio Clips")]
    public AudioClip audioClipTakeOutWeapon;    // ���� ���� ����
    public AudioClip audioClipFire;             // ���� ����
    public AudioClip audioClipReload;           // ������ ����

    [Header("Weapon Setting")]
    public WeaponSetting weaponSetting;         // ���⼳��

    private float lastAttackTime = 0;           // ������ �߻�ð� üũ
    private bool isReload = false;              // ������ ������ üũ

    private AudioSource audioSource;            // ���� ��� ������Ʈ
    private PlayerAnim animator;                // �ִϸ��̼� ��� ����
    private CasingMemoryPool casingMemoryPool;  // ź�� ���� �� Ȱ��/��Ȱ�� ����

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();              // AudioSource ������Ʈ�� �����´�.
        animator = GetComponentInParent<PlayerAnim>();          // PlayerAnim��ũ��Ʈ�� ���� �θ� ������Ʈ�� ������Ʈ�� �����´�.
        casingMemoryPool = GetComponent<CasingMemoryPool>();    // CasingMemoryPool��ũ��Ʈ ������Ʈ�� �����´�.

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
    }

    // ���� ��� ����
    public void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼��� �� �� ����.
        if (isReload == true) return;

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
    }

    // ���� ��� ����
    public void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ�� (���� ����)
        if(type == 0)
        {
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
            // �ѱ� ����Ʈ ��� (�򶧸���)
            Debug.Log("��¦");
            StartCoroutine("OnMuzzleFlashEffect");
            // ���� ���� ���
            Debug.Log(audioClipFire);
            PlaySound(audioClipFire);
            // ź�� ����
            Debug.Log("ź�� �εε�");
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);
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

                // ���� ��â ���� 1 ���ҽ�Ű��, �ٲ� źâ ������ Text UI�� ������Ʈ
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

    // ���� ���
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // ������ ������� ���带 �����ϰ�,
        audioSource.clip = clip;    // ���ο� ���� clip���� ��ü ��
        audioSource.Play();         // ���� ���
    }
}
