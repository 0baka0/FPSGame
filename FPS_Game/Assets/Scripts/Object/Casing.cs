using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    public float deactivateTime = 5.0f;     // ź�� ���� �� ��Ȱ��ȭ �Ǵ� �ð�
    public float casingSpin = 1.0f;         // ź�ǰ� ȸ���ϴ� �ӷ� ����
    public AudioClip[] audioClips;          // ź�ǰ� �ε����� �� ����Ǵ� ����

    private Rigidbody rigidbody3D;      //
    private AudioSource audioSource;    //
    private MemoryPool memoryPool;      //

    public void SetUp(MemoryPool pool, Vector3 direction)
    {

    }
}
