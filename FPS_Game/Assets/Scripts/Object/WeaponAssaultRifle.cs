using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssaultRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip   audioClipTakeOutWeapon;  // ���� ���� ����

    private AudioSource audioSource;             // ���� ��� ������Ʈ

    private void Awake()
    {
        // AudioSource ������Ʈ�� �����´�.
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Debug.Log("��Ĭ");
        // ���� ������ Ȱ��ȭ �ɶ����� ���� ���
        PlaySound(audioClipTakeOutWeapon);
    }

    // ���� ���
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // ������ ������� ���带 �����ϰ�,
        audioSource.clip = clip;    // ���ο� ���� clip���� ��ü ��
        audioSource.Play();         // ���� ���
    }
}
