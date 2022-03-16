using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    public float deactivateTime = 5.0f;     // 탄피 등장 후 비활성화 되는 시간
    public float casingSpin = 1.0f;         // 탄피가 회전하는 속력 개수
    public AudioClip[] audioClips;          // 탄피가 부딪혔을 때 재생되는 사운드

    private Rigidbody rigidbody3D;      //
    private AudioSource audioSource;    //
    private MemoryPool memoryPool;      //

    public void SetUp(MemoryPool pool, Vector3 direction)
    {

    }
}
