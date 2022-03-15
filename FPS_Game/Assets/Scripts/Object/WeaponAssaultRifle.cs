using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssaultRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip   audioClipTakeOutWeapon;  // 무기 장착 사운드

    private AudioSource audioSource;             // 사운드 재생 컴포넌트

    private void Awake()
    {
        // AudioSource 컴포넌트를 가져온다.
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Debug.Log("찰칵");
        // 무기 장착이 활성화 될때마다 사운드 재생
        PlaySound(audioClipTakeOutWeapon);
    }

    // 사운드 재생
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();         // 기존에 재생중인 사운드를 정지하고,
        audioSource.clip = clip;    // 새로운 사운드 clip으로 교체 후
        audioSource.Play();         // 사운드 재생
    }
}
