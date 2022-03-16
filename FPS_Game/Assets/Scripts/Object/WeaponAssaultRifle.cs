using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssaultRifle : MonoBehaviour
{
    [Header("Fire Effects")]
    public GameObject muzzleFlashEffect;        // �ѱ� ����Ʈ (On/Off)

    [Header("Audio Clips")]
    public AudioClip audioClipTakeOutWeapon;    // ���� ���� ����
    public AudioClip audioClipFire;             // ���� ����

    [Header("Weapon Setting")]
    public WeaponSetting weaponSetting;         // ���⼳��
    private float lastAttackTime = 0;           // ������ �߻�ð� üũ

    private AudioSource audioSource;            // ���� ��� ������Ʈ
    private PlayerAnim animator;                // �ִϸ��̼� ��� ����

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();      // AudioSource ������Ʈ�� �����´�.
        animator = GetComponentInParent<PlayerAnim>();  //
    }

    private void OnEnable()
    {
        // ���� ������ Ȱ��ȭ �ɶ����� ���� ���
        Debug.Log("��Ĭ");
        PlaySound(audioClipTakeOutWeapon);
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);
    }

    // ���� ��� ����
    public void StartWeaponAction(int type = 0)
    {
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

            // ���� �ִϸ��̼� ���
            // ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���
            animator.Play("Fire", -1, 0);
            // �ѱ� ����Ʈ ��� (�򶧸���)
            Debug.Log("��¦");
            StartCoroutine("OnMuzzleFlashEffect");
            // ���� ���� ���
            Debug.Log(audioClipFire);
            PlaySound(audioClipFire);
        }
    }

    // ������ ���ݼӵ����� ������ muzzleFlashEffect�� Ȱ��ȭ�� �� ��Ȱ��ȭ�Ѵ�.
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * .3f);

        muzzleFlashEffect.SetActive(false);
    }

    // ���� ���
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // ������ ������� ���带 �����ϰ�,
        audioSource.clip = clip;    // ���ο� ���� clip���� ��ü ��
        audioSource.Play();         // ���� ���
    }
}
